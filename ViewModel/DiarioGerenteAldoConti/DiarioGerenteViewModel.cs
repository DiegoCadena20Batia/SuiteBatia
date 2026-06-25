using BatiaSuite.Data;
using BatiaSuite.Models.CheckListSupervisionesAldoConti.singamobiletest.Models;
using BatiaSuite.Models.EntidadesLocal;
using BatiaSuite.Models.Supervision;
using BatiaSuite.Selectors;
using BatiaSuite.Utils;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BatiaSuite.ViewModel.DiarioGerenteAldoConti {
    public partial class DiarioGerenteViewModel : ObservableObject {
        private readonly HttpClient _httpClient;
        private const string BaseApiUrl = $"{Constants.API_BASE_URL}";

        [ObservableProperty]
        private string _nombreSupervisor = UserSession.NOMBRE;

        [ObservableProperty]
        private int _clienteId = UserSession.Cliente;

        [ObservableProperty]
        private int _tiendaId = 0;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _tiendaNombre = string.Empty;

        [ObservableProperty]
        private string _gerenteNombre = string.Empty;

        [ObservableProperty]
        private bool _showClearSupervisor;

        [ObservableProperty]
        private bool _showClearGerente;

        public ObservableCollection<Inmueble> ListaInmuebles { get; } = new();
        public ObservableCollection<SeccionTemplate> Secciones { get; } = new();

        private Inmueble? _inmuebleSeleccionado;
        public Inmueble? InmuebleSeleccionado {
            get => _inmuebleSeleccionado;
            set {
                if(SetProperty(ref _inmuebleSeleccionado, value)) {
                    if(value != null) {
                        TiendaId = value.IdInmueble;
                        TiendaNombre = value.Nombre;
                    } else {
                        TiendaId = 0;
                        TiendaNombre = string.Empty;
                    }
                }
            }
        }

        public Func<Task<bool>>? AntesDeEnviarChecklist { get; set; }
        public byte[]? FirmaSupervisorBytes { get; set; }
        public byte[]? FirmaGerenteBytes { get; set; }

        public static event Action? OnClearSupervisorRequested;
        public static event Action? OnClearGerenteRequested;

        public DiarioGerenteViewModel() {
            _httpClient = new HttpClient();

            // Inicialización segura de catálogos sin congelar el constructor
            _ = InitializeAsync();
        }

        private async Task InitializeAsync() {
            await CargarInmueblesClienteAsync();
            await CargarTemplateAsync();
        }

        [RelayCommand]
        private void ClearSupervisor() {
            OnClearSupervisorRequested?.Invoke();
        }

        [RelayCommand]
        private void ClearGerente() {
            OnClearGerenteRequested?.Invoke();
        }

        [RelayCommand]
        private async Task CargarInmueblesClienteAsync() {
            try {
                int idEstadoDefault = 0;
                var localDb = new LocalDbContext();

              
                List<InmuebleEntity> inmueblesLocal = await localDb.ObtenerListaLocalAsync<InmuebleEntity>(x =>
                    x.IdCliente == ClienteId 
                );

                MainThread.BeginInvokeOnMainThread(() => {
                    ListaInmuebles.Clear();
                    foreach(var entity in inmueblesLocal) {
                        ListaInmuebles.Add(new Inmueble {
                            IdInmueble = entity.IdInmueble,
                            Nombre = entity.Nombre
                        });
                    }
                });
            } catch(Exception ex) {
                Console.WriteLine($"Error al leer inmuebles de la BD local: {ex.Message}");
                await Toast.Make($"Error al cargar inmuebles: {ex.Message}", ToastDuration.Short).Show();
            }
        }

        [RelayCommand]
        public async Task CargarTemplateAsync() {
            if(IsLoading) return;
            IsLoading = true;

            try {
                var localDb = new LocalDbContext();
                // Extraemos el JSON plano que se guardó en la sincronización previa
                var cache = await localDb.BuscarLocalAsync<CatalogoCacheEntity>(x => x.Clave == "DiarioGerente");
                string? jsonLocal = cache?.JsonData;

                List<SeccionTemplate>? resultado = null;

                if(!string.IsNullOrEmpty(jsonLocal)) {
                    // Configuramos las opciones para que sea tolerante con las mayúsculas/minúsculas
                    var opciones = new JsonSerializerOptions {
                        PropertyNameCaseInsensitive = true
                    };

                    resultado = JsonSerializer.Deserialize<List<SeccionTemplate>>(jsonLocal, opciones);
                }

                MainThread.BeginInvokeOnMainThread(() => {
                    try {
                        Secciones.Clear();
                        if(resultado != null) {
                            foreach(var seccion in resultado) {
                                foreach(var pregunta in seccion.Preguntas) {
                                    if(!string.IsNullOrEmpty(pregunta.TextoPregunta) &&
                                        pregunta.TextoPregunta.StartsWith("TABLA:", StringComparison.OrdinalIgnoreCase)) {
                                        int filasGenerar = (pregunta.Id == 612 || pregunta.TextoPregunta.Contains("VENDEDOR", StringComparison.OrdinalIgnoreCase)) ? 6 : 1;

                                        pregunta.FilasTablaVentas = new ObservableCollection<FilaVentaModel>();

                                        for(int i = 1; i <= filasGenerar; i++) {
                                            pregunta.FilasTablaVentas.Add(new FilaVentaModel { NumeroFila = i });
                                        }
                                    }
                                }
                                Secciones.Add(seccion);
                            }
                        } else {
                            Toast.Make("Formulario no disponible. Requiere una sincronización inicial con internet.", ToastDuration.Long).Show();
                        }
                    } finally {
                        IsLoading = false;
                    }
                });
            } catch(Exception ex) {
                MainThread.BeginInvokeOnMainThread(async () => {
                    IsLoading = false;
                    await Toast.Make($"Error al procesar el formulario local: {ex.Message}", ToastDuration.Short).Show();
                });
            }
        }

        [RelayCommand]
        private async Task EnviarChecklistAsync() {
            if(string.IsNullOrWhiteSpace(TiendaNombre) || TiendaId == 0) {
                await Toast.Make("Por favor selecciona una Tienda / Sucursal obligatoriamente.", ToastDuration.Short).Show();
                return;
            }

            if(string.IsNullOrWhiteSpace(GerenteNombre)) {
                await Toast.Make("Por favor ingresa el nombre del Gerente.", ToastDuration.Short).Show();
                return;
            }

            if(AntesDeEnviarChecklist != null) {
                bool firmasValidas = await AntesDeEnviarChecklist.Invoke();
                if(!firmasValidas) return;
            }

            MainThread.BeginInvokeOnMainThread(() => Shell.Current?.CurrentPage?.Focus());
            await Task.Delay(100);

            IsLoading = true;

            try {
                // 1. Construir el cuerpo de las respuestas (idéntico a tu lógica)
                var respuestasEnvio = new List<object>();

                foreach(var seccion in Secciones) {
                    foreach(var pregunta in seccion.Preguntas) {
                        string? valorFinal = pregunta.ValorRespondido;

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

                bool guardadoOEnviadoConExito = false;

                if(Utils.InternetUtil.IsConnectedInternet()) {
                    string? firmaSupervisorBase64 = FirmaSupervisorBytes != null ? Convert.ToBase64String(FirmaSupervisorBytes) : null;
                    string? firmaGerenteBase64 = FirmaGerenteBytes != null ? Convert.ToBase64String(FirmaGerenteBytes) : null;

                    var payload = new {
                        SucursalId = TiendaId,
                        UsuarioId = UserSession.IdPersonal,
                        GerenteNombre = GerenteNombre,
                        FechaRegistro = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Detalles = respuestasEnvio,
                        FirmaSupervisor = firmaSupervisorBase64,
                        FirmaGerente = firmaGerenteBase64
                    };

                    var url = $"{BaseApiUrl}ChecklistAC/diariogerente";
                    var response = await _httpClient.PostAsJsonAsync(url, payload);

                    if(response.IsSuccessStatusCode) {
                        await Toast.Make("¡Checklist Diario Gerente enviado con éxito!", ToastDuration.Short).Show();
                        guardadoOEnviadoConExito = true;
                    } else {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        await Toast.Make($"Error del servidor ({response.StatusCode}): {errorContent}", ToastDuration.Long).Show();
                        return;
                    }
                } else {

                    var payloadTexto = new {
                        SucursalId = TiendaId,
                        UsuarioId = UserSession.IdPersonal,
                        GerenteNombre = GerenteNombre,
                        FechaRegistro = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Detalles = respuestasEnvio
                    };

                    string jsonTexto = JsonSerializer.Serialize(payloadTexto);

                    string? rutaFirmaSup = null;
                    string? rutaFirmaGer = null;
                    string carpetaCache = FileSystem.CacheDirectory;

                    if(FirmaSupervisorBytes != null) {
                        rutaFirmaSup = Path.Combine(carpetaCache, $"firma_sup_{Guid.NewGuid()}.png");
                        await File.WriteAllBytesAsync(rutaFirmaSup, FirmaSupervisorBytes);
                    }

                    if(FirmaGerenteBytes != null) {
                        rutaFirmaGer = Path.Combine(carpetaCache, $"firma_ger_{Guid.NewGuid()}.png");
                        await File.WriteAllBytesAsync(rutaFirmaGer, FirmaGerenteBytes);
                    }

                    var checklistPendiente = new ChecklistPendiente {
                        TipoChecklist = "DiarioGerente",
                        JsonData = jsonTexto,
                        RutaFirmaSupervisor = rutaFirmaSup,
                        RutaFirmaGerente = rutaFirmaGer,
                        FechaCaptura = DateTime.Now
                    };

                    var localDb = new LocalDbContext();

                    await localDb.GuardarLocalAsync<ChecklistPendiente>(checklistPendiente);

                    await App.Current.MainPage.DisplayAlert("Modo Offline", "Checklist guardado localmente en el dispositivo. Se enviará de forma automática al detectar conexión a internet.", Constants.ACEPTAR);
                    guardadoOEnviadoConExito = true;
                }

                if(guardadoOEnviadoConExito) {
                    await Shell.Current.GoToAsync("//MyMenu");

                    GerenteNombre = string.Empty;
                    InmuebleSeleccionado = null;
                    FirmaSupervisorBytes = null;
                    FirmaGerenteBytes = null;
                    ShowClearSupervisor = false;
                    ShowClearGerente = false;

                    OnClearSupervisorRequested?.Invoke();
                    OnClearGerenteRequested?.Invoke();

                    await CargarTemplateAsync();
                }
            } catch(Exception ex) {
                await Toast.Make($"Ocurrió un error al procesar el envío: {ex.Message}", ToastDuration.Short).Show();
            } finally {
                IsLoading = false;
            }
        }
    }
}