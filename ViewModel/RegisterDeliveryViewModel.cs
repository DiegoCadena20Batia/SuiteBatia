using BatiaSuite.Data;
using BatiaSuite.Models;
using BatiaSuite.Models.Entregas;
using BatiaSuite.Models.EntidadesLocal.RutasEntregas; // Namespace donde reside RutasInmuebles y tu cola de pendientes
using BatiaSuite.Utils;
using BatiaSuite.Views;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using System.Text;
using System.Windows.Input;

namespace BatiaSuite.ViewModel;

public partial class RegisterDeliveryViewModel : BaseViewModel, IQueryAttributable {

    private readonly DrawingView _drawingView;
    private readonly LocalDbContext _dbContext; // Uso de tu contexto genérico universal

    // Eliminada la entidad intermedia; ahora usamos directamente la colección pura de la base de datos
    private ObservableCollection<RutasInmuebles> materiales;

    private ObservableCollection<PhotosModel> _photoPaths = new ObservableCollection<PhotosModel>();
    public ObservableCollection<PhotosModel> photoPaths {
        get { return _photoPaths; }
        set {
            _photoPaths = value;
            OnPropertyChanged();
        }
    }

    private string _NombreRecibe = string.Empty;
    private string _Comentarios = string.Empty;
    private int _Bidones = 0;
    private int _IdListado = 0;
    private int CountPhoto = 0;

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
        _dbContext = new LocalDbContext(); // Inicialización de tu nuevo contexto genérico
    }

    /// <summary>
    /// Recibe la información directa de navegación tipada como RutasInmuebles
    /// </summary>
    public void ApplyQueryAttributes(IDictionary<string, object> query) {
        materiales = (ObservableCollection<RutasInmuebles>)query["MaterialsList"];
        _NombreRecibe = query["NombreRecibe"].ToString();
        _Comentarios = query["Comentarios"].ToString();

        if(query["Bidones"] is null || query["Bidones"].ToString() == "")
            _Bidones = 0;
        else
            _Bidones = Convert.ToInt32(query["Bidones"]);

        _IdListado = (int)query["IdListado"];
    }

    private async Task ClearDrawingView() {
        _drawingView.Clear();
        PathFirmaLocal = null;
    }

    private async Task<bool> ValidarFirma() {
        try {
            // Se genera una ruta temporal en caché para las validaciones en caliente
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

    /// <summary>
    /// Guarda el registro en la cola de pendientes moviendo los archivos físicos a AppDataDirectory
    /// </summary>
    private async Task GuardarEnPendientesOffline() {
        try {
            var fotosPermanentes = new List<string>();

            // 1. Mover Fotos del Carrusel a AppDataDirectory (Seguro y Permanente)
            foreach(var item in photoPaths) {
                if(File.Exists(item.UrlPhoto)) {
                    string destinoFoto = Path.Combine(FileSystem.AppDataDirectory, Path.GetFileName(item.UrlPhoto));
                    File.Copy(item.UrlPhoto, destinoFoto, true);
                    fotosPermanentes.Add(destinoFoto);
                }
            }

            // 2. Mover la Firma a AppDataDirectory
            string destinoFirma = string.Empty;
            if(!string.IsNullOrEmpty(PathFirmaLocal) && File.Exists(PathFirmaLocal)) {
                destinoFirma = Path.Combine(FileSystem.AppDataDirectory, Path.GetFileName(PathFirmaLocal));
                File.Copy(PathFirmaLocal, destinoFirma, true);
            }

            // 3. Mapear Materiales (Usando las propiedades en Mayúsculas correspondientes a RutasInmuebles)
            var materialesConvertidos = materiales.Select(m => new RegisterMaterialsModel.Materiale {
                Entregado = m.Entregado,
                Cantidad = m.Cantidad,
                Clave = m.Clave
            }).ToArray();

            // 4. Armar el payload JSON idéntico anexando las rutas fijas del disco local
            var payloadCompleto = new {
                Usuario = UserSession.IdPersonal,
                NombreRecibe = _NombreRecibe,
                ComentarioMateriales = _Comentarios,
                Bidones = _Bidones,
                IdListado = _IdListado,
                Materiales = materialesConvertidos,
                Fentrega = DateTime.Now,
                RutasFotosLocales = fotosPermanentes,
                RutaFirmaLocal = destinoFirma
            };

            // 5. Instanciar tu objeto ISincronizable
            var pendiente = new RutaInmueblePendiente {
                JsonData = JsonConvert.SerializeObject(payloadCompleto),
                FechaCaptura = DateTime.Now
            };

            // 6. Guardar usando tu método genérico universal
            await _dbContext.GuardarLocalAsync(pendiente);

            // 7. Intentar reportar ubicación en local si falla red
            await ReportarUbicacionDeEntrega();

            // 8. ACTUALIZACIÓN LOCAL: Marcamos los materiales como entregados en la base de datos local
            var materialesLocal = await _dbContext.ObtenerListaLocalAsync<RutasInmuebles>(m => m.IdListado == _IdListado);
            if(materialesLocal != null && materialesLocal.Count > 0) {
                foreach(var material in materialesLocal) {
                    // Buscamos el valor capturado en la pantalla actual para este producto específico
                    var modificado = materiales.FirstOrDefault(x => x.Clave == material.Clave);
                    if(modificado != null) {
                        material.Entregado = modificado.Entregado; // Actualizamos con la cantidad real entregada
                        material.Estatusl = "Entregado";            // Modificamos su estatus si lo usas para tus estilos de celda
                    }

                    // Guardamos la actualización usando tu método genérico universal (hace un InsertOrReplace)
                    await _dbContext.GuardarLocalAsync(material);
                }
            }

            await DisplayAlert("Modo Offline", "Sin conexión a internet. La entrega se guardó en el dispositivo para un envío posterior automático.", "Ok");
            IsBusy = false;

            // 3. Regreso seguro al menú de sucursales
            await Shell.Current.Navigation.PopToRootAsync(false);
            await Shell.Current.GoToAsync(nameof(DeliveriesDetail), true);

            await Shell.Current.GoToAsync(nameof(DeliveriesDetail), true);
        } catch(Exception ex) {
            await DisplayAlert("Error", "Ocurrió un error al guardar localmente: " + ex.Message, "Cerrar");
            IsBusy = false;
        }
    }

    /// <summary>
    /// Método principal de ejecución híbrida (Online/Offline)
    /// </summary>
    private async Task RegisterMaterials() {
        try {
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

            // Validación de conectividad por Utilería
            if(!InternetUtil.IsConnectedInternet()) {
                await GuardarEnPendientesOffline();
            } else {
                // Modo Online: Intenta subir archivos físicos al API
                if(!await SendFiles()) {
                    await GuardarEnPendientesOffline();
                    return;
                }

                var materialesConvertidos = materiales.Select(m => new RegisterMaterialsModel.Materiale {
                    Entregado = m.Entregado,
                    Cantidad = m.Cantidad,
                    Clave = m.Clave
                }).ToArray();

                var data = new RegisterMaterialsModel {
                    Usuario = UserSession.IdPersonal,
                    NombreRecibe = _NombreRecibe,
                    ComentarioMateriales = _Comentarios,
                    Bidones = _Bidones,
                    IdListado = _IdListado,
                    Materiales = materialesConvertidos,
                    Fentrega = DateTime.Now
                };

                Uri RequestUri = new Uri(Constants.API_BASE_URL + "EntregaAppN");
                var client = new HttpClient();
                var json = JsonConvert.SerializeObject(data);
                var contentJson = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(RequestUri, contentJson);

                if(response.StatusCode == HttpStatusCode.OK) {

                    await _dbContext.BorrarPorPredicadoAsync<RutasInmuebles>(x => x.IdListado == _IdListado);
                    // 1. Reportamos la coordenada en tiempo real al servidor
                    await ReportarUbicacionDeEntrega();

                    await DisplayAlert("Mensaje", "Registrado correctamente", "Ok");
                    IsBusy = false;



                    // 3. Regreso seguro al menú de sucursales
                    await Shell.Current.Navigation.PopToRootAsync(false);
                    await Shell.Current.GoToAsync(nameof(DeliveriesDetail), true);
                } else {
                    await GuardarEnPendientesOffline();
                }
            }
        } catch(Exception ex) when(ex is HttpRequestException || ex is TaskCanceledException) {
            await GuardarEnPendientesOffline();
        }
    }

    public async Task<bool> ReportarUbicacionDeEntrega() {
        Location location = null;
        try {
            location = await Utils.LocationUtil.GetCurrentLocationAsync();
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
                    await _dbContext.GuardarLocalAsync(entrega); // Metodo Genérico
                }
                return false;
            }
            return true;
        } catch(Exception) {
            if(location != null) {
                var entrega = new EntregaReporteUbicacionLocal {
                    IdPersonal = UserSession.IdPersonal,
                    IdInmueble = UserSession.IdInmuebleTracking,
                    Latitud = location.Latitude.ToString(),
                    Longitud = location.Longitude.ToString(),
                    IdListado = 0,
                    IdTipo = 4,
                    Fecha = DateTime.Now
                };
                await _dbContext.GuardarLocalAsync(entrega); // Metodo Genérico
            }
            return false;
        }
    }

    private async Task Photo() {
        try {
            if(CountPhoto < 5) {
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
                    PathPhotoLocal = LocalFilePath;
                    CountPhoto++;
                }
            } else {
                await DisplayAlert("Mensaje", "Se ha alcanzado el número máximo de fotos permitidas", "Cerrar");
            }
        } catch(Exception ex) {
            await DisplayAlert("Error", ex.Message, "Cerrar");
        }
    }

    private List<string> archivos = new List<string>();

    public async Task<bool> SendFiles() {
        try {
            archivos.Clear();
            if(photoPaths != null) {
                foreach(var item in photoPaths) {
                    archivos.Add(item.UrlPhoto);
                }
            }
            if(PathFirmaLocal != null)
                archivos.Add(PathFirmaLocal);

            var UrlFiles = await UploadFiles(archivos, "Doctos");
            if(UrlFiles != null) {
                string[] splits = UrlFiles.Split("|");
                foreach(string split in splits) {
                    if(split.Contains(".png")) {
                        PathFirmaLocal = split;
                    } else if(split.Contains(".jpg") || split.Contains(".jpeg")) {
                        PathPhotoLocal = split;
                    }
                }
                var EndPath = PathFirmaLocal.TrimEnd('|');
                PathFirmaLocal = EndPath;
                return true;
            } else {
                return false;
            }
        } catch(Exception) {
            return false;
        }
    }

    public async Task<string> UploadFiles(List<string> files, string folderName) {
        try {
            HttpClient client = new HttpClient();
            var formData = new MultipartFormDataContent();

            foreach(var file in files) {
                byte[] fileBytes = await File.ReadAllBytesAsync(file);
                bool isSignature = Path.GetFileName(file).StartsWith("Firma", StringComparison.OrdinalIgnoreCase);

                byte[] resizedImage = await ImageResizerHelper.ResizeImage(
                    fileBytes,
                    480,
                    640,
                    !isSignature);

                var byteArrayContent = new ByteArrayContent(resizedImage);
                formData.Add(byteArrayContent, "files", Path.GetFileName(file));
            }

            var response = await client.PostAsync(Constants.API_BASE_URL + $"FilesEntregaApp/CargaMul?folio={_IdListado}", formData);

            if(response.IsSuccessStatusCode) {
                return await response.Content.ReadAsStringAsync();
            } else {
                return null;
            }
        } catch(Exception) {
            return null;
        }
    }
}