using BatiaSuite.Data;
using BatiaSuite.Models.CheckListSupervisionesAldoConti.singamobiletest.Models;
using BatiaSuite.Models.Supervision;
using BatiaSuite.Utils;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace BatiaSuite.ViewModel.CheckListSupervisionesAldoConti {

    public class AparadoristasViewModel : INotifyPropertyChanged {
        private readonly HttpClient _httpClient;
        private const string BaseApiUrl = $"{Constants.API_BASE_URL}";

        private readonly string _nommbreAparadorista = UserSession.NOMBRE;
        public string NombreAparadorista => _nommbreAparadorista;

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

        private DateTime _fechaUltimaVisita = DateTime.Today;

        public DateTime FechaUltimaVisita {
            get => _fechaUltimaVisita;
            set { _fechaUltimaVisita = value; OnPropertyChanged(); }
        }

        private bool _showClearAparadorista;

        public bool ShowClearAparadorista {
            get => _showClearAparadorista;
            set { _showClearAparadorista = value; OnPropertyChanged(); }
        }

        private bool _showClearGerente;

        public bool ShowClearGerente {
            get => _showClearGerente;
            set { _showClearGerente = value; OnPropertyChanged(); }
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

        public ICommand DrawAparadoristaCommand { get; }
        public ICommand ClearAparadoristaCommand { get; }
        public ICommand DrawGerenteCommand { get; }
        public ICommand ClearGerenteCommand { get; }

        public AparadoristasViewModel() {
            _httpClient = new HttpClient();

            CargarTemplateCommand = new Command(async () => await CargarTemplateAsync());
            EnviarChecklistCommand = new Command(async () => await EnviarChecklistAsync());

            DrawAparadoristaCommand = new Command(OnDrawAparadorista);
            ClearAparadoristaCommand = new Command(OnClearAparadorista);
            DrawGerenteCommand = new Command(OnDrawGerente);
            ClearGerenteCommand = new Command(OnClearGerente);

            Task.Run(async () => {
                await CargarTemplateAsync();
                await CargarInmueblesClienteAsync();
            });
        }

        public static event Action? OnClearAparadoristaRequested;

        public static event Action? OnClearGerenteRequested;

        private void OnDrawAparadorista() {
        }

        private void OnClearAparadorista() {
            OnClearAparadoristaRequested?.Invoke();
        }

        private void OnDrawGerente() {
        }

        private void OnClearGerente() {
            OnClearGerenteRequested?.Invoke();
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

        // ====================================================================
        // 1. OBTENER EL CATÁLOGO DINÁMICO DE APARADORISTAS (HTTP GET)
        // ====================================================================
        private async Task CargarTemplateAsync() {
            if(IsLoading) return;
            IsLoading = true;

            try {
                var url = $"{BaseApiUrl}estructura/Aparadorista";
                var resultado = await _httpClient.GetFromJsonAsync<List<SeccionTemplate>>(url);

                MainThread.BeginInvokeOnMainThread(() => {
                    try {
                        Secciones.Clear();
                        if(resultado != null) {
                            foreach(var seccion in resultado) {
                                Secciones.Add(seccion);
                            }
                        }
                    } finally {
                        IsLoading = false;
                    }
                });
            } catch(Exception ex) {
                MainThread.BeginInvokeOnMainThread(async () => {
                    IsLoading = false;
                    await Toast.Make($"No se pudo descargar el catálogo: {ex.Message}", ToastDuration.Short).Show();

                });
            }
        }

        public Func<Task<bool>>? AntesDeEnviarChecklist { get; set; }
        public byte[]? FirmaAparadoristaBytes { get; set; }
        public byte[]? FirmaGerenteBytes { get; set; }

        // ====================================================================
        // 2. ENVIAR EL PAYLOAD DE RESPUESTAS Y FIRMAS (MULTIPART POST)
        // ====================================================================
        private async Task EnviarChecklistAsync() {
            if(string.IsNullOrWhiteSpace(TiendaNombre) || _tiendaId == 0) {
                await Toast.Make($"Por favor selecciona una Tienda / Sucursal obligatoriamente.", ToastDuration.Short).Show();
                return;
            }

            if(string.IsNullOrWhiteSpace(GerenteNombre)) {
                await Toast.Make($"Por favor ingresa el nombre del Gerente.", ToastDuration.Short).Show();
                return;
            }

            if(AntesDeEnviarChecklist != null) {
                bool firmasValidas = await AntesDeEnviarChecklist.Invoke();
                if(!firmasValidas) return;
            }

            IsLoading = true;

            try {
                var respuestasEnvio = new List<object>();

                foreach(var seccion in Secciones) {
                    foreach(var pregunta in seccion.Preguntas) {
                        string? valorFinal = pregunta.ValorRespondido;

                        if(pregunta.TipoDatoId == 1 && string.IsNullOrEmpty(valorFinal)) {
                            valorFinal = pregunta.RespuestaBool ? "1" : "0";
                        }

                        if(pregunta.TipoDatoId == 5 && string.IsNullOrEmpty(valorFinal)) {
                            valorFinal = DateTime.Now.ToString("yyyy-MM-dd");
                        }

                        respuestasEnvio.Add(new {
                            PreguntaId = pregunta.Id,
                            ValorRespondido = valorFinal,
                            Observaciones = pregunta.Observaciones ?? string.Empty
                        });
                    }
                }

                string? firmaAparadoristaBase64 = FirmaAparadoristaBytes != null ? Convert.ToBase64String(FirmaAparadoristaBytes) : null;
                string? firmaGerenteBase64 = FirmaGerenteBytes != null ? Convert.ToBase64String(FirmaGerenteBytes) : null;

                var payload = new {
                    SucursalId = this.TiendaId,
                    UsuarioId = UserSession.IdPersonal,
                    GerenteNombre = this.GerenteNombre,
                    FechaUltimaVisita = this.FechaUltimaVisita.ToString("yyyy-MM-dd"),
                    FechaRegistro = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Detalles = respuestasEnvio,
                    FirmaAparadorista = firmaAparadoristaBase64, 
                    FirmaGerente = firmaGerenteBase64           
                };

                var url = $"{BaseApiUrl}aparadoristas";

                var response = await _httpClient.PostAsJsonAsync(url, payload);

                if(response.IsSuccessStatusCode) {
                    await Toast.Make($"¡Checklist de aparadores y firmas enviados con éxito!", ToastDuration.Short).Show();
                    await Shell.Current.GoToAsync("//MyMenu");
                    // Limpieza de estados e  de usuario
                    GerenteNombre = string.Empty;
                    InmuebleSeleccionado = null;
                    FechaUltimaVisita = DateTime.Today;

                    FirmaAparadoristaBytes = null;
                    FirmaGerenteBytes = null;

                    ShowClearAparadorista = false;
                    ShowClearGerente = false;

                    OnClearAparadoristaRequested?.Invoke();
                    OnClearGerenteRequested?.Invoke();

                    await CargarTemplateAsync();
                } else {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    await Toast.Make($"Error del servidor ({response.StatusCode}): {errorContent}", ToastDuration.Long).Show();
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