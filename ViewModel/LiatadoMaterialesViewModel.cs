using BatiaSuite.Data;
using BatiaSuite.Models.EntidadesLocal.RutasEntregas;
using BatiaSuite.Utils;
using BatiaSuite.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows.Input;

namespace BatiaSuite.ViewModel {

    public partial class ListadoMaterialesViewModel : ViewModelBase, IQueryAttributable {
        private ObservableCollection<RutasInmuebles> _listMateriales;

        public ObservableCollection<RutasInmuebles> ListMateriales {
            get => _listMateriales;
            set { _listMateriales = value; OnPropertyChanged(); }
        }

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _textLoading;

        [ObservableProperty]
        private string _origen;

        [ObservableProperty]
        private string _destino;

        [ObservableProperty]
        private string _clienteInmueble;

        [ObservableProperty]
        private string _folioEntry;

        [ObservableProperty]
        private string _bidones;

        [ObservableProperty]
        private string _comentarios;

        [ObservableProperty]
        private string _nombreRecibe;

        private readonly LocalDbContext _dbContext;

        public ICommand GuardarDatosCommand { get; set; }

        public ListadoMaterialesViewModel() {
            _dbContext = new LocalDbContext();
            GuardarDatosCommand = new Command(async () => await GuardarDatos());
        }

        /// <summary>
        /// Reacción de entrada al cargar la pantalla. Ahora lee directo de UserSession.
        /// </summary>
        public void ApplyQueryAttributes(IDictionary<string, object> query) {
            ClienteInmueble = UserSession.InmuebleNameTracking;

            _ = CargarMaterialesDeSucursal();
        }

        /// <summary>
        /// Obtiene los productos filtrando la tabla desnormalizada por la Sucursal (IdInmueble) activa en sesión.
        /// </summary>
        public async Task CargarMaterialesDeSucursal() {
            IniciaCarga("Cargando materiales...");
            await Task.Delay(300);

            if(!InternetUtil.IsConnectedInternet()) {
                await ObtenerMaterialesLocal();
            } else {
                try {
                    var request = new HttpRequestMessage {
                        RequestUri = new Uri(Constants.API_BASE_URL + $"RutasOperador?idoperador={UserSession.IdPersonal}"),
                        Method = HttpMethod.Get
                    };
                    request.Headers.Add("Accept", "application/json");

                    var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
                    HttpResponseMessage response = await client.SendAsync(request);

                    if(response.StatusCode == HttpStatusCode.OK) {
                        string content = await response.Content.ReadAsStringAsync();
                        if(!string.IsNullOrEmpty(content)) {
                            var todosLosMateriales = JsonConvert.DeserializeObject<List<RutasInmuebles>>(content);

                            var primerRegistro = todosLosMateriales.FirstOrDefault();
                            if(primerRegistro != null) {
                                FolioEntry = primerRegistro.IdListado.ToString();
                            }

                            todosLosMateriales = todosLosMateriales
                                .Where(r => r.IdRuta == UserSession.IdRutaTracking && r.IdInmueble == UserSession.IdInmuebleTracking)
                                .ToList();
                            ListMateriales = new ObservableCollection<RutasInmuebles>(todosLosMateriales);
                        }
                        DetenerCarga();
                    } else {
                        await ObtenerMaterialesLocal();
                    }
                } catch(Exception ex) {
                    DetenerCarga();
                    await ObtenerMaterialesLocal();
                    Console.WriteLine($"Error de API: {ex.Message}. Cargando datos de contingencia local.");
                }
            }
        }

        /// <summary>
        /// Consulta Offline usando la expresión Lambda genérica de tu LocalDbContext
        /// </summary>
        public async Task ObtenerMaterialesLocal() {
            // Filtramos la tabla maestra desnormalizada usando los IDs que dejamos guardados en la sesión
            var materialesLocal = await _dbContext.ObtenerListaLocalAsync<RutasInmuebles>(r =>
                r.IdRuta == UserSession.IdRutaTracking &&
                r.IdInmueble == UserSession.IdInmuebleTracking
            );

            if(materialesLocal != null && materialesLocal.Count > 0) {
                FolioEntry = materialesLocal.First().IdListado.ToString();
                ListMateriales = new ObservableCollection<RutasInmuebles>(materialesLocal);
            } else {
                ListMateriales = new ObservableCollection<RutasInmuebles>();
                FolioEntry = "N/A";
            }
            DetenerCarga();
        }

        /// <summary>
        /// Valida los datos e inicia el paso a la pantalla de firmas y evidencias
        /// </summary>
        public async Task GuardarDatos() {
            if(string.IsNullOrEmpty(NombreRecibe)) {
                await Shell.Current.DisplayAlert("Campo requerido", "Por favor, ingresa el nombre de la persona que recibe los materiales.", "Ok");
                return;
            }

            try {
                IniciaCarga("Guardando información...");
                await Task.Delay(300);

                // Asegurar que comentarios no vaya como null
                Comentarios = Comentarios ?? "";

                // Mapeamos los datos al diccionario de navegación para la vista de firma
                var data = new Dictionary<string, object>
                {
                    { "MaterialsList", ListMateriales.ToList() },
                    { "NombreRecibe", NombreRecibe },
                    { "Comentarios", Comentarios },
                    { "Bidones", Bidones ?? "0" },
                    { "IdListado", ListMateriales.FirstOrDefault()?.IdListado ?? 0 }
                };

                await Shell.Current.GoToAsync(nameof(RegisterDelivery), true, data);
                DetenerCarga();
            } catch(Exception ex) {
                DetenerCarga();
                await Shell.Current.DisplayAlert("Error", $"No se pudo continuar: {ex.Message}", "Ok");
            }
        }

        /// <summary>
        /// Revisa el GPS y arma el destino basándose en el tracking activo de la sesión
        /// </summary>
        public async Task<bool> ValidarRutaDisponible() {
            var ubicacionActual = await Utils.LocationUtil.GetCurrentLocationAsync();
            if(ubicacionActual != null) {
                Origen = $"{ubicacionActual.Latitude},{ubicacionActual.Longitude}";
            } else {
                await Shell.Current.DisplayAlert("Alerta", "No se pudo obtener tu ubicación actual por GPS.", "OK");
                return false;
            }

            if(string.IsNullOrEmpty(UserSession.InmuebleLatitudTracking) || string.IsNullOrEmpty(UserSession.InmuebleLongitudTracking)) {
                await Shell.Current.DisplayAlert("Alerta", "Esta sucursal no cuenta con coordenadas geográficas registradas en la base de datos.", "OK");
                return false;
            }

            Destino = $"{UserSession.InmuebleLatitudTracking},{UserSession.InmuebleLongitudTracking}";
            return true;
        }

        [RelayCommand]
        public async Task AbrirGoogleMaps() {
            IniciaCarga("Abriendo Google Maps...");
            if(await ValidarRutaDisponible()) {
                string url = $"geo:0,0?q={Destino}";
                try {
                    await Launcher.Default.OpenAsync(new Uri(url));
                } catch(Exception ex) {
                    Console.WriteLine($"Error al abrir Google Maps: {ex.Message}");
                }
            }
            DetenerCarga();
        }

        [RelayCommand]
        public async Task AbrirWaze() {
            IniciaCarga("Abriendo Waze...");
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