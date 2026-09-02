using BatiaSuite.Models;
using BatiaSuite.Models.SupervisionMantenimiento.Operarios;
using BatiaSuite.Services.SupervisionesMantenimiento;
using BatiaSuite.Utils;
using BatiaSuite.Views.SupervisionMantenimiento;
using BatiaSuite.Views.SupervisionMantenimiento.Operarios;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private string urlApiBase = Constants.API_BASE_URL;

        public SupervisionMantenimientoSupervisorViewModel(SupervisionStateService stateService) {
            _stateService = stateService;
            LlenarPickers();
        }

        private void LlenarPickers() {
            ObtenerClientes();
            ObtenerTiposServicio();
        }

        private async Task ObtenerTiposServicio() {
            try {
                string url = $"{urlApiBase}TipoServicio";
                var response = await _httpHelper.GetAsync<ObservableCollection<TipoServicioModel>>(url);

                if(response != null) {
                    TiposServicio = response;
                } else {
                    TiposServicio = new ObservableCollection<TipoServicioModel>();
                }
            } catch(Exception ex) {
                Debug.WriteLine($"Error al obtener los tipos de servicio: {ex.Message}");
                Shell.Current.DisplayAlert("Error", "No se pudieron obtener los tipos de servicio. Por favor, inténtelo de nuevo más tarde.", "OK");
            }
        }

        private async Task ObtenerClientes() {
            try {
                string url = $"{urlApiBase}Cliente/ClientesMatenimiento";
                var response = await _httpHelper.GetAsync<ObservableCollection<ClientsModel>>(url);

                if(response != null) {
                    Clientes = response;
                } else {
                    Clientes = new ObservableCollection<ClientsModel>();
                }
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
                // Usamos la propiedad seleccionada (asumiendo que IdClienteSelected es un objeto o tiene el ID)
                int idCliente = ClienteSelected.idCliente;

                string url = $"{urlApiBase}Inmueble?idcliente={idCliente}";
                var response = await _httpHelper.GetAsync<List<InmuebleModel>>(url);

                if(response != null) {
                    Inmuebles = new ObservableCollection<InmuebleModel>(response);
                } else {
                    Inmuebles = new ObservableCollection<InmuebleModel>();
                }
            } catch(Exception ex) {
                Debug.WriteLine($"Error al obtener los inmuebles: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "No se pudieron obtener los inmuebles. Por favor, inténtelo de nuevo más tarde.", "OK");
            }
        }

        [RelayCommand]
        private async Task IrASeccionesAsync() {
            if(TipoServicioSelected == null || InmuebleSelected == null || ClienteSelected == null) {
                await Shell.Current.DisplayAlert("Error", "Por favor, seleccione un tipo de servicio, un inmueble y un cliente antes de continuar.", "OK");
                return;
            }
            // 1. Limpiar sesión e inicializar la orden seleccionada en el StateService
            _stateService.LimpiarSesion();
            _stateService.FechaInicio = DateTime.Now;
            _stateService.IdCliente = ClienteSelected.idCliente;
            _stateService.Inmueble = InmuebleSelected;
            _stateService.IdTipoServicio = TipoServicioSelected.IdTipoServicio;

            // 2. Si la plantilla de secciones y preguntas no está cargada, la consultamos
            if(_stateService.PlantillaBaseSecciones == null || !_stateService.PlantillaBaseSecciones.Any()) {
                string urlPlantilla = $"{urlApiBase}SupervisionMantenimeintoChecklist?id_rol={UserSession.IdRol}";
                var plantilla = await _httpHelper.GetAsync<List<SeccionModel>>(urlPlantilla);

                if(plantilla == null || !plantilla.Any()) {
                    await Shell.Current.DisplayAlert("Atención", "No se pudo obtener el catálogo de secciones y preguntas.", "OK");
                    return;
                }

                // Guardamos la plantilla base con sus preguntas anidadas en el StateService
                _stateService.PlantillaBaseSecciones = plantilla;
            }
            // 3. Navegamos a la pantalla de selección de pisos
            await Shell.Current.GoToAsync("SeleccionPisoSupervisorPage");
        }
    }
}