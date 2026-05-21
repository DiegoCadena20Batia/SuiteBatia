using BatiaSuite.Models.CheckListSupervisionesAldoConti;
using BatiaSuite.Models.CheckListSupervisionesAldoConti.singamobiletest.Models;
using BatiaSuite.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BatiaSuite.ViewModel.CheckListSupervisionesAldoConti {
    public class ChecklistViewModel : INotifyPropertyChanged {
        private readonly HttpClient _httpClient;
        private const string BaseApiUrl = $"{Constants.API_BASE_URL}"; // Cambia por tu URL real

        // --- PROPIEDADES DE CONTROL DE UI ---
        private bool _isLoading;
        public bool IsLoading {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        // --- PROPIEDADES DEL ENCABEZADO ---
        private string _supervisorNombre = string.Empty;
        public string SupervisorNombre {
            get => _supervisorNombre;
            set { _supervisorNombre = value; OnPropertyChanged(); }
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

        // --- LISTA DINÁMICA DE SECCIONES Y PREGUNTAS ---
        public ObservableCollection<SeccionTemplate> Secciones { get; set; } = new ObservableCollection<SeccionTemplate>();

        // --- COMANDOS ---
        public ICommand CargarTemplateCommand { get; }
        public ICommand EnviarChecklistCommand { get; }

        public ChecklistViewModel() {
            _httpClient = new HttpClient();

            CargarTemplateCommand = new Command(async () => await CargarTemplateAsync());
            EnviarChecklistCommand = new Command(async () => await EnviarChecklistAsync());

            // Carga automática del catálogo al iniciar
            Task.Run(async () => await CargarTemplateAsync());
        }

        // 1. OBTENER EL CATÁLOGO DESDE LA API (HTTP GET)
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
                await Application.Current!.MainPage!.DisplayAlert("Error", $"No se pudo descargar el catálogo: {ex.Message}", "OK");
            } finally {
                IsLoading = false;
            }
        }

        // 2. ENVIAR EL PAYLOAD DE RESPUESTAS A LA API (HTTP POST)
        private async Task EnviarChecklistAsync() {
            if(string.IsNullOrWhiteSpace(SupervisorNombre) || string.IsNullOrWhiteSpace(TiendaNombre)) {
                await Application.Current!.MainPage!.DisplayAlert("Atención", "Por favor llena los datos obligatorios del encabezado.", "OK");
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

                        if(pregunta.TipoDatoId==5 && string.IsNullOrEmpty(valorFinal)) {
                            valorFinal = DateTime.Now.ToString();
                        }

                        detallesEnvio.Add(new {
                            PreguntaId = pregunta.Id,
                            ValorRespondido = valorFinal, // Mandará "1" o "0" si es Switch, o su texto/null normal si es otra cosa
                            Observaciones = pregunta.Observaciones // Se va null o vacío de forma correcta si no escribieron nada
                        });
                    }
                }

                // Armamos el Payload transaccional EXACTAMENTE como lo espera el DTO de la API
                var payload = new {
                    SupervisorNombre = this.SupervisorNombre,
                    TiendaNombre = this.TiendaNombre,
                    GerenteNombre = this.GerenteNombre,
                    FechaRegistro = DateTime.Now,
                    Detalles = detallesEnvio
                };

                var url = $"{BaseApiUrl}evaluaciones";
                var response = await _httpClient.PostAsJsonAsync(url, payload);

                if(response.IsSuccessStatusCode) {
                    await Application.Current!.MainPage!.DisplayAlert("Éxito", "¡Checklist de supervisión guardado y enviado correctamente!", "OK");

                    // Limpiamos los campos del formulario tras un envío exitoso
                    SupervisorNombre = string.Empty;
                    TiendaNombre = string.Empty;
                    GerenteNombre = string.Empty;

                    IsLoading = false;

                    // Recargamos el catálogo limpio para una nueva auditoría
                    await CargarTemplateAsync();
                } else {
                    await Application.Current!.MainPage!.DisplayAlert("Error", $"El servidor respondió con código: {response.StatusCode}", "OK");
                }
            } catch(Exception ex) {
                await Application.Current!.MainPage!.DisplayAlert("Error de red", $"Ocurrió un error al enviar: {ex.Message}", "OK");
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
