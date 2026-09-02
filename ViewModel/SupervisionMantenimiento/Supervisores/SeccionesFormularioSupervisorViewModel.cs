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
    [QueryProperty(nameof(Piso), "PisoSeleccionado")]
    public partial class SeccionesFormularioSupervisorViewModel : ObservableObject {
        private readonly SupervisionStateService _stateService;

        #region Propiedades
        [ObservableProperty]
        private string _nombrePiso = string.Empty;

        [ObservableProperty]
        private PisoModel _piso;

        [ObservableProperty]
        private string _nombreSucursal = string.Empty;

        [ObservableProperty]
        private ObservableCollection<SeccionModel> _secciones = new();

        [ObservableProperty]
        private double _progresoGeneralPiso;

        [ObservableProperty]
        private string _textoProgresoGeneral = "0 / 0 Respondidas";

        [ObservableProperty]
        private bool _isLoading;
        #endregion

        public SeccionesFormularioSupervisorViewModel(SupervisionStateService stateService) {
            _stateService = stateService;
            CargarSecciones();
        }



        /// <summary>
        /// Invocar este método desde el OnAppearing de la View para actualizar
        /// los contadores y avances cada vez que se regrese de contestar preguntas.
        /// </summary>
        [RelayCommand]
        public void CargarSecciones() {

            var pisoActual = _stateService.PisoActual;
            if(pisoActual == null) return;

            NombrePiso = pisoActual.Nombre;
            NombreSucursal = _stateService.Inmueble.nombre ?? string.Empty;

            // Cargar secciones del piso activo
            Secciones = new ObservableCollection<SeccionModel>(pisoActual.Secciones);

            // Calcular el avance global del piso
            CalcularAvanceGeneral();
        }


        [RelayCommand]
        private async Task SeleccionarSeccionAsync(SeccionModel seccionSeleccionada) {
            if(seccionSeleccionada == null || IsLoading) return;

            try {
                IsLoading = true;

                // 1. Guardar la sección seleccionada en el StateService
                _stateService.SeccionActual = seccionSeleccionada;

                // 2. Navegar a la pantalla de iteración de preguntas
                await Shell.Current.GoToAsync("IteracionesSeccionSupervisorPage");
            } catch(Exception ex) {
                System.Diagnostics.Debug.WriteLine($"Error al seleccionar sección: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "No se pudo abrir la sección seleccionada.", "OK");
            } finally {
                IsLoading = false;
            }
        }
        private void CalcularAvanceGeneral() {
            if(Secciones == null || !Secciones.Any()) {
                ProgresoGeneralPiso = 0;
                TextoProgresoGeneral = "0 / 0 Respondidas";
                return;
            }

            // Usamos la propiedad computada 'EstaRespondida' definida en PreguntaModel
            int totalSecciones = Secciones.Count();
            int totalRespondidas = Secciones.Count(s => s.EstaCompletada);

            if(totalSecciones > 0) {
                ProgresoGeneralPiso = (double)totalRespondidas / totalSecciones;
                TextoProgresoGeneral = $"{totalRespondidas} de {totalSecciones} secciones completadas";
            } else {
                ProgresoGeneralPiso = 0;
                TextoProgresoGeneral = "Sin preguntas en este piso";
            }
        }

        [RelayCommand]
        private async Task FinalizarPisoAsync() {
            int totalSeciones = Secciones.Count();
            int totalCompletadas = Secciones.Count(s => s.EstaCompletada);
            if(totalCompletadas < 1) {
                await Shell.Current.DisplayAlert(
                    "Piso Incompleto",
                    $"Tienes que completar al menos una sección en este piso",
                    "Ok");
                return;
            }
            if(totalCompletadas < totalSeciones) {
                bool confirmar = await Shell.Current.DisplayAlert(
                    "Piso Incompleto",
                    $"Tienes {totalSeciones - totalCompletadas} secciones pendientes en este piso. ¿Deseas regresar a la lista de pisos de todos modos?",
                    "Volver",
                    "Cancelar");

                if(!confirmar) return;
            }

            _stateService.PisoActual.EstaCompletado = true;

            // Simplemente regresamos. 'PisoActual.EstaCompletado' se calculará solo automáticamente.
            await Shell.Current.GoToAsync("..");
        }
        [RelayCommand]
        private async Task VolverPantallaSeleccionPiso() {
            int totalSeciones = Secciones.Count();
            int totalCompletadas = Secciones.Count(s => s.EstaCompletada);

            if(totalCompletadas < totalSeciones || totalCompletadas > 0) {
                bool confirmar = await Shell.Current.DisplayAlert(
                    "Piso Incompleto",
                    $"Tienes secciones pendientes en este piso. ¿Deseas volver a la pantalla anterior? Es posible que los datos no se guarden.",
                    "Volver",
                    "Cancelar");

                if(!confirmar) return;
            }

            await Shell.Current.GoToAsync("..");

        }
    }
}
