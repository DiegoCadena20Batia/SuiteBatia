using BatiaSuite.Data;
using BatiaSuite.Models;
using BatiaSuite.Models.EntidadesLocal.RutasEntregas;
using BatiaSuite.Models.Entregas;
using BatiaSuite.Popups;
using BatiaSuite.Popups.RutasEntregas;
using BatiaSuite.Utils;
using BatiaSuite.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using Newtonsoft.Json;
using Shiny;
using Shiny.Locations;
using System.Collections.ObjectModel;
using System.Net;
using System.Text;
using System.Windows.Input;

namespace BatiaSuite.ViewModel {

    public partial class DeliveriesViewModel : ViewModelBase {

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private bool isTracking;

        [ObservableProperty]
        private bool enRuta;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _textLoading;

        // Contexto Genérico Universal
        private LocalDbContext _localDbContext;

        private string httpBaseUrl = Constants.API_BASE_URL;

        #region Picker: Ruta

        private ObservableCollection<RutasInmuebles> _rutas;

        public ObservableCollection<RutasInmuebles> Rutas {
            get { return _rutas; }
            set { _rutas = value; OnPropertyChanged(); }
        }

        private RutasInmuebles _idRutaSelected;

        public RutasInmuebles IdRutaSelected {
            get { return _idRutaSelected; }
            set {
                if(_idRutaSelected != value && value != null) {
                    _idRutaSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion Picker: Ruta

        #region Picker: Mes

        private MesModel _idMesSelected;

        public MesModel IdMesSelected {
            get { return _idMesSelected; }
            set {
                if(_idMesSelected != value && value != null) {
                    _idMesSelected = value;
                    OnPropertyChanged();

                    _ = ValidarYBuscarRutasAsync();
                }
            }
        }

        private ObservableCollection<MesModel> _mesList;

        public ObservableCollection<MesModel> MesList {
            get { return _mesList; }
            set { _mesList = value; OnPropertyChanged(); }
        }

        #endregion Picker: Mes

        #region Picker: Año

        private string _year = DateTime.Today.Year.ToString(); // Ahora es string

        public string Year {
            get { return _year; }
            set {
                if(_year != value) {
                    _year = value;
                    OnPropertyChanged();

                    // Disparar la validación con cada dígito que cambia
                    _ = ValidarYBuscarRutasAsync();
                }
            }
        }

        #endregion Picker: Año

        private int idPersonal = 0;
        public ICommand RegisterCommand { get; set; }
        private readonly IGpsManager _gpsManager;

        public DeliveriesViewModel(IGpsManager gpsManager) {
            _localDbContext = new LocalDbContext();
            RegisterCommand = new Command(async () => await Register());
            idPersonal = UserSession.IdPersonal;

            InicializarDatos();

            _gpsManager = gpsManager;
            IsTracking = UserSession.SeguimientoGps;
            UserSession.IsDelivering = false;

            DetenerCarga();
            ShowLocationUse();
            ConsultaryEnviarReportesUbicacionLocales();
        }

        private async void InicializarDatos() {
            GetMes();

            int mesActual = DateTime.Now.Month;

            if(MesList != null && MesList.Any()) {
                IdMesSelected = MesList.FirstOrDefault(x => x.idMes == mesActual);
            }

            string mesFormateado = IdMesSelected != null ? IdMesSelected.idMes.ToString("D2") : mesActual.ToString("D2");

            await GetRutas(mesFormateado, Year);
        }

        private async Task ValidarYBuscarRutasAsync() {
            if(string.IsNullOrWhiteSpace(Year) || Year.Length != 4) {
                LimpiarRutas();
                return;
            }

            if(IdMesSelected == null || IdMesSelected.idMes <= 0) {
                return;
            }

            string mesFormateado = IdMesSelected.idMes.ToString("D2");

            await GetRutas(mesFormateado, Year);
        }

        private async Task<List<RutasInmuebles>> ConfirmarRutasLocal() {
            try {
                var todasLasRutasGuardadas = await _localDbContext.ObtenerListaLocalAsync<RutasInmuebles>(x => true);

                if(todasLasRutasGuardadas.Count > 0) {
                    return todasLasRutasGuardadas;
                } else {
                    return new List<RutasInmuebles>();
                }
            } catch(Exception ex) {
                System.Diagnostics.Debug.WriteLine($"Error al obtener: {ex.Message}");
                return new List<RutasInmuebles>();
            }
        }

        private async Task GetRutas(string mes, string anio) {
            string mesFormateado = mes.PadLeft(2, '0');

            if(InternetUtil.IsConnectedInternet()) {
                try {
                    IniciaCarga("Cargando rutas...");

                    string urlEndpoint = $"{httpBaseUrl}RutasOperador?idoperador={UserSession.IdPersonal}&mes={mesFormateado}&anio={anio}";
                    var listaCompleta = await _httpHelper.GetAsync<List<RutasInmuebles>>(urlEndpoint);

                    if(listaCompleta != null && listaCompleta.Any()) {
                        //TODO: preguntar si es factible borrar datos de la tabla completa y guardar los nuevos datos, para evitar duplicados y mantener la información actualizada
                        //Borramos datos si los hay para evitar duplicados y mantener la información actualizada
                        await _localDbContext.BorrarTablaCompletaAsync<RutasInmuebles>();
                        // Guardamos las nuevas rutas traídas del servidor en SQLite
                        foreach(var ruta in listaCompleta) {
                            await _localDbContext.GuardarLocalAsync<RutasInmuebles>(ruta);
                        }

                        var datosFiltrados = listaCompleta.DistinctBy(x => x.IdRuta);
                        Rutas = new ObservableCollection<RutasInmuebles>(datosFiltrados);
                    } else {
                        LimpiarRutas();
                        await App.Current.MainPage.DisplayAlert("Aviso", "No se encontraron rutas asignadas para el mes y año seleccionados.", "OK");
                    }
                } catch(Exception ex) {
                    LimpiarRutas();
                    System.Diagnostics.Debug.WriteLine($"[GetRutas_Error] {ex.Message}");
                    await App.Current.MainPage.DisplayAlert("Error de Conexión", "No se pudo obtener la información. Intente de nuevo en un momento.", "OK");
                } finally {
                    DetenerCarga();
                }
            } else {
                LimpiarRutas();
                await App.Current.MainPage.DisplayAlert("Error", "No hay datos conexión, necesitas conexión a internet para ver las rutas.", "OK");
            }
        }

        private void LimpiarRutas() {
            Rutas = new ObservableCollection<RutasInmuebles>();
            IdRutaSelected = null;
        }

        private async Task GetMes() {
            IsBusy = true;
            var meses = new ObservableCollection<MesModel> {
                new MesModel { idMes = 1, mes = "Enero" },
                new MesModel { idMes = 2, mes = "Febrero" },
                new MesModel { idMes = 3, mes = "Marzo" },
                new MesModel { idMes = 4, mes = "Abril" },
                new MesModel { idMes = 5, mes = "Mayo" },
                new MesModel { idMes = 6, mes = "Junio" },
                new MesModel { idMes = 7, mes = "Julio" },
                new MesModel { idMes = 8, mes = "Agosto" },
                new MesModel { idMes = 9, mes = "Septiembre" },
                new MesModel { idMes = 10, mes = "Octubre" },
                new MesModel { idMes = 11, mes = "Noviembre" },
                new MesModel { idMes = 12, mes = "Diciembre" }
            };
            MesList = meses;
            IsBusy = false;
        }

        private async Task Register() {
            try {
                if(await ValidaCampos()) {
                    if(!IsTracking) {
                        DetenerCarga();

                        var popup = new ConfirmarInicioRutaPopup();
                        await MopupService.Instance.PushAsync(popup);

                        bool aceptarInicioRuta = await popup.PopupResult;

                        if(!aceptarInicioRuta) {
                            return;
                        }

                        await IniciarRuta();

                        if(!IsTracking) {
                            return;
                        }
                    }

                    IniciaCarga("Cargando...");
                    await Task.Delay(500);

                    // 1. Guardamos el periodo (Mes y Año) en la sesión global
                    UserSession.IdMesTracking = IdMesSelected.idMes;
                    UserSession.IdAnioTracking = int.Parse(Year);

                    // 2. Guardamos la información de la RUTA seleccionada en las nuevas propiedades
                    UserSession.IdRutaTracking = IdRutaSelected.IdRuta;
                    UserSession.RutaNameTracking = IdRutaSelected.Ruta;

                    // 3. Navegamos sin adjuntar un JSON pesado para forzar la recarga limpia por sesión
                    var navigationParameters = new Dictionary<string, object> {
                { "navigation_origin", "DeliveriesViewModel" }
            };

                    await Shell.Current.GoToAsync(nameof(DeliveriesDetail), true, navigationParameters);

                    DetenerCarga();
                    IsBusy = false;
                }
            } catch(Exception ex) {
                DetenerCarga();
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        private async Task<bool> ValidaCampos() {
            if(IdRutaSelected == null) {
                await App.Current.MainPage.DisplayAlert("Error", "Seleccione una ruta", "Ok");
                return false;
            }
            if(IdMesSelected == null || Year == null) {
                await App.Current.MainPage.DisplayAlert("Error", "Seleccione un mes y año", "Ok");
                return false;
            }
            return true;
        }

        #region Mecánica de Ubicación e Inicio/Fin de Ruta

        [RelayCommand]
        public async Task IniciarRuta() {
            if(IsTracking) {
                await Shell.Current.DisplayAlert("Ruta", "El rastreo ya se encuentra activo", "OK");
                return;
            }
            if(_gpsManager.IsListening()) {
                await _gpsManager.StopListener();
                IsTracking = false;
                EnRuta = false;
                UserSession.SeguimientoGps = false;
            }
            try {
                IniciaCarga("Iniciando GPS...");
                await Task.Delay(1000);
                var request0 = new GpsRequest {
                    BackgroundMode = GpsBackgroundMode.Realtime,
                    Accuracy = GpsAccuracy.Normal,
                    DistanceFilterMeters = 5000
                };

                var access = await _gpsManager.RequestAccess(request0);

                if(access == AccessState.Available) {
                    await ReportarUbicacionInicial();
                    await _gpsManager.StartListener(request0);
                    IsTracking = true;
                    UserSession.SeguimientoGps = true;
                    UserSession.IdInmuebleTracking = 0;
                    UserSession.IsDelivering = true;
                    DetenerCarga();
                } else {
                    DetenerCarga();
                    await Shell.Current.DisplayAlert("Error", "Se requieren permisos de ubicación", "OK");
                }
            } catch(Exception ex) {
                DetenerCarga();
                Console.WriteLine($"Error al iniciar GPS: {ex.Message}");
            }
        }

        [RelayCommand]
        public async Task FinalizarRuta() {
            try {
                if(_gpsManager.IsListening()) {
                    IniciaCarga("Apagando GPS...");
                    await Task.Delay(1000);
                    await _gpsManager.StopListener();
                    IsTracking = false;
                    EnRuta = false;
                    UserSession.SeguimientoGps = false;
                    UserSession.IdInmuebleTracking = 0;
                    UserSession.IdMesTracking = 0;
                    UserSession.IdAnioTracking = 0;
                    await ReportarUbicacionFinal();
                    DetenerCarga();
                    await Shell.Current.DisplayAlert("Alerta", "Ruta finalizada correctamente", "OK");
                    return;
                }
            } catch(Exception ex) {
                DetenerCarga();
                Console.WriteLine($"Error al detener GPS: {ex.Message}");
            }
        }

        public async Task<bool> ReportarUbicacionInicial() {
            Location location = null;
            try {
                location = await Utils.LocationUtil.GetCurrentLocationAsync();
                string url = Constants.API_BASE_URL + "SeguimientoRuta";
                var data = new {
                    IdPersonal = UserSession.IdPersonal,
                    IdInmueble = UserSession.IdInmuebleTracking,
                    Latitud = location.Latitude,
                    Longitud = location.Longitude,
                    IdListado = 0,
                    IdTipo = 1,
                    Fecha = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
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
                            IdListado = 0,
                            IdTipo = 1,
                            Fecha = DateTime.Now
                        };
                        // Uso del nuevo contexto local genérico universal para ISincronizable
                        await _localDbContext.GuardarLocalAsync(entrega);
                    }
                    return false;
                }
                return true;
            } catch(Exception ex) when(ex is HttpRequestException || ex is TaskCanceledException) {
                if(location != null) {
                    var entrega = new EntregaReporteUbicacionLocal {
                        IdPersonal = UserSession.IdPersonal,
                        IdInmueble = UserSession.IdInmuebleTracking,
                        Latitud = location.Latitude.ToString(),
                        Longitud = location.Longitude.ToString(),
                        IdListado = 0,
                        IdTipo = 1,
                        Fecha = DateTime.Now
                    };
                    await _localDbContext.GuardarLocalAsync(entrega);
                }
                return false;
            }
        }

        public async Task<bool> ReportarUbicacionFinal() {
            Location location = null;
            try {
                location = await Utils.LocationUtil.GetCurrentLocationAsync();
                string url = Constants.API_BASE_URL + "SeguimientoRuta";
                var data = new {
                    IdPersonal = UserSession.IdPersonal,
                    IdInmueble = UserSession.IdInmuebleTracking,
                    Latitud = location.Latitude,
                    Longitud = location.Longitude,
                    IdListado = 0,
                    IdTipo = 7,
                    Fecha = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
                };

                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var _httpClient = new HttpClient();
                var response = await _httpClient.PostAsync(url, content);
                return response.IsSuccessStatusCode;
            } catch(Exception ex) when(ex is HttpRequestException || ex is TaskCanceledException) {
                if(location != null) {
                    var entrega = new EntregaReporteUbicacionLocal {
                        IdPersonal = UserSession.IdPersonal,
                        IdInmueble = UserSession.IdInmuebleTracking,
                        Latitud = location.Latitude.ToString(),
                        Longitud = location.Longitude.ToString(),
                        IdListado = 0,
                        IdTipo = 7,
                        Fecha = DateTime.Now
                    };
                    await _localDbContext.GuardarLocalAsync(entrega);
                }
                return false;
            }
        }

        public async Task ConsultaryEnviarReportesUbicacionLocales() {
            if(InternetUtil.IsConnectedInternet()) {
                // Consultamos mediante la abstracción genérica universal ObtenerTodosLocalAsync
                var reportesLocales = await _localDbContext.ObtenerTodosLocalAsync<EntregaReporteUbicacionLocal>();
                if(reportesLocales != null && reportesLocales.Count > 0) {
                    foreach(var reporte in reportesLocales) {
                        bool exito = await EnviarReporteUbicacionLocal(reporte);
                        if(exito) {
                            await _localDbContext.BorrarLocalAsync(reporte);
                        }
                    }
                }
            }
        }

        public async Task<bool> EnviarReporteUbicacionLocal(EntregaReporteUbicacionLocal reporte) {
            try {
                string url = Constants.API_BASE_URL + "SeguimientoRuta";
                var json = JsonConvert.SerializeObject(reporte);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var _httpClient = new HttpClient();
                var response = await _httpClient.PostAsync(url, content);
                return response.IsSuccessStatusCode;
            } catch(Exception ex) when(ex is HttpRequestException || ex is TaskCanceledException) {
                return false;
            }
        }

        #endregion Mecánica de Ubicación e Inicio/Fin de Ruta

        public void IniciaCarga(string mensaje) {
            IsLoading = true;
            TextLoading = mensaje;
        }

        public void DetenerCarga() {
            IsLoading = false;
            TextLoading = "";
        }

        private async Task ShowLocationUse() {
            if(UserSession.ShowAcceptTracking == false) {
                var popup = new LocationUse();
                await MopupService.Instance.PushAsync(popup);
            }
        }
    }
}