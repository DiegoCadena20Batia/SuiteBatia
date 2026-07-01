using BatiaSuite.Data;
using BatiaSuite.Models;
using BatiaSuite.Models.EntidadesLocal.RutasEntregas;
using BatiaSuite.Models.Entregas;
using BatiaSuite.Utils;
using BatiaSuite.Views;
using BatiaSuite.Views.RutasEntregas;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Devices.Sensors;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using System.Text;

namespace BatiaSuite.ViewModel {

    public partial class DeliveriesDetailViewModel : ViewModelBase, IQueryAttributable {

        [ObservableProperty]
        private bool _isDelivering;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _textLoading;

        [ObservableProperty]
        private string _origen;

        [ObservableProperty]
        private string _destino;

        [ObservableProperty]
        private string _rutaEntry;

        [ObservableProperty]
        private string _userName;

        // Cambiamos el nombre conceptual para reflejar que son Sucursales/Inmuebles de la Ruta
        [ObservableProperty]
        private ObservableCollection<RutasInmuebles> _listSucursales;

        [ObservableProperty]
        private bool _availableDeliveries;

        private readonly LocalDbContext _dbContext;
        public BackButtonBehavior BackButtonBehavior { get; set; }
        private readonly HttpHelper _httpHelper;

        public DeliveriesDetailViewModel(HttpHelper httpHelper) {
            _httpHelper = httpHelper;
            BackButtonBehavior = new BackButtonBehavior {
                Command = new Command(async () => {
                    await Shell.Current.GoToAsync("..");
                })
            };

            UserName = UserSession.NOMBRE;
            IsDelivering = UserSession.IsDelivering;
            _dbContext = new LocalDbContext();
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query) {
            // Validamos de forma segura si venimos de regreso o sin parámetros
            if(query == null || !query.ContainsKey("json")) {
                _ = CargarSucursalesDeRuta();
            } else {
                string content = query["json"]?.ToString() ?? string.Empty;
                if(!string.IsNullOrEmpty(content)) {
                    ListSucursales = JsonConvert.DeserializeObject<ObservableCollection<RutasInmuebles>>(content);
                }
            }

            RutaEntry = $"Ruta: {UserSession.RutaNameTracking}"; // Muestra la ruta actual
            AvailableDeliveries = ListSucursales != null && ListSucursales.Count > 0;
        }

        public async Task CargarSucursalesDeRuta() {
            try {
                IniciaCarga("Cargando sucursales...");
                await Task.Delay(500);
                var listaInmuebeles = await ObtenerSucursalesLocal();
                 DetenerCarga();
                if(listaInmuebeles.Any()) {
                    ListSucursales = new ObservableCollection<RutasInmuebles>(listaInmuebeles);
                } else {
                    if(InternetUtil.IsConnectedInternet()) {
                        int idRuta = UserSession.IdRutaTracking;
                        if(idRuta == 0) {
                            DetenerCarga();
                            return;
                        }

                        string urlEndpoint = $"RutasOperador?idoperador={UserSession.IdPersonal}&mes={UserSession.IdMesTracking}&anio={UserSession.IdAnioTracking}";

                        var todasLasFilas = await _httpHelper.GetAsync<List<RutasInmuebles>>(urlEndpoint);

                        if(todasLasFilas != null) {
                            var sucursalesUnicas = todasLasFilas
                                .Where(r => r.IdRuta == idRuta)
                                .GroupBy(r => r.IdInmueble)
                                .Select(g => g.First())
                                .ToList();

                            ListSucursales = new ObservableCollection<RutasInmuebles>(sucursalesUnicas);
                            AvailableDeliveries = ListSucursales.Count > 0;

                            DetenerCarga();
                        } else {
                            await ObtenerSucursalesLocal();
                        }
                    }
                }
            } catch(Exception ex) {
                DetenerCarga();
                await Shell.Current.DisplayAlert("Error", ex.Message, "ok");
            }
        }

        public async Task<List<RutasInmuebles>> ObtenerSucursalesLocal() {
            var todasLasFilasLocal = await _dbContext.ObtenerListaLocalAsync<RutasInmuebles>(r => r.IdRuta == UserSession.IdRutaTracking);

            if(todasLasFilasLocal != null && todasLasFilasLocal.Count > 0) {
                var sucursalesUnicasLocal = todasLasFilasLocal
                    .GroupBy(r => r.IdInmueble)
                    .Select(g => g.First())
                    .ToList();

                AvailableDeliveries = true;
                return sucursalesUnicasLocal;
            } else {
                AvailableDeliveries = false;
                return null;
            }
            DetenerCarga();
        }

        [RelayCommand]
        private async Task SucursalSelec(RutasInmuebles sucursal) {
            if(sucursal == null) return;

            try {
                IniciaCarga("Abriendo listados...");
                await Task.Delay(500);

                // 1. Guardamos los datos de tracking de la sucursal elegida en la sesión global
                UserSession.IdInmuebleTracking = sucursal.IdInmueble;
                UserSession.InmuebleNameTracking = sucursal.Ruta; // "CHOPO XOCHIMILCO"
                UserSession.InmuebleLatitudTracking = sucursal.Latitud;
                UserSession.InmuebleLongitudTracking = sucursal.Longitud;

                // 2. NUEVA NAVEGACIÓN: Avanzamos hacia la pantalla intermedia de selección de tipos
                await Shell.Current.GoToAsync(nameof(TiposListadoPage), true);

                DetenerCarga();
            } catch(Exception ex) {
                DetenerCarga();
                await Shell.Current.DisplayAlert("Error", ex.Message, "ok");
            }
        }

        public async Task<bool> ValidarRutaDisponible() {
            var ubicacionActual = await Utils.LocationUtil.GetCurrentLocationAsync();
            if(ubicacionActual != null) {
                Origen = $"{ubicacionActual.Latitude},{ubicacionActual.Longitude}";
            } else {
                DetenerCarga();
                await Shell.Current.DisplayAlert("Alerta", "No se pudo obtener la ubicación actual", "OK");
                return false;
            }

            // Buscamos las coordenadas de la primera sucursal pendiente para guiar al operador
            var proximaSucursal = ListSucursales?.FirstOrDefault();
            if(proximaSucursal == null || string.IsNullOrEmpty(proximaSucursal.Latitud) || string.IsNullOrEmpty(proximaSucursal.Longitud)) {
                DetenerCarga();
                await Shell.Current.DisplayAlert("Alerta", "No hay coordenadas disponibles para la siguiente sucursal", "OK");
                return false;
            } else {
                Destino = $"{proximaSucursal.Latitud},{proximaSucursal.Longitud}";
                return true;
            }
        }

        [RelayCommand]
        public async Task IniciarEntrega() {
            if(ListSucursales != null && ListSucursales.Count > 0) {
                IniciaCarga("Iniciando ruta...");
                await Task.Delay(500);
                IsDelivering = true;
                UserSession.IsDelivering = true;
                await ReportarUbicacion(3); // Código 3: Inicio de ruta
                DetenerCarga();
            } else {
                DetenerCarga();
                await App.Current.MainPage.DisplayAlert("Error", "No hay sucursales en esta ruta", "Cerrar");
            }
        }

        [RelayCommand]
        public async Task FinalizarEntrega() {
            IniciaCarga("Finalizando ruta...");
            await Task.Delay(500);
            IsDelivering = false;
            UserSession.IsDelivering = false;
            await ReportarUbicacion(5); // Código 5: Fin de ruta
            DetenerCarga();

            await App.Current.MainPage.DisplayAlert("Éxito", "Se ha concluido la ruta de entregas", "Ok");

            var pages = Shell.Current.Navigation.NavigationStack.ToList();
            if(pages.Count > 2) {
                Shell.Current.Navigation.RemovePage(pages[1]);
                Shell.Current.Navigation.RemovePage(pages[2]);
            }

            await Constants.GoToAsync(nameof(Deliveries));
        }

        public async Task<bool> ReportarUbicacion(int idTipo) {
            Location location = null;
            try {
                location = await Utils.LocationUtil.GetCurrentLocationAsync();
                string url = Constants.API_BASE_URL + "SeguimientoRuta";
                var data = new {
                    IdPersonal = UserSession.IdPersonal,
                    IdInmueble = UserSession.IdInmuebleTracking,
                    Latitud = location?.Latitude ?? 0,
                    Longitud = location?.Longitude ?? 0,
                    IdListado = 0,
                    IdTipo = idTipo
                };

                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var _httpClient = new HttpClient();
                var response = await _httpClient.PostAsync(url, content);

                if(!response.IsSuccessStatusCode) {
                    if(location != null) {
                        await GuardarUbicacionLocal(location, idTipo);
                    }
                    return false;
                }
                return true;
            } catch(Exception ex) when(ex is HttpRequestException || ex is TaskCanceledException) {
                if(location != null) {
                    await GuardarUbicacionLocal(location, idTipo);
                }
                return false;
            }
        }

        private async Task GuardarUbicacionLocal(Location location, int idTipo) {
            var entrega = new EntregaReporteUbicacionLocal {
                IdPersonal = UserSession.IdPersonal,
                IdInmueble = UserSession.IdInmuebleTracking,
                Latitud = location.Latitude.ToString(),
                Longitud = location.Longitude.ToString(),
                IdListado = 0,
                IdTipo = idTipo,
                Fecha = DateTime.Now
            };

            await _dbContext.GuardarLocalAsync(entrega);
        }

        [RelayCommand]
        public async Task AbrirGoogleMaps() {
            IniciaCarga("Iniciando Google Maps...");
            await Task.Delay(500);
            if(await ValidarRutaDisponible()) {
                string url = $"geo:0,0?q={Destino}";
                try {
                    await Launcher.Default.OpenAsync(new Uri(url));
                } catch(Exception ex) {
                    Console.WriteLine($"Error al abrir Google Maps nativo: {ex.Message}");
                }
            }
            DetenerCarga();
        }

        [RelayCommand]
        public async Task AbrirWaze() {
            IniciaCarga("Iniciando Waze...");
            await Task.Delay(500);
            if(await ValidarRutaDisponible()) {
                string wazeUrl = $"https://waze.com/ul?ll={Destino}&navigate=yes";
                try {
                    await Launcher.Default.OpenAsync(new Uri(wazeUrl));
                } catch(Exception ex) {
                    Console.WriteLine($"Error al abrir Waze: {ex.Message}");
                }
            }
            DetenerCarga();
        }

        public void IniciaCarga(string mensaje) {
            IsLoading = true;
            TextLoading = mensaje;
        }

        public void DetenerCarga() {
            IsLoading = false;
            TextLoading = "";
        }
    }
}