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
                    await Application.Current!.MainPage!.DisplayAlert("Error", $"No se pudo descargar el catálogo: {ex.Message}", "OK");
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
                await Application.Current!.MainPage!.DisplayAlert("Atención", "Por favor selecciona una Tienda / Sucursal obligatoriamente.", "OK");
                return;
            }

            if(string.IsNullOrWhiteSpace(GerenteNombre)) {
                await Application.Current!.MainPage!.DisplayAlert("Atención", "Por favor ingresa el nombre del Gerente.", "OK");
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

                //string? firmaAparadoristaJson = FirmaAparadoristaBytes != null
                //    ? $"[{string.Join(",", FirmaAparadoristaBytes)}]"
                //    : null;

                //string? firmaGerenteJson = FirmaGerenteBytes != null
                //    ? $"[{string.Join(",", FirmaGerenteBytes)}]"
                //    : null;

                var payload = new {
                    SucursalId = this.TiendaId,
                    UsuarioId = UserSession.IdPersonal,
                    GerenteNombre = this.GerenteNombre,
                    FechaUltimaVisita = this.FechaUltimaVisita.ToString("yyyy-MM-dd"),
                    FechaRegistro = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Detalles = respuestasEnvio, 
                    FirmaAparadorista = FirmaAparadoristaBytes, 
                    FirmaGerente = FirmaGerenteBytes
                };

                using var content = new MultipartFormDataContent();

                // 1. Adjuntar el JSON del formato original
                var jsonPayload = System.Text.Json.JsonSerializer.Serialize(payload);
                content.Add(new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json"), "DatosChecklist");

                var url = $"{BaseApiUrl}aparadoristas";
                var response = await _httpClient.PostAsync(url, content);

                if(response.IsSuccessStatusCode) {
                    await Application.Current!.MainPage!.DisplayAlert("Éxito", "¡Checklist de aparadores y firmas enviados con éxito!", "OK");

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
                    await Application.Current!.MainPage!.DisplayAlert("Error", $"El servidor respondió con código: {response.StatusCode}", "OK");
                }
            } catch(Exception ex) {
                await Application.Current!.MainPage!.DisplayAlert("Error", $"Ocurrió un error al enviar: {ex.Message}", "OK");
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