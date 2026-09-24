using BatiaSuite.Models.SupervisionMantenimiento;
using BatiaSuite.Models.SupervisionMantenimiento.Operarios;
using BatiaSuite.Services.SupervisionesMantenimiento;
using BatiaSuite.Utils;
using BatiaSuite.Views.SupervisionMantenimiento.Operarios;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Views;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;

using System.Collections.ObjectModel;

namespace BatiaSuite.ViewModel.SupervisionMantenimiento.Operarios {

    public partial class ResumenSupervisionViewModel : ViewModelBase {
        #region Servicios y Propiedades de Cabecero

        private readonly SupervisionStateService _stateService;
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

        #endregion Servicios y Propiedades de Cabecero

        #region Constructor

        public ResumenSupervisionViewModel(SupervisionStateService stateService, SupervisionesService supervisionesService) {
            _stateService = stateService;
            _supervisionesService = supervisionesService;
        }

        #endregion Constructor

        #region Métodos Auxiliares

        private async Task<string> ConvertStreamToBase64Async(Stream? stream) {
            if(stream == null) return string.Empty;

            using var memoryStream = new MemoryStream();
            if(stream.CanSeek) stream.Position = 0;
            await stream.CopyToAsync(memoryStream);
            return Convert.ToBase64String(memoryStream.ToArray());
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

                // 2. Mapear rutas locales con un GUID único
                var mapaFotos = todasLasRutasFotos.ToDictionary(
                    ruta => ruta,
                    ruta => $"foto_{Guid.NewGuid()}{Path.GetExtension(ruta)}"
                );

                // 3. Convertir Streams de firmas a Base64
                string base64Tecnico = await ConvertStreamToBase64Async(streamTecnico);
                string base64Cliente = await ConvertStreamToBase64Async(streamCliente);

                // 4. Construir DTO Principal
                var payload = new SupervisionPayloadDto {
                    IdOrdenSupervisionM = _stateService.OrdenActual.idOrden,
                    IdPersonal = UserSession.IdPersonal,
                    FechaIni = _stateService.FechaInicio,
                    FechaFin = DateTime.Now,
                    IdCliente = _stateService.OrdenActual.idCliente,
                    IdInmueble = _stateService.OrdenActual.idInmueble,
                    Latitud = _stateService.OrdenActual.latitud,
                    Longitud = _stateService.OrdenActual.longitud,
                    Observaciones = ObservacionesGenerales,
                    IdRol = UserSession.IdRol,
                    Firmas = new List<FirmaDto>
                    {
                        new FirmaDto { IdFirma = 1, Nombre = Guid.NewGuid().ToString(), Firmas = base64Tecnico },
                        new FirmaDto { IdFirma = 2, Nombre = Guid.NewGuid().ToString(), Firmas = base64Cliente }
                    }
                };

                // 5. Aplanar la estructura de UI
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

                // 6. Delegar el guardado (Online/Offline) al Servicio
                var (fueEnviadoOnline, mensaje) = await _supervisionesService.ProcesarGuardadoSupervisionAsync(
                    payload,
                    mapaFotos,
                    todasLasRutasFotos
                );

                // 7. Mostrar resultado al usuario y navegar
                string tituloAlerta = fueEnviadoOnline ? "Éxito" : "Modo Offline";
                await Shell.Current.DisplayAlert(tituloAlerta, mensaje, "OK");

                await Shell.Current.Navigation.PopToRootAsync(false);
                await Shell.Current.GoToAsync(nameof(SupervisionesMantenimientoProgramadasPage), true);

            } catch(Exception ex) {
                await Shell.Current.DisplayAlert("Error", $"Excepción al guardar: {ex.Message}", "OK");
            } finally {
                IsBusy = false;
            }
        }

        #endregion Comando Principal

#if DEBUG
        #region Modo Debug

        [RelayCommand]
        private void AutoLlenarSupervisionModoDebug()
        {
            // 1. Llenar campos del cabecero
            ObservacionesGenerales = "Prueba automatizada de supervisión en modo Debug.";

            // 2. Simular trazado ficticio de firma
            LineasTecnico.Clear();
            LineasCliente.Clear();

            var trazoMockTecnico = new DrawingLine
            {
                LineColor = Colors.Black,
                LineWidth = 3,
                Points = new ObservableCollection<PointF>
                {
                    new PointF(10, 50), new PointF(50, 20), new PointF(100, 80), new PointF(150, 30)
                }
            };

            var trazoMockCliente = new DrawingLine
            {
                LineColor = Colors.Blue,
                LineWidth = 3,
                Points = new ObservableCollection<PointF>
                {
                    new PointF(15, 60), new PointF(60, 30), new PointF(110, 90), new PointF(160, 40)
                }
            };

            LineasTecnico.Add(trazoMockTecnico);
            LineasCliente.Add(trazoMockCliente);

            // 3. Responder automáticamente todas las preguntas
            if (_stateService.Pisos != null)
            {
                foreach (var piso in _stateService.Pisos)
                {
                    foreach (var seccion in piso.Secciones)
                    {
                        if (!seccion.Iteraciones.Any())
                        {
                            var nuevaIteracion = new IteracionModel
                            {
                                Nombre = $"{seccion.Seccion} #1",
                                Preguntas = seccion.Preguntas.Select(p => new PreguntaModel
                                {
                                    IdPregunta = p.IdPregunta,
                                    Pregunta = p.Pregunta,
                                    Respuesta = 2, // Bueno
                                    Observaciones = "OK Debug"
                                }).ToList()
                            };
                            seccion.Iteraciones.Add(nuevaIteracion);
                        }
                        else
                        {
                            foreach (var iteracion in seccion.Iteraciones)
                            {
                                foreach (var pregunta in iteracion.Preguntas)
                                {
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

        #endregion Modo Debug
#endif
    }
}