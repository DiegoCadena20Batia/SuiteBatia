using BatiaSuite.Models.OrdenesTrabajo;
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

namespace BatiaSuite.ViewModel.SupervisionMantenimiento.Operarios {
    public partial class SeleccionPisosViewModel : ViewModelBase {
        private readonly SupervisionStateService _stateService;

        [ObservableProperty]
        private string _nuevoPisoNombre = string.Empty;

        // Directamente enlazado a la lista central de pisos del StateService
        public ObservableCollection<PisoModel> Pisos => _stateService.Pisos;
        public OrdenTrabajoModel? Orden => _stateService.OrdenActual;

        public SeleccionPisosViewModel(SupervisionStateService stateService) {
            _stateService = stateService;
        }

        public void OnAppearing() {
            // Si hay un piso actual seleccionado en el StateService, notificamos su cambio
            if(_stateService.PisoActual != null) {
                _stateService.PisoActual.NotificarCambios();
            } else {
                // Opcional: Notificar a todos por si hubo cambios múltiples
                foreach(var piso in Pisos) {
                    piso.NotificarCambios();
                }
            }
        }

        [RelayCommand]
        private async Task AgregarPisoAsync() {
            if(string.IsNullOrWhiteSpace(NuevoPisoNombre)) return;

            string nombreLimpio = NuevoPisoNombre.Trim();

            // Validar duplicados en la lista de pisos
            if(Pisos.Any(p => p.Nombre.Equals(nombreLimpio, StringComparison.OrdinalIgnoreCase))) {
                await Shell.Current.DisplayAlert("Aviso", "Ya agregaste un piso o área con este nombre.", "OK");
                return;
            }

            // Creamos el piso y le asignamos sus secciones clonadas desde el StateService (Paso 2)
            var nuevoPiso = new PisoModel {
                Nombre = nombreLimpio,
                Secciones = _stateService.GenerarSeccionesParaNuevoPiso()
            };

            Pisos.Add(nuevoPiso);
            NuevoPisoNombre = string.Empty; // Limpiamos la entrada
        }

        [RelayCommand]
        private async Task SeleccionarPisoAsync(PisoModel piso) {
            if(piso == null) return;

            _stateService.PisoActual = piso;

            // Pasamos únicamente el objeto PisoModel seleccionado a la pantalla de Secciones (Paso 4)
            var navigationParameter = new Dictionary<string, object>
            {
            { "PisoSeleccionado", piso }
        };

            await Shell.Current.GoToAsync("SeccionesFormularioPage", navigationParameter);
        }

        [RelayCommand]
        private async Task EliminarPisoAsync(PisoModel piso) {
            if(piso == null) return;

            bool confirmar = await Shell.Current.DisplayAlert(
                "Eliminar Área",
                $"¿Deseas eliminar '{piso.Nombre}' y sus respuestas?",
                "Eliminar",
                "Cancelar");

            if(confirmar) {
                Pisos.Remove(piso);
            }
        }

        [RelayCommand]
        private async Task ContinuarCierreAsync() {
            if(!Pisos.Any()) {
                await Shell.Current.DisplayAlert("Atención", "Debes agregar al menos un piso o área para continuar.", "OK");
                return;
            }

            var pisosJson = System.Text.Json.JsonSerializer.Serialize(Pisos);
            _stateService.Pisos = Pisos; // Aseguramos que el StateService tenga la lista actualizada

            await Shell.Current.GoToAsync("ResumenSupervisionPage");
               }
    }
}
