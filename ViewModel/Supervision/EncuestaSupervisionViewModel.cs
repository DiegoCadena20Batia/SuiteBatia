using BatiaSuite.Data;
using BatiaSuite.Models.Supervision;
using BatiaSuite.Utils;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using System.Text;
using System.Diagnostics;
using Microsoft.Maui.Controls.PlatformConfiguration;
using System.IO.Compression;

namespace BatiaSuite.ViewModel.Supervision;

public partial class EncuestaSupervisionViewModel : ViewModelBase, IQueryAttributable {

    [ObservableProperty]
    SupervisionRequestDataModel _supervisionRequestData;

    [ObservableProperty]
    ObservableCollection<string> _photoList;

    [ObservableProperty]
    bool _isLoading;

    [ObservableProperty]
    string _textLoading;

    [ObservableProperty]
    bool _showCleaner;

    DrawingView _drawingView;
    int _filesMaxNum = 5;
    string _pathFirmaCliente;
    SupervisionRequestDataModel _data;
    DbContext _dbContext;

    public EncuestaSupervisionViewModel(DrawingView drawingView) {
        SupervisionRequestData = new SupervisionRequestDataModel();
        _drawingView = drawingView;
        PhotoList = new ObservableCollection<string>();
    }

    [RelayCommand]
    async Task TakePhoto() {
        if(PhotoList.Count >= _filesMaxNum) {
            await Toast.Make($"{Constants.NUMERO_MAXIMO} {_filesMaxNum}", ToastDuration.Short).Show();
            return;
        }

        try {
            if(MediaPicker.Default.IsCaptureSupported) {
                if(await PopupUtil.HasCameraPermissions()) {
                    FileResult? fileResult = await MediaPicker.CapturePhotoAsync();

                    if(fileResult != null) {
                        string localFilePath = Path.Combine(FileSystem.CacheDirectory, fileResult.FileName);

                        using(Stream stream = await fileResult.OpenReadAsync()) {
                            using FileStream localFileStream = File.OpenWrite(localFilePath);
                            await stream.CopyToAsync(localFileStream);
                        }

                        PhotoList.Add(localFilePath);
                    }
                }
            }
        } catch(Exception) { }
    }

    [RelayCommand]
    void RemovePhoto(string filePath) =>
        PhotoList.Remove(filePath);

    [RelayCommand]
    void ClearDrawingView() {
        _pathFirmaCliente = null;
        _drawingView.Clear();
        ShowCleaner = false;
    }

    [RelayCommand]
    void Draw(IDrawingLine line) =>
        ShowCleaner = true;

    [RelayCommand]
    async Task SendData() {

        _data.Fechafin = DateTime.Now;
        if(!await ValidarFormulario()) {
            IsLoading = false;
            TextLoading = "";
            return;
        }

        IsLoading = true;
        TextLoading = "Preparando envío...";
        _data.Preguntas = new List<SupervisionPregunta>();
        var secciones = new[] {
            _data.PreguntasSeccion1,
            _data.PreguntasSeccion2,
            _data.PreguntasSeccion3,
            _data.PreguntasSeccion4
        };

        foreach(var seccion in secciones) {
            if(seccion != null) {
                _data.Preguntas.AddRange(seccion);
            }
        }
        _data.Fechafin = DateTime.Now;

        if(!await UploadPhotosAsync()) {
            //await App.Current.MainPage.DisplayAlert("", "Error al enviar archivos, la supervision de guard.", Constants.ACEPTAR);
            // GUARDAR SUPERVISION EN LOCAL

            _dbContext = new DbContext();
            
            TextLoading = "Guardando en el dispositivo...";
            if (await _dbContext.InsertSupervisionTotal(_data))
            {
                //BORRAR LA SUPERVISION PROGRAMADA
                if (_data.IdOrden > 0) {
                    await _dbContext.DeleteSupervisionProgramadaLocal(_data.IdOrden);
                }
                await App.Current.MainPage.DisplayAlert("", "Error de conexión, la supervisión se guardo en el dispositivo para un envio posterior.", Constants.ACEPTAR);
                await Shell.Current.GoToAsync("//MyMenu");
            }
            IsLoading = false;
            TextLoading = "";
            return;
        }

        //---------------------------------------------------------------------
        HttpClient cli = new HttpClient {
            //BaseAddress = new Uri("https://www.singa.com.mx:8086/api/SupervisionN")
            BaseAddress = new Uri(Constants.API_BASE_URL + "SupervisionN")
        };
        cli.DefaultRequestHeaders.Add("Accept", "application/json");
        //string json = JsonConvert.SerializeObject(_data);

        int result = 0;
        try {
            string json = JsonConvert.SerializeObject(_data);
            StringContent stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage res = await cli.PostAsync("SupervisionN", stringContent);

            if(res.IsSuccessStatusCode) {
                result = JsonConvert.DeserializeObject<int>(res.Content.ReadAsStringAsync().Result);
            } else {
                await App.Current.MainPage.DisplayAlert("", "Error al registrar supervisión", Constants.ACEPTAR);
                IsLoading = false;
            }
        } catch(Exception ex) {
            await App.Current.MainPage.DisplayAlert("", "Error al enviar supervision: " + ex.Message, Constants.ACEPTAR);
            //await Toast.Make($"Error al enviar supervision", ToastDuration.Short).Show();
            IsLoading = false;
            TextLoading = "";
        }


        //int res = await _httpHelper.PostBodyAsync<SupervisionRequestDataModel, int>(Constants.SUP_POST_ENVIAR_SUPERVISION, SupervisionRequestData);

        IsLoading = false;
        TextLoading = "";

        if(result > 0) {
            await Shell.Current.GoToAsync("//MyMenu");
            await App.Current.MainPage.DisplayAlert("", Constants.SUPERVISION_ENVIADA, Constants.ACEPTAR);
        } else {
            await App.Current.MainPage.DisplayAlert("", Constants.ERROR_API, Constants.ACEPTAR);
        }
    }
    async Task<bool> UploadPhotosAsync() {
        _data.Archivos = new List<ArchivoModel>();
        _data.Archivos.AddRange(_data.FotosPantalla1);
        _data.Archivos.AddRange(_data.FotosPantalla2);
        _data.Archivos.AddRange(_data.FotosPantalla3);
        _data.Archivos.AddRange(_data.FotosPantalla4);
        _data.Archivos.AddRange(_data.FotosPantalla5);

        if (_data.FotosPantalla6 != null)
        {
            _data.Archivos.AddRange(_data.FotosPantalla6);
        }

        if (!string.IsNullOrWhiteSpace(_data.PathFirmaOperador))
        {
            _data.Archivos.Add(new ArchivoModel
            {
                Path = _data.PathFirmaOperador,
                Seccion = 5
            });
        }

        _data.Archivos.Add(new ArchivoModel
        {
            Path = _data.PathVideo,
            Seccion = 6
        });

        if (SupervisionRequestData.Clienteentrevista)
        {
            _data.Archivos.Add(new ArchivoModel
            {
                Path = _pathFirmaCliente,
                Seccion = 7
            });

            _data.Archivos.AddRange(ConvertertFotoList(PhotoList, 7));
        }
        List<ArchivoModelNew> imagenes = new List<ArchivoModelNew>();
        ArchivoModelNew video = new ArchivoModelNew();

        foreach (var item in _data.Archivos)
        {

            if (item.Seccion == 6)
            {
                video.Nombre = item.Nombre;
                video.Seccion = item.Seccion;
                video.Path = item.Path;
                video.Tamano = item.Tamano;
            }
            else
            {
                ArchivoModelNew archivo = new ArchivoModelNew
                {
                    Nombre = item.Nombre,
                    Seccion = item.Seccion,
                    Path = item.Path,
                    Tamano = item.Tamano
                };
                imagenes.Add(archivo);
            }
        }
        string folder = "F" + DateTime.Now.Year.ToString() + "_" + DateTime.Now.Month.ToString();

        var lista = await UploadFilesAsyncStreaming(imagenes, video, folder);
        if (lista != null && lista.Count > 0)
        {
            foreach (var archivo in _data.Archivos)
            {
                var match = lista.FirstOrDefault(x => x.Path == archivo.Path);
                if (match != null)
                {
                    archivo.Nombre = match.NombreGenerado;
                }
            }
            return true;
        }
        else
        {
            return false;
        }

        //return false;


        //using(MultipartFormDataContent multipartContent = new MultipartFormDataContent()) {
        //    foreach(ArchivoModel archivo in _data.Archivos) {
        //        if(archivo.Seccion == 6) {
        //            //AQUI SE DEBE COMPRIMIR EL VIDEO LOCALIZADO EN archivo.Path
        //            FileStream fileStream = File.OpenRead(archivo.Path);
        //            StreamContent fileStreamContent = new StreamContent(fileStream);
        //            multipartContent.Add(fileStreamContent, "files", archivo.Nombre);
        //        } else {
        //            byte[] fileBytesArray = File.ReadAllBytes(archivo.Path);
        //            byte[] resizedImage = await ImageResizerHelper.ResizeImage(fileBytesArray, 480, 640);
        //            ByteArrayContent byteArrayContent = new ByteArrayContent(resizedImage);
        //            multipartContent.Add(byteArrayContent, "files", archivo.Nombre);
        //            archivo.Tamano = fileBytesArray.Length;
        //        }
        //    }
        //    List<ArchivoModel> result = await _httpHelper.PostMultipartAsync<List<ArchivoModel>>(Constants.SUP_POST_FOTOS, multipartContent, true);
        //    return result is not null;
        //}
    }
    public async Task<List<ArchivoModelNew>> UploadFilesAsyncStreaming(List<ArchivoModelNew> fotos, ArchivoModelNew video, string subDirectory) {
        try {
            string boundary = $"----Boundary{DateTime.Now.Ticks:x}";

            using var content = new MultipartFormDataContent(boundary);

            content.Headers.Remove("Content-Type");
            content.Headers.TryAddWithoutValidation("Content-Type", $"multipart/form-data; boundary={boundary}");

            content.Add(new StringContent(subDirectory, Encoding.UTF8), "subDirectory");
            int consecF = 0;

            foreach(var filePath in fotos) {
                byte[] fileBytes = await File.ReadAllBytesAsync(filePath.Path);
                bool posicionImagen = true;
                if (filePath.Nombre.StartsWith("Firma"))
                {
                    posicionImagen = false;
                }
                byte[] resizedImage = await ImageResizerHelper.ResizeImage(fileBytes, 480, 640, posicionImagen);

                var byteArrayContent = new ByteArrayContent(resizedImage);
                byteArrayContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

                var fileName = "F_" + filePath.Seccion.ToString() + "_" + _data.Id_Inmueble.ToString() + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + consecF.ToString() + Path.GetExtension(filePath.Path).ToString(); content.Add(byteArrayContent, "files", fileName);
                consecF++;
                filePath.NombreGenerado = fileName;
            }
            //CAMBIO PARA ZIP del video -----------------------------------------------
            if (!string.IsNullOrEmpty(video.Path) && File.Exists(video.Path))
            {
                //string? compressedPath = await CompressVideoWithLaerdalFFmpeg(video.Path);
                var videoFileName = "V_" + _data.Id_Inmueble.ToString() + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_1";
                var videoMemoryStream = new MemoryStream();
                using (var zipArchive = new ZipArchive(videoMemoryStream, ZipArchiveMode.Create, leaveOpen: true))
                {
                    ZipArchiveEntry videoEntry = zipArchive.CreateEntry(videoFileName + Path.GetExtension(video.Path), CompressionLevel.SmallestSize); //, CompressionLevel.SmallestSize = -861kb, optimal = 827kb
                    using (Stream entryStream = videoEntry.Open())
                    {
                        using (var fileStream = new FileStream(video.Path, FileMode.Open))
                        {
                            await fileStream.CopyToAsync(entryStream);
                        }
                    }
                }
                // IMPORTANTE: Rebobinar el stream antes de usarlo
                videoMemoryStream.Position = 0;
                var streamContent = new StreamContent(videoMemoryStream);
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/zip");
                string zfname = videoFileName + "!" + Path.GetExtension(video.Path).Replace(".", "") + ".zip";
                video.NombreGenerado = videoFileName + Path.GetExtension(video.Path);
                content.Add(streamContent, "files", zfname);
            }

            //Cambio para mandar video con streaming --------------------------------------------------
            //if (!string.IsNullOrEmpty(video.Path) && File.Exists(video.Path)) {
            //    var stream = new FileStream(video.Path, FileMode.Open, FileAccess.Read);
            //    var streamContent = new StreamContent(stream);
            //    streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("video/mp4"); // o ajusta al tipo real

            //    var videoFileName = "V_" + _data.Id_Inmueble.ToString() + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_1" + Path.GetExtension(video.Path).ToString();
            //    video.NombreGenerado = videoFileName;
            //    content.Add(streamContent, "files", videoFileName);
            //}

            var datosDeCargaDetalle = new { SubDirectorio = subDirectory, Fotos = fotos, Video = video };
            string jsonArchivosEnviados = JsonConvert.SerializeObject(datosDeCargaDetalle, Formatting.Indented);
            System.Diagnostics.Debug.WriteLine("=== DATOS ENVIADOS A STREAMING ===\n" + jsonArchivosEnviados);


            var handler = new HttpClientHandler();
            var httpClient = new HttpClient(handler) {
                Timeout = TimeSpan.FromMinutes(5)
            };
            using var request = new HttpRequestMessage(HttpMethod.Post, new Uri(Constants.API_BASE_URL + Constants.SUP_POST_FOTOS_STREAMING)) {
                Content = content
            };
            TextLoading = "Enviando supervision...";
            var resultEndpoint = await httpClient.SendAsync(request);
            if(resultEndpoint.IsSuccessStatusCode) {
                var responseContent = await resultEndpoint.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<List<ArchivoModel>>(responseContent);
                fotos.Add(video);
                return fotos;
            } else {
                Console.WriteLine($"Error: {resultEndpoint.StatusCode}");
                return new List<ArchivoModelNew>();
            }
        } catch(Exception ex) {
            Console.WriteLine($"ERROR: {ex.Message}");
            return new List<ArchivoModelNew>();
        }
    }
    //public async Task<string?> CompressVideoWithLaerdalFFmpeg(string inputPath)
    //{
    //    try
    //    {
    //        // 1. Verificar que el archivo existe
    //        if (!File.Exists(inputPath))
    //        {
    //            Console.WriteLine("Archivo de entrada no encontrado");
    //            return null;
    //        }

    //        // 2. Generar ruta de salida
    //        string outputPath = Path.Combine(FileSystem.CacheDirectory, $"compressed_{DateTime.Now:yyyyMMddHHmmss}.mp4");

    //        // 3. Configurar el comando FFmpeg
    //        string ffmpegCommand = $"-y -i \"{inputPath}\" " +
    //                             "-vcodec libx264 " +
    //                             "-crf 23 " +         // Calidad (23 es buen balance)
    //                             "-preset fast " +    // Velocidad de compresión
    //                             "-vf scale=480:-2 " + // Reducción a 480p
    //                             "-acodec aac " +
    //                             "-b:a 128k " +       // Bitrate de audio
    //                             "-movflags faststart " + // Para streaming
    //                             $"\"{outputPath}\"";
    //        if (File.Exists(inputPath))
    //        {
    //            Console.WriteLine($"El video esta listo para comprimir");
    //        }


    //        // 4. Ejecutar el comando (sin necesidad de Initialize)
    //        int result = FFmpeg.Execute(ffmpegCommand.Split(' '));

    //        if (result == 0) // Código de éxito de FFmpeg
    //        {
    //            // Verificar que el archivo de salida existe
    //            return File.Exists(outputPath) ? outputPath : null;
    //        }

    //        Console.WriteLine($"Error en compresión. Código: {result}");
    //        return null;
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine($"Error al comprimir video: {ex.Message}");
    //        return null;
    //    }
    //}
    //    public async Task<string> ComprimirVideoAsync(string inputPath)
    //    {
    //        string outputPath = GenerateOutputPath(inputPath);

    //        // Comando FFmpeg
    //        string command = $"-y -i \"{inputPath}\" -vcodec libx264 -crf 28 -preset fast -c:a copy \"{outputPath}\"";

    //        // Ejecutar el comando de forma asíncrona
    //#if ANDROID
    //        var session = Ffmpegkit.Droid.FFmpegKit.Execute(command);
    //#endif
    //        // Puedes verificar si fue exitoso así:
    //        //var returnCode = session.ReturnCode; 
    //        return outputPath;
    //    }
    //    public static async Task<string> CompressVideoAsync(
    //         string inputPath,
    //         int crf = 28,
    //         string preset = "fast",
    //         int? width = null,
    //         int? height = null)
    //    {
    //        Generar ruta de salida
    //        string outputPath = GenerateOutputPath(inputPath);

    //        Construir comando FFmpeg
    //        string command = BuildFFmpegCommand(inputPath, outputPath, crf, preset, width, height);

    //#if ANDROID        // Ejecutar comando (versión síncrona)
    //        await Task.Run(() =>
    //        {

    //            Laerdal.FFmpeg.Android.FFmpeg.Execute(command);
    //        });
    //#endif
    //        return outputPath;
    //    }


    //private static string GenerateOutputPath(string inputPath)
    //{
    //    string directory = Path.GetDirectoryName(inputPath);
    //    string fileName = Path.GetFileNameWithoutExtension(inputPath);
    //    string extension = Path.GetExtension(inputPath);
    //    return Path.Combine(directory, $"{fileName}_compressed{extension}");
    //}

    //    private static string BuildFFmpegCommand(
    //    string inputPath,
    //    string outputPath,
    //    int crf,
    //    string preset,
    //    int? width,
    //    int? height)
    //{
    //    string scale = (width != null && height != null) ? $"-vf scale={width}:{height} " : "";
    //    return $"-y -i \"{inputPath}\" {scale}-c:v h264 -crf {crf} -preset {preset} " +
    //           $"-c:a aac -movflags +faststart \"{outputPath}\"";
    //}

    //[RelayCommand]
    //async Task SendData() {
    //    if(!await ValidarFormulario()) {
    //        IsLoading = false;
    //        TextLoading = "";
    //        return;
    //    }

    //    IsLoading = true;
    //    TextLoading = "enviando supervisión ...";
    //    _data.Preguntas = new List<SupervisionPregunta>();
    //    var secciones = new[] {
    //        _data.PreguntasSeccion1,
    //        _data.PreguntasSeccion2,
    //        _data.PreguntasSeccion3,
    //        _data.PreguntasSeccion4
    //    };

    //    foreach(var seccion in secciones) {
    //        if(seccion != null) {
    //            _data.Preguntas.AddRange(seccion);
    //        }
    //    }

    //    if(!await UploadPhotosAsync()) {
    //        //await App.Current.MainPage.DisplayAlert("", "Error al enviar archivos, la supervisión se guardo en el dispositivo para enviarla mas tarde.", Constants.ACEPTAR);
    //        // GUARDAR SUPERVISION EN LOCAL
    //        DbContext _dbContext = new DbContext();
    //        var insertresult = await _dbContext.InsertSupervisionTotal(_data);
    //        if(insertresult) {
    //            await Shell.Current.GoToAsync("//MyMenu");
    //            await App.Current.MainPage.DisplayAlert("", "Error al enviar archivos, la supervisión se guardó en el dispositivo para enviarla mas tarde.", Constants.ACEPTAR);

    //        } else {
    //            await App.Current.MainPage.DisplayAlert("", "Error al enviar archivos, la supervision no se guardo en el dispositivo", Constants.ACEPTAR);

    //        }

    //        IsLoading = false;
    //        TextLoading = "";
    //        return;
    //    }

    //    //---------------------------------------------------------------------
    //    HttpClient cli = new HttpClient {
    //        //BaseAddress = new Uri("https://www.singa.com.mx:8086/api/SupervisionN")
    //        BaseAddress = new Uri(Constants.API_BASE_URL + "SupervisionN")
    //    };
    //    cli.DefaultRequestHeaders.Add("Accept", "application/json");

    //    int result = 0;
    //    try {
    //        string json = JsonConvert.SerializeObject(_data);
    //        StringContent stringContent = new StringContent(json, Encoding.UTF8, "application/json");

    //        HttpResponseMessage res = await cli.PostAsync("SupervisionN", stringContent);

    //        if(res.IsSuccessStatusCode) {
    //            result = JsonConvert.DeserializeObject<int>(res.Content.ReadAsStringAsync().Result);
    //        } else {
    //            await App.Current.MainPage.DisplayAlert("", "Error al registrar supervisión", Constants.ACEPTAR);
    //            IsLoading = false;
    //        }
    //    } catch(Exception ex) {
    //        await App.Current.MainPage.DisplayAlert("", "Error al enviar supervision: " + ex.Message, Constants.ACEPTAR);
    //        //await Toast.Make($"Error al enviar supervision", ToastDuration.Short).Show();
    //        IsLoading = false;
    //        TextLoading = "";
    //    }


    //    //int res = await _httpHelper.PostBodyAsync<SupervisionRequestDataModel, int>(Constants.SUP_POST_ENVIAR_SUPERVISION, SupervisionRequestData);

    //    IsLoading = false;
    //    TextLoading = "";

    //    if(result > 0) {
    //        await Shell.Current.GoToAsync("//MyMenu");
    //        await App.Current.MainPage.DisplayAlert("", Constants.SUPERVISION_ENVIADA, Constants.ACEPTAR);
    //    } else {
    //        await App.Current.MainPage.DisplayAlert("", Constants.ERROR_API, Constants.ACEPTAR);
    //    }
    //}

    public async Task<bool> SaveInLocal() {
        //GUARDAR EL CABECERO PARA OBTENER EL Id
        //METER ARCHIVOS EN UNA TABLA DE MODO QE YA ESTAN ESTRUCTURADOS;
        //DE IGUAL MANERA GUARDAR LAS PREGUNTAS YA QUE EN CONJUNTO YA ESTAN IDENTIFICADAS

        //PARA EL ENVIO POSTERIOR SI SE DEBERAN PROCESAR LAS IMAGENER Y VIDEO COMO STREAM


        //CLIENTE ENTREVISTA:
        //_data.Clienteentrevista = SupervisionRequestData.Clienteentrevista;
        //_data.Clientenombre = SupervisionRequestData.Clientenombre;
        //_data.Clientecomentario = SupervisionRequestData.Clientecomentario;
        //_data.Evalua = SupervisionRequestData.Evalua;
        //_data.Trabrealizados = SupervisionRequestData.Trabrealizados;
        //_data.Tratopersonal = SupervisionRequestData.Tratopersonal;
        //_data.Uniformcompleto = SupervisionRequestData.Uniformcompleto;
        //_data.Suprecorrido = SupervisionRequestData.Suprecorrido;
        //_data.Areaoportunidad = SupervisionRequestData.Areaoportunidad;
        //_data.Plancorrectivo = SupervisionRequestData.Plancorrectivo;
        //_data.Calificasup = SupervisionRequestData.Calificasup;
        ////_data.Ejecutivocgo = SupervisionRequestData.Ejecutivocgo;
        //_data.Reporteasiscgo = SupervisionRequestData.Reporteasiscgo;
        //_data.Matetiquetados = SupervisionRequestData.Matetiquetados;
        //_data.Matrequerimientos = SupervisionRequestData.Matrequerimientos;
        return true;
    }

    [RelayCommand]
    void IsChecked(bool value) {
        if(!value) { // en el control CustomCheckBox, el Command se ejecuta antes que Evento clicked
            return;
        }

        PhotoList.Clear();
        ClearDrawingView();
    }

    //async Task<bool> UploadPhotosAsync() {
    //        _data.Archivos = new List<ArchivoModel>();
    //        _data.Archivos.AddRange(_data.FotosPantalla1);
    //        _data.Archivos.AddRange(_data.FotosPantalla2);
    //        _data.Archivos.AddRange(_data.FotosPantalla3);
    //        _data.Archivos.AddRange(_data.FotosPantalla4);
    //        _data.Archivos.AddRange(_data.FotosPantalla5);

    //        if(_data.FotosPantalla6 != null) {
    //            _data.Archivos.AddRange(_data.FotosPantalla6);
    //        }

    //        if(!string.IsNullOrWhiteSpace(_data.PathFirmaOperador)) {
    //            _data.Archivos.Add(new ArchivoModel {
    //                Path = _data.PathFirmaOperador,
    //                Seccion = 5
    //            });
    //        }

    //        _data.Archivos.Add(new ArchivoModel {
    //            Path = _data.PathVideo,
    //            Seccion = 6
    //        });

    //        if(SupervisionRequestData.Clienteentrevista) {
    //            _data.Archivos.Add(new ArchivoModel {
    //                Path = _pathFirmaCliente,
    //                Seccion = 7
    //            });

    //            _data.Archivos.AddRange(ConvertertFotoList(PhotoList, 7));
    //        }

    //        using(MultipartFormDataContent multipartContent = new MultipartFormDataContent()) {
    //            foreach(ArchivoModel archivo in _data.Archivos) {
    //                if(archivo.Seccion == 6) {
    //                //AQUI SE DEBE COMPRIMIR EL VIDEO LOCALIZADO EN archivo.Path
    //                    FileStream fileStream = File.OpenRead(archivo.Path);
    //                    StreamContent fileStreamContent = new StreamContent(fileStream);
    //                    multipartContent.Add(fileStreamContent, "files", archivo.Nombre);
    //                } else {
    //                    byte[] fileBytesArray = File.ReadAllBytes(archivo.Path);
    //                    byte[] resizedImage = await ImageResizerHelper.ResizeImage(fileBytesArray, 450, 650);
    //                    ByteArrayContent byteArrayContent = new ByteArrayContent(resizedImage);
    //                    multipartContent.Add(byteArrayContent, "files", archivo.Nombre);
    //                    archivo.Tamano = fileBytesArray.Length;
    //                }
    //            }
    //            List<ArchivoModel> result = await _httpHelper.PostMultipartAsync<List<ArchivoModel>>(Constants.SUP_POST_FOTOS, multipartContent, true);
    //        if (result is not null) {
    //            return result is not null;
    //        } else {
    //            //guardar path de fotos en SQLITE
    //            //_data.FotosPantalla1;
    //            return false;
    //        }
    //        }
    //    }

    async Task<bool> ValidarFirma() {
        try {
            _pathFirmaCliente = Path.Combine(FileSystem.CacheDirectory, $"Firma_{Guid.NewGuid()}.png");

            using(Stream stream = await _drawingView.GetImageStream(512, 512)) {
                using FileStream localFileStream = File.OpenWrite(_pathFirmaCliente);
                await stream.CopyToAsync(localFileStream);
            }

            return true;
        } catch(Exception) {
            _pathFirmaCliente = null;
            return false;
        }
    }

    async Task<bool> ValidarFormulario() {
        if(!SupervisionRequestData.Clienteentrevista) {
            return true;
        } else {
            if(string.IsNullOrWhiteSpace(SupervisionRequestData.Clientenombre)) {
                await Toast.Make(Constants.INGRESE_ENCUESTADO, ToastDuration.Short).Show();
                return false;
            }

            if(_drawingView.Lines.Count == 0) {
                await Toast.Make(Constants.INGRESE_FIRMA, ToastDuration.Short).Show();
                return false;
            }
        }

        await ValidarFirma();
        _data.Clienteentrevista = SupervisionRequestData.Clienteentrevista;
        _data.Clientenombre = SupervisionRequestData.Clientenombre;
        _data.Clientecomentario = SupervisionRequestData.Clientecomentario;
        _data.Evalua = SupervisionRequestData.Evalua;
        _data.Trabrealizados = SupervisionRequestData.Trabrealizados;
        _data.Tratopersonal = SupervisionRequestData.Tratopersonal;
        _data.Uniformcompleto = SupervisionRequestData.Uniformcompleto;
        _data.Suprecorrido = SupervisionRequestData.Suprecorrido;
        _data.Areaoportunidad = SupervisionRequestData.Areaoportunidad;
        _data.Plancorrectivo = SupervisionRequestData.Plancorrectivo;
        _data.Calificasup = SupervisionRequestData.Calificasup;
        //_data.Ejecutivocgo = SupervisionRequestData.Ejecutivocgo;
        _data.Reporteasiscgo = SupervisionRequestData.Reporteasiscgo;
        _data.Matetiquetados = SupervisionRequestData.Matetiquetados;
        _data.Matrequerimientos = SupervisionRequestData.Matrequerimientos;
        return true;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query) {
        if(query.ContainsKey(Constants.SUPERVISION_REQUEST_DATA_KEY)) {
            _data = (SupervisionRequestDataModel)query[Constants.SUPERVISION_REQUEST_DATA_KEY];

            query.Remove(Constants.SUPERVISION_REQUEST_DATA_KEY);
        }
    }
}