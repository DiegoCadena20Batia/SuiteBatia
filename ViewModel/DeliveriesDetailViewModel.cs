using BatiaSuite.Data;
using BatiaSuite.Models;
using BatiaSuite.Models.Entregas;
using BatiaSuite.Utils;
using BatiaSuite.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Devices.Sensors;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using System.Text;
using System.Windows.Input;


namespace BatiaSuite.ViewModel {
    public partial class DeliveriesDetailViewModel : ViewModelBase, IQueryAttributable
    {
        [ObservableProperty]
        bool _isDelivering;
        [ObservableProperty]
        bool _isLoading;

        [ObservableProperty]
        string _textLoading;

        [ObservableProperty]
        string origen;

        [ObservableProperty]
        string destino;
        [ObservableProperty]
        string clienteEntry;

        DbContext _dbContext;
        public BackButtonBehavior BackButtonBehavior { get; set; }

        private string _userName ;

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ListApp> listApps;

        public ObservableCollection<ListApp> ListApps
        {
            get { return listApps; }
            set { listApps = value; OnPropertyChanged(); }
        }
        public string Inmueble { get; set; }
        public string Cliente { get; set; }
        public ICommand CommandListadoSelec { get; set; }
        public DeliveriesDetailViewModel()
        {
            BackButtonBehavior = new BackButtonBehavior
            {
                Command = new Command(async () =>
                {
                    // Do something here
                    await Shell.Current.GoToAsync("..");
                })
            };
            UserName += UserSession.NOMBRE;
            CommandListadoSelec = new Command<ListApp>(async (l) => await ListadoSelec(l));
            IsDelivering = UserSession.IsDelivering;
            _dbContext = new DbContext();
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            //SI SE REDIRECCIONA DESDE UNA ENTREGA
            if (query.Count == 0) {
                //OBTENER LISTADOS CON LA INFORMACION GUARDADA EN SESIÓN
                RutaEnCurso();
                //SI YA NO HAY LISTADOS DISPONIBLES ENTONCES REGRESAR A LA SELECCION DE INMUEBLE 

            } else {
                string content = query["json"].ToString();
                ListApps = JsonConvert.DeserializeObject<ObservableCollection<ListApp>>(content);
                Inmueble = ListApps[0].inmueble;
                Cliente = query["clienteselected"].ToString();
            }
            ClienteEntry = UserSession.ClienteNameTracking + " - " + UserSession.InmuebleNameTracking;
        }
        public async Task RutaEnCurso() {
            try {
                if(!InternetUtil.IsConnectedInternet()) {
                    await ObtenerListadosLocal();
                } else {
                    IniciaCarga("Cargando...");
                    await Task.Delay(500);
                    var request = new HttpRequestMessage();
                    int idInmueble = UserSession.IdInmuebleTracking;
                    int idMes = UserSession.IdMesTracking;
                    int IdAnio = UserSession.IdAnioTracking;
                    if(idInmueble == 0 || idMes == 0 || IdAnio == 0) {
                        // el usuario aun no selecciona una ruta o acaba de actualizar su app
                        return;
                    }

                    request.RequestUri = new Uri(Constants.API_BASE_URL + $"ListadoApp?idinmueble={idInmueble}&anio={IdAnio}&mes={idMes}");

                    request.Method = HttpMethod.Get;

                    request.Headers.Add("Accept", "application/json");

                    var client = new HttpClient {
                        Timeout = TimeSpan.FromSeconds(10)
                    };

                    HttpResponseMessage response = await client.SendAsync(request);

                    if(response.StatusCode == HttpStatusCode.OK) {

                        string content = await response.Content.ReadAsStringAsync();
                        if(content != null) {
                            ListApps = JsonConvert.DeserializeObject<ObservableCollection<ListApp>>(content);
                            DetenerCarga();
                        }
                        if(content == "[]") {
                            await FinalizarEntrega();
                        }
                        DetenerCarga();
                    } else {
                        await ObtenerListadosLocal();
                    }
                }
                
            }
            catch(Exception ex) {
                DetenerCarga();
                await Shell.Current.DisplayAlert("Error", ex.Message, "ok");
            }

        }

        //OBTENER LISTADOS LOCAL
        public async Task ObtenerListadosLocal() {
            //Obtener listados restantes del local
            var entregasLocal = await _dbContext.ObtenerListadosEntregaPrecargaByIdInmueble(UserSession.IdInmuebleTracking);

            if(entregasLocal != null && entregasLocal.Count > 0) {
                //AUN HAY ENTREGAS DISPONIBLES, ASIGNAR AL MODELO PARA CONTINUAR
                ListApps = new ObservableCollection<ListApp>();
                foreach(var entrega in entregasLocal) {
                    ListApps.Add(new ListApp {
                        idlistado = entrega.IdListado,
                        inmueble = UserSession.InmuebleNameTracking,
                    });
                }
            } else {
                //SE TERMINARON LAS ENTREGAS, FINALIZAR RUTA
                await FinalizarEntrega();
            }

            DetenerCarga();
        }
        //metodo que reciba todos los datos del id que halla tocado el usuario
        private async Task ListadoSelec(ListApp listApp)//pasa como tipo de dato
        {
            try
            {
                IniciaCarga("Cargando listado");
                await Task.Delay(500);
                var idlistado = listApp.idlistado;

                Dictionary<string, object> data = new Dictionary<string, object>
                    {
                        { "idlist", idlistado },
                        {"inmueble",UserSession.InmuebleNameTracking },
                        {"clienteselected", UserSession.ClienteNameTracking }
                    };
                //var route = $"{nameof(ListadoMateriales)}";
                //await Shell.Current.GoToAsync(route, data);
                await Shell.Current.GoToAsync(nameof(ListadoMateriales), true, data);

                DetenerCarga();

                //await Shell.Current.GoToAsync($"/MyDeliveries/MyListaMaterales", true, data);
            }
            catch (Exception ex)
            {
                DetenerCarga();
                await Shell.Current.DisplayAlert("Error", ex.Message, "ok");
            }
            
        }
        public async Task<bool> ValidarRutaDisponible() {
            //VALIDAR UBICACION ACTUAL
            var ubicacionActual = await Utils.LocationUtil.GetCurrentLocationAsync();
            if(ubicacionActual != null) {
                Origen = ubicacionActual.Latitude + ", " + ubicacionActual.Longitude;
            } else {
                DetenerCarga();
                await Shell.Current.DisplayAlert("Alerta", "No se pudo obtener la ubicación actual", "OK");
                return false;
            }
            //VALIDAR SI ESTA SELECCIONADO UN INMUEBLE O NO
            //SI NO ESTA SELECCIONADO
            if(UserSession.InmuebleLatitudTracking == "" || UserSession.InmuebleLongitudTracking == "") {
                DetenerCarga();
                await Shell.Current.DisplayAlert("Alerta", "No se han registrado coordenadas para el inmueble seleccionado", "OK");
                return false;
            } else {
                Destino = UserSession.InmuebleLatitudTracking + ", " + UserSession.InmuebleLongitudTracking;
                return true;
            }

        }

        [RelayCommand]
        public async Task IniciarEntrega() {
            if(ListApps!= null && ListApps.Count > 0) {
                IniciaCarga("Iniciando entrega");
                await Task.Delay(500);
                IsDelivering = true;
                UserSession.IsDelivering = true;
                await ReportarUbicacion(3);
                DetenerCarga();
            } else {
                DetenerCarga();
                await App.Current.MainPage.DisplayAlert("Error", "No hay entregas disponibles", "Cerrar");
            }
        }

        [RelayCommand]
        public async Task FinalizarEntrega() {
            IniciaCarga("Finalizando entrega");
            await Task.Delay(500);
            IsDelivering = false;
            UserSession.IsDelivering = false;
            await ReportarUbicacion(5);
            DetenerCarga();

            await App.Current.MainPage.DisplayAlert("Alerta", "Se terminaron las entregas para el inmueble especificado", "Ok");
            var pages = Shell.Current.Navigation.NavigationStack.ToList();
            Shell.Current.Navigation.RemovePage(pages[1]);
            Shell.Current.Navigation.RemovePage(pages[2]);
            string route = $"{nameof(Deliveries)}";
            await Constants.GoToAsync(route);
            DetenerCarga();
            return;
        }

        public async Task<bool> ReportarUbicacion(int idTipo) {
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
                    IdTipo = idTipo
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
                            IdListado = 0,
                            IdTipo = idTipo,
                            Fecha = DateTime.Now
                        };
                        _dbContext = new DbContext();
                        await _dbContext.InsertarUbicacionesEntrega(entrega);
                    }
                    return false;
                }
                return true;
                //SI SE PRODUCION UN ERROR AL ENVIAR EL REGISTRO-------------------------------------------->
            } catch(Exception ex) when(ex is HttpRequestException || ex is TaskCanceledException) {
                Console.WriteLine($"Sin conexión o timeout: {ex.Message}");
                if(location != null) // 👈 Validar que sí se haya obtenido antes del fallo
                    {
                    var entrega = new EntregaReporteUbicacionLocal {
                        IdPersonal = UserSession.IdPersonal,
                        IdInmueble = UserSession.IdInmuebleTracking,
                        Latitud = location.Latitude.ToString(),
                        Longitud = location.Longitude.ToString(),
                        IdListado = 0,
                        IdTipo = idTipo,
                        Fecha = DateTime.Now
                    };
                    _dbContext = new DbContext();
                    await _dbContext.InsertarUbicacionesEntrega(entrega);
                }

                return false;
            }
        }

        [RelayCommand]
        public async Task AbrirGoogleMaps() {
            IniciaCarga("Iniciando Google Maps...");
            await Task.Delay(500);
            if(await ValidarRutaDisponible()) {
                string url = $"https://www.google.com/maps/dir/?api=1&origin={Origen}&destination={Destino}&travelmode=driving";

                try {
                    await Launcher.Default.OpenAsync(new Uri(url));
                    DetenerCarga();
                } catch(Exception ex) {
                    DetenerCarga();
                    Console.WriteLine($"Error al abrir Google Maps: {ex.Message}");
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
                    DetenerCarga();
                } catch(Exception ex) {
                    DetenerCarga();
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