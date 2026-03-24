using BatiaSuite.Data;
using BatiaSuite.Models;
using BatiaSuite.Models.Entregas;
using BatiaSuite.Utils;
using BatiaSuite.Views;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Devices.Sensors;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using System.Text;
using System.Windows.Input;

namespace BatiaSuite.ViewModel;

public partial class RegisterDeliveryViewModel : BaseViewModel, IQueryAttributable {

    DrawingView _drawingView;
    DbContext _dbContext;

    ObservableCollection<ListadoMaterialesModel> materiales;

    private ObservableCollection<PhotosModel> _photoPaths = new ObservableCollection<PhotosModel>();
    public ObservableCollection<PhotosModel> photoPaths {
        get { return _photoPaths; }
        set {
            _photoPaths = value;
            OnPropertyChanged();
        }
    }

    string _NombreRecibe = string.Empty, _Comentarios = string.Empty;
    int _Bidones = 0, _IdListado = 0, CountPhoto = 0;
    public IMediaPicker mediaPicker;

    public ICommand RegisterCommand { get; set; }
    public ICommand PhotoCommand { get; set; }
    public ICommand DeletePhotoCommand { get; }
    public ICommand ClearDrawingCommand { get; }

    private bool _isSignature;

    public bool IsSignature {
        get { return _isSignature; }
        set { _isSignature = value; OnPropertyChanged(); }
    }

    private string _pathPhotoLocal;

    public string PathPhotoLocal {
        get { return _pathPhotoLocal; }
        set { _pathPhotoLocal = value; OnPropertyChanged(); }
    }

    private string _pathFirmaLocal;

    public string PathFirmaLocal {
        get { return _pathFirmaLocal; }
        set { _pathFirmaLocal = value; }
    }

    public RegisterDeliveryViewModel(DrawingView drawingView) {
        
        RegisterCommand = new Command(async () => await RegisterMaterials());
        PhotoCommand = new Command(async () => await Photo());
        ClearDrawingCommand = new Command(async () => await ClearDrawingView());
        DeletePhotoCommand = new Command<PhotosModel>((elemento) => {
            photoPaths.Remove(elemento);
            CountPhoto--;
        });
        _drawingView = drawingView;
        IsSignature = true;
        IsBusy = false;
        _dbContext = new DbContext();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query) {
        materiales = (ObservableCollection<ListadoMaterialesModel>)query["MaterialsList"];
        _NombreRecibe = query["NombreRecibe"].ToString();
        _Comentarios = query["Comentarios"].ToString();
        if(query["Bidones"] is null)
            _Bidones = 0;
        else if(query["Bidones"].ToString() == "")
            _Bidones = 0;
        else
            _Bidones = Convert.ToInt32(query["Bidones"]);
        _IdListado = (int)query["IdListado"];
    }

    private async Task ClearDrawingView() {
        _drawingView.Clear();
        PathFirmaLocal = null;
    }

    async Task<bool> ValidarFirma() {
        try {
            PathFirmaLocal = Path.Combine(FileSystem.CacheDirectory, $"Firma_{Guid.NewGuid()}.png");

            using(Stream stream = await _drawingView.GetImageStream(512, 512)) {
                using FileStream localFileStream = File.OpenWrite(PathFirmaLocal);
                await stream.CopyToAsync(localFileStream);
            }

            return true;
        } catch(Exception) {
            PathFirmaLocal = null;
            return false;
        }
    }
    async Task  GuardarLocal() {
        try {
            if(!await ValidarFirma()) {
                await DisplayAlert("Alerta", "Debe enviar Foto y Firma", "Cerrar");
                IsBusy = false;
                return;
            }
            if(photoPaths.Count == 0 || photoPaths == null || PathFirmaLocal == null) {
                await DisplayAlert("Alerta", "Debe enviar Foto y Firma", "Cerrar");
                IsBusy = false;
                return;
            }
            var materialesConvertidos = materiales.Select(m => new RegisterMaterialsModel.Materiale {
                Entregado = m.entregado,
                Cantidad = m.cantidad,
                Clave = m.clave,

            }).ToArray();

            //HEADER =>
            var entregaLocal = new EntregaLocal {
                Usuario = UserSession.IdPersonal,
                NombreRecibe = _NombreRecibe,
                ComentarioMateriales = _Comentarios,
                Bidones = _Bidones,
                IdListado = _IdListado,
                Fentrega = DateTime.Now,
            };
            //MATERIALES =>
            var entregaMaterialLocal = new List<EntregaMaterialLocal>();
            foreach(var mat in materialesConvertidos) {
                var material = new EntregaMaterialLocal {
                    Entregado = mat.Entregado,
                    Cantidad = mat.Cantidad,
                    Clave = mat.Clave,
                };
                entregaMaterialLocal.Add(material);
            }
            //ARCHIVOS => 
            //FOTO
            var fotosLocal = new List<FotoEntregaLocal>();
            if(photoPaths != null) {
                foreach(var item in photoPaths) {
                    var foto = new FotoEntregaLocal {
                        IdEntregaLocal = 0,
                        Path = item.UrlPhoto
                    };
                    fotosLocal.Add(foto);
                }
            }
            //FIRMA

            if(PathFirmaLocal != null) {
                var firma = new FotoEntregaLocal {
                    IdEntregaLocal = 0,
                    Path = PathFirmaLocal
                };
                fotosLocal.Add(firma);
            }
            await _dbContext.InsertarEntrega(entregaLocal, entregaMaterialLocal, fotosLocal);


            //SI TODO SALE BIEN ENTONCES SE REPORTA LA UBICACION DE ENTREGA, SE ELIMINA EL LISTADO DEL LOCAL

            await ReportarUbicacionDeEntrega();
            await _dbContext.EliminarListadoMaterialPrecarga(_IdListado);
            //DetenerCarga();
            await DisplayAlert("Mensaje", "Sin conexión a internet, la entrega se guardó en el dispositivo para un envío posterior", "Ok");
            IsBusy = false;

            // Limpiar la pila de navegación
            var pages = Shell.Current.Navigation.NavigationStack.ToList();
            Shell.Current.Navigation.RemovePage(pages[2]);
            Shell.Current.Navigation.RemovePage(pages[3]);
            Shell.Current.Navigation.RemovePage(pages[4]);

            await Shell.Current.GoToAsync(nameof(DeliveriesDetail), true);

            return;
        }
        catch(Exception ex) {
            await DisplayAlert("Error", "Ocurrió un error al guardar localmente: " + ex.Message, "Cerrar");
            return;
        }
    }
    //try {

    //    } 
    //    catch(Exception ex) when(ex is HttpRequestException || ex is TaskCanceledException) {
    //    await DisplayAlert("Error", "Ocurrió un error: " + ex.Message, "Cerrar");
    //    return;
    //}
    private async Task RegisterMaterials() {
        try {
            //SI NO TIENE INTERNET GUARDAR EN LOCAL DIRECTAMENTE

            //CONTINUAR NORMALMENTE SI HAY CONEXION A INTERNET

            IsBusy = true;
            if(!await ValidarFirma()) {
                await DisplayAlert("Alerta", "Debe enviar Foto y Firma", "Cerrar");
                IsBusy = false;
                return;
            }
            if(photoPaths.Count == 0 || photoPaths == null || PathFirmaLocal == null) {
                await DisplayAlert("Alerta", "Debe enviar Foto y Firma", "Cerrar");
                IsBusy = false;
                return;
            }
            if(!InternetUtil.IsConnectedInternet()) {
                await GuardarLocal();

            } else {
                //IniciaCarga("Enviando archivos...");
                // Crear una instancia del modelo RegisterMaterialsModel con los datos proporcionados
                if(!await SendFiles()) {
                    //await DisplayAlert("Error", "Ocurrió un error al subir los archivos", "Cerrar");
                    
                    //AQUI SE GUARDARA EL OBJETO EN EL LOCAL DEL DISPOSITIVO: RegisterMaterialsModel
                    await GuardarLocal();
                    IsBusy = false;
                    return;
                }
                //IniciaCarga("Enviando registro...");
                // Convertir los materiales de ListadoMaterialesModel a RegisterMaterialsModel.Materiale
                var materialesConvertidos = materiales.Select(m => new RegisterMaterialsModel.Materiale {
                    Entregado = m.entregado,
                    Cantidad = m.cantidad,
                    Clave = m.clave,

                }).ToArray();

                // Crear una instancia del modelo RegisterMaterialsModel con los datos proporcionados
                var data = new RegisterMaterialsModel {
                    Usuario = UserSession.IdPersonal,
                    NombreRecibe = _NombreRecibe,
                    ComentarioMateriales = _Comentarios,
                    Bidones = _Bidones,
                    IdListado = _IdListado,
                    Materiales = materialesConvertidos,
                    Fentrega = DateTime.Now,

                };

                Uri RequestUri = new Uri(Constants.API_BASE_URL + "EntregaAppN");
                var client = new HttpClient();
                var json = JsonConvert.SerializeObject(data);
                var contentJson = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(RequestUri, contentJson);

                if(response.StatusCode == HttpStatusCode.OK) {
                    await ReportarUbicacionDeEntrega();
                    //DetenerCarga();
                    await DisplayAlert("Mensaje", "Registrado correctamente", "Ok");
                    IsBusy = false;

                    // Limpiar la pila de navegación
                    var pages = Shell.Current.Navigation.NavigationStack.ToList();
                    Shell.Current.Navigation.RemovePage(pages[2]);
                    Shell.Current.Navigation.RemovePage(pages[3]);
                    Shell.Current.Navigation.RemovePage(pages[4]);
                    //string route = $"{nameof(DeliveriesDetail)}";
                    //await Constants.GoToAsync(route);
                    await Shell.Current.GoToAsync(nameof(DeliveriesDetail), true);

                    return;
                } else {
                    IsBusy = false;
                    await GuardarLocal();
                    return;
                    //await DisplayAlert("Error", "Ocurrió un error al registrar la información", "Cerrar");
                }
            }
        } 
        catch(Exception ex) when(ex is HttpRequestException || ex is TaskCanceledException) {
            //await DisplayAlert("Error", "Ocurrió un error: " + ex.Message, "Cerrar");
            await GuardarLocal();
            return;
        }
    }
    public async Task<bool> ReportarUbicacionDeEntrega() {
        Location location = null;
        try {
            location = await Utils.LocationUtil.GetCurrentLocationAsync();
            //VERIFICAR QUE PASA CON ESE METODO PPARA GUARDAR E REPORTE DE UBICACION EN LOCAL
             string url = Constants.API_BASE_URL + "SeguimientoRuta";
            var data = new {
                IdPersonal = UserSession.IdPersonal,
                IdInmueble = UserSession.IdInmuebleTracking,
                Latitud = location.Latitude,
                Longitud = location.Longitude,
                IdListado = _IdListado,
                IdTipo = 4,
                Fecha = DateTime.Now
            };

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var _httpClient = new HttpClient();
            var response = await _httpClient.PostAsync(url, content);
            if(!response.IsSuccessStatusCode) {
                //SI EL SERVIDOR NO ESTA DISPONIBLE-------------------------------------------->
                string errorBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error al enviar ubicación: {response.StatusCode} - {errorBody}");
                if(location != null) {
                    var entrega = new EntregaReporteUbicacionLocal {
                        IdPersonal = UserSession.IdPersonal,
                        IdInmueble = UserSession.IdInmuebleTracking,
                        Latitud = location.Latitude.ToString(),
                        Longitud = location.Longitude.ToString(),
                        IdListado = _IdListado,
                        IdTipo = 4,
                        Fecha = DateTime.Now
                    };
                    _dbContext = new DbContext();
                    await _dbContext.InsertarUbicacionesEntrega(entrega);
                }
                return false;
            } 
            return true;
        } catch(Exception ex) when(ex is HttpRequestException || ex is TaskCanceledException) {
            Console.WriteLine($"Sin conexión o timeout: {ex.Message}");
            if(location != null) // Validar que sí se haya obtenido antes del fallo
                {
                var entrega = new EntregaReporteUbicacionLocal {
                    IdPersonal = UserSession.IdPersonal,
                    IdInmueble = UserSession.IdInmuebleTracking,
                    Latitud = location.Latitude.ToString(),
                    Longitud = location.Longitude.ToString(),
                    IdListado = 0,
                    IdTipo = 4,
                    Fecha = DateTime.Now
                };
                _dbContext = new DbContext();
                await _dbContext.InsertarUbicacionesEntrega(entrega);
            }

            return false;
        }
    }

    private async Task Photo() {
        try {
            if(CountPhoto < 5) {
                if(this.mediaPicker.IsCaptureSupported) {
                    FileResult photo = await MediaPicker.CapturePhotoAsync();
                    if(photo != null) {
                        string LocalFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
                        using(Stream source = await photo.OpenReadAsync()) {
                            using FileStream localFile = File.OpenWrite(LocalFilePath);
                            await source.CopyToAsync(localFile);
                        }
                        PhotosModel photosModel = new PhotosModel();
                        photosModel.UrlPhoto = LocalFilePath;
                        _photoPaths.Add(photosModel);
                        //photoPaths.Add(photosModel);
                        PathPhotoLocal = LocalFilePath;
                        CountPhoto++;
                    }
                }
            } else {
                await DisplayAlert("Mensaje", "Se a alcanzado el número máximo de fotos permitidas", "Cerrar");
            }
        } catch(Exception ex) {
            await DisplayAlert("Error", ex.Message, "Cerrar");
        }
    }

    private List<string> archivos = new List<string>();

    public async Task<bool> SendFiles() {
        try {
            if(photoPaths != null) {
                foreach(var item in photoPaths) {
                    archivos.Add(item.UrlPhoto);
                }
            }
            //archivos.Add(PathPhotoLocal);
            if(PathFirmaLocal != null)
                archivos.Add(PathFirmaLocal);

            var UrlFiles = await UploadFiles(archivos, "Doctos");//ESTO DEVUELVE ASI: "a75cf246c74a4587b93b6a2d3a8fabb0.jpg|Firma_1cb30d0a-9181-4dad-bec8-acccbdbb753a.png"
            if (UrlFiles != null) {
                string[] splits = UrlFiles.Split("|");// AQUI DEBEMOS INCLUIR EL SIGNO "|" SIN ESPAICIOS  // SE SEPARAN LAS STRINGS POR |
                var PathFile = string.Empty;
                foreach(string split in splits) {
                    if(split.Contains(".png")) {
                        // Si la extensión es PDF, asigna a pathFile y rompe el bucle
                        PathFirmaLocal = split;
                    } else if(split.Contains(".jpg") || split.Contains(".jpeg")) {
                        // Si es una imagen (JPG, JPEG o PNG), asigna a pathPhoto y continúa el bucle
                        PathPhotoLocal = split;
                    }
                }
                var EndPath = PathFirmaLocal.TrimEnd('|');
                PathFirmaLocal = EndPath;
                return true;
            } else {
                return false;
            }
            

        } catch(Exception ex) when(ex is HttpRequestException || ex is TaskCanceledException) {
            await DisplayAlert("Error", "Ocurrió un error: " + ex.Message, "Cerrar");
            return false;
        }
        
    }

    public async Task<string> UploadFiles(List<string> files, string folderName) {
        try {
            HttpClient client = new HttpClient();
            var formData = new MultipartFormDataContent();

            foreach(var file in files) {
                // Leer la imagen original
                byte[] fileBytes = await File.ReadAllBytesAsync(file);

                // Determinar si es una firma (asumo que buscas en el nombre del archivo)
                bool isSignature = Path.GetFileName(file).StartsWith("Firma", StringComparison.OrdinalIgnoreCase);

                // Redimensionar la imagen
                byte[] resizedImage = await ImageResizerHelper.ResizeImage(
                    fileBytes,
                    480,
                    640,
                    !isSignature); // Invertir para posicionImagen si es necesario

                // Crear el contenido para subir
                var byteArrayContent = new ByteArrayContent(resizedImage);

                // Agregar al formulario con el nombre original del archivo
                formData.Add(byteArrayContent, "files", Path.GetFileName(file));
            }

            var response = await client.PostAsync(Constants.API_BASE_URL + $"FilesEntregaApp/CargaMul?folio={_IdListado}", formData);

            if(response.IsSuccessStatusCode) {
                return await response.Content.ReadAsStringAsync();
            } else {
                return null;
            }
        } catch(Exception ex) when(ex is HttpRequestException || ex is TaskCanceledException) {
            return null;
        }
        
    }
}