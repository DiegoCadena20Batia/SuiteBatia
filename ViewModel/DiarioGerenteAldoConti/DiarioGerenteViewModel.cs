using BatiaSuite.Data;
using BatiaSuite.Models.CheckListSupervisionesAldoConti.singamobiletest.Models;
using BatiaSuite.Models.Supervision;
using BatiaSuite.Selectors;
using BatiaSuite.Utils;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BatiaSuite.ViewModel.DiarioGerenteAldoConti {

    public class DiarioGerenteViewModel : INotifyPropertyChanged {
        private readonly HttpClient _httpClient;
        private const string BaseApiUrl = $"{Constants.API_BASE_URL}";

        private readonly string _nombreSupervisor = UserSession.NOMBRE;
        public string NombreSupervisor => _nombreSupervisor;

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

        private bool _showClearSupervisor;

        public bool ShowClearSupervisor {
            get => _showClearSupervisor;
            set { _showClearSupervisor = value; OnPropertyChanged(); }
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
        public ICommand DrawSupervisorCommand { get; }
        public ICommand ClearSupervisorCommand { get; }
        public ICommand DrawGerenteCommand { get; }
        public ICommand ClearGerenteCommand { get; }

        public DiarioGerenteViewModel() {
            _httpClient = new HttpClient();

            CargarTemplateCommand = new Command(async () => await CargarTemplateAsync());
            EnviarChecklistCommand = new Command(async () => await EnviarChecklistAsync());

            DrawSupervisorCommand = new Command(OnDrawSupervisor);
            ClearSupervisorCommand = new Command(OnClearSupervisor);
            DrawGerenteCommand = new Command(OnDrawGerente);
            ClearGerenteCommand = new Command(OnClearGerente);

            Task.Run(async () => {
                await CargarTemplateAsync();
                await CargarInmueblesClienteAsync();
            });
        }

        public static event Action? OnClearSupervisorRequested;

        public static event Action? OnClearGerenteRequested;

        private void OnDrawSupervisor() {
        }

        private void OnClearSupervisor() {
            OnClearSupervisorRequested?.Invoke();
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

        private async Task CargarTemplateAsync() {
            if(IsLoading) return;
            IsLoading = true;

            try {
                var url = $"{BaseApiUrl}estructura/DiarioGerente";
                var resultado = await _httpClient.GetFromJsonAsync<List<SeccionTemplate>>(url);

                // ¡Todo lo que toque colecciones que van a la vista debe estar aquí adentro!
                MainThread.BeginInvokeOnMainThread(() => {
                    try {
                        Secciones.Clear();
                        if(resultado != null) {
                            foreach(var seccion in resultado) {
                                foreach(var pregunta in seccion.Preguntas) {
                                    if(!string.IsNullOrEmpty(pregunta.TextoPregunta) &&
                                        pregunta.TextoPregunta.StartsWith("TABLA:", StringComparison.OrdinalIgnoreCase)) {
                                        int filasGenerar = (pregunta.Id == 612 || pregunta.TextoPregunta.Contains("VENDEDOR", StringComparison.OrdinalIgnoreCase)) ? 6 : 1;

                                        // Instanciamos la colección reactiva de manera correcta en el hilo de UI
                                        pregunta.FilasTablaVentas = new System.Collections.ObjectModel.ObservableCollection<FilaVentaModel>();

                                        for(int i = 1; i <= filasGenerar; i++) {
                                            pregunta.FilasTablaVentas.Add(new FilaVentaModel { NumeroFila = i });
                                        }
                                    }
                                }
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
        public byte[]? FirmaSupervisorBytes { get; set; }
        public byte[]? FirmaGerenteBytes { get; set; }

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

            MainThread.BeginInvokeOnMainThread(() => {
                Shell.Current?.CurrentPage?.Focus();
            });

            await Task.Delay(100);

            IsLoading = true;

            try {
                var respuestasEnvio = new List<object>();

                foreach(var seccion in Secciones) {
                    foreach(var pregunta in seccion.Preguntas) {
                        string? valorFinal = pregunta.ValorRespondido;

                        // Si es una tabla dinámica, convertimos sus filas capturadas en formato JSON plano
                        if(!string.IsNullOrEmpty(pregunta.TextoPregunta) &&
                            pregunta.TextoPregunta.StartsWith("TABLA:", StringComparison.OrdinalIgnoreCase)) {
                            valorFinal = JsonSerializer.Serialize(pregunta.FilasTablaVentas);
                        } else if(pregunta.TipoDatoId == 1 && string.IsNullOrEmpty(valorFinal)) {
                            valorFinal = pregunta.RespuestaBool ? "1" : "0";
                        } else if(pregunta.TipoDatoId == 5 && string.IsNullOrEmpty(valorFinal)) {
                            valorFinal = DateTime.Now.ToString("yyyy-MM-dd");
                        }

                        respuestasEnvio.Add(new {
                            PreguntaId = pregunta.Id,
                            ValorRespondido = valorFinal,
                            Observaciones = pregunta.Observaciones ?? string.Empty
                        });
                    }
                }

                string? firmaSupervisorBase64 = FirmaSupervisorBytes != null ? Convert.ToBase64String(FirmaSupervisorBytes) : null;
                string? firmaGerenteBase64 = FirmaGerenteBytes != null ? Convert.ToBase64String(FirmaGerenteBytes) : null;

                var payload = new {
                    SucursalId = this.TiendaId,
                    UsuarioId = UserSession.IdPersonal,
                    GerenteNombre = this.GerenteNombre,
                    FechaRegistro = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Detalles = respuestasEnvio,
                    FirmaSupervisor = firmaSupervisorBase64,
                    FirmaGerente = firmaGerenteBase64
                };

                var url = $"{BaseApiUrl}ChecklistAC/diariogerente";
                var json = JsonSerializer.Serialize(payload);
                var response = await _httpClient.PostAsJsonAsync(url, payload);

                if(response.IsSuccessStatusCode) {
                    await Toast.Make($"¡Checklist Diario Gerente y firmas enviados con éxito!", ToastDuration.Short).Show();
                    await Shell.Current.GoToAsync("//MyMenu");

                    // Limpieza de estados del usuario
                    GerenteNombre = string.Empty;
                    InmuebleSeleccionado = null;
                    FirmaSupervisorBytes = null;
                    FirmaGerenteBytes = null;
                    ShowClearSupervisor = false;
                    ShowClearGerente = false;

                    OnClearSupervisorRequested?.Invoke();
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