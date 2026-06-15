using BatiaSuite.Data;
using BatiaSuite.Models.CheckListSupervisionesAldoConti.singamobiletest.Models;
using BatiaSuite.Models.Supervision;
using BatiaSuite.Selectors;
using BatiaSuite.Utils;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel; // <-- Agregado para usar ObservableObject
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json; // <-- Agregado para serializar la tabla de ventas
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BatiaSuite.ViewModel.ReporteMantenimientoAldoConti {

    // Cambiamos herencia a ObservableObject para unificar la arquitectura reactiva
    public class ReporteMantenimientoViewModel : ObservableObject {

        #region Variables y campos

        private readonly HttpClient _httpClient;
        private const string BaseApiUrl = $"{Constants.API_BASE_URL}";

        private readonly string _nombreUsuario = UserSession.NOMBRE;
        public string NombreUsuario => _nombreUsuario;

        private readonly int _clienteId = UserSession.Cliente;
        public int ClienteId => _clienteId;

        private int _tiendaId = 0;
        public int TiendaId => _tiendaId;

        private bool _isLoading;

        public bool IsLoading {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private string _tiendaNombre = string.Empty;

        public string TiendaNombre {
            get => _tiendaNombre;
            set => SetProperty(ref _tiendaNombre, value);
        }

        private string _gerenteNombre = string.Empty;

        public string GerenteNombre {
            get => _gerenteNombre;
            set => SetProperty(ref _gerenteNombre, value);
        }

        private bool _showClearResponsable;

        public bool ShowClearResponsable {
            get => _showClearResponsable;
            set => SetProperty(ref _showClearResponsable, value);
        }

        private bool _showClearTienda;

        public bool ShowClearTienda {
            get => _showClearTienda;
            set => SetProperty(ref _showClearTienda, value);
        }

        public ObservableCollection<Inmueble> ListaInmuebles { get; set; } = new ObservableCollection<Inmueble>();

        private Inmueble? _inmuebleSeleccionado;

        public Inmueble? InmuebleSeleccionado {
            get => _inmuebleSeleccionado;
            set {
                if(SetProperty(ref _inmuebleSeleccionado, value)) {
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
        public ICommand DrawResponsableCommand { get; }
        public ICommand ClearResponsableCommand { get; }
        public ICommand DrawTiendaCommand { get; }
        public ICommand ClearTiendaCommand { get; }

        #endregion Variables y campos

        public ReporteMantenimientoViewModel() {
            _httpClient = new HttpClient();

            CargarTemplateCommand = new Command(async () => await CargarTemplateAsync());
            EnviarChecklistCommand = new Command(async () => await EnviarChecklistAsync());

            DrawResponsableCommand = new Command(OnDrawResponsable);
            ClearResponsableCommand = new Command(OnClearResponsable);
            DrawTiendaCommand = new Command(OnDrawTienda);
            ClearTiendaCommand = new Command(OnClearTienda);

            // Inicialización segura de datos en segundo plano
            _ = InicializarDatosAsync();
        }

        private async Task InicializarDatosAsync() {
            await CargarTemplateAsync();
            await CargarInmueblesClienteAsync();
        }

        public static event Action? OnClearResponsableRequested;

        public static event Action? OnClearTiendaRequested;

        private void OnDrawResponsable() {
        }

        private void OnClearResponsable() => OnClearResponsableRequested?.Invoke();

        private void OnDrawTienda() {
        }

        private void OnClearTienda() => OnClearTiendaRequested?.Invoke();

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
                var url = $"{BaseApiUrl}estructura/ReporteMantenimiento";
                var resultado = await _httpClient.GetFromJsonAsync<List<SeccionTemplate>>(url);

                MainThread.BeginInvokeOnMainThread(() => {
                    try {
                        Secciones.Clear();
                        if(resultado != null) {
                            foreach(var seccion in resultado) {
                                foreach(var pregunta in seccion.Preguntas) {
                                    if(pregunta.TipoDatoId == 6) {
                                        pregunta.OpcionesDisponibles = new List<string> { "BUENO", "REGULAR", "MALO" };
                                    }
                                    if(pregunta.TipoDatoId == 7) {
                                        pregunta.OpcionesDisponibles = new List<string> { "PREVENTIVO", "CORRECTIVO" };
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
        public byte[]? FirmaResponsableBytes { get; set; }
        public byte[]? FirmaTiendaBytes { get; set; }

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

                        if(pregunta.TipoDatoId == 6 || pregunta.TipoDatoId == 7) {
                            valorFinal = pregunta.ValorSeleccionado;
                        }

                        respuestasEnvio.Add(new {
                            PreguntaId = pregunta.Id,
                            ValorRespondido = valorFinal ?? string.Empty,
                            Observaciones = pregunta.Observaciones ?? string.Empty
                        });
                    }
                }

                string? firmaResponsableBase64 = FirmaResponsableBytes != null ? Convert.ToBase64String(FirmaResponsableBytes) : null;
                string? firmaTiendaBase64 = FirmaTiendaBytes != null ? Convert.ToBase64String(FirmaTiendaBytes) : null;

                var payload = new {
                    SucursalId = this.TiendaId,
                    UsuarioId = UserSession.IdPersonal,
                    GerenteNombre = this.GerenteNombre,
                    FechaRegistro = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Detalles = respuestasEnvio,
                    FirmaResponsable = firmaResponsableBase64,
                    FirmaTienda = firmaTiendaBase64
                };

                var url = $"{BaseApiUrl}ChecklistAC/mantenimiento";
                var jsonPayload = JsonSerializer.Serialize(payload);
                var response = await _httpClient.PostAsJsonAsync(url, payload);

                if(response.IsSuccessStatusCode) {
                    await Toast.Make($"¡Checklist enviado con éxito!", ToastDuration.Short).Show();
                    await Shell.Current.GoToAsync("//MyMenu");

                    GerenteNombre = string.Empty;
                    InmuebleSeleccionado = null;
                    FirmaResponsableBytes = null;
                    FirmaTiendaBytes = null;
                    ShowClearResponsable = false;
                    ShowClearTienda = false;

                    OnClearResponsableRequested?.Invoke();
                    OnClearTiendaRequested?.Invoke();

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
    }
}