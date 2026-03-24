using BatiaSuite.Models;
using BatiaSuite.Utils;
using BatiaSuite.Views;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using System.Text.Json;
using Shiny;
using Shiny.Locations;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows.Input;
using BatiaSuite.Models.Entregas;

namespace BatiaSuite.ViewModel {
    public partial class DeliveriesRouteViewModel : ViewModelBase {
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

        #region Cliente
        private ObservableCollection<ClientsModel> _clients;

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

        public DeliveriesRouteViewModel(IGpsManager gpsManager) {
            //RegisterCommand = new Command(async () => await Register());
            idPersonal = UserSession.IdPersonal;
            GetClients();
            //GetEstado();
            GetMes();
            _gpsManager = gpsManager;
            IsTracking = UserSession.SeguimientoGps;
            //SI TIENE ID significa que esta en ruta seleccionada
            if (UserSession.IdInmuebleTracking != 0) {
                RutaEnCurso();
            }
            DetenerCarga();
        }
        public async Task RutaEnCurso() {
            IniciaCarga("Cargando...");
            await Task.Delay(500);
            var request = new HttpRequestMessage();
            int idInmueble = UserSession.IdInmuebleTracking;
            int idMes = UserSession.IdMesTracking;
            int IdAnio = UserSession.IdAnioTracking;
            if (idInmueble == 0 || idMes == 0 || IdAnio == 0) {
                // el usuario aun no selecciona una ruta o acaba de actualizar su app
                return;
            }

            request.RequestUri = new Uri(Constants.API_BASE_URL + $"ListadoApp?idinmueble={idInmueble}&anio={IdAnio}&mes={idMes}");

            request.Method = HttpMethod.Get;

            request.Headers.Add("Accept", "application/json");

            var client = new HttpClient();

            HttpResponseMessage response = await client.SendAsync(request);

            if(response.StatusCode == HttpStatusCode.OK) {

                string content = await response.Content.ReadAsStringAsync();
                if(content != null) {
                    InmuebleText = UserSession.InmuebleTracking;
                    AvailableDeliveries = true;
                    EntregasDisponibles = JsonConvert.DeserializeObject<List<ListApp>>(content);
                }

                if(content == "[]") {
                    AvailableDeliveries = false;
                    DetenerCarga();
                    await App.Current.MainPage.DisplayAlert("Alerta", "Se terminaron las entregas para el inmueble especificado", "Ok");
                    return;
                }



                Dictionary<string, object> data = new Dictionary<string, object>
                {
                        { "json", content },
                        {"clienteselected", _idClientSelected.nombre }
                        };
                //var route = $"{nameof(DeliveriesDetail)}";
                //await Shell.Current.GoToAsync(route, data);

                // Asignar la colección de inmuebles a la propiedad 'Inmueble'.
                //MesList = data;
                DetenerCarga();
                IsBusy = false;


            } else {
                DetenerCarga();
                await App.Current.MainPage.DisplayAlert("Error", "Ocurrio un error al consultar los listados", "Ok");
            }
        }



        public async Task<bool> ValidarRutaDisponible() {
            //VALIDAR UBICACION ACTUAL
            var ubicacionActual = await Utils.LocationUtil.GetCurrentLocationAsync();
            if (ubicacionActual != null) {
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

        [RelayCommand]
        public async Task ListadoSelect(ListApp listApp)//pasa como tipo de dato
        {
            IniciaCarga("Cargando listado...");
            await Task.Delay(500);
            try {
                var idlistado = listApp.idlistado;

                Dictionary<string, object> data = new Dictionary<string, object>
                    {
                        { "idlist", idlistado },
                        {"inmueble",UserSession.InmuebleNameTracking },
                        {"clienteselected", UserSession.ClienteNameTracking }
                    };
                var route = $"{nameof(ListadoMateriales)}";
                await Shell.Current.GoToAsync(route, data);
                DetenerCarga();
                //await Shell.Current.GoToAsync($"/MyDeliveries/MyListaMaterales", true, data);
            } catch(Exception ex) {
                DetenerCarga();
                //await DisplayAlert("Error", ex.Message, "ok");
                await Shell.Current.DisplayAlert("Error", ex.Message, "ok");
            }
        }

        //BOTON QUE REESTABLEZCA VALORES DE BUSQUEDA 

        private async Task GetEstado() {
            IsBusy = true;

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

            }
        }

        private async Task GetInmuebleByIdClient() {
            AvailableDeliveries = false;
            IsBusy = true;

            // Verificar si la colección 'Inmueble' no es nula y, si no lo es, limpiarla.
            //if (Inmueble != null)
            //    Inmueble.Clear();

            // Crear una solicitud HTTP.
            var request = new HttpRequestMessage();

            // Establecer la URL de la solicitud con el ID de cliente proporcionado.
            //request.RequestUri = new Uri(Constants.API_BASE_URL + $"Inmueble?idcliente={IdClientSelected._idCliente}");

            if(IdEstadoSelected == null) {
                //request.RequestUri = new Uri($"https://www.singa.com.mx:5500/api/InmuebleEntrega?idusuario={idPersonal}&idcliente={IdClientSelected.idCliente}&idestado={0}");
                request.RequestUri = new Uri(Constants.API_BASE_URL + $"InmuebleEntrega?idpersonal={idPersonal}&idcliente={IdClientSelected.idCliente}&idestado={0}");
            } else {

                request.RequestUri = new Uri(Constants.API_BASE_URL + $"InmuebleEntrega?idpersonal={idPersonal}&idcliente={IdClientSelected.idCliente}&idestado={IdEstadoSelected.id_estado}");

            }
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
                var data = JsonConvert.DeserializeObject<ObservableCollection<InmuebleByIdClienteModel.InmuebleModel>>(content);

                // Asignar la colección de inmuebles a la propiedad 'Inmueble'.
                Inmueble = data;
                IsBusy = false;

            }
        }

        private async Task GetClients() {
            // Crear una solicitud HTTP.
            var request = new HttpRequestMessage();

            // Establecer la URL de la solicitud.
            request.RequestUri = new Uri(Constants.API_BASE_URL + $"ClienteEntrega?idusuario={idPersonal}");

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

                // Deserializar el contenido JSON en una colección observable de clientes.
                var data = JsonConvert.DeserializeObject<ObservableCollection<ClientsModel>>(content);

                // Asignar la colección de clientes a la propiedad 'Clients'.
                Clients = data;
            }
        }

        private async Task GetMes() {
            IsBusy = true;

            // Crear una solicitud HTTP.
            var request = new HttpRequestMessage();

            // Establecer la URL de la solicitud con el ID de cliente proporcionado.
            request.RequestUri = new Uri($"https://www.singa.com.mx:5500/api/Mes");

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
                var data = JsonConvert.DeserializeObject<ObservableCollection<MesModel>>(content);

                // Asignar la colección de inmuebles a la propiedad 'Inmueble'.
                MesList = data;
                IsBusy = false;

            }
        }

        //private async Task Register() {
        //    try {
        //        if(await ValidaCampos()) {

        //            IniciaCarga("Cargando...");
        //            await Task.Delay(1000);
        //            // Crear una solicitud HTTP. 
        //            var request = new HttpRequestMessage();

        //            // Establecer la URL de la solicitud con el ID de cliente proporcionado.
        //            request.RequestUri = new Uri(Constants.API_BASE_URL + $"ListadoApp?idinmueble={IdInmubleSelected.idInmueble}&anio={Year}&mes={IdMesSelected.idMes}");

        //            // Establecer el método de la solicitud como GET.
        //            request.Method = HttpMethod.Get;

        //            // Agregar un encabezado "Accept" para indicar que se acepta JSON como respuesta.
        //            request.Headers.Add("Accept", "application/json");

        //            // Crear una nueva instancia de HttpClient.
        //            var client = new HttpClient();

        //            // Enviar la solicitud HTTP y esperar la respuesta.
        //            HttpResponseMessage response = await client.SendAsync(request);

        //            // Verificar si la respuesta tiene un estado OK (código 200).
        //            if(response.StatusCode == HttpStatusCode.OK) {
        //                // Leer el contenido de la respuesta como una cadena.

        //                string content = await response.Content.ReadAsStringAsync();
        //                if (content != null) {
        //                    EntregasDisponibles = JsonConvert.DeserializeObject<List<ListApp>>(content);
        //                }

        //                if(content == "[]") {
        //                    AvailableDeliveries = false;
        //                    DetenerCarga();
        //                    await App.Current.MainPage.DisplayAlert("Alerta", "No hay entregas disponibles para el inmueble especificado", "Ok");
        //                    return;
        //                }
        //                UserSession.IdInmuebleTracking = IdInmubleSelected.idInmueble;
        //                UserSession.IdMesTracking = IdMesSelected.idMes;
        //                UserSession.IdAnioTracking = Year;
        //                UserSession.InmuebleNameTracking = IdInmubleSelected.nombre;
        //                UserSession.ClienteNameTracking = IdClientSelected.nombre;
        //                if(IdInmubleSelected != null) {
        //                    if(IdInmubleSelected.latitud != null && IdInmubleSelected.longitud != null) {
        //                        UserSession.InmuebleLatitudTracking = IdInmubleSelected.latitud;
        //                        UserSession.InmuebleLongitudTracking = IdInmubleSelected.longitud;
        //                    } else {
        //                        UserSession.InmuebleLatitudTracking = "";
        //                        UserSession.InmuebleLongitudTracking = "";
        //                    }
        //                }
        //                AvailableDeliveries = true;

        //                InmuebleText = IdClientSelected.nombre + " - " + IdInmubleSelected.nombre;
        //                UserSession.InmuebleTracking = InmuebleText;
        //                //await Toast.Make($"{Constants.NUMERO_MAXIMO} {content.}", ToastDuration.Short).Show();

        //                // Deserializar el contenido JSON en una colección observable de inmuebles.

        //                Dictionary<string, object> data = new Dictionary<string, object>
        //                {
        //                { "json", content },
        //                {"clienteselected", _idClientSelected.nombre }
        //                };
        //                //var route = $"{nameof(DeliveriesDetail)}";
        //                //await Shell.Current.GoToAsync(route, data);

        //                // Asignar la colección de inmuebles a la propiedad 'Inmueble'.
        //                //MesList = data;
        //                DetenerCarga();
        //                IsBusy = false;


        //            } else {
        //                DetenerCarga();
        //                await App.Current.MainPage.DisplayAlert("Error", "Ocurrio un error al consultar los listados", "Ok");
        //            }
        //        }
        //    } catch(Exception ex) {
        //        DetenerCarga();
        //        await App.Current.MainPage.DisplayAlert("Error", ex.Message, "Ok");
        //    }

        //}

        //public void ApplyQueryAttributes(IDictionary<string, object> query) {
        //    string content = query["json"].ToString();
        //    ListApps = JsonConvert.DeserializeObject<ObservableCollection<ListApp>>(content);
        //    Inmueble = ListApps[0].inmueble;
        //    Cliente = query["clienteselected"].ToString();
        //}

        public void  IniciaCarga(string mensaje) {
            IsLoading = true;
            TextLoading = mensaje;
        }
        public void DetenerCarga() {
            IsLoading = false;
            TextLoading = "";
        }
    }
}
