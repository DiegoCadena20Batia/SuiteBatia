using BatiaSuite.Models.OrdenesTrabajo;
using BatiaSuite.Models.SupervisionMantenimiento.Operarios;
using BatiaSuite.Popups.SupervisionMantenimiento;
using BatiaSuite.Services.SupervisionesMantenimiento;
using BatiaSuite.Utils;
using BatiaSuite.Views.SupervisionMantenimiento.Operarios;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BatiaSuite.ViewModel.SupervisionMantenimiento.Operarios {

    public partial class SupervisionMantenimientoOperarioViewModel : ObservableObject, IQueryAttributable {
        private readonly HttpHelper _httpHelper;
        private readonly SupervisionMantenimientoStateService _stateService;

        private string baseUrlApi = Constants.API_BASE_URL;

        [ObservableProperty]
        private OrdenTrabajoModel _orden;

        [ObservableProperty]
        private bool _isLoadingSecciones;

        public ObservableCollection<SeccionModel> Secciones { get; } = new();

        public void ApplyQueryAttributes(IDictionary<string, object> query) {
            if(query.TryGetValue("OrdenSeleccionada", out var ordenObj) && ordenObj is OrdenTrabajoModel ordenValida) {
                Orden = ordenValida;
            }
        }

        public SupervisionMantenimientoOperarioViewModel(HttpHelper httpHelper, SupervisionMantenimientoStateService stateService) {
            _httpHelper = httpHelper;
            _stateService = stateService;

           
        }

        partial void OnOrdenChanged(OrdenTrabajoModel value) {
            if(value != null) {
                _ = CargarSeccionesAsync();
            }
        }

        [RelayCommand]
        private async Task CargarSeccionesAsync() {
            if(IsLoadingSecciones) return;

            try {
                IsLoadingSecciones = true;
                Secciones.Clear();

                // Petición al endpoint
                string url = $"{baseUrlApi}SupervisionMantenimeintoChecklist";
                var resultado = await _httpHelper.GetAsync<List<SeccionModel>>(url);

                if(resultado != null) {
                    foreach(var seccion in resultado) {
                        Secciones.Add(seccion);
                    }
                }
            } catch(Exception ex) {
                System.Diagnostics.Debug.WriteLine($"Error al consumir el checklist: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "No se pudo cargar el checklist de supervisión.", "OK");
            } finally {
                IsLoadingSecciones = false;
            }
        }

        [RelayCommand]
        private async Task EnviarSupervisionAsync() {
            if(IsLoadingSecciones) return;

            var popup = new CierreSupervisionPopup();
            var resultado = await Shell.Current.ShowPopupAsync(popup);
            if(resultado is not CierreSupervisionResult datosCierre)
                return;

            try {
                IsLoadingSecciones = true;

                // 0. La cabecera YA debe existir (se creó al entrar al checklist)
                int idSupervision = _stateService.IdSupervisionActual;
                if(idSupervision <= 0) {
                    await Shell.Current.DisplayAlert("Error", "No se encontró la supervisión iniciada. Vuelve a intentarlo desde el inicio.", "OK");
                    return;
                }

                // 1. Mapeo de respuestas
                var listaPreguntasDTO = Secciones
                    .Where(s => s.Preguntas != null)
                    .SelectMany(s => s.Preguntas
                        .Where(p => p.EstaRespondida)
                        .Select(p => new RespuestasDTO {
                            IdSeccion = s.IdSeccion,
                            IdPregunta = p.IdPregunta,
                            Estado = (int)p.Respuesta,
                            DispositivosPorNivel = 0,
                            Comentarios = p.Observaciones
                        }))
                    .ToList();

                // 2. Payload de cierre: SIN datos de cabecera (ya existen), solo respuestas + firma
                var payloadCierre = new SupervisionMantenimientoDTO {
                    IdSupervision = _stateService.IdSupervisionActual, // clave: le dice al backend "ya existe, no crear cabecera"
                    IdOrden= Orden.idOrden,
                    IdPersonal =UserSession.IdPersonal,
                    IdCliente=Orden.idCliente,
                    IdInmueble=Orden.idInmueble,
                    Latitud=Orden.latitud,
                    Longitud=Orden.longitud,
                    Observaciones = datosCierre.Observaciones,
                    Fechainicio=_stateService.FechaInicio,
                    Fechafin = DateTime.Now,
                    Preguntas = listaPreguntasDTO,
                    FirmasBytes = new List<FirmasSeccionDTO> {
                new FirmasSeccionDTO {
                    IdSupervision = Orden.idOrden,
                    FirmaBytes = datosCierre.FirmaBytes
                }
            }
                };

                string json = JsonConvert.SerializeObject(payloadCierre);

                string urlCierre = $"{baseUrlApi}SupervisionMantenimiento";
                bool ok = await _httpHelper.PostBodyAsync<SupervisionMantenimientoDTO, bool>(urlCierre, payloadCierre);
                if(!ok) {
                    await Shell.Current.DisplayAlert("Error", "No se pudo guardar la supervisión.", "OK");
                    return;
                }

                // 3. Subir fotos pendientes por sección (las que no se subieron en GuardarSeccionAsync)
                var listaFotosDTO = _stateService.ObtenerTodasLasFotos();
                var fotosPorSeccion = listaFotosDTO
                    .Where(f => !f.Subida)
                    .GroupBy(f => f.IdSeccion)
                    .ToList();

                var seccionesConError = new List<int>();

                foreach(var grupo in fotosPorSeccion) {
                    var rutas = grupo.Select(f => f.LocalPath).ToList();
                    bool okFotos = await SubirFotosDeSeccionAsync(idSupervision, grupo.Key, rutas);

                    if(!okFotos) {
                        seccionesConError.Add(grupo.Key);
                    } else {
                        foreach(var foto in grupo) {
                            foto.Subida = true;
                        }
                    }
                }

                if(seccionesConError.Any()) {
                    await Shell.Current.DisplayAlert(
                        "Aviso",
                        $"La supervisión se guardó, pero fallaron las fotos de la(s) sección(es): {string.Join(", ", seccionesConError)}. Puedes reintentar su envío.",
                        "OK");
                } else {
                    await Shell.Current.DisplayAlert("Éxito", "Supervisión registrada correctamente.", "OK");
                }

                _stateService.Limpiar();
                await Shell.Current.Navigation.PopToRootAsync(false);
                await Shell.Current.GoToAsync(nameof(SupervisionesMantenimientoProgramadasPage), true);
                //await Shell.Current.GoToAsync("..");
            } catch(Exception ex) {
                await Shell.Current.DisplayAlert("Error", $"Excepción en el envío: {ex.Message}", "OK");
            } finally {
                IsLoadingSecciones = false;
            }
        }

        private async Task<bool> SubirFotosDeSeccionAsync(int idSupervision, int idSeccion, List<string> rutas) {
            try {
                using var content = new MultipartFormDataContent();
                content.Add(new StringContent(idSeccion.ToString()), "idSecion");

                var streams = new List<Stream>();
                try {
                    foreach(var ruta in rutas) {
                        var stream = File.OpenRead(ruta);
                        streams.Add(stream);
                        var fileContent = new StreamContent(stream);
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                        content.Add(fileContent, "fotos", Path.GetFileName(ruta));
                    }

                    string url = $"{baseUrlApi}SupervisionMantenimiento/{idSupervision}/fotos";
    return await _httpHelper.PostMultipartAsync(url, content);
                } finally {
                    foreach(var s in streams) s.Dispose();
                }
            } catch {
                return false;
            }
        }

        [RelayCommand]
        private async Task AbrirPreguntasSeccionAsync(SeccionModel seccionSeleccionada) {
            if(seccionSeleccionada == null || IsLoadingSecciones) return;

            try {
                IsLoadingSecciones = true;

                await Task.Yield();

                var navigationParameters = new Dictionary<string, object> {
                    {"SeccionSeleccionada", seccionSeleccionada },
                    {"Orden",Orden }
                };

                await Shell.Current.GoToAsync(nameof(PreguntasSeccionPage), navigationParameters);
            } catch(Exception ex) {
                Debug.WriteLine($"Error: {ex.Message}");
            } finally {
                IsLoadingSecciones = false;
            }
        }

#if DEBUG
        [RelayCommand]
        private void LlenarDatosPruebaDebug() {
            foreach(var seccion in Secciones) {
                if(seccion.Preguntas == null) continue;
                foreach(var pregunta in seccion.Preguntas) {
                    Random random = new Random();
                    int res = random.Next(0, 3);
                    pregunta.Respuesta =res; // o el enum que uses
                    pregunta.Observaciones = "TEST DIEGO";
                }
            }

             Shell.Current.DisplayAlert("Éxito", "Datos de prueba llenados correctamente.", "OK");
        }
#endif
    }
}