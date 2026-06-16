using BatiaSuite.Data;
using BatiaSuite.Models.Supervision;
using BatiaSuite.Utils;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using BatiaSuite.Models.DiarioLimpieza;

namespace BatiaSuite.ViewModel.ChecklistMonitoreo {
    public class DiarioLimpiezaViewModel : INotifyPropertyChanged {

        #region CAMPOS Y PROPIEDADES
        private readonly HttpClient _httpClient;
        private const string BaseApiUrl = $"{Constants.API_BASE_URL}";

        private readonly string _nombreEmpleado = UserSession.NOMBRE;
        public string NombreEmpleado => _nombreEmpleado;

        private readonly int _clienteId = UserSession.Cliente;
        public int ClienteId => _clienteId;

        private int _tiendaId = 0;
        public int TiendaId => _tiendaId;

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

        private bool _showClearEmpleado;
        public bool ShowClearEmpleado {
            get => _showClearEmpleado;
            set { _showClearEmpleado = value; OnPropertyChanged(); }
        }

        private bool _showClearGerente;
        public bool ShowClearGerente {
            get => _showClearGerente;
            set { _showClearGerente = value; OnPropertyChanged(); }
        }

        // --- CONTROL DE MULTIMEDIA ---
        public List<string> FotosBase64 { get; set; } = new List<string>();

        private string _conteoFotos = "Fotos: 0 / 8";
        public string ConteoFotos {
            get => _conteoFotos;
            set { _conteoFotos = value; OnPropertyChanged(); }
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

        // --- LA COLECCIÓN AHORA ES PLANA (COINCIDE CON LA TABLA) ---
        public ObservableCollection<DiarioLimpiezaItem> TareasLimpieza { get; set; } = new ObservableCollection<DiarioLimpiezaItem>();

        // --- COMANDOS ---
        public ICommand CargarTemplateCommand { get; }
        public ICommand EnviarChecklistCommand { get; }
        public ICommand TomarFotoCommand { get; }

        public ICommand DrawEmpleadoCommand { get; }
        public ICommand ClearEmpleadoCommand { get; }
        public ICommand DrawGerenteCommand { get; }
        public ICommand ClearGerenteCommand { get; }

        #endregion

        public DiarioLimpiezaViewModel() {
            _httpClient = new HttpClient();

            CargarTemplateCommand = new Command(async () => await CargarTemplateAsync());
            EnviarChecklistCommand = new Command(async () => await EnviarChecklistAsync());
            TomarFotoCommand = new Command(async () => await TomarFotoAsync());

            DrawEmpleadoCommand = new Command(OnDrawEmpleado);
            ClearEmpleadoCommand = new Command(OnClearEmpleado);
            DrawGerenteCommand = new Command(OnDrawGerente);
            ClearGerenteCommand = new Command(OnClearGerente);

            Task.Run(async () => {
                await CargarTemplateAsync();
                await CargarInmueblesClienteAsync();
            });
        }

        public static event Action? OnClearMonitoreoRequeste;
        public static event Action? OnClearGerenteRequested;

        private void OnDrawEmpleado() { }
        private void OnClearEmpleado() { OnClearMonitoreoRequeste?.Invoke(); }
        private void OnDrawGerente() { }
        private void OnClearGerente() { OnClearGerenteRequested?.Invoke(); }

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
            }
        }

        // ====================================================================
        // 1. GET: CARGA PLANA DIRECTA DESDE TU NUEVA RUTA DE LIMPIEZA
        // ====================================================================
        private async Task CargarTemplateAsync() {
            if(IsLoading) return;
            IsLoading = true;

            try {
                var url = $"{BaseApiUrl}limpieza/nuevo";
                var resultadoDto = await _httpClient.GetFromJsonAsync<ChecklistFormularioDto>(url);

                MainThread.BeginInvokeOnMainThread(() => {
                    try {
                        TareasLimpieza.Clear();
                        if(resultadoDto != null && resultadoDto.Directivas != null) {
                            foreach(var item in resultadoDto.Directivas) {
                                // Insertamos directamente el objeto DiarioLimpiezaItem que la UI va a leer de forma corrida
                                TareasLimpieza.Add(item);
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

        // ====================================================================
        // 2. CAPTURA DE FOTOS HASTA LLEGAR A UN MÁXIMO DE 8
        // ====================================================================
        private async Task TomarFotoAsync() {
            if(FotosBase64.Count >= 8) {
                await Toast.Make("Ya has capturado el límite máximo de 8 fotografías.", ToastDuration.Short).Show();
                return;
            }

            try {
                if(MediaPicker.Default.IsCaptureSupported) {
                    FileResult photo = await MediaPicker.Default.CapturePhotoAsync();

                    if(photo != null) {
                        IsLoading = true;

                        using Stream sourceStream = await photo.OpenReadAsync();
                        using MemoryStream ms = new MemoryStream();
                        await sourceStream.CopyToAsync(ms);
                        byte[] imageBytes = ms.ToArray();

                        string base64String = Convert.ToBase64String(imageBytes);
                        FotosBase64.Add(base64String);

                        ConteoFotos = $"Fotos: {FotosBase64.Count} / 8";

                        await Toast.Make($"Foto {FotosBase64.Count} / 8 capturada.", ToastDuration.Short).Show();
                    }
                }
            } catch(Exception ex) {
                await Toast.Make($"Error en cámara: {ex.Message}", ToastDuration.Short).Show();
            } finally {
                IsLoading = false;
            }
        }

        public Func<Task<bool>>? AntesDeEnviarChecklist { get; set; }
        public byte[]? FirmaEmpleadoBytes { get; set; }
        public byte[]? FirmaGerenteBytes { get; set; }

        // ====================================================================
        // 3. POST: ENVÍO FLUIDO SIN ITERACIONES COMPLEJAS DE SECCIONES
        // ====================================================================
        private async Task EnviarChecklistAsync() {
            if(string.IsNullOrWhiteSpace(TiendaNombre) || _tiendaId == 0) {
                await Toast.Make($"Por favor selecciona una Sucursal.", ToastDuration.Short).Show();
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
                string? firmaGerenteBase64 = FirmaGerenteBytes != null ? Convert.ToBase64String(FirmaGerenteBytes) : null;

                var payload = new ChecklistFormularioDto {
                    SucursalId = this.TiendaId,
                    GerenteNombre = this.GerenteNombre,
                    FechaRegistro = DateTime.Today,
                    ColaboradoresRol = "ASIGNADOS",
                    FirmaGerente = firmaGerenteBase64,
                    FotosBase64 = this.FotosBase64,
                    Directivas = this.TareasLimpieza.ToList() 
                };

                var url = $"{BaseApiUrl}limpieza/guardadiario";
                var jsonPayload = System.Text.Json.JsonSerializer.Serialize(payload);   
                var response = await _httpClient.PostAsJsonAsync(url, payload);

                if(response.IsSuccessStatusCode) {
                    await Toast.Make($"¡Checklist enviado con éxito!", ToastDuration.Short).Show();

                    FotosBase64.Clear();
                    ConteoFotos = "Fotos: 0 / 8";
                    FirmaEmpleadoBytes = null;
                    FirmaGerenteBytes = null;
                    GerenteNombre = string.Empty;
                    InmuebleSeleccionado = null;

                    ShowClearEmpleado = false;
                    ShowClearGerente = false;

                    OnClearMonitoreoRequeste?.Invoke();
                    OnClearGerenteRequested?.Invoke();

                    await Shell.Current.GoToAsync("//MyMenu");
                } else {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    await Toast.Make($"Error: {errorContent}", ToastDuration.Long).Show();
                }
            } catch(Exception ex) {
                await Toast.Make($"Error al enviar: {ex.Message}", ToastDuration.Short).Show();
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