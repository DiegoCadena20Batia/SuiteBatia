using BatiaSuite.Models.SupervisionMantenimiento;
using BatiaSuite.Models.SupervisionMantenimiento.Operarios;
using BatiaSuite.Services.SupervisionesMantenimiento;
using BatiaSuite.Utils;
using BatiaSuite.Views.SupervisionMantenimiento.Supervisores;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Views;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace BatiaSuite.ViewModel.SupervisionMantenimiento.Supervisores {

    public partial class ResumenSupervisionSupervisorViewModel : ViewModelBase {

        #region Propiedades de Cabecero

        private SupervisionStateService _stateService;

        [ObservableProperty]
        private string _observacionesGenerales = string.Empty;

        [ObservableProperty]
        private ObservableCollection<IDrawingLine> _lineasTecnico = new();

        [ObservableProperty]
        private ObservableCollection<IDrawingLine> _lineasCliente = new();

        [ObservableProperty]
        private bool _isBusy;

        private string ApiBaseUrl => Constants.API_BASE_URL;

        #endregion Propiedades de Cabecero

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CondicionRespondida))]
        private string? _condicionSeleccionada;

        public bool CondicionRespondida => !string.IsNullOrWhiteSpace(CondicionSeleccionada);

        public ResumenSupervisionSupervisorViewModel(SupervisionStateService stateService) {
            _stateService = stateService;
        }

        #region Métodos Auxiliares

        private async Task<string> ConvertStreamToBase64Async(Stream? stream) {
            if(stream == null) return string.Empty;

            using var memoryStream = new MemoryStream();
            if(stream.CanSeek) stream.Position = 0;
            await stream.CopyToAsync(memoryStream);
            return Convert.ToBase64String(memoryStream.ToArray());
        }

        private async Task<bool> SubirFotografiasAsync(List<string> rutasLocales, int idSupervisionM, Dictionary<string, string> mapaNombres) {
            if(rutasLocales == null || !rutasLocales.Any())
                return true;

            try {
                using var content = new MultipartFormDataContent();

                // 1. Agregar el ID generado para que el API cree la subcarpeta en IIS
                content.Add(new StringContent(idSupervisionM.ToString()), "idSupervisionM");

                // 2. Adjuntar cada archivo con el nombre pre-asignado guardado en BD
                foreach(var ruta in rutasLocales) {
                    if(File.Exists(ruta)) {
                        var fileBytes = await File.ReadAllBytesAsync(ruta);
                        var byteContent = new ByteArrayContent(fileBytes);
                        byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

                        string nombreArchivoServidor = mapaNombres.TryGetValue(ruta, out var nombre)
                            ? nombre
                            : Path.GetFileName(ruta);

                        content.Add(byteContent, "files", nombreArchivoServidor);
                    }
                }

                string url = $"{ApiBaseUrl}SupervisionMantenimiento/subir-fotos";

                var json= Newtonsoft.Json.JsonConvert.SerializeObject(content);

                var respuesta = await _httpHelper.PostMultipartAsync<List<string>>(
                    url,
                    content,
                    isSupervisionModule: true
                );

                return respuesta != null;
            } catch(Exception ex) {
                Console.WriteLine($"Error al subir fotografías: {ex.Message}");
                return false;
            }
        }

        #endregion Métodos Auxiliares

        #region Comando Principal

        [RelayCommand]
        public async Task GuardarSupervisionAsync() {
            if(IsBusy) return;

            try {
                IsBusy = true;
                if(LineasTecnico.Count == 0 || LineasCliente.Count == 0) {
                    await Shell.Current.DisplayAlert("Advertencia", "Faltan las firmas correspondientes.", "OK");
                    return;
                }

                // Generar Stream a partir de la colección de trazos
                using var streamTecnico = await DrawingView.GetImageStream(LineasTecnico, new Size(300, 120), Colors.White);
                using var streamCliente = await DrawingView.GetImageStream(LineasCliente, new Size(300, 120), Colors.White);

                // 1. Recolectar rutas locales de fotos
                var todasLasRutasFotos = _stateService.Pisos
                    .SelectMany(p => p.Secciones)
                    .SelectMany(s => s.Iteraciones)
                    .SelectMany(i => i.Fotos)
                    .Select(f => f.LocalPath)
                    .Where(path => !string.IsNullOrWhiteSpace(path))
                    .Distinct()
                    .ToList();

                // 2. Mapear rutas locales con un GUID único para asignarlo en la BD desde antes de subir el archivo
                var mapaFotos = todasLasRutasFotos.ToDictionary(
                    ruta => ruta,
                    ruta => $"{Guid.NewGuid()}{Path.GetExtension(ruta)}"
                );

                // 3. Convertir Streams de firmas a Base64
                string base64Tecnico = await ConvertStreamToBase64Async(streamTecnico);
                string base64Cliente = await ConvertStreamToBase64Async(streamCliente);

                // 4. Construir DTO Principal
                var payload = new SupervisionPayloadDto {
                    IdOrdenSupervisionM = 0,
                    IdPersonal = UserSession.IdPersonal,
                    FechaIni = _stateService.FechaInicio,
                    FechaFin = DateTime.Now,
                    IdCliente = _stateService.IdCliente,
                    IdInmueble = _stateService.Inmueble.id_inmueble,
                    Latitud = _stateService.Inmueble.latitud,
                    Longitud = _stateService.Inmueble.longitud,
                    Observaciones = ObservacionesGenerales,
                    IdRol = UserSession.IdRol,
                    IdTipoServicio = _stateService.IdTipoServicio,
                    ResumenSupervision = CondicionSeleccionada ?? string.Empty,
                    Firmas = new List<FirmaDto>
                    {
                        new FirmaDto { IdFirma = 1, Nombre = Guid.NewGuid().ToString(), Firmas = base64Tecnico },
                        new FirmaDto { IdFirma = 2, Nombre = Guid.NewGuid().ToString(), Firmas = base64Cliente }
                    }
                };

                // 5. Aplanar la estructura de UI e incorporar los nombres mapeados de fotos
                int contadorIteracionGlobal = 1;

                foreach(var piso in _stateService.Pisos) {
                    foreach(var seccion in piso.Secciones) {
                        foreach(var iteracion in seccion.Iteraciones) {
                            var instanciaDto = new InstanciaDto {
                                IdSeccion = seccion.IdSeccion,
                                AreaPisoUbicacion = piso.Nombre,
                                NumeroIteracion = contadorIteracionGlobal,
                                Respuestas = iteracion.Preguntas.Select(p => new RespuestaDto {
                                    IdPregunta = p.IdPregunta,
                                    Estado = p.Respuesta,
                                    DispNivel = 0,
                                    Comentarios = p.Observaciones ?? string.Empty
                                }).ToList(),
                                Fotos = iteracion.Fotos
                                    .Select(f => mapaFotos.TryGetValue(f.LocalPath, out var nombreServidor) ? nombreServidor : f.LocalPath)
                                    .ToList()
                            };

                            payload.Instancias.Add(instanciaDto);
                            contadorIteracionGlobal++;
                        }
                    }
                }

                // 6. Enviar payload a la BD para obtener el ID de supervisión generado
                string urlGuardar = $"{ApiBaseUrl}SupervisionMantenimiento/GuardarCompleta";
                var response = await _httpHelper.PostBodyAsync<SupervisionPayloadDto, SupervisionResponseDto>(urlGuardar, payload);

                if(response != null && response.Success && response.Id_Supervisionm > 0) {
                    // 7. Ya con el ID devuelto por SQL Server, subir los archivos físicos de fotos a su carpeta
                    if(todasLasRutasFotos.Any()) {
                        await SubirFotografiasAsync(todasLasRutasFotos, response.Id_Supervisionm, mapaFotos);
                    }

                    await Shell.Current.DisplayAlert("Éxito", response.Mensaje ?? "Supervisión guardada correctamente.", "OK");
                    await Shell.Current.Navigation.PopToRootAsync(false);
                    await Shell.Current.GoToAsync(nameof(SupervisionMantenimientoSupervisorPage), true);
                } else {
                    string mensajeError = response?.Mensaje ?? "Ocurrió un error al procesar la solicitud en el servidor.";
                    await Shell.Current.DisplayAlert("Error", mensajeError, "OK");
                }
            } catch(Exception ex) {
                await Shell.Current.DisplayAlert("Error", $"Excepción al guardar: {ex.Message}", "OK");
            } finally {
                IsBusy = false;
            }
        }

        #endregion Comando Principal

        [RelayCommand]
        private void SeleccionarCondicion(string valor) {
            if(string.IsNullOrWhiteSpace(valor)) return;

            if(CondicionSeleccionada == valor) {
                CondicionSeleccionada = null;
            } else {
                CondicionSeleccionada = valor;
            }
        }

#if DEBUG

        [RelayCommand]
        private void AutoLlenarSupervisionModoDebug() {
            // 1. Llenar campos del cabecero
            ObservacionesGenerales = "Prueba automatizada de supervisión en modo Debug.";

            CondicionSeleccionada = "Excelente";

            // 2. Simular trazado ficticio de firma
            LineasTecnico.Clear();
            LineasCliente.Clear();

            var trazoMockTecnico = new DrawingLine {
                LineColor = Colors.Black,
                LineWidth = 3,
                Points = new System.Collections.ObjectModel.ObservableCollection<PointF>
                {
            new PointF(10, 50), new PointF(50, 20), new PointF(100, 80), new PointF(150, 30)
        }
            };

            var trazoMockCliente = new DrawingLine {
                LineColor = Colors.Blue,
                LineWidth = 3,
                Points = new System.Collections.ObjectModel.ObservableCollection<PointF>
                {
            new PointF(15, 60), new PointF(60, 30), new PointF(110, 90), new PointF(160, 40)
        }
            };

            LineasTecnico.Add(trazoMockTecnico);
            LineasCliente.Add(trazoMockCliente);

            // 3. Responder automáticamente todas las preguntas
            if(_stateService.Pisos != null) {
                foreach(var piso in _stateService.Pisos) {
                    foreach(var seccion in piso.Secciones) {
                        if(!seccion.Iteraciones.Any()) {
                            var nuevaIteracion = new IteracionModel {
                                Nombre = $"{seccion.Seccion} #1",
                                Preguntas = seccion.Preguntas.Select(p => new PreguntaModel {
                                    IdPregunta = p.IdPregunta,
                                    Pregunta = p.Pregunta,
                                    Respuesta = 2, // Bueno
                                    Observaciones = "OK Debug"
                                }).ToList()
                            };
                            seccion.Iteraciones.Add(nuevaIteracion);
                        } else {
                            foreach(var iteracion in seccion.Iteraciones) {
                                foreach(var pregunta in iteracion.Preguntas) {
                                    pregunta.Respuesta = 2; // Bueno
                                    pregunta.Observaciones = "Sin hallazgos - Test Debug";
                                }
                            }
                        }
                    }
                }
            }

            Shell.Current.DisplayAlert("Debug", "Supervisión auto-llenada exitosamente.", "OK");
        }

#endif
    }
}