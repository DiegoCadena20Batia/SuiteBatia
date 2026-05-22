using BatiaSuite.Data;
using BatiaSuite.Models.CheckListSupervisionesAldoConti.singamobiletest.Models;
using BatiaSuite.Models.Supervision;
using BatiaSuite.Utils;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace BatiaSuite.ViewModel.CheckListSupervisionesAldoConti {

    public class ChecklistViewModel : INotifyPropertyChanged {
        private readonly HttpClient _httpClient;
        private const string BaseApiUrl = $"{Constants.API_BASE_URL}";

        private readonly string _userSessionCliente = "Aldo Conti";
        public string UserSessionCliente => _userSessionCliente;

        private readonly int _clienteId = UserSession.Cliente;
        public int ClienteId => _clienteId;

        private int _tiendaId = 0;
        public int TiendaId => _tiendaId;

        // --- PROPIEDADES DE CONTROL DE UI ---
        private bool _isLoading;

        public bool IsLoading {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        private string _tiendaNombre = string.Empty;

        public string TiendaNombre {
            get => _tiendaNombre;
            set { _tiendaNombre = value; OnPropertyChanged(); }
        }

        private string _gerenteNombre = string.Empty;

        public string GerenteNombre {
            get => _gerenteNombre;
            set { _gerenteNombre = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Inmueble> ListaInmuebles { get; set; } = new ObservableCollection<Inmueble>();
 
        private Inmueble? _inmuebleSeleccionado;

        public Inmueble? InmuebleSeleccionado {
            get => _inmuebleSeleccionado;
            set {
                if(_inmuebleSeleccionado != value) {
                    _inmuebleSeleccionado = value;
                    OnPropertyChanged();

                    if(_inmuebleSeleccionado != null) {
                        _tiendaId = _inmuebleSeleccionado.IdInmueble;
                        TiendaNombre = _inmuebleSeleccionado.Nombre;
                    } else {
                        _tiendaId = 0;
                        TiendaNombre = string.Empty;
                    }
                }
            }
        }

        public ObservableCollection<SeccionTemplate> Secciones { get; set; } = new ObservableCollection<SeccionTemplate>();

        public ICommand CargarTemplateCommand { get; }
        public ICommand EnviarChecklistCommand { get; }

        public ChecklistViewModel() {
            _httpClient = new HttpClient();

            CargarTemplateCommand = new Command(async () => await CargarTemplateAsync());
            EnviarChecklistCommand = new Command(async () => await EnviarChecklistAsync());

            Task.Run(async () => {
                await CargarTemplateAsync();
                await CargarInmueblesClienteAsync();
            });
        }

        private async Task CargarInmueblesClienteAsync() {
            try {
                int idEstadoDefault = 0;

                string url = $"{BaseApiUrl}Sucursales?idcliente={_clienteId}&idestado={idEstadoDefault}";

                List<Inmueble>? inmueblesDescargados = null;

                if(InternetUtil.IsConnectedInternet()) {
                    var response = await _httpClient.GetAsync(url);
                    if(response.IsSuccessStatusCode) {
                        inmueblesDescargados = await response.Content.ReadFromJsonAsync<List<Inmueble>>();
                    }
                } else {
                    var _dbContext = new DbContext();
                    inmueblesDescargados = await _dbContext.GetinmueblesLocal(_clienteId, idEstadoDefault);
                }

                if(inmueblesDescargados != null) {
                    MainThread.BeginInvokeOnMainThread(() => {
                        ListaInmuebles.Clear();
                        foreach(var inmueble in inmueblesDescargados) {
                            ListaInmuebles.Add(inmueble);
                        }
                    });
                }
            } catch(Exception ex) {
                Console.WriteLine($"Error al precargar la lista de inmuebles: {ex.Message}");
                await Toast.Make($"Error al precargar la lista de inmuebles: {ex.Message}", ToastDuration.Short).Show();

            }
        }

        // 1. OBTENER EL CATÁLOGO DESDE LA API
        private async Task CargarTemplateAsync() {
            if(IsLoading) return;
            IsLoading = true;

            try {
                var url = $"{BaseApiUrl}checklists/template-actual";
                var resultado = await _httpClient.GetFromJsonAsync<List<SeccionTemplate>>(url);

                MainThread.BeginInvokeOnMainThread(() => {
                    Secciones.Clear();
                    if(resultado != null) {
                        foreach(var seccion in resultado) {
                            Secciones.Add(seccion);
                        }
                    }
                });
            } catch(Exception ex) {
                MainThread.BeginInvokeOnMainThread(async () => {
                    await Application.Current!.MainPage!.DisplayAlert("Error", $"No se pudo descargar el catálogo: {ex.Message}", "OK");
                });
            } finally {
                IsLoading = false;
            }
        }

        // 2. ENVIAR EL PAYLOAD DE RESPUESTAS A LA API (HTTP POST)
        private async Task EnviarChecklistAsync() {
            if(string.IsNullOrWhiteSpace(TiendaNombre) || _tiendaId == 0) {
                await Toast.Make("Por favor selecciona una Tienda / Sucursal obligatoriamente.", ToastDuration.Short).Show();

                return;
            }

            IsLoading = true;

            try {
                var detallesEnvio = new List<object>();

                foreach(var seccion in Secciones) {
                    foreach(var pregunta in seccion.Preguntas) {
                        string? valorFinal = pregunta.ValorRespondido;

                        if(pregunta.TipoDatoId == 1 && string.IsNullOrEmpty(valorFinal)) {
                            valorFinal = pregunta.RespuestaBool ? "1" : "0";
                        }

                        if(pregunta.TipoDatoId == 5 && string.IsNullOrEmpty(valorFinal)) {
                            valorFinal = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        }

                        detallesEnvio.Add(new {
                            PreguntaId = pregunta.Id,
                            ValorRespondido = valorFinal,
                            Observaciones = pregunta.Observaciones
                        });
                    }
                }

                var payload = new {
                    ClienteId = this.ClienteId,
                    TiendaId = this.TiendaId,
                    GerenteNombre = this.GerenteNombre,
                    FechaRegistro = DateTime.Now,
                    Detalles = detallesEnvio
                };

                var url = $"{BaseApiUrl}evaluaciones";
                var jsonPayload = System.Text.Json.JsonSerializer.Serialize(payload);
                var response = await _httpClient.PostAsJsonAsync(url, payload);

                if(response.IsSuccessStatusCode) {
                    //await Application.Current!.MainPage!.DisplayAlert("Éxito", "¡Checklist de supervisión guardado y enviado correctamente!", "OK");
                    await Toast.Make($"¡Checklist de supervisión guardado y enviado correctamente!", ToastDuration.Long).Show();

                    GerenteNombre = string.Empty;
                    InmuebleSeleccionado = null;

                    IsLoading = false;

                    await CargarTemplateAsync();
                    await CargarInmueblesClienteAsync();
                } else {
                    await Toast.Make($"El servidor respondió con código: {response.StatusCode}", ToastDuration.Short).Show();

                }
            } catch(Exception ex) {
                await Toast.Make($"Ocurrió un error al enviar: {ex.Message}", ToastDuration.Short).Show();

            } finally {
                IsLoading = false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}