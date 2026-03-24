using BatiaSuite.Converters;
using BatiaSuite.Data;
using BatiaSuite.Models;
using BatiaSuite.Models.Entregas;
using BatiaSuite.Popups;
using BatiaSuite.Utils;
using BatiaSuite.Views;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Devices.Sensors;
using Mopups.Services;
using Newtonsoft.Json;
using Shiny;
using Shiny.Locations;
using System.Collections.ObjectModel;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Windows.Input;

namespace BatiaSuite.ViewModel {
    public partial class DeliveriesViewModel : ViewModelBase {
        [ObservableProperty]
        bool _isBusy;

        [ObservableProperty]
        bool isTracking;

        [ObservableProperty]
        bool enRuta;

        [ObservableProperty]
        bool availableDeliveries;

        [ObservableProperty]
        List<ListApp> entregasDisponibles;

        [ObservableProperty]
        string inmuebleText;

        [ObservableProperty]
        bool _isLoading;

        [ObservableProperty]
        string _textLoading;

        [ObservableProperty]
        string origen;

        [ObservableProperty]
        string destino;

        [ObservableProperty]
        bool _isRefreshing;
        DbContext _dbContextg;

        [ObservableProperty]
        DateTime _fechaCarga;

        [ObservableProperty]
        bool _fechaCargaValida;

        [ObservableProperty]
        bool _existeProgramadas;
        [ObservableProperty]
        bool _existeLocal;

        List<EntregaLocal> _entregasLocal;

        [ObservableProperty]
        ObservableCollection<EntregaLocal> _entregasLocalList;


        #region Cliente
        private ObservableCollection<ClientsModel> _clients;

        DbContext _dbContext;

        // Declaración de una propiedad pública llamada 'Clients' que encapsula la colección observable privada '_clients'.
        public ObservableCollection<ClientsModel> Clients {
            get { return _clients; } // Obtener la colección '_clients'.
            set { _clients = value; OnPropertyChanged(); } // Asignar valor a '_clients' y notificar que la propiedad 'Clients' ha cambiado.
        }
        private ClientsModel _idClientSelected;

        // Declaración de una propiedad pública llamada 'IdInmubleSelected' que encapsula la propiedad privada '_idInmubleSelected'.
        public ClientsModel IdClientSelected {
            get { return _idClientSelected; } // Obtener el valor de '_idInmubleSelected'.
            set {
                // Comprobar si el valor nuevo es diferente del valor actual y no es nulo.
                if(_idClientSelected != value && value != null) {
                    AvailableDeliveries = false;
                    // Asignar el valor nuevo a '_idInmubleSelected'.
                    _idClientSelected = value;

                    // Notificar que la propiedad 'IdInmubleSelected' ha cambiado.
                    OnPropertyChanged();
                    GetEstado();
                    // Llamar al método 'GetInmuebleByIdClient' y pasar el ID del cliente seleccionado.
                    GetInmuebleByIdClient();
                }
            }
        }

        public BackButtonBehavior BackButtonBehavior { get; }


        #endregion

        #region Estado

        private ObservableCollection<EstadoModel> _estadoList;

        public ObservableCollection<EstadoModel> EstadoList {
            get { return _estadoList; }
            set { _estadoList = value; OnPropertyChanged(); }
        }

        // Declaración de una propiedad privada llamada '_idInmubleSelected' del tipo 'InmuebleByIdClienteModel.InmuebleModel'.
        private EstadoModel _idEstadoSelected;

        // Declaración de una propiedad pública llamada 'IdInmubleSelected' que encapsula la propiedad privada '_idInmubleSelected'.
        public EstadoModel IdEstadoSelected {
            get { return _idEstadoSelected; } // Obtener el valor de '_idInmubleSelected'.
            set {
                // Comprobar si el valor nuevo es diferente del valor actual y no es nulo.
                if(_idEstadoSelected != value && value != null) {
                    AvailableDeliveries = false;
                    // Asignar el valor nuevo a '_idInmubleSelected'.
                    _idEstadoSelected = value;

                    // Notificar que la propiedad 'IdInmubleSelected' ha cambiado.
                    OnPropertyChanged();
                    // Llamar al método 'GetInmuebleByIdClient' y pasar el ID del cliente seleccionado.
                    GetInmuebleByIdClient();
                }
            }
        }
        #endregion

        #region Inmueble
        private ObservableCollection<InmuebleByIdClienteModel.InmuebleModel> _inmueble;

        public ObservableCollection<InmuebleByIdClienteModel.InmuebleModel> Inmueble {
            get { return _inmueble; }
            set { _inmueble = value; OnPropertyChanged(); }
        }

        private InmuebleByIdClienteModel.InmuebleModel _idInmubleSelected;

        // Declaración de una propiedad pública llamada 'IdInmubleSelected' que encapsula la propiedad privada '_idInmubleSelected'.
        public InmuebleByIdClienteModel.InmuebleModel IdInmubleSelected {
            get { return _idInmubleSelected; } // Obtener el valor de '_idInmubleSelected'.
            set {
                // Comprobar si el valor nuevo es diferente del valor actual y no es nulo.
                if(_idInmubleSelected != value && value != null) {
                    AvailableDeliveries = false;
                    // Asignar el valor nuevo a '_idInmubleSelected'.
                    _idInmubleSelected = value;

                    // Notificar que la propiedad 'IdInmubleSelected' ha cambiado.
                    OnPropertyChanged();

                    //GetInmuebleByIdClient();
                }
            }
        }
        #endregion

        #region Mes
        private MesModel _idMesSelected;

        // Declaración de una propiedad pública llamada 'IdInmubleSelected' que encapsula la propiedad privada '_idInmubleSelected'.
        public MesModel IdMesSelected {
            get { return _idMesSelected; } // Obtener el valor de '_idInmubleSelected'.
            set {
                // Comprobar si el valor nuevo es diferente del valor actual y no es nulo.
                if(_idMesSelected != value && value != null) {
                    // Asignar el valor nuevo a '_idInmubleSelected'.
                    _idMesSelected = value;

                    // Notificar que la propiedad 'IdInmubleSelected' ha cambiado.
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<MesModel> _mesList;

        public ObservableCollection<MesModel> MesList {
            get { return _mesList; }
            set { _mesList = value; OnPropertyChanged(); }
        }
        #endregion

        #region Year

        private int _year = DateTime.Today.Year;

        public int Year {
            get { return _year; }
            set { _year = value; OnPropertyChanged(); }
        }

        #endregion

        int idPersonal = 0;

        public ICommand RegisterCommand { get; set; }



        private readonly IGpsManager _gpsManager;

        public DeliveriesViewModel(IGpsManager gpsManager) {
            _dbContext = new DbContext();
            RegisterCommand = new Command(async () => await Register());
            idPersonal = UserSession.IdPersonal;
            GetClients();
            //GetEstado();
            GetMes();
            _gpsManager = gpsManager;
            IsTracking = UserSession.SeguimientoGps;
            UserSession.IsDelivering = false;

            //SI TIENE ID significa que esta en ruta seleccionada
            //if (UserSession.IdInmuebleTracking != 0) {
            //    RutaEnCurso();
            //}
            DetenerCarga();
            ShowLocationUse();
            CargarListadosLocal();
            ConsultaryEnviarReportesUbicacionLocales();
        }

        [RelayCommand]
        public async Task IniciarRuta() {

            if(IsTracking) {
                await Shell.Current.DisplayAlert("Ruta", "El rastreo ya se encuentra activo", "OK");
                return;
            }
            //DETENER SI EXISTE UN GPS ACTIVO
            if(_gpsManager.IsListening()) {
                await _gpsManager.StopListener();
                IsTracking = false;
                EnRuta = false;
                AvailableDeliveries = false;
                EntregasDisponibles = new List<ListApp>();
                InmuebleText = "";
                UserSession.SeguimientoGps = false;
            }
            try {
                IniciaCarga("Iniciando GPS...");
                await Task.Delay(1000);
                var request0 = new GpsRequest {
                    BackgroundMode = GpsBackgroundMode.Realtime,
                    Accuracy = GpsAccuracy.Normal,

                    DistanceFilterMeters = 5000// Solo actualiza si se mueve más de 5000 metros
                };

                var access = await _gpsManager.RequestAccess(request0);

                if(access == AccessState.Available) {
                    await ReportarUbicacionInicial();
                    await _gpsManager.StartListener(request0);
                    IsTracking = true;
                    AvailableDeliveries = false;
                    UserSession.SeguimientoGps = true;
                    UserSession.IdInmuebleTracking = 0;
                    UserSession.IsDelivering = false;

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
                    AvailableDeliveries = false;
                    AvailableDeliveries = false;
                    EntregasDisponibles = new List<ListApp>();
                    InmuebleText = "";
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




        //BOTON QUE REESTABLEZCA VALORES DE BUSQUEDA 

        private async Task GetEstado() {
            try {
                if(!InternetUtil.IsConnectedInternet()) {
                    await GetEstadoLocal();
                } else {
                    // Crear una solicitud HTTP.
                    var request = new HttpRequestMessage();

                    // Establecer la URL de la solicitud con el ID de cliente proporcionado.
                    request.RequestUri = new Uri(Constants.API_BASE_URL + $"EstadoEntrega?idcliente={IdClientSelected.idCliente}&idpersonal={idPersonal}");

                    // Establecer el método de la solicitud como GET.
                    request.Method = HttpMethod.Get;

                    // Agregar un encabezado "Accept" para indicar que se acepta JSON como respuesta.
                    request.Headers.Add("Accept", "application/json");

                    // Crear una nueva instancia de HttpClient.
                    var client = new HttpClient();

                    // Enviar la solicitud HTTP y esperar la respuesta.
                    HttpResponseMessage response = await client.SendAsync(request);

                    // Verificar si la respuesta tiene un estado OK (código 200).
                    if(response.StatusCode == HttpStatusCode.OK) {
                        // Leer el contenido de la respuesta como una cadena.
                        string content = await response.Content.ReadAsStringAsync();

                        // Deserializar el contenido JSON en una colección observable de inmuebles.
                        var data = JsonConvert.DeserializeObject<ObservableCollection<EstadoModel>>(content);

                        // Asignar la colección de inmuebles a la propiedad 'Inmueble'.
                        EstadoList = data;
                        IsBusy = false;

                    } else {
                        await GetEstadoLocal();
                    }
                }
            }
            catch(Exception ex) when(ex is HttpRequestException || ex is TaskCanceledException) {
                await GetEstadoLocal();
            }
        }

        public async Task GetEstadoLocal() {
            IsBusy = true;
            var estados = new ObservableCollection<EstadoModel>
            {
                new EstadoModel { id_estado =1, descripcion ="Aguascalientes" },
                new EstadoModel { id_estado =2, descripcion ="Baja California" },
                new EstadoModel { id_estado =3, descripcion ="Baja California sur" },
                new EstadoModel { id_estado =4, descripcion ="Campeche" },
                new EstadoModel { id_estado =5, descripcion ="Chiapas" },
                new EstadoModel { id_estado =6, descripcion ="Chihuahua" },
                new EstadoModel { id_estado =7, descripcion ="Ciudad de Mexico" },
                new EstadoModel { id_estado =8, descripcion ="Coahuila" },
                new EstadoModel { id_estado =9, descripcion ="Colima" },
                new EstadoModel { id_estado =10, descripcion ="Durango" },
                new EstadoModel { id_estado =11, descripcion ="Estado de México" },
                new EstadoModel { id_estado =12, descripcion ="Guanajuato" },
                new EstadoModel { id_estado =13, descripcion ="Guerrero" },
                new EstadoModel { id_estado =14, descripcion ="Hidalgo" },
                new EstadoModel { id_estado =15, descripcion ="Jalisco" },
                new EstadoModel { id_estado =16, descripcion ="Michoacán" },
                new EstadoModel { id_estado =17, descripcion ="Morelos" },
                new EstadoModel { id_estado =18, descripcion ="Nuevo León" },
                new EstadoModel { id_estado =19, descripcion ="Nayarit" },
                new EstadoModel { id_estado =20, descripcion ="Oaxaca" },
                new EstadoModel { id_estado =21, descripcion ="Puebla" },
                new EstadoModel { id_estado =22, descripcion ="Querétaro" },
                new EstadoModel { id_estado =23, descripcion ="Quintana Roo" },
                new EstadoModel { id_estado =24, descripcion ="San Luis Potosí" },
                new EstadoModel { id_estado =25, descripcion ="Sinaloa" },
                new EstadoModel { id_estado =26, descripcion ="Sonora" },
                new EstadoModel { id_estado =27, descripcion ="Tabasco" },
                new EstadoModel { id_estado =28, descripcion ="Tamaulipas" },
                new EstadoModel { id_estado =29, descripcion ="Tlaxcala" },
                new EstadoModel { id_estado =30, descripcion ="Veracruz" },
                new EstadoModel { id_estado =31, descripcion ="Yucatán" },
                new EstadoModel { id_estado =32, descripcion ="Zacatecas" }
            };
            EstadoList = estados;
            IsBusy = false;
        }

        private async Task GetInmuebleByIdClient() {
            AvailableDeliveries = false;
            IsBusy = true;
            if(!InternetUtil.IsConnectedInternet()) {
                await GetinmuebleByIdClientLocal();
                IsBusy = false;
            } else {
                var request = new HttpRequestMessage();

                if(IdEstadoSelected == null) {
                    request.RequestUri = new Uri(Constants.API_BASE_URL + $"InmuebleEntrega?idpersonal={idPersonal}&idcliente={IdClientSelected.idCliente}&idestado={0}");
                } else {

                    request.RequestUri = new Uri(Constants.API_BASE_URL + $"InmuebleEntrega?idpersonal={idPersonal}&idcliente={IdClientSelected.idCliente}&idestado={IdEstadoSelected.id_estado}");
                }
                request.Method = HttpMethod.Get;
                request.Headers.Add("Accept", "application/json");
                var client = new HttpClient();
                HttpResponseMessage response = await client.SendAsync(request);

                if(response.StatusCode == HttpStatusCode.OK) {
                    string content = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<ObservableCollection<InmuebleByIdClienteModel.InmuebleModel>>(content);
                    Inmueble = data;
                    IsBusy = false;
                } else {
                    await GetinmuebleByIdClientLocal();
                    IsBusy = false;
                }
            }
        }

        public async Task GetinmuebleByIdClientLocal() {
            //OBTENER CLIENTES DE LOCAL
            var localInmuebles = await _dbContext.ObtenerInmueblesEntregaPrecargaByIdCliente(IdClientSelected.idCliente);
            if(localInmuebles != null && localInmuebles.Count > 0) {
                Inmueble = new ObservableCollection<InmuebleByIdClienteModel.InmuebleModel>(
                localInmuebles.Select(c => new InmuebleByIdClienteModel.InmuebleModel {
                    idInmueble = c.IdInmueble,
                    nombre = c.Nombre,
                    latitud = c.Latitud,
                    longitud = c.Longitud
                })
             );
            }
        }

        private async Task GetClients() {

            if(!InternetUtil.IsConnectedInternet()) {
                await GetClientsLocal();
            } else {
                var request = new HttpRequestMessage();
                request.RequestUri = new Uri(Constants.API_BASE_URL + $"ClienteEntrega?idusuario={idPersonal}");
                request.Method = HttpMethod.Get;
                request.Headers.Add("Accept", "application/json");
                var client = new HttpClient();
                HttpResponseMessage response = await client.SendAsync(request);

                if(response.StatusCode == HttpStatusCode.OK) {
                    string content = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<ObservableCollection<ClientsModel>>(content);
                    Clients = data;
                } else {
                    await GetClientsLocal();
                }
            }
        }

        public async Task GetClientsLocal() {
            //OBTENER CLIENTES DE LOCAL
            var localClients = await _dbContext.ObtenerClientesEntregaPrecarga();
            if(localClients != null && localClients.Count > 0) {
                Clients = new ObservableCollection<ClientsModel>(
                localClients.Select(c => new ClientsModel {
                    idCliente = c.IdCliente,
                    nombre = c.Nombre
                })
             );
            }
        }

        private async Task GetMes() {
            IsBusy = true;

            var meses = new ObservableCollection<MesModel>
            {
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
            //// Crear una solicitud HTTP.
            //var request = new HttpRequestMessage();

            //// Establecer la URL de la solicitud con el ID de cliente proporcionado.
            //request.RequestUri = new Uri($"https://www.singa.com.mx:5500/api/Mes");

            //// Establecer el método de la solicitud como GET.
            //request.Method = HttpMethod.Get;

            //// Agregar un encabezado "Accept" para indicar que se acepta JSON como respuesta.
            //request.Headers.Add("Accept", "application/json");

            //// Crear una nueva instancia de HttpClient.
            //var client = new HttpClient();

            //// Enviar la solicitud HTTP y esperar la respuesta.
            //HttpResponseMessage response = await client.SendAsync(request);

            //// Verificar si la respuesta tiene un estado OK (código 200).
            //if(response.StatusCode == HttpStatusCode.OK) {
            //    // Leer el contenido de la respuesta como una cadena.
            //    string content = ""

            //    // Deserializar el contenido JSON en una colección observable de inmuebles.
            //    var data = JsonConvert.DeserializeObject<ObservableCollection<MesModel>>(content);

            //    // Asignar la colección de inmuebles a la propiedad 'Inmueble'.
            //    MesList = data;
            //    IsBusy = false;

            //}
        }

        private async Task Register() {
            try {
                if(await ValidaCampos()) {
                    if(!InternetUtil.IsConnectedInternet()) {
                        //OBTENER LISTADOS DE LOCAL
                        IniciaCarga("Cargando...");
                        var localListados = await _dbContext.ObtenerListadosEntregaPrecargaByIdInmueble(IdInmubleSelected.idInmueble);
                        if(localListados != null && localListados.Count > 0) {
                            EntregasDisponibles = new List<ListApp>(
                            localListados.Select(c => new ListApp {
                                idlistado = c.IdListado,
                                inmueble = c.IdInmueble.ToString()
                            })
                         );
                        }
                        if(localListados == null || localListados.Count == 0) {
                            AvailableDeliveries = false;
                            DetenerCarga();
                            await App.Current.MainPage.DisplayAlert("Alerta", "No hay entregas disponibles para el inmueble especificado", "Ok");
                            return;
                        }

                        string content = JsonConvert.SerializeObject(localListados);

                        UserSession.IdInmuebleTracking = IdInmubleSelected.idInmueble;
                        UserSession.IdMesTracking = IdMesSelected.idMes;
                        UserSession.IdAnioTracking = Year;
                        UserSession.InmuebleNameTracking = IdInmubleSelected.nombre;
                        UserSession.ClienteNameTracking = IdClientSelected.nombre;
                        UserSession.IsDelivering = false;

                        if(IdInmubleSelected != null) {
                            if(IdInmubleSelected.latitud != null && IdInmubleSelected.longitud != null) {
                                UserSession.InmuebleLatitudTracking = IdInmubleSelected.latitud;
                                UserSession.InmuebleLongitudTracking = IdInmubleSelected.longitud;
                            } else {
                                UserSession.InmuebleLatitudTracking = "";
                                UserSession.InmuebleLongitudTracking = "";
                            }
                        }
                        AvailableDeliveries = true;

                        InmuebleText = IdClientSelected.nombre + " - " + IdInmubleSelected.nombre;
                        UserSession.InmuebleTracking = InmuebleText;
                        Dictionary<string, object> data = new Dictionary<string, object>
                        {
                        { "json", content },
                        {"clienteselected", _idClientSelected.nombre }
                    };
                        //var route = $"{nameof(DeliveriesDetail)}";
                        //await Shell.Current.GoToAsync(route, data);   
                        await Shell.Current.GoToAsync(nameof(DeliveriesDetail), true, data);

                        // Asignar la colección de inmuebles a la propiedad 'Inmueble'.
                        //MesList = data;
                        DetenerCarga();
                        IsBusy = false;




                    } else {
                        IniciaCarga("Cargando...");
                        await Task.Delay(1000);
                        // Crear una solicitud HTTP. 
                        var request = new HttpRequestMessage();

                        // Establecer la URL de la solicitud con el ID de cliente proporcionado.
                        request.RequestUri = new Uri(Constants.API_BASE_URL + $"ListadoApp?idinmueble={IdInmubleSelected.idInmueble}&anio={Year}&mes={IdMesSelected.idMes}");

                        // Establecer el método de la solicitud como GET.
                        request.Method = HttpMethod.Get;

                        // Agregar un encabezado "Accept" para indicar que se acepta JSON como respuesta.
                        request.Headers.Add("Accept", "application/json");

                        // Crear una nueva instancia de HttpClient.
                        var client = new HttpClient();

                        // Enviar la solicitud HTTP y esperar la respuesta.
                        HttpResponseMessage response = await client.SendAsync(request);

                        // Verificar si la respuesta tiene un estado OK (código 200).
                        if(response.StatusCode == HttpStatusCode.OK) {
                            // Leer el contenido de la respuesta como una cadena.

                            string content = await response.Content.ReadAsStringAsync();
                            if(content != null) {
                                EntregasDisponibles = JsonConvert.DeserializeObject<List<ListApp>>(content);
                            }

                            if(content == "[]") {
                                AvailableDeliveries = false;
                                DetenerCarga();
                                await App.Current.MainPage.DisplayAlert("Alerta", "No hay entregas disponibles para el inmueble especificado", "Ok");
                                return;
                            }
                            UserSession.IdInmuebleTracking = IdInmubleSelected.idInmueble;
                            UserSession.IdMesTracking = IdMesSelected.idMes;
                            UserSession.IdAnioTracking = Year;
                            UserSession.InmuebleNameTracking = IdInmubleSelected.nombre;
                            UserSession.ClienteNameTracking = IdClientSelected.nombre;
                            UserSession.IsDelivering = false;

                            if(IdInmubleSelected != null) {
                                if(IdInmubleSelected.latitud != null && IdInmubleSelected.longitud != null) {
                                    UserSession.InmuebleLatitudTracking = IdInmubleSelected.latitud;
                                    UserSession.InmuebleLongitudTracking = IdInmubleSelected.longitud;
                                } else {
                                    UserSession.InmuebleLatitudTracking = "";
                                    UserSession.InmuebleLongitudTracking = "";
                                }
                            }
                            AvailableDeliveries = true;

                            InmuebleText = IdClientSelected.nombre + " - " + IdInmubleSelected.nombre;
                            UserSession.InmuebleTracking = InmuebleText;
                            //await Toast.Make($"{Constants.NUMERO_MAXIMO} {content.}", ToastDuration.Short).Show();

                            // Deserializar el contenido JSON en una colección observable de inmuebles.

                            //Dictionary<string, object> data = new Dictionary<string, object>
                            //{
                            //{ "json", content },
                            //{"clienteselected", _idClientSelected.nombre }
                            //};
                            //var route = $"{nameof(DeliveriesRoute)}";
                            //await Shell.Current.GoToAsync(route, data);

                            Dictionary<string, object> data = new Dictionary<string, object>
                            {
                        { "json", content },
                        {"clienteselected", _idClientSelected.nombre }
                    };
                            //var route = $"{nameof(DeliveriesDetail)}";
                            //await Shell.Current.GoToAsync(route, data);
                            await Shell.Current.GoToAsync(nameof(DeliveriesDetail), true, data);

                            // Asignar la colección de inmuebles a la propiedad 'Inmueble'.
                            //MesList = data;
                            DetenerCarga();
                            IsBusy = false;


                        } else {
                            DetenerCarga();
                            await App.Current.MainPage.DisplayAlert("Error", "Ocurrio un error al consultar los listados", "Ok");
                        }
                    }
                }
            } catch(Exception ex) {
                DetenerCarga();
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "Ok");
            }

        }

        private async Task<bool> ValidaCampos() {
            bool esValido = true;

            if(IdInmubleSelected == null) {
                await App.Current.MainPage.DisplayAlert("Error", "Seleccione un inmueble", "Ok");
                return false;
            }
            //if(IdInmubleSelected != null) {
            //    if(IdInmubleSelected.latitud == null || IdInmubleSelected.latitud == "" || IdInmubleSelected.longitud == null || IdInmubleSelected.longitud == "") {
            //        await App.Current.MainPage.DisplayAlert("Aviso", "La sucursal seleccionada no cuenta con coordenadas registradas, verifique con su encargado", "Ok");
            //        //return false;
            //    }
            //}

            if(IdMesSelected == null) {
                await App.Current.MainPage.DisplayAlert("Error", "Seleccione un mes", "Ok");
                return false;
            }
            return esValido;
        }

        async Task LoadListOffline() {
            var list = await _dbContextg.GetSupervisionesSinEnviar();
            if(list != null && list.Count > 0) {
                ExisteLocal = true;
            }
        }

        [RelayCommand]
        private async Task PrecargarDatosEntregas() {
            try {
                if(IdMesSelected == null) {
                    await Toast.Make("Primero seleccione un mes", ToastDuration.Short).Show();
                    return;
                }
                IsLoading = true;
                IsBusy = true;
                TextLoading = "Descargando información...";

                var request = new HttpRequestMessage();

                request.RequestUri = new Uri(Constants.API_BASE_URL + Constants.ENT_GET_PRECARGA_API + $"?idpersonal={UserSession.IdPersonal}" + $"&mesapp={IdMesSelected.idMes}" + $"&anioapp={Year}");

                request.Method = HttpMethod.Get;

                request.Headers.Add("Accept", "application/json");

                var client = new HttpClient();

                HttpResponseMessage response = await client.SendAsync(request);

                if(response.StatusCode == HttpStatusCode.OK) {
                    string content = await response.Content.ReadAsStringAsync();

                    var settings = new JsonSerializerSettings {
                        FloatParseHandling = FloatParseHandling.Double,
                        Converters = new List<JsonConverter> { new FloatToIntConverter() }
                    };

                    var data = JsonConvert.DeserializeObject<EntregaPrecarga>(content, settings);
                    //MENSAJE DEBUG

                    //INSERTAR EN LOCAL DEL CEL
                    if(data != null) {
                        TextLoading = "Guardando en el dispositivo...";
                        await _dbContext.InsertPrecargaEntregasLocal(data);
                        await _dbContext.InsertFechaCargaEntrega();
                        FechaCarga = DateTime.Now;
                        FechaCargaValida = true;
                    }
                } else {
                    IsLoading = false;
                    IsBusy = false;
                    TextLoading = "";
                    await Toast.Make("Ocurrió un error al descargar la información.", ToastDuration.Short).Show();
                }
                IsLoading = false;
                IsBusy = false;
                TextLoading = "";
                await Toast.Make("Precarga exitosa", ToastDuration.Short).Show();

                //CLIENTES
                //List<ClientsModel> clientes = await _httpHelper.GetAsync<List<ClientsModel>>(Constants.GET_CLIENTES_API);
                //if(clientes != null && clientes.Count > 0) {
                //    await _dbContext.InsertClientesLocal(clientes);
                //}
            } catch(Exception ex) {
                Console.WriteLine("Error: " + ex.Message.ToString());
                await App.Current.MainPage.DisplayAlert("Error", ex.Message.ToString(), "Ok");
                DetenerCarga();
                IsLoading = false;
                return;
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
                    //SI EL SERVIDOR NO ESTA DISPONIBLE----------------------=>
                    string errorBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error al enviar ubicación: {response.StatusCode} - {errorBody}");
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
                        _dbContext = new DbContext();
                        await _dbContext.InsertarUbicacionesEntrega(entrega);
                    }
                    return false;
                }
                return true;
                //SI SE PRODUCION UN ERROR AL ENVIAR EL REGISTRO----------------------=>
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
                        IdTipo = 1,
                        Fecha = DateTime.Now
                    };
                    _dbContext = new DbContext();
                    await _dbContext.InsertarUbicacionesEntrega(entrega);
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
                if(!response.IsSuccessStatusCode) {
                    string errorBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error al enviar ubicación: {response.StatusCode}");
                    return false;
                }
                return true;
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
                        IdTipo = 7,
                        Fecha = DateTime.Now
                    };
                    _dbContext = new DbContext();
                    await _dbContext.InsertarUbicacionesEntrega(entrega);
                }

                return false;
            }
        }

        public async Task ConsultaryEnviarReportesUbicacionLocales() {
            if(InternetUtil.IsConnectedInternet()) {
                _dbContext = new DbContext();
                var reportesLocales = await _dbContext.ObtenerReportesUbicacionLocales();
                if(reportesLocales != null && reportesLocales.Count > 0) {
                    foreach(var reporte in reportesLocales) {
                        bool exito = await EnviarReporteUbicacionLocal(reporte);
                        if(exito) {
                            await _dbContext.EliminarReportesUbicacionLocal(reporte.IdLocal);
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
                if(!response.IsSuccessStatusCode) {
                    string errorBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error al enviar ubicación local: {response.StatusCode}");
                    return false;
                }
                return true;
            } catch(Exception ex) when(ex is HttpRequestException || ex is TaskCanceledException) {
                Console.WriteLine($"Error al enviar ubicación local: {ex.Message}");
                return false;
            }
        }

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

        #region Envio de entrega local masivo
        [RelayCommand]
        public async Task EnviarTodoLocal() {
            if(!InternetUtil.IsConnectedInternet()) {
                await Toast.Make("Sin conexión a internet", ToastDuration.Short).Show();
                return;
            }

            if(EntregasLocalList == null || EntregasLocalList.Count == 0) {
                await Toast.Make("No hay entregas pendientes", ToastDuration.Short).Show();
                return;
            }

            IsLoading = true;

            int total = EntregasLocalList.Count;
            int actual = 1;

            foreach(var entrega in EntregasLocalList.ToList()) // copiar lista para evitar problemas
            {
                TextLoading = $"Enviando {actual} de {total}...";

                var ok = await SelectedEntregaLocalMasivo(entrega);

                if(!ok) {
                    // Si una falla NO detiene todo, tú decides
                    await Toast.Make($"Error enviando {entrega.IdLocal}", ToastDuration.Short).Show();
                }

                actual++;
            }
            await CargarListadosLocal();
            TextLoading = "";
            IsLoading = false;

            await Toast.Make("Envió masivo finalizado", ToastDuration.Short).Show();
        }
        [RelayCommand]
        public async Task<bool> SelectedEntregaLocalMasivo(EntregaLocal entrega) {
            try {
                if(!InternetUtil.IsConnectedInternet()) {
                    await Toast.Make("Sin conexión a internet, verifique", ToastDuration.Short).Show();
                    return false;
                } else {
                    IsLoading = true;
                    
                    //CONSULTAR DATOS DE LA ENTREGA EN LOCAL
                    var entregaLocal = await _dbContext.ObtenerEntregaLocalParaEnvio(entrega.IdLocal);
                    archivos.Clear();
                    //LLENAR ARREGLO DE ARCHIVOS
                    if(entregaLocal != null && entregaLocal.Archivos != null && entregaLocal.Archivos.Count > 0) {
                        foreach(var archivo in entregaLocal.Archivos) {
                            archivos.Add(archivo.Path);
                        }
                    }
                    
                    //ENVIAR LOS ARCHIVOS
                    if(!await SendFiles(entrega.IdListado)) {
                        await Toast.Make("Ocurrió un error al subir los archivos", ToastDuration.Short).Show();
                        IsBusy = false;
                        
                        IsLoading = false;
                        return false;
                    }
                    //PREPARAR LOS MATERIALES EN EL FORMATO CORRECTO
                    if(entregaLocal != null && entregaLocal.Materiales != null && entregaLocal.Materiales.Count > 0) {
                        var materialesConvertidos = entregaLocal.Materiales.Select(m => new RegisterMaterialsModel.Materiale {
                            Entregado = m.Entregado,
                            Cantidad = m.Cantidad,
                            Clave = m.Clave,
                        }).ToArray();

                        //ESTRUCTURAR MODELO PARA ENVIO
                        var data = new RegisterMaterialsModel {
                            Usuario = UserSession.IdPersonal,
                            NombreRecibe = entregaLocal.Header.NombreRecibe,
                            ComentarioMateriales = entregaLocal.Header.ComentarioMateriales,
                            Bidones = entregaLocal.Header.Bidones,
                            IdListado = entregaLocal.Header.IdListado,
                            Materiales = materialesConvertidos,
                            Fentrega = entregaLocal.Header.Fentrega,
                        };
                        //REALIZAR ENVIO
                        Uri RequestUri = new Uri(Constants.API_BASE_URL + "EntregaAppNOffline");
                        var client = new HttpClient();
                        var json = JsonConvert.SerializeObject(data);
                        var contentJson = new StringContent(json, Encoding.UTF8, "application/json");
                        var response = await client.PostAsync(RequestUri, contentJson);
                        //VERIFICAR RESPUESTA
                        if(response.StatusCode == HttpStatusCode.OK) {
                            
                            //SE ENVIO CORRECTAMENTE ENTONCES ELIMINAR DEL LOCAL EN RAMA
                            await _dbContext.EliminarEntregaLocalEnviada(entrega.IdLocal);

                            await Toast.Make("Entrega enviada correctamente", ToastDuration.Short).Show();

                            //PENDIENTE
                            //FINALMENTE RECARGAR LISTADOS LOCALES Y ENVIAR REPORTES DE UBICACION LOCALES
                            await CargarListadosLocal();
                            await ConsultaryEnviarReportesUbicacionLocales();
                            
                            IsLoading = false;
                            return true;
                        } else {
                            //OCURRIO UN ERROR, ALERTAR
                            await Toast.Make("Ocurrió un error al enviar la entrega", ToastDuration.Short).Show();
                            IsBusy = false;
                            return false;
                        }
                    }
                }
                return true;
            } catch(Exception ex) {
                Console.WriteLine($"Error al seleccionar entrega local: {ex.Message}");
                return false;
            }

        }
        #endregion

        #region Envio de emtrega local
        [RelayCommand]
        public async Task CargarListadosLocal() {
            _dbContext = new DbContext();
            _entregasLocal = await _dbContext.ObtenerEntregasLocal();
            if(_entregasLocal != null && _entregasLocal.Count > 0) {
                ExisteLocal = true;
                EntregasLocalList = new ObservableCollection<EntregaLocal>(_entregasLocal);

            } else {
                ExisteLocal = false;
                EntregasLocalList = new ObservableCollection<EntregaLocal>();
            }

            FechaCarga = await _dbContext.GetUltimaCargaEntregas();
            FechaCargaValida = FechaCarga != DateTime.MinValue;
        }

        private List<string> archivos = new List<string>();

        [RelayCommand]
        public async Task<bool> SelectedEntregaLocal(EntregaLocal entrega) {
            try {
                if(!InternetUtil.IsConnectedInternet()) {
                    await Toast.Make("Sin conexión a internet, verifique", ToastDuration.Short).Show();
                    return false;
                } else {
                    IsLoading = true;
                    TextLoading = "Obteniendo entrega...";
                    //CONSULTAR DATOS DE LA ENTREGA EN LOCAL
                    var entregaLocal = await _dbContext.ObtenerEntregaLocalParaEnvio(entrega.IdLocal);
                    archivos.Clear();
                    //LLENAR ARREGLO DE ARCHIVOS
                    if(entregaLocal != null && entregaLocal.Archivos != null && entregaLocal.Archivos.Count > 0) {
                        foreach(var archivo in entregaLocal.Archivos) {
                            archivos.Add(archivo.Path);
                        }
                    }
                    TextLoading = "Enviando archivos...";
                    //ENVIAR LOS ARCHIVOS
                    if(!await SendFiles(entrega.IdListado)) {
                        await Toast.Make("Ocurrió un error al subir los archivos", ToastDuration.Short).Show();
                        IsBusy = false;
                        TextLoading = "";
                        IsLoading = false;
                        return false;
                    }
                    //PREPARAR LOS MATERIALES EN EL FORMATO CORRECTO
                    if(entregaLocal != null && entregaLocal.Materiales != null && entregaLocal.Materiales.Count > 0) {
                        var materialesConvertidos = entregaLocal.Materiales.Select(m => new RegisterMaterialsModel.Materiale {
                            Entregado = m.Entregado,
                            Cantidad = m.Cantidad,
                            Clave = m.Clave,
                        }).ToArray();

                        //ESTRUCTURAR MODELO PARA ENVIO
                        var data = new RegisterMaterialsModel {
                            Usuario = UserSession.IdPersonal,
                            NombreRecibe = entregaLocal.Header.NombreRecibe,
                            ComentarioMateriales = entregaLocal.Header.ComentarioMateriales,
                            Bidones = entregaLocal.Header.Bidones,
                            IdListado = entregaLocal.Header.IdListado,
                            Materiales = materialesConvertidos,
                            Fentrega = entregaLocal.Header.Fentrega,
                        };
                        //REALIZAR ENVIO
                        TextLoading = "Enviando registro...";
                        Uri RequestUri = new Uri(Constants.API_BASE_URL + "EntregaAppNOffline");
                        var client = new HttpClient();
                        var json = JsonConvert.SerializeObject(data);
                        var contentJson = new StringContent(json, Encoding.UTF8, "application/json");

                        var response = await client.PostAsync(RequestUri, contentJson);
                        //VERIFICAR RESPUESTA
                        if(response.StatusCode == HttpStatusCode.OK) {
                            TextLoading = "Envío exitoso, elimando entrega guardada...";
                            //SE ENVIO CORRECTAMENTE ENTONCES ELIMINAR DEL LOCAL EN RAMA
                            await _dbContext.EliminarEntregaLocalEnviada(entrega.IdLocal);
                            
                            await Toast.Make("Entrega enviada correctamente", ToastDuration.Short).Show();

                            //PENDIENTE
                            //FINALMENTE RECARGAR LISTADOS LOCALES Y ENVIAR REPORTES DE UBICACION LOCALES
                            await CargarListadosLocal();
                            await ConsultaryEnviarReportesUbicacionLocales();
                            TextLoading = "";
                            IsLoading = false;
                            return true;
                        } else {
                            //OCURRIO UN ERROR, ALERTAR
                            await Toast.Make("Ocurrió un error al enviar la entrega", ToastDuration.Short).Show();
                            IsBusy = false;
                            IsLoading = false;
                            return false;
                        }
                    }
                }
                return true;
            } catch(Exception ex) {
                Console.WriteLine($"Error al seleccionar entrega local: {ex.Message}");
                return false;
            }

        }

        public async Task<bool> SendFiles(int idListado) {
            try {
                var UrlFiles = await UploadFiles(archivos, "Doctos", idListado);
                return true;

            } catch(Exception ex) {
                Console.WriteLine($"Error al enviar archivos: {ex.Message}");
                return false;
            }
        }

        public async Task<string> UploadFiles(List<string> files, string folderName, int idListado) {
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

            var response = await client.PostAsync(Constants.API_BASE_URL + $"FilesEntregaApp/CargaMul?folio={idListado}", formData);

            if(response.IsSuccessStatusCode) {
                return await response.Content.ReadAsStringAsync();
            } else {
                //await DisplayAlert("Error", $"La solicitud al API falló con el código {response.StatusCode}", "Cerrar");
                return null;
            }
        }
        #endregion
    }
}
