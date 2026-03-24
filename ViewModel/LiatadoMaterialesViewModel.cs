using BatiaSuite.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using BatiaSuite.Views;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using BatiaSuite.Utils;
using BatiaSuite.Data;

namespace BatiaSuite.ViewModel
{
    public partial class LiatadoMaterialesViewModel : ViewModelBase, IQueryAttributable
    {
        private ObservableCollection<ListadoMaterialesModel> listMateteriales;
        int idlistado = 0;
        public ObservableCollection<ListadoMaterialesModel> ListMateriales
        {
            get { return listMateteriales; }
            set { listMateteriales = value; OnPropertyChanged(); }
        }
        [ObservableProperty]
        bool _isLoading;

        [ObservableProperty]
        string _textLoading;

        [ObservableProperty]
        string origen;

        [ObservableProperty]
        string destino;

        [ObservableProperty]
        string clienteInmueble;

        DbContext _dbContextg;

        private string _cliente;

        public string Cliente
        {
            get { return _cliente; }
            set { _cliente = value; OnPropertyChanged(); }
        }
        private string _clienteEntry;

        public string ClienteEntry
        {
            get { return _clienteEntry; }
            set { _clienteEntry = value; OnPropertyChanged(); }
        }

        private int _folio;

        public int Folio
        {
            get { return _folio; }
            set { _folio = value; OnPropertyChanged(); }
        }
        private string _folioEntry;

        public string FolioEntry
        {
            get { return _folioEntry; }
            set { _folioEntry = value; OnPropertyChanged(); }
        }

        private string _sucursal;

        public string Sucursal
        {
            get { return _sucursal; }
            set { _sucursal = value; OnPropertyChanged(); }
        }
        private string _sucursalEntry;

        public string SucursalEntry
        {
            get { return _sucursalEntry; }
            set { _sucursalEntry = value; OnPropertyChanged(); }
        }

        private string _bidones;

        public string Bidones
        {
            get { return _bidones; }
            set { _bidones = value; OnPropertyChanged(); }
        }

        private string _comentarios;

        public string Comentarios
        {
            get { return _comentarios; }
            set { _comentarios = value; OnPropertyChanged(); }
        }
        private string _nombreRecibe;

        public string NombreRecibe
        {
            get { return _nombreRecibe; }
            set { _nombreRecibe = value; OnPropertyChanged(); }
        }




        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            idlistado = (int)query["idlist"];
            Folio = idlistado;
            FolioEntry = $"FOLIO: {idlistado}";
            Cliente = query["clienteselected"].ToString();
            ClienteEntry = $"CLIENTE: {Cliente}";
            Sucursal = query["inmueble"].ToString();
            SucursalEntry = $"SUCURSAL: {Sucursal}";
            ClienteInmueble = $"{Cliente} - {Sucursal}";
            GetListado();
        }
        public ICommand GuardarDatosCommand { get; set; }
        public LiatadoMaterialesViewModel()
        {
            _dbContextg = new DbContext();
            GuardarDatosCommand = new Command(() => GuardarDatos());
        }
        private async Task GetListado() {
            if(!InternetUtil.IsConnectedInternet()) {
                await GetListadoDetalleLocal();
            } else {
                try {
                    var request = new HttpRequestMessage();
                    request.RequestUri = new Uri(Constants.API_BASE_URL + $"ListadoDetalle?idlistado={idlistado}");
                    request.Method = HttpMethod.Get;
                    request.Headers.Add("Accept", "application/json");
                    var client = new HttpClient();
                    HttpResponseMessage response = await client.SendAsync(request);

                    if(response.StatusCode == HttpStatusCode.OK) {
                        string content = await response.Content.ReadAsStringAsync();
                        ListMateriales = JsonConvert.DeserializeObject<ObservableCollection<ListadoMaterialesModel>>(content);
                    } else {
                        await GetListadoDetalleLocal();
                    }
                }
                catch(Exception ex) {
                    await Shell.Current.DisplayAlert("Error", ex.Message, "Ok");
                }
            }
        }

        public async Task GetListadoDetalleLocal() {
            //OBTENER CLIENTES DE LOCAL
            var localDetalleListado = await _dbContextg.ObtenerListadoMaterialEntregaPrecargaByIdListado(idlistado);
            if(localDetalleListado != null && localDetalleListado.Count > 0) {
                ListMateriales = new ObservableCollection<ListadoMaterialesModel>(
                localDetalleListado.Select(c => new ListadoMaterialesModel {
                    clave = c.clave,
                    descripcion = c.producto,
                    cantidad = c.cantidad,
                    entregado = c.entregado,
                    unidad = c.unidad
                })
             );
            }
        }

        public async void GuardarDatos()
        {
            // Deserializar el contenido JSON en una colección observable de inmuebles.
            if (string.IsNullOrEmpty(Comentarios))
            {
                Comentarios = "";
            }
            if (string.IsNullOrEmpty(NombreRecibe))
            {
                await Shell.Current.DisplayAlert("Error", "Ingrese el nombre de quién recibe","Ok");
                return;
            }
            Dictionary<string, object> data = new Dictionary<string, object>
                    {
                        { "MaterialsList", ListMateriales },
                        {"NombreRecibe", NombreRecibe},
                        {"Comentarios", Comentarios },
                        {"Bidones", Bidones },
                        {"IdListado", idlistado }
                    };
            var route = $"{nameof(RegisterDelivery)}";
            await Shell.Current.GoToAsync(route, data);
            //await Shell.Current.GoToAsync($"///RegisterMateriales", true, data);

            //foreach (var material in ListMateriales)
            //{
            //    var valorEntry = material.entregado; //Obtienes el valor de la propiedad entregado
            //}
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
