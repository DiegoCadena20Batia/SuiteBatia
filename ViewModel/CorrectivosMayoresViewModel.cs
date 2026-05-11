using BatiaSuite.Data;
using BatiaSuite.Models;
using BatiaSuite.Utils;
using BatiaSuite.Views;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Windows.Input;

namespace BatiaSuite.ViewModel {

    public class CorrectivosMayoresViewModel : BaseViewModel {
        private HttpClient client;
        private DbContext _dbContext;

        private HttpHelper _httpClient;

        private ObservableCollection<ListCorrecM> listApps;

        private bool _isEnabled;

        public bool IsEnabled {
            get { return _isEnabled; }
            set { _isEnabled = value; OnPropertyChanged(); }
        }
        private bool _hayCorrectivosPendientes;

        public bool HayCorrectivosPendientes {
            get => _hayCorrectivosPendientes;
            set {
                _hayCorrectivosPendientes = value;
                OnPropertyChanged();
            }
        }

        private int _cantidadCorrectivosPendientes;
        public int CantidadCorrectivosPendientes {
            get => _cantidadCorrectivosPendientes;
            set {
                _cantidadCorrectivosPendientes = value;
                OnPropertyChanged();
            }
        }

        public ICommand SincronizarCorrectivosCommand { get; set; }

        public ObservableCollection<ListCorrecM> ListApps {
            get { return listApps; }
            set { listApps = value; OnPropertyChanged(); }
        }

        #region IdClave

        private int _idClave;

        public int IdClave {
            get { return _idClave; }
            set {
                _idClave = value;

                OnPropertyChanged();
            }
        }

        #endregion IdClave

        private ObservableCollection<ClienteCmModel.ClienteCorrec> _clienteCm;

        public ObservableCollection<ClienteCmModel.ClienteCorrec> ClienteCm {
            get { return _clienteCm; }
            set { _clienteCm = value; OnPropertyChanged(); }
        }

        private ClienteCmModel.ClienteCorrec _idClientSelected;

        // Declaración de una propiedad pública llamada 'IdSelected' que encapsula la propiedad privada '_idSelected'.
        public ClienteCmModel.ClienteCorrec IdClientSelected {
            get => _idClientSelected;
            set {
                IdClave = 0;

                if(_idClientSelected != value && value != null) {
                    _idClientSelected = value;

                    CargarInmuebles();
                    GetInfoSelect();

                    _dbContext.TestDB();

                    OnPropertyChanged();
                }
            }
        }

        private async void cargarDatosLocales() {
            if(!InternetUtil.IsConnectedInternet())
                ClienteCm = new ObservableCollection<ClienteCmModel.ClienteCorrec>(await _dbContext.ObtenerClientesLocales());
        }

        private async void CargarInmuebles() {
            try {
                if(InternetUtil.IsConnectedInternet()) {
                    GetInmueble();
                } else {
                    var inmueblesLocales = await _dbContext.ObtenerInmueblesLocales(IdClientSelected.idCliente);

                    InmuebleCm = new ObservableCollection<InmuebleCmModel.InmuebleCorrec>(inmueblesLocales);
                }
            } catch(Exception ex) {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private ObservableCollection<InmuebleCmModel.InmuebleCorrec> _inmuebleCm;

        public ObservableCollection<InmuebleCmModel.InmuebleCorrec> InmuebleCm {
            get { return _inmuebleCm; }
            set { _inmuebleCm = value; OnPropertyChanged(); }
        }

        // Declaración de una propiedad privada llamada '_idInmubleSelected' del tipo 'InmuebleByIdClienteModel.InmuebleModel'.
        private InmuebleCmModel.InmuebleCorrec _idInmubleSelected;

        // Declaración de una propiedad pública llamada 'IdInmubleSelected' que encapsula la propiedad privada '_idInmubleSelected'.
        public InmuebleCmModel.InmuebleCorrec IdInmubleSelected {
            get { return _idInmubleSelected; } // Obtener el valor de '_idInmubleSelected'.
            set {
                IdClave = 0;
                // Comprobar si el valor nuevo es diferente del valor actual y no es nulo.
                if(_idInmubleSelected != value && value != null) {
                    // Asignar el valor nuevo a '_idInmubleSelected'.
                    _idInmubleSelected = value;

                    GetInfoSelect();
                    // Notificar que la propiedad 'IdInmubleSelected' ha cambiado.
                    OnPropertyChanged();
                }
            }
        }

        public ICommand GetInfoEmpleadoCommand { get; set; }
        public ICommand CommandListadoSelec { get; set; }
        public ICommand PrecargarDatosCorrectivosMayoresCommand { get; set; }

        public CorrectivosMayoresViewModel(DbContext dbContext) {
            _dbContext = dbContext;
            Debug.WriteLine($"DbContext hash: {_dbContext.GetHashCode()}");
            IsEnabled = true;
            client = new HttpClient();

            _httpClient = new HttpHelper();
            GetClients();
            cargarDatosLocales();

            //_dbContext.TestDB();

            GetInfoEmpleadoCommand = new Command(async () => await GetInfoIDClave());

            CommandListadoSelec = new Command<ListCorrecM>(async (k) => await ListadoSelec(k));

            PrecargarDatosCorrectivosMayoresCommand = new Command(async () => PrecargarDatosCorrectivosMayores());

            SincronizarCorrectivosCommand = new Command(async () => {
                if(!InternetUtil.IsConnectedInternet()) {
                    await DisplayAlert(
                        "Sin conexión",
                        "Necesitas internet para enviar correctivos pendientes.",
                        "OK"
                    );
                    return;
                }

                IsBusy = true;

                try {

                    await _dbContext.SincronizarCorrectivosPendientes();

                    await VerificarCorrectivosPendientes();

                    await DisplayAlert(
                        "Éxito",
                        "Correctivos pendientes sincronizados.",
                        "OK"
                    );

                } catch(Exception ex) {

                    await DisplayAlert(
                        "Error",
                        ex.Message,
                        "OK"
                    );

                } finally {

                    IsBusy = false;
                }
            });
        }

        private async Task GetClients() {
            // Crear una solicitud HTTP.
            var request = new HttpRequestMessage();

            // Establecer la URL de la solicitud.
            request.RequestUri = new Uri($"{Constants.API_BASE_URL}ClienteCM");

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
                var data = JsonConvert.DeserializeObject<ObservableCollection<ClienteCmModel.ClienteCorrec>>(content);

                // Asignar la colección de clientes a la propiedad 'Clients'.
                ClienteCm = data;
            }
        }

        private async Task<List<ClienteCmModel.ClienteCorrec>> GetClientsToLocal() {
            try {
                var request = new HttpRequestMessage();

                request.RequestUri = new Uri($"{Constants.API_BASE_URL}ClienteCM");

                request.Method = HttpMethod.Get;

                request.Headers.Add("Accept", "application/json");

                var client = new HttpClient();

                HttpResponseMessage response = await client.SendAsync(request);

                if(response.StatusCode == HttpStatusCode.OK) {
                    string content = await response.Content.ReadAsStringAsync();

                    var data = JsonConvert.DeserializeObject<List<ClienteCmModel.ClienteCorrec>>(content);

                    return data;
                } else {
                    await DisplayAlert("Error", $"Error al obtener clientes: {response.StatusCode}", "Ok");
                    return null;
                }
            } catch(Exception ex) {
                await DisplayAlert("Error", ex.Message, "Ok");
                return null;
            }
        }

        private async Task GetInmueble() {
            var request = new HttpRequestMessage();

            request.RequestUri = new Uri($"{Constants.API_BASE_URL}Inmueble?idcliente={IdClientSelected.idCliente}");

            request.Method = HttpMethod.Get;

            request.Headers.Add("Accept", "application / json");

            var client = new HttpClient();

            HttpResponseMessage response = await client.SendAsync(request);

            if(response.StatusCode == HttpStatusCode.OK) {
                string content = await response.Content.ReadAsStringAsync();

                var data = JsonConvert.DeserializeObject<ObservableCollection<InmuebleCmModel.InmuebleCorrec>>(content);

                InmuebleCm = data;
            }
        }

        private async Task<List<InmuebleCmModel.InmuebleCorrec>> GetInmuebleToLocal(int idCliente) {
            try {
                var request = new HttpRequestMessage();

                request.RequestUri = new Uri($"{Constants.API_BASE_URL}Inmueble?idcliente={idCliente}");
                request.Method = HttpMethod.Get;

                request.Headers.Add("Accept", "application / json");

                var client = new HttpClient();

                HttpResponseMessage response = await client.SendAsync(request);

                if(response.StatusCode == HttpStatusCode.OK) {
                    string content = await response.Content.ReadAsStringAsync();

                    var data = JsonConvert.DeserializeObject<List<InmuebleCmModel.InmuebleCorrec>>(content);

                    return data;
                } else {
                    await DisplayAlert("Error", $"Error al obtener inmuebles: {response.StatusCode}", "Ok");
                    return null;
                }
            } catch(Exception ex) {
                await DisplayAlert("Error", ex.Message, "Ok");
                return null;
            }
        }

        private async Task GetInfoIDClave() {
            try {
                // Crear una solicitud HTTP.
                var request = new HttpRequestMessage();

                // Establecer la URL de la solicitud con el ID de cliente proporcionado.
                //request.RequestUri = new Uri($"http://singa.com.mx:5500/api/CorrectivosM?idclavecm={}&idcliente={}&idinmueble={}");
                IsBusy = true;
                if(IdClave != 0)
                    request.RequestUri = new Uri($"{Constants.API_BASE_URL}CorrectivosM?idclavecm={IdClave}&idcliente={0}&idinmueble={0}");
                else
                    return;

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
                    string contentCM = await response.Content.ReadAsStringAsync();

                    ListApps = JsonConvert.DeserializeObject<ObservableCollection<ListCorrecM>>(contentCM);
                }
                IsBusy = false;
            } catch(Exception ex) {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        private async Task<List<ListCorrecM>> GetInfoIDClaveToLocal(int idCliente, int idInmueble) {
            try {
                IsBusy = true;

                var request = new HttpRequestMessage();

                // Mantener mismo endpoint que usas en consultas normales
                if(IdClave != 0)
                    request.RequestUri = new Uri(
                        $"{Constants.API_BASE_URL}CorrectivosM?idclavecm={IdClave}&idcliente=0&idinmueble=0");
                else
                    request.RequestUri = new Uri(
                        $"{Constants.API_BASE_URL}CorrectivosM?idclavecm=0&idcliente={idCliente}&idinmueble={idInmueble}");

                request.Method = HttpMethod.Get;
                request.Headers.Add("Accept", "application/json");

                var client = new HttpClient();

                HttpResponseMessage response = await client.SendAsync(request);

                if(response.StatusCode == HttpStatusCode.OK) {

                    string contentCM = await response.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<List<ListCorrecM>>(contentCM);

                    return result;
                } else {

                    await DisplayAlert("Error", $"Error al obtener datos: {response.StatusCode}", "Ok");

                    return null;
                }

            } catch(Exception ex) {

                await DisplayAlert("Error", ex.Message, "Ok");

                return null;

            } finally {

                IsBusy = false;
            }
        }

        private async Task GetInfoSelect() {
            try {
                IsBusy = true;

                List<ListCorrecM> localData = new();

                // =========================
                // 1) BUSCAR LOCAL
                // =========================
                if(IdClientSelected != null && IdInmubleSelected == null) {

                    localData = await _dbContext.ObtenerCorrectivosPorCliente(
                        IdClientSelected.idCliente
                    );

                } else if(IdClientSelected != null && IdInmubleSelected != null) {

                    localData = await _dbContext.ObtenerCorrectivosPorClienteInmueble(
                        IdClientSelected.idCliente,
                        IdInmubleSelected.id_inmueble
                    );
                }

                // =========================
                // 2) SI HAY DATOS LOCALES
                // =========================
                if(localData != null && localData.Any()) {

                    ListApps = new ObservableCollection<ListCorrecM>(localData);

                    return;
                }

                // =========================
                // 3) SIN INTERNET
                // =========================
                if(!InternetUtil.IsConnectedInternet()) {

                    await DisplayAlert("Sin conexión", "No hay datos locales disponibles.", "Ok");

                    return;
                }

                // =========================
                // 4) API
                // =========================
                var request = new HttpRequestMessage();

                if(IdClientSelected != null && IdInmubleSelected == null) {

                    request.RequestUri = new Uri(
                        $"{Constants.API_BASE_URL}CorrectivosMPruebas?idclavecm=0&idcliente={IdClientSelected.idCliente}&idinmueble=0");

                } else if(IdClientSelected != null && IdInmubleSelected != null) {

                    request.RequestUri = new Uri(
                        $"{Constants.API_BASE_URL}CorrectivosMPruebas?idclavecm=0&idcliente={IdClientSelected.idCliente}&idinmueble={IdInmubleSelected.id_inmueble}");
                }

                request.Method = HttpMethod.Get;
                request.Headers.Add("Accept", "application/json");

                var client = new HttpClient();

                HttpResponseMessage response = await client.SendAsync(request);

                if(response.StatusCode == HttpStatusCode.OK) {

                    string contentCM = await response.Content.ReadAsStringAsync();

                    var apiData = JsonConvert.DeserializeObject<List<ListCorrecM>>(contentCM);

                    if(apiData != null && apiData.Any()) {

                        foreach(var item in apiData)
                            item.SyncDate = DateTime.Now;

                        // Guardar cache local
                        await _dbContext.GuardarCorrectivosLocal(apiData);

                        ListApps = new ObservableCollection<ListCorrecM>(apiData);
                    }
                } else {

                    await DisplayAlert("Error", $"Error al obtener datos: {response.StatusCode}", "Ok");
                }

            } catch(Exception ex) {

                await DisplayAlert("Error", ex.Message, "Ok");

            } finally {

                IsBusy = false;
            }
        }

        public async Task PrecargarDatosCorrectivosMayores() {
            try {
                IsBusy = true;

                // =========================
                // LIMPIEZA INICIAL
                // =========================
                await _dbContext.DeleteAllDataCorrectivos();

                // =========================
                // 1) CLIENTES
                // =========================
                var clientes = await GetClientsToLocal();

                ClienteCm = new ObservableCollection<ClienteCmModel.ClienteCorrec>(clientes);

                if(clientes == null || !clientes.Any())
                    return;

                foreach(var cliente in clientes)
                    cliente.SyncDate = DateTime.Now;

                await _dbContext.GuardarClientesLocal(clientes);

                // =========================
                // 2) INMUEBLES + CORRECTIVOS
                // =========================
                foreach(var cliente in clientes) {

                    var inmuebles = await GetInmuebleToLocal(cliente.idCliente);

                    if(inmuebles == null || !inmuebles.Any())
                        continue;

                    foreach(var inmueble in inmuebles) {
                        inmueble.SyncDate = DateTime.Now;
                        inmueble.id_cliente = cliente.idCliente;
                    }

                    // Guardar inmuebles del cliente actual SIN borrar anteriores
                    await _dbContext.GuardarInmueblesLocal(inmuebles);

                    // =========================
                    // CORRECTIVOS POR CLIENTE + INMUEBLE
                    // =========================
                    foreach(var inmueble in inmuebles) {

                        var correctivos = await GetInfoIDClaveToLocal(
                            cliente.idCliente,
                            inmueble.id_inmueble
                        );

                        if(correctivos == null || !correctivos.Any())
                            continue;

                        foreach(var item in correctivos)
                            item.SyncDate = DateTime.Now;

                        await _dbContext.GuardarCorrectivosLocal(correctivos);
                    }
                }

                await DisplayAlert("Éxito", "Datos precargados para uso sin conexión.", "OK");

            } catch(Exception ex) {

                await DisplayAlert("Error", ex.Message, "OK");

            } finally {

                IsBusy = false;
            }
        }


        private async Task ListadoSelec(ListCorrecM listCorrecM)//pasa como tipo de dato
        {
            try {
                IsBusy = true;
                IsEnabled = false;
                var idClaveCM = listCorrecM.idClaveCM;

                Dictionary<string, object> Listdata = new Dictionary<string, object>
                    {
                         {"idClave", listCorrecM.idClaveCM},
                         //{"Cliente", listCorrecM.cliente },
                         //{"Inmueble", listCorrecM.inmueble },
                         {"Tipo", listCorrecM.tipo},
                         {"Fecha", listCorrecM.fregistro},
                         {"Detalles", listCorrecM.desTrabajos},
                    };
                var route = $"{nameof(ListaCorrectivosM)}";
                await Shell.Current.GoToAsync(route, true, Listdata);
                //await Shell.Current.GoToAsync($"/MyDeliveries/MyListaMaterales", true, data);
            } catch(Exception ex) {
                await DisplayAlert("Error", ex.Message, "ok");
            }
            IsBusy = false;
            IsEnabled = true;
        }


        public async Task VerificarCorrectivosPendientes() {
            try {

                await _dbContext.EnsureInitialized();

                var pendientes = await _dbContext._dbConn
                    .Table<CorrectivoMPendienteLocal>()
                    .Where(x => x.Sincronizado == false)
                    .ToListAsync();

                CantidadCorrectivosPendientes = pendientes.Count;

                HayCorrectivosPendientes = CantidadCorrectivosPendientes > 0;

            } catch(Exception ex) {

                System.Diagnostics.Debug.WriteLine(
                    $"Error verificando pendientes: {ex.Message}"
                );

                CantidadCorrectivosPendientes = 0;

                HayCorrectivosPendientes = false;
            }
        }
    }
}