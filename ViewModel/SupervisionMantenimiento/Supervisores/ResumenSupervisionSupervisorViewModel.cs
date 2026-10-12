using BatiaSuite.Models.SupervisionMantenimiento;
using BatiaSuite.Models.SupervisionMantenimiento.Operarios;
using BatiaSuite.Models.SupervisionMantenimiento.Supervisores;
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
        private readonly SupervisionesService _supervisionesService;

        [ObservableProperty]
        private string _observacionesGenerales = string.Empty;

        [ObservableProperty]
        private ObservableCollection<IDrawingLine> _lineasTecnico = new();

        [ObservableProperty]
        private ObservableCollection<IDrawingLine> _lineasCliente = new();

        [ObservableProperty]
        private bool _isBusy;

        private string ApiBaseUrl => Constants.API_BASE_URL;

        [ObservableProperty]
        private ObservableCollection<PreguntaResumenModel> _preguntas = new();

        #endregion Propiedades de Cabecero

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CondicionRespondida))]
        private string? _condicionSeleccionada;

        public bool CondicionRespondida => !string.IsNullOrWhiteSpace(CondicionSeleccionada);

        public ResumenSupervisionSupervisorViewModel(SupervisionStateService stateService, SupervisionesService supervisionesService) {
            _stateService = stateService;
            _supervisionesService = supervisionesService;
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

        public async Task CargarPreguntasAsync() {
            var preguntasResumen = _stateService.PlantillaPreguntasResumen;
            Preguntas = new ObservableCollection<PreguntaResumenModel>(preguntasResumen);
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

                var preguntasSinResponder = Preguntas.Where(p => p.Respuesta == 0).ToList();
                if(preguntasSinResponder.Any()) {
                    await Shell.Current.DisplayAlert("Atención", "Por favor califique todas las preguntas antes de enviar.", "OK");
                    return;
                }

                List<RespuestaResumenDto> RespuestasResumen = Preguntas.Select(p => new RespuestaResumenDto {
                    IdPregunta = p.id_pregunta, 
                    Estado = p.Respuesta      
                }).ToList();

                // Generar Stream a partir de la colección de trazos
                using var streamTecnico = await DrawingViewService.GetImageStream(ImageLineOptions.JustLines(LineasTecnico, new Size(300, 120), Brush.White));
                using var streamCliente = await DrawingViewService.GetImageStream(ImageLineOptions.JustLines(LineasCliente, new Size(300, 120), Brush.White));

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
                    Firmas = new List<FirmaDto>
                    {
                        new FirmaDto { IdFirma = 1, Nombre = Guid.NewGuid().ToString(), Firmas = base64Tecnico },
                        new FirmaDto { IdFirma = 2, Nombre = Guid.NewGuid().ToString(), Firmas = base64Cliente }
                    },
                    ResumenSupervision = RespuestasResumen
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

                // 6. Delegar el guardado al servicio
                var (fueEnviadoOnline, mensaje) = await _supervisionesService.ProcesarGuardadoSupervisionAsync(
                    payload,
                    mapaFotos,
                    todasLasRutasFotos
                );

                // Check de conectividad para determinar el tipo de resultado real
                bool hayInternet = Utils.InternetUtil.IsConnectedInternet();

                // 7. Evaluar el resultado y determinar el comportamiento
                if(fueEnviadoOnline) {
                    // CASO 1: Envío Online Exitoso
                    await Shell.Current.DisplayAlert("Éxito", mensaje, "OK");

                    // Navegar de regreso
                    await Shell.Current.Navigation.PopToRootAsync(false);
                    await Shell.Current.GoToAsync(nameof(SupervisionMantenimientoSupervisorPage), true);
                } else if(!hayInternet) {
                    // CASO 2: Guardado Offline Exitoso (SQLite)
                    await Shell.Current.DisplayAlert("Modo Offline", mensaje, "OK");

                    // Navegar de regreso permitiendo que la app continúe
                    await Shell.Current.Navigation.PopToRootAsync(false);
                    await Shell.Current.GoToAsync(nameof(SupervisionMantenimientoSupervisorPage), true);
                } else {
                    // CASO 3: Error de API / Validaciones en Servidor (Había Internet pero falló)
                    // NO se navega fuera para que el usuario pueda corregir los datos o reintentar
                    await Shell.Current.DisplayAlert("Atención", mensaje, "Entendido");
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