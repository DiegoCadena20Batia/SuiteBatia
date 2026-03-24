using BatiaSuite.Models;
using BatiaSuite.Views;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows.Input;

namespace BatiaSuite.ViewModel {
    public class CorrectivosMayoresViewModel : BaseViewModel
    {
        HttpClient client;

        private ObservableCollection<ListCorrecM> listApps;

        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; OnPropertyChanged(); }
        }

        public ObservableCollection<ListCorrecM> ListApps
        {
            get { return listApps; }
            set { listApps = value; OnPropertyChanged(); }
        }
        #region IdClave

        private int _idClave;

        public int IdClave
        {
            get { return _idClave; }
            set
            {
                _idClave = value;

                OnPropertyChanged();
            }
        }

        #endregion
        private ObservableCollection<ClienteCmModel.ClienteCorrec> _clienteCm;

        public ObservableCollection<ClienteCmModel.ClienteCorrec> ClienteCm
        {
            get { return _clienteCm; }
            set { _clienteCm = value; OnPropertyChanged(); }
        }

        private ClienteCmModel.ClienteCorrec _idClientSelected;

        // Declaración de una propiedad pública llamada 'IdSelected' que encapsula la propiedad privada '_idSelected'.
        public ClienteCmModel.ClienteCorrec IdClientSelected
        {
            get { return _idClientSelected; } // Obtener el valor de '_idSelected'.
            set
            {
                IdClave = 0;
                // Comprobar si el valor nuevo es diferente del valor actual y no es nulo.
                if (_idClientSelected != value && value != null)
                {
                    // Asignar el valor nuevo a '_idInmubleSelected'.
                    _idClientSelected = value;

                    GetInfoSelect();

                    // Notificar que la propiedad 'IdInmubleSelected' ha cambiado.
                    OnPropertyChanged();
                    GetInmueble();
                }
            }
        }

        private ObservableCollection<InmuebleCmModel.InmuebleCorrec> _inmuebleCm;

        public ObservableCollection<InmuebleCmModel.InmuebleCorrec> InmuebleCm
        {
            get { return _inmuebleCm; }
            set { _inmuebleCm = value; OnPropertyChanged(); }
        }
        // Declaración de una propiedad privada llamada '_idInmubleSelected' del tipo 'InmuebleByIdClienteModel.InmuebleModel'.
        private InmuebleCmModel.InmuebleCorrec _idInmubleSelected;

        // Declaración de una propiedad pública llamada 'IdInmubleSelected' que encapsula la propiedad privada '_idInmubleSelected'.
        public InmuebleCmModel.InmuebleCorrec IdInmubleSelected
        {
            get { return _idInmubleSelected; } // Obtener el valor de '_idInmubleSelected'.
            set
            {
                IdClave = 0;
                // Comprobar si el valor nuevo es diferente del valor actual y no es nulo.
                if (_idInmubleSelected != value && value != null)
                {
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
       
        public CorrectivosMayoresViewModel()
        {
            IsEnabled = true;
            client = new HttpClient();
            GetClients();
            GetInfoEmpleadoCommand = new Command(async () => await GetInfoIDClave());

            CommandListadoSelec = new Command<ListCorrecM>(async (k) => await ListadoSelec(k));
        }
       
        private async Task GetClients()
        {
            // Crear una solicitud HTTP.
            var request = new HttpRequestMessage();

            // Establecer la URL de la solicitud.
            request.RequestUri = new Uri("https://www.singa.com.mx:5500/api/ClienteCM");

            // Establecer el método de la solicitud como GET.
            request.Method = HttpMethod.Get;

            // Agregar un encabezado "Accept" para indicar que se acepta JSON como respuesta.
            request.Headers.Add("Accept", "application/json");

            // Crear una nueva instancia de HttpClient.
            var client = new HttpClient();

            // Enviar la solicitud HTTP y esperar la respuesta.
            HttpResponseMessage response = await client.SendAsync(request);

            // Verificar si la respuesta tiene un estado OK (código 200).
            if (response.StatusCode == HttpStatusCode.OK)
            {
                // Leer el contenido de la respuesta como una cadena.
                string content = await response.Content.ReadAsStringAsync();

                // Deserializar el contenido JSON en una colección observable de clientes.
                var data = JsonConvert.DeserializeObject<ObservableCollection<ClienteCmModel.ClienteCorrec>>(content);

                // Asignar la colección de clientes a la propiedad 'Clients'.
                ClienteCm = data;
            }
        }

        private async Task GetInmueble()
        {
            var request = new HttpRequestMessage();

            request.RequestUri = new Uri($"https://www.singa.com.mx:5500/api/Inmueble?idcliente={IdClientSelected.idCliente}");

            request.Method = HttpMethod.Get;

            request.Headers.Add("Accept", "application / json");

            var client = new HttpClient();

            HttpResponseMessage response = await client.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                string content = await response.Content.ReadAsStringAsync();

                var data = JsonConvert.DeserializeObject<ObservableCollection<InmuebleCmModel.InmuebleCorrec>>(content);

                InmuebleCm = data;
            }
        }
        private async Task GetInfoIDClave()
        {
            try
            {
                // Crear una solicitud HTTP. 
                var request = new HttpRequestMessage();

                // Establecer la URL de la solicitud con el ID de cliente proporcionado.
                //request.RequestUri = new Uri($"http://singa.com.mx:5500/api/CorrectivosM?idclavecm={}&idcliente={}&idinmueble={}");
                IsBusy = true;
                if (IdClave != 0)
                    request.RequestUri = new Uri($"https://www.singa.com.mx:5500/api/CorrectivosMPruebas?idclavecm={IdClave}&idcliente={0}&idinmueble={0}");
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
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    // Leer el contenido de la respuesta como una cadena.
                    string contentCM = await response.Content.ReadAsStringAsync();

                    ListApps = JsonConvert.DeserializeObject<ObservableCollection<ListCorrecM>>(contentCM);

                  
                }
                IsBusy = false;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        private async Task GetInfoSelect()
        {
            try
            {
                IsBusy = true;
                // Crear una solicitud HTTP. 
                var request = new HttpRequestMessage();
                if (IdClientSelected != null && IdInmubleSelected == null)
                    request.RequestUri = new Uri($"https://www.singa.com.mx:5500/api/CorrectivosMPruebas?idclavecm={0}&idcliente={IdClientSelected.idCliente}&idinmueble={0}");
                else if (IdClientSelected != null && IdInmubleSelected != null)
                    request.RequestUri = new Uri($"https://www.singa.com.mx:5500/api/CorrectivosMPruebas?idclavecm={0}&idcliente={IdClientSelected.idCliente}&idinmueble={IdInmubleSelected.id_inmueble}");
                // Establecer el método de la solicitud como GET.
                request.Method = HttpMethod.Get;
                // Agregar un encabezado "Accept" para indicar que se acepta JSON como respuesta.
                request.Headers.Add("Accept", "application/json");
                // Crear una nueva instancia de HttpClient.
                var client = new HttpClient();
                // Enviar la solicitud HTTP y esperar la respuesta.
                HttpResponseMessage response = await client.SendAsync(request);
                // Verificar si la respuesta tiene un estado OK (código 200).
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    // Leer el contenido de la respuesta como una cadena.
                    string contentCM = await response.Content.ReadAsStringAsync();
                    ListApps = JsonConvert.DeserializeObject<ObservableCollection<ListCorrecM>>(contentCM);
                    IsBusy = false;
                }
            }

            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
            IsBusy = false;
        }

        private async Task ListadoSelec(ListCorrecM listCorrecM)//pasa como tipo de dato
        {
            try
            {
                
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
            }
             
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "ok");
            }
            IsBusy = false;
            IsEnabled = true;
        }
    }
}