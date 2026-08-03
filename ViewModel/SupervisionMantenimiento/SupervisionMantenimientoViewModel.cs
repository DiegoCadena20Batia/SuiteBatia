using BatiaSuite.Data;
using BatiaSuite.Models.Supervision;
using BatiaSuite.Utils;
using BatiaSuite.Views.SupervisionMantenimiento;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO.Compression;
using System.Text;

namespace BatiaSuite.ViewModel.SupervisionMantenimiento;

public partial class SupervisionMantenimientoViewModel : ViewModelBase
{

    List<object> _yearList;
    int _filterMonth;
    List<SupervisionModel> _supervisiones;
    List<SupervisionLocal> _supervisionesP;

    [ObservableProperty]
    ObservableCollection<SupervisionModel> _ordenSupervisionList;
    [ObservableProperty]
    ObservableCollection<SupervisionLocal> _ordenSupervisionListP;

    [ObservableProperty]
    string _selectedMonth;

    [ObservableProperty]
    int _filterYear;

    [ObservableProperty]
    bool _isLoading;

    [ObservableProperty]
    string _textLoading;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(GetYearCommand), nameof(GetMonthCommand))]
    bool _isBusy;

    [ObservableProperty]
    string _query;

    [ObservableProperty]
    bool _isRefreshing;
    DbContext _dbContext;

    [ObservableProperty]
    DateTime _fechaCarga;

    [ObservableProperty]
    bool _fechaCargaValida;

    [ObservableProperty]
    bool _existeProgramadas;
    [ObservableProperty]
    bool _existeLocal;
    public SupervisionMantenimientoViewModel()
    {
        InitValues();
        _dbContext = new DbContext();
    }

    [RelayCommand]
    async Task RefreshPage()
    {
        IniciarCarga("Cargando...");
        //await LoadListOffline();
        await LoadList();
        DetenerCarga();
        IsRefreshing = false;
    }

    [RelayCommand]
    void FiltarInmueble(string query)
    {
        try {

        //PROGRAMADAS
        OrdenSupervisionList.Clear();
        foreach (SupervisionModel supervision in _supervisiones)
        {
            if (supervision.Inmueble.Contains(query, StringComparison.OrdinalIgnoreCase))
            {
                OrdenSupervisionList.Add(supervision);
            }
        }
        ExisteProgramadas = OrdenSupervisionList.Count > 0;

        //LOCALES
        OrdenSupervisionListP.Clear();
        foreach(SupervisionLocal supervision in _supervisionesP) {
                if(supervision.Inmueble == null) {
                    supervision.Inmueble = "";
                }
            if(supervision.Inmueble.Contains(query, StringComparison.OrdinalIgnoreCase)) {
                OrdenSupervisionListP.Add(supervision);
            }
        }
        ExisteLocal = OrdenSupervisionListP.Count > 0;
        }
        catch(Exception ex){
            Console.WriteLine("Error:" + ex.Message);
        }
    }

    [RelayCommand(CanExecute = nameof(CanExecute))]
    async Task GetYear()
    {
        double size = Constants.IS_IOS ? Constants.IS_TABLET ? 5 : 7 : Constants.IS_TABLET ? 5 : 5;
        IsBusy = true;
        int value = (int)await PopupUtil.GetObjectAsync(FilterYear, _yearList, size);
        if (value == FilterYear)
        {
            IsBusy = false;
            return;
        }

        await Task.Delay(150);

        FilterYear = value;

        IsLoading = true;
        await LoadList();
        IsLoading = false;

        IsBusy = false;
    }

    [RelayCommand(CanExecute = nameof(CanExecute))]
    async Task GetMonth()
    {
        IsBusy = true;
        double size = Constants.IS_IOS ? Constants.IS_TABLET ? 5 : 5 : Constants.IS_TABLET ? 3 : 3;
        string value = (string)await PopupUtil.GetObjectAsync(SelectedMonth, Constants.MonthList, size);

        if (value == SelectedMonth)
        {
            IsBusy = false;
            return;
        }

        await Task.Delay(150);

        SelectedMonth = value;
        _filterMonth = Constants.GetMonthNumber(SelectedMonth);
        IsLoading = true;
        await LoadList();
        IsLoading = false;

        IsBusy = false;
    }

    [RelayCommand]
    async Task SelectedOrdenLocal(SupervisionLocal orden)
    {
        ////return; //COMENTAR PARA PROD
        //try {

        
        //if(!Utils.InternetUtil.IsConnectedInternet()) {
        //    await Toast.Make(Constants.ERROR_INTERNET, ToastDuration.Short).Show();
        //    return;
        //}
        //IsBusy = true;
        //IniciarCarga("Enviando supervisión...");
        //var supervision = await _dbContext.GetSupervisionLocal(orden.IdLocal);
        //if (supervision == null) {
        //    await App.Current.MainPage.DisplayAlert("Error", "Ocurrió un error al leer la supervisión. la lectura devolvió NULL", Constants.ACEPTAR);
        //    return;
        //}
        ////ENVIAR SUPERVISION Y ACTUALIZAR LA LOCAL COMO ENVIADA
        //if(await UploadPhotosAsync(supervision)) {
        //    if(await EnviarSupervision(supervision)) {
        //        await _dbContext.MarcarSupervisionEnviada(orden.IdLocal);
        //        OrdenSupervisionList = new ObservableCollection<SupervisionModel>();
        //        OrdenSupervisionListP = new ObservableCollection<SupervisionLocal>();
        //        await RefreshPage();
        //        IsBusy = false;
        //        DetenerCarga();
        //    }

        //} else {
        //    await App.Current.MainPage.DisplayAlert("Error", "Mala conexión a internet, inténtelo más tarde.", Constants.ACEPTAR);
        //    DetenerCarga();
        //    IsBusy = false;
        //}
        //DetenerCarga();
        //IsBusy = false;
        //}
        //catch(Exception ex) {
        //    await App.Current.MainPage.DisplayAlert("Error", "Ocurrió un error al leer la supervisión: \n\n " + ex.Message, Constants.ACEPTAR);
        //    DetenerCarga();
        //    IsBusy = false;
        //}
    }
    async Task<bool> UploadPhotosAsync(SupervisionRequestDataModel supervision)
    {
        List<ArchivoModelNew> imagenes = new List<ArchivoModelNew>();
        ArchivoModelNew video = new ArchivoModelNew();

        foreach (var item in supervision.Archivos)
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

        var lista = await UploadFilesAsyncStreaming(imagenes, video, folder, supervision.Id_Inmueble);
        if (lista != null && lista.Count > 0)
        {
            foreach (var archivo in supervision.Archivos)
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
    }

    public async Task<List<ArchivoModelNew>> UploadFilesAsyncStreaming(List<ArchivoModelNew> fotos, ArchivoModelNew video, string subDirectory, int idInmueble)
    {
        try
        {
            string boundary = $"----Boundary{DateTime.Now.Ticks:x}";
            using var content = new MultipartFormDataContent(boundary);

            content.Headers.Remove("Content-Type");
            content.Headers.TryAddWithoutValidation("Content-Type", $"multipart/form-data; boundary={boundary}");

            content.Add(new StringContent(subDirectory, Encoding.UTF8), "subDirectory");
            int consecF = 0;

            foreach (var filePath in fotos)
            {
                if(!string.IsNullOrEmpty(filePath.Path) && File.Exists(filePath.Path)) {
                    byte[] fileBytes = await File.ReadAllBytesAsync(filePath.Path);
                    bool posicionImagen = true;
                    if(filePath.Nombre.StartsWith("Firma")) {
                        posicionImagen = false;
                    }
                    byte[] resizedImage = await ImageResizerHelper.ResizeImage(fileBytes, 480, 640, posicionImagen); // Ajusta dimensiones según tu necesidad

                    var byteArrayContent = new ByteArrayContent(resizedImage);
                    byteArrayContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

                    var fileName = "F_" + idInmueble.ToString() + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + consecF.ToString() + Path.GetExtension(filePath.Path).ToString();
                    content.Add(byteArrayContent, "files", fileName);
                    consecF++;
                    filePath.NombreGenerado = fileName;
                }
            }
            //CAMBIO PARA ZIP del video -----------------------------------------------
            if (!string.IsNullOrEmpty(video.Path) && File.Exists(video.Path))
            {
                //string? compressedPath = await CompressVideoWithLaerdalFFmpeg(video.Path);
                var videoFileName = "V_" + idInmueble.ToString() + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_1";
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
            var handler = new HttpClientHandler();
            var httpClient = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromMinutes(5)
            };
            using var request = new HttpRequestMessage(HttpMethod.Post, new Uri(Constants.API_BASE_URL + Constants.SUP_POST_FOTOS_STREAMING))
            {
                Content = content
            };
            TextLoading = "Enviando supervision...";
            var resultEndpoint = await httpClient.SendAsync(request);
            if (resultEndpoint.IsSuccessStatusCode)
            {
                var responseContent = await resultEndpoint.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<List<ArchivoModel>>(responseContent);
                fotos.Add(video);
                return fotos;
            }
            else
            {
                Console.WriteLine($"Error: {resultEndpoint.StatusCode}");
                return new List<ArchivoModelNew>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}");
            return new List<ArchivoModelNew>();
        }
    }

    public async Task<bool> EnviarSupervision(SupervisionRequestDataModel supervision)
    {
        HttpClient cli = new HttpClient
        {
            BaseAddress = new Uri(Constants.API_BASE_URL + "SupervisionN")
        };
        cli.DefaultRequestHeaders.Add("Accept", "application/json");

        int result = 0;
        try
        {
            string json = JsonConvert.SerializeObject(supervision);
            StringContent stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage res = await cli.PostAsync("SupervisionN", stringContent);

            if (res.IsSuccessStatusCode)
            {
                result = JsonConvert.DeserializeObject<int>(res.Content.ReadAsStringAsync().Result);
            }
            else
            {
                string jsonString = await res.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<int>(res.Content.ReadAsStringAsync().Result);

                await App.Current.MainPage.DisplayAlert("", "Error al registrar supervisión", Constants.ACEPTAR);
                IsLoading = false;
            }
        }
        catch (Exception ex)
        {
            await App.Current.MainPage.DisplayAlert("", "Error al enviar supervision: " + ex.Message, Constants.ACEPTAR);
            //await Toast.Make($"Error al enviar supervision", ToastDuration.Short).Show();
            IsLoading = false;
            TextLoading = "";
        }


        //int res = await _httpHelper.PostBodyAsync<SupervisionRequestDataModel, int>(Constants.SUP_POST_ENVIAR_SUPERVISION, SupervisionRequestData);

        IsLoading = false;
        TextLoading = "";

        if (result > 0)
        {
            await App.Current.MainPage.DisplayAlert("", Constants.SUPERVISION_ENVIADA, Constants.ACEPTAR);
            return true;
        }
        else
        {
            await App.Current.MainPage.DisplayAlert("", Constants.ERROR_API + "\n\n" + result, Constants.ACEPTAR);
            return false;
        }
    }
    [RelayCommand]
    async Task SelectedOrden(SupervisionModel orden)
    {
        //IsLoading = true;
        //TextLoading = "Cargando supervisión...";
        //IsBusy = true;

        //TipoSucursal tipoInmueble = orden.Tipo == 0 ? (TipoSucursal)1 : (TipoSucursal)orden.Tipo;

        //SupervisionRequestDataModel supervisionRequestData = new SupervisionRequestDataModel
        //{
        //    IdOrden = orden.Orden,
        //    Id_Cliente = orden.IdCliente,
        //    Cliente = orden.Cliente,
        //    Id_Inmueble = orden.IdInmueble,
        //    Inmueble = orden.Inmueble,
        //    TipoSucursal = tipoInmueble,
        //    Fechaini = DateTime.Now,
        //    Usuario = UserSession.IdPersonal,
        //    Anio = FilterYear,
        //    Mes = _filterMonth
        //};

        //List<SeccionTipoSucursal> secciones = await SeccionTipoSucursal.ObtenerSeccionesPorTipoSucursal(tipoInmueble, supervisionRequestData.Id_Inmueble);

        //if (secciones.Count == 0)
        //{
        //    await Toast.Make(Constants.NO_EXISTEN_SECCIONES, ToastDuration.Short).Show();
        //    IsBusy = false;
        //    IsLoading = false;
        //    TextLoading = "";
        //    return;
        //}
        //Dictionary<string, object> data = new Dictionary<string, object>{
        //    { Constants.SUPERVISION_REQUEST_DATA_KEY, supervisionRequestData },
        //    { Constants.SECCIONES_KEY, secciones },
        //    { Constants.INDICE_KEY, 0 }
        //};

        //await Constants.GoToAsync(nameof(SupervisionMantenimientoPreguntasPage), data);
        //IsLoading = false;
        //TextLoading = "";
        //IsBusy = false;
    }

    [RelayCommand]
    async Task NewSupervision()
    {
        await Constants.GoToAsync(nameof(SupervisionMantenimientoInmueblePage));
    }

    async void InitValues()
    {
        _filterMonth = DateTime.Now.Month;
        SelectedMonth = Constants.GetMonthName(_filterMonth);

        FilterYear = DateTime.Now.Year;
        _yearList = new List<object>();
        for (int year = FilterYear - 1; year <= FilterYear + 1; year++)
        {
            _yearList.Add(year);
        }

        IsLoading = true;
        await LoadList();
        IsLoading = false;
    }

    async Task LoadList()
    {
        //ExisteProgramadas = false;
        //ExisteLocal = false;
        //if (!InternetUtil.IsConnectedInternet())
        //{
        //    _dbContext = new DbContext();
        //    _supervisiones = await _dbContext.GetSupervisionesLocal();
        //    if (_supervisiones != null && _supervisiones.Count > 0)
        //    {
        //        ExisteProgramadas = true;

        //    }
        //    else
        //    {
        //        ExisteProgramadas = false;
        //        OrdenSupervisionList = new ObservableCollection<SupervisionModel>();
                
        //    }
        //    RotarLista();
        //    OrdenSupervisionList = new ObservableCollection<SupervisionModel>(_supervisiones);
        //}
        //else
        //{
        //    string url = $"{Constants.SUP_GET_ORDENES_API}?idsupervisor={UserSession.IdEmpleado}&anio={FilterYear}&mes={_filterMonth}";

        //    _supervisiones = await _httpHelper.GetAsync<List<SupervisionModel>>(url);

        //    if (_supervisiones != null && _supervisiones.Count > 0)
        //    {
        //        ExisteProgramadas = true;
        //    }
        //    else
        //    {
        //        ExisteProgramadas = false;
        //        OrdenSupervisionList = new ObservableCollection<SupervisionModel>();
               
        //    }
        //    RotarLista();
        //    OrdenSupervisionList = new ObservableCollection<SupervisionModel>(_supervisiones);
        //}

        ////OBTENER SUPERVISIONES PENDIENTES DE ENVIO
        //_dbContext = new DbContext();
        //_supervisionesP = await _dbContext.GetSupervisionesSinEnviar();
        //if (_supervisionesP != null && _supervisionesP.Count > 0)
        //{
        //    ExisteLocal = true;
        //    OrdenSupervisionListP = new ObservableCollection<SupervisionLocal>(_supervisionesP);
        //}
        //else
        //{
        //    ExisteLocal = false;
        //}

        //FechaCarga = await _dbContext.GetUltimaCarga();
        //FechaCargaValida = FechaCarga != DateTime.MinValue;

        ////DESHABILITAR ORDENES EXISTENTES EN LOCAL
        //if(_supervisiones != null && _supervisionesP != null) {
        //    var ordenesLocales = _supervisionesP.Select(p => p.IdOrden).ToHashSet();

        //    foreach(var supervision in _supervisiones) {
        //        supervision.IsEnabled = true;
        //        if(ordenesLocales.Contains(supervision.Orden)) {
        //            supervision.IsEnabled = false;
        //        }
        //    }
        //}
    }

    bool CanExecute()
    {
        return !IsBusy;
    }

    void RotarLista()
    {
        int indice = 0;

        foreach (SupervisionModel orden in _supervisiones)
        {
            if (orden.Fecha.Date.Equals(DateTime.Now.Date))
            {
                indice = _supervisiones.IndexOf(orden);
                break;
            }
        }

        if (indice == 0) return;

        List<SupervisionModel> listAux = new List<SupervisionModel>();

        for (int i = 0; i < _supervisiones.Count(); i++)
        {
            listAux.Add(_supervisiones[indice++]);
            if (indice == _supervisiones.Count())
            {
                indice = 0;
            }
        }

        _supervisiones = new List<SupervisionModel>(listAux);
    }

    async Task LoadListOffline()
    {
        //var list = await _dbContext.GetSupervisionesSinEnviar();
        //if (list!= null && list.Count > 0) {
        //    ExisteLocal = true;
        //}
    }

    [RelayCommand]
    async Task PregargarDatosSupervision()
    {
        //if (!Utils.InternetUtil.IsConnectedInternet())
        //{
        //    await Toast.Make(Constants.ERROR_INTERNET, ToastDuration.Short).Show();
        //    return;
        //}
        //IniciarCarga("Descargando información");
        //HttpHelper _httpHelper = new HttpHelper();
        //_dbContext = new DbContext();
        ////OBTENER SUPERVISIONES PROGRAMADAS
        //string year = DateTime.Now.Year.ToString();
        //string month = DateTime.Now.Month.ToString();

        ////OBTENER CLIENTES
        //try
        //{
            
        //    List<ClientsModel> clientes = await _httpHelper.GetAsync<List<ClientsModel>>(Constants.GET_CLIENTES_API);
        //    if (clientes != null && clientes.Count > 0)
        //    {
        //        await _dbContext.InsertClientesLocal(clientes);
        //    }
        //}
        //catch (Exception ex)
        //{
        //    Console.WriteLine("Error al precargar ordenes de supervisión programadas: " + ex.Message.ToString());
        //    await Toast.Make(Constants.ERROR_API_GET, ToastDuration.Short).Show();
        //    DetenerCarga();
        //    return;
        //}

        ////OBTENER ESTADOS
        //try
        //{
             
        //    List<EstadoModel> estados = await _httpHelper.GetAsync<List<EstadoModel>>(Constants.GET_ESTADOS_API);
        //    if (estados != null && estados.Count > 0)
        //    {
        //        await _dbContext.InsertEstadosLocal(estados);
        //    }
        //}
        //catch (Exception ex)
        //{
        //    Console.WriteLine("Error al precargar estados: " + ex.Message.ToString());
        //    await Toast.Make(Constants.ERROR_API_GET, ToastDuration.Short).Show();
        //    DetenerCarga();
        //    return;
        //}

        // //OBTENER INMUEBLES
        //try
        //{
        //    List<InmuebleLocal> inmuebles = await _httpHelper.GetAsync<List<InmuebleLocal>>(Constants.GET_TOTAL_INMUEBLES_API);
        //    if (inmuebles != null && inmuebles.Count > 0)
        //    {
        //        await _dbContext.InsertInmueblesLocal(inmuebles);
        //    } 
        //}
        //catch (Exception ex)
        //{
        //    Console.WriteLine("Error al precargar inmuebles: " + ex.Message.ToString());
        //    await Toast.Make(Constants.ERROR_API_GET, ToastDuration.Short).Show();
        //    DetenerCarga();
        //    return;
        //}

        ////OBTENER SUPERVISIONES PROGRAMADAS
        //try {
        //    string url = $"{Constants.SUP_GET_ORDENES_API}?idsupervisor={UserSession.IdEmpleado}&anio={FilterYear}&mes={_filterMonth}";

        //    var supervisiones = await _httpHelper.GetAsync<List<SupervisionModel>>(url);

        //    if(supervisiones != null && supervisiones.Count > 0) {
        //        await _dbContext.InsertSupervisionProgramadaLocal(supervisiones);
        //    }
        //} catch(Exception ex) {
        //    Console.WriteLine("Error al precargar inmuebles: " + ex.Message.ToString());
        //    await Toast.Make(Constants.ERROR_API_GET, ToastDuration.Short).Show();
        //    DetenerCarga();
        //    return;
        //}

        //await _dbContext.InsertFechaCarga();
        //FechaCarga = DateTime.Now;
        //DetenerCarga();
        //await Toast.Make(Constants.DATOS_PRECARGADOS, ToastDuration.Short).Show();
    }

    public void IniciarCarga(string mensaje)
    {
        IsLoading = true;
        TextLoading = mensaje;
    }

    public void DetenerCarga()
    {
        IsLoading = false;
        TextLoading = "";
    }
}