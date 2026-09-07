using BatiaSuite.Models.OrdenesTrabajo;
using BatiaSuite.Utils;
using BatiaSuite.ViewModel.OrdenesTrabajo;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SlackAPI;
using System.Collections.ObjectModel;

namespace BatiaSuite.ViewModel {

    using BatiaSuite.Data;
    using BatiaSuite.Interfaz.Repositories;
    using BatiaSuite.Models.EntidadesLocal.Supervisiones;
    using BatiaSuite.Models.SupervisionMantenimiento.Operarios;
    using BatiaSuite.Repositories;
    using BatiaSuite.Services.SupervisionesMantenimiento;
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using System.Collections.ObjectModel;
    using System.Diagnostics;

    public partial class SupervisionesMantenimientoProgramadasViewModel : ObservableObject {

        #region Variables y Servicios

        private readonly SupervisionStateService _stateService;
        private readonly IOrdenesRepository _ordenesRepository;
        private readonly IPlantillasRepository _plantillasRepository;

        [ObservableProperty]
        private ObservableCollection<OrdenTrabajoModel> _ordenes = new();

        [ObservableProperty]
        private ObservableCollection<OrdenTrabajoModel> _sucursalesFiltradas = new();

        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _selectedMonth = string.Empty;

        [ObservableProperty]
        private int _filterYear;

        private int _filterMonth;

        [ObservableProperty]
        private string _filtroTexto = string.Empty;

        private readonly List<int> _yearList = new();

        #endregion Variables y Servicios

        public SupervisionesMantenimientoProgramadasViewModel(SupervisionStateService stateService,IOrdenesRepository ordenesRepository,IPlantillasRepository plantillasRepository) {
            _ordenesRepository = ordenesRepository;
            _stateService = stateService;
            _plantillasRepository = plantillasRepository;

            InitValues();
           
        }

        private void InitValues() {
            _filterMonth = DateTime.Now.Month;
            SelectedMonth = Constants.GetMonthName(_filterMonth);
            FilterYear = DateTime.Now.Year;

            _yearList.Clear();
            for(int year = FilterYear - 1; year <= FilterYear + 1; year++) {
                _yearList.Add(year);
            }
        }

        partial void OnFiltroTextoChanged(string value) {
            if(string.IsNullOrWhiteSpace(value)) {
                SucursalesFiltradas = new ObservableCollection<OrdenTrabajoModel>(Ordenes);
            } else {
                var filtrados = Ordenes
                    .Where(x => !string.IsNullOrEmpty(x.sucursal) && x.sucursal.Contains(value, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                SucursalesFiltradas = new ObservableCollection<OrdenTrabajoModel>(filtrados);
            }
        }

        [RelayCommand]
        private async Task GetYearAsync() {
            double size = Constants.IS_IOS ? (Constants.IS_TABLET ? 5 : 7) : (Constants.IS_TABLET ? 5 : 5);

            var result = await PopupUtil.GetObjectAsync(FilterYear, _yearList.Cast<object>().ToList(), size);
            if(result is int value && value != FilterYear && value != 0) {
                FilterYear = value;
                await CargarOrdenesAsync();
            }
        }

        [RelayCommand]
        private async Task GetMonthAsync() {
            double size = Constants.IS_IOS ? (Constants.IS_TABLET ? 5 : 5) : (Constants.IS_TABLET ? 3 : 3);

            var result = await PopupUtil.GetObjectAsync(SelectedMonth, Constants.MonthList, size);
            if(result is string value && !string.IsNullOrEmpty(value) && value != SelectedMonth) {
                SelectedMonth = value;
                _filterMonth = Constants.GetMonthNumber(SelectedMonth);
                await CargarOrdenesAsync();
            }
        }

        [RelayCommand]
        public async Task CargarOrdenesAsync() {
            if(IsLoading) return;
            try {
                IsLoading=true; 

                int idTecnico = UserSession.IdEmpleado;  
                int mes= _filterMonth;
                int anio= FilterYear;

                var resultado=await _ordenesRepository.ObtenerOrdenesAsync(idTecnico, mes, anio);

                Ordenes=new ObservableCollection<OrdenTrabajoModel>(resultado);
                OnFiltroTextoChanged(FiltroTexto);
            } catch(Exception ex) {

                await Shell.Current.DisplayAlert("Error", "No se cargó la información correctamente. Intente de nuevo", "Ok");

                Debug.WriteLine($"Error en CargarOrdenesAsync: {ex.Message}");
            } finally {
                IsLoading = false;
                IsRefreshing=false;
            }
        }

        [RelayCommand]
        private async Task VerFormularioSupervision(OrdenTrabajoModel ordenSeleccionada) {
            if(ordenSeleccionada == null || IsLoading) return;
            try {
             IsLoading = true;
                // 1. Inicializar sesión y estado
                _stateService.LimpiarSesion();
                _stateService.OrdenActual = ordenSeleccionada;
                _stateService.FechaInicio = DateTime.Now;

                // 2. Cargar plantilla usando el repositorio si no está en memoria
                if(_stateService.PlantillaBaseSecciones == null || !_stateService.PlantillaBaseSecciones.Any()) {

                    var plantilla = await _plantillasRepository.ObtenerPlantillaAsync(UserSession.IdRol);

                    if(plantilla == null || !plantilla.Any()) {
                        await Shell.Current.DisplayAlert("Atención", "No se pudo obtener el catálogo de secciones y preguntas.", "OK");
                        return;
                    }

                    _stateService.PlantillaBaseSecciones = plantilla;
                }

                // 3. Navegación
                await Shell.Current.GoToAsync("SeleccionPisosPage");

            } catch(Exception ex) {
                System.Diagnostics.Debug.WriteLine($"Error en VerFormularioSupervision: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "Ocurrió un inconveniente al abrir la supervisión.", "OK");
            } finally {
                IsLoading = false;
            }
        }
    }
}