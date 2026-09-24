using BatiaSuite.Interfaz.Repositories;
using BatiaSuite.Models;
using BatiaSuite.Services.SupervisionesMantenimiento;
using BatiaSuite.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace BatiaSuite.ViewModel.SupervisionMantenimiento.Supervisores {

    public partial class SupervisionMantenimientoSupervisorViewModel : ViewModelBase {

        [ObservableProperty]
        private SupervisionStateService _stateService;

        [ObservableProperty]
        private ObservableCollection<TipoServicioModel> _tiposServicio = new();

        [ObservableProperty]
        private ObservableCollection<InmuebleModel> _inmuebles = new();

        [ObservableProperty]
        private ObservableCollection<ClientsModel> _clientes = new();

        [ObservableProperty]
        private TipoServicioModel _tipoServicioSelected;

        [ObservableProperty]
        private InmuebleModel _inmuebleSelected;

        [ObservableProperty]
        private ClientsModel _clienteSelected;

        [ObservableProperty]
        private bool _isLoading;

        private string urlApiBase = Constants.API_BASE_URL;

        private readonly ITiposServicioRepository _tiposServicioRepository;
        private readonly IClientesRepository _clientesRepository;
        private readonly IInmueblesRepository _inmueblesRepository;
        private readonly IPlantillasRepository _plantillasRepository;

        public SupervisionMantenimientoSupervisorViewModel(SupervisionStateService stateService, ITiposServicioRepository tiposServicioRepository, IClientesRepository clientesRepository, IInmueblesRepository inmueblesRepository,IPlantillasRepository plantillasRepository) {
            _stateService = stateService;
            _tiposServicioRepository = tiposServicioRepository;
            _clientesRepository = clientesRepository;
            _inmueblesRepository = inmueblesRepository;
            _plantillasRepository = plantillasRepository;
            LlenarPickers();
        }

        private void LlenarPickers() {
            ObtenerClientes();
            ObtenerTiposServicio();
        }

        private async Task ObtenerTiposServicio() {
            try {
               var resultado= await _tiposServicioRepository.ObtenerTiposServicioAsync();
                TiposServicio = new ObservableCollection<TipoServicioModel>(resultado);
            } catch(Exception ex) {
                Debug.WriteLine($"Error al obtener los tipos de servicio: {ex.Message}");
                Shell.Current.DisplayAlert("Error", "No se pudieron obtener los tipos de servicio. Por favor, inténtelo de nuevo más tarde.", "OK");
            }
        }

        private async Task ObtenerClientes() {
            try {
                var resultado = await _clientesRepository.ObtenerClientesAsync();

                Clientes = new ObservableCollection<ClientsModel>(resultado);
            } catch(Exception ex) {
                Debug.WriteLine($"Error al obtener los clientes: {ex.Message}");
                Shell.Current.DisplayAlert("Error", "No se pudieron obtener los clientes. Por favor, inténtelo de nuevo más tarde.", "OK");
            }
        }

        [RelayCommand]
        private async Task ObtenerSucursales() {
            // Verificamos que haya un cliente seleccionado
            if(ClienteSelected == null) return;

            try {
               var resultado = await _inmueblesRepository.ObtenerInmueblesAsync(ClienteSelected.idCliente);
                Inmuebles = new ObservableCollection<InmuebleModel>(resultado);
            } catch(Exception ex) {
                Debug.WriteLine($"Error al obtener los inmuebles: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "No se pudieron obtener los inmuebles. Por favor, inténtelo de nuevo más tarde.", "OK");
            }
        }

        [RelayCommand]
        private async Task IrASeccionesAsync() {
            if(IsLoading) return;
            if(TipoServicioSelected == null || InmuebleSelected == null || ClienteSelected == null) {
                await Shell.Current.DisplayAlert("Error", "Por favor, seleccione un tipo de servicio, un inmueble y un cliente antes de continuar.", "OK");
                return;
            }

            try {
                IsLoading = true;
           
            // 1. Limpiar sesión e inicializar la orden seleccionada en el StateService
            _stateService.LimpiarSesion();
            _stateService.FechaInicio = DateTime.Now;
            _stateService.IdCliente = ClienteSelected.idCliente;
            _stateService.Inmueble = InmuebleSelected;
            _stateService.IdTipoServicio = TipoServicioSelected.IdTipoServicio;

            // 2. Si la plantilla de secciones y preguntas no está cargada, la consultamos
            if(_stateService.PlantillaBaseSecciones == null || !_stateService.PlantillaBaseSecciones.Any()) {
                var plantilla = await _plantillasRepository.ObtenerPlantillaAsync(UserSession.IdRol);

                if(plantilla == null || !plantilla.Any()) {
                    await Shell.Current.DisplayAlert("Atención", "No se pudo obtener el catálogo de secciones y preguntas.", "OK");
                    return;
                }

                // Guardamos la plantilla base con sus preguntas anidadas en el StateService
                _stateService.PlantillaBaseSecciones = plantilla;
            }
            // 3. Navegamos a la pantalla de selección de pisos
            await Shell.Current.GoToAsync("SeleccionPisoSupervisorPage");
            } catch(Exception ex) {
                Debug.WriteLine($"Error al navegar a la pantalla de selección de pisos: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "No se pudo navegar a la pantalla de selección de pisos. Por favor, inténtelo de nuevo más tarde.", "OK");
            } finally {
                IsLoading = false;
            }
        }
    }
}