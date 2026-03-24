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
using BatiaSuite.Views.SupplierDeliveries;
using BatiaSuite.Data;


namespace BatiaSuite.ViewModel.SupplierDeliveries
{
    public partial class SupplierLiatadoMaterialesViewModel : BaseViewModel, IQueryAttributable
    {
        private ObservableCollection<ListadoMaterialesModel> listMateteriales;
        int idlistado = 0;
        public ObservableCollection<ListadoMaterialesModel> ListMateriales
        {
            get { return listMateteriales; }
            set { listMateteriales = value; OnPropertyChanged(); }
        }

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
            GetListado();
        }
        public ICommand GuardarDatosCommand { get; set; }
        public SupplierLiatadoMaterialesViewModel()
        {
            GuardarDatosCommand = new Command(() => GuardarDatos());
            _dbContextg = new DbContext();
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
                } catch(Exception ex) {
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

        public async void GuardarDatos() {
            // Deserializar el contenido JSON en una colección observable de inmuebles.
            if(string.IsNullOrEmpty(Comentarios)) {
                Comentarios = "";
            }
            if(string.IsNullOrEmpty(NombreRecibe)) {
                await Shell.Current.DisplayAlert("Error", "Ingrese el nombre de quién recibe", "Ok");
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
            var route = $"{nameof(SupplierRegisterDelivery)}";
            try {
                //var page = new SupplierRegisterDelivery();
                //page.SetMediaPicker(MediaPicker.Default);
                //await Shell.Current.Navigation.PushAsync(page);
                await Shell.Current.GoToAsync(route, data);
            }
            catch (Exception ex) {
                await Shell.Current.DisplayAlert("Error", ex.Message, "Ok");

            }


            //await Shell.Current.GoToAsync($"///RegisterMateriales", true, data);

            //foreach (var material in ListMateriales)
            //{
            //    var valorEntry = material.entregado; //Obtienes el valor de la propiedad entregado
            //}
        }
    }
}
