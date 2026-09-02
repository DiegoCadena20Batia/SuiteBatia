using BatiaSuite.Models.SupervisionMantenimiento;
using BatiaSuite.Models.SupervisionMantenimiento.Operarios;
using BatiaSuite.Services.SupervisionesMantenimiento;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.ViewModel.SupervisionMantenimiento.Supervisores {

    public partial class IteracionesSeccionSupervisorViewModel : ViewModelBase {
        private readonly SupervisionStateService _stateService;

        [ObservableProperty]
        private string _nombreSeccion = string.Empty;

        [ObservableProperty]
        private string _nombrePiso = string.Empty;

        [ObservableProperty]
        private SeccionModel _seccion;

        [ObservableProperty]
        private PisoModel _piso;

        [ObservableProperty]
        private ObservableCollection<IteracionModel> _iteraciones = new();

        public IteracionesSeccionSupervisorViewModel(SupervisionStateService supervisionStateService) {
            _stateService = supervisionStateService;
            CargarIteraciones();
        }

        [RelayCommand]
        public void CargarIteraciones() {
            Seccion = _stateService.SeccionActual;
            Piso = _stateService.PisoActual;

            if(Seccion == null || Piso == null) return;

            NombreSeccion = Seccion.Seccion;
            NombrePiso = Piso.Nombre;
            Iteraciones = Seccion.Iteraciones;
        }

        [RelayCommand]
        private async Task AgregarIteracionAsync() {
            string nombreDefecto = $"{NombreSeccion} #{Iteraciones.Count + 1}";

            string resultado = await Shell.Current.DisplayPromptAsync(
                "Nueva Iteración",
                "Ingresa el nombre o identificador del elemento: ",
                initialValue: nombreDefecto,
                accept: "Agregar",
                cancel: "Cancelar");

            if(string.IsNullOrEmpty(resultado)) return;

            // Se creas nuevas instancias de PreguntaModel a partir de la plantilla
            var preguntasLimpias = _stateService.SeccionActual.Preguntas
                .Select(p => new PreguntaModel {
                    IdPregunta = p.IdPregunta,
                    Pregunta = p.Pregunta,
                    Respuesta = -1,              // Forzar estado "Sin responder"
                    Observaciones = string.Empty // Limpiar observaciones
                })
                .ToList();

            var nuevaIteracion = new IteracionModel {
                Nombre = resultado.Trim(),
                Preguntas = preguntasLimpias
            };

            Iteraciones.Add(nuevaIteracion);
        }

        [RelayCommand]
        private async Task SeleccionarIteracionAsync(IteracionModel iteracion) {
            if(iteracion == null) return;

            _stateService.IteracionActual = iteracion;
            await Shell.Current.GoToAsync("PreguntasSeccionSupervisorPage");
        }

        [RelayCommand]
        private async Task EliminarIteracionAsync(IteracionModel iteracion) {
            if(iteracion == null) return;

            bool confirmar = await Shell.Current.DisplayAlert(
                "Eliminar Iteración",
                $"Estás seguro de eliminar '{iteracion.Nombre}' y todas sus respuestas?",
                "Eliminar",
                "Cancelar");

            if(confirmar) {
                Iteraciones.Remove(iteracion);
            }
        }

        [RelayCommand]
        private async Task TerminarSeccionAsync() {
            bool confirmar = await Shell.Current.DisplayAlert(
                "Terminar Sección",
                $"Estás seguro de terminar la sección '{NombreSeccion}'?",
                "Terminar",
                "Cancelar");
            if(confirmar) {
                _stateService.PisoActual.Secciones.FirstOrDefault(s => s.IdSeccion == Seccion.IdSeccion).EstaCompletada = true;

                await Shell.Current.GoToAsync("..");
            }
        }

        [RelayCommand]
        private async Task VolverPantallaSecciones() {
            if(!Iteraciones.Any()) {
                await Shell.Current.GoToAsync("..");
            }
            if(Iteraciones.Any() && !Iteraciones.All(i => i.EstaCompletada)) {
                bool confirmar = await Shell.Current.DisplayAlert(
                    "Terminar Sección",
                    $"Si regresas es posible que los datos no se guarden, ¿Deseas volver?",
                    "Volver",
                    "Permanecer");
                if(confirmar) {
                    await Shell.Current.GoToAsync("..");
                }
                return;
            }
        }
    }
}