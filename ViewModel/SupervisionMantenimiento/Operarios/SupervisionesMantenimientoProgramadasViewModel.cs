using BatiaSuite.Models.OrdenesTrabajo;
using BatiaSuite.Utils;
using BatiaSuite.ViewModel.OrdenesTrabajo;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SlackAPI;
using System.Collections.ObjectModel;

namespace BatiaSuite.ViewModel {

    using BatiaSuite.Models.SupervisionMantenimiento.Operarios;
    using BatiaSuite.Services.SupervisionesMantenimiento;
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using System.Collections.ObjectModel;

    public partial class SupervisionesMantenimientoProgramadasViewModel : ObservableObject {
        #region Variables y Servicios
        private readonly HttpHelper _httpHelper;
        private readonly SupervisionStateService _stateService;
        private readonly string _baseUrlApi = Constants.API_BASE_URL;

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
        #endregion

        public SupervisionesMantenimientoProgramadasViewModel(
            HttpHelper httpHelper,
            SupervisionStateService stateService) {
            _httpHelper = httpHelper;
            _stateService = stateService;

            InitValues();
            _ = CargarOrdenesAsync();
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
                IsLoading = true;

                int idTecnico = UserSession.IdEmpleado;
                int mes = _filterMonth;
                int anio = FilterYear;

                string url = $"{_baseUrlApi}SupervisionMantenimientoProgramada/SupervisionesProgramadas?idTecnico={idTecnico}";
                var resultado = await _httpHelper.GetAsync<List<OrdenTrabajoModel>>(url);

                if(resultado != null) {
                    var filtradas = resultado.Where(x => {
                        if(DateTime.TryParse(x.falta, out DateTime fecha)) {
                            return fecha.Month == mes && fecha.Year == anio;
                        }
                        return false;
                    }).ToList();

                    Ordenes = new ObservableCollection<OrdenTrabajoModel>(filtradas);
                    OnFiltroTextoChanged(FiltroTexto);
                }
            } catch(Exception ex) {
                System.Diagnostics.Debug.WriteLine($"Error al cargar órdenes: {ex.Message}");
            } finally {
                IsLoading = false;
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private async Task VerFormularioSupervision(OrdenTrabajoModel ordenSeleccionada) {
            if(ordenSeleccionada == null || IsLoading) return;

            try {
                IsLoading = true;

                // 1. Limpiar sesión e inicializar la orden seleccionada en el StateService
                _stateService.LimpiarSesion();
                _stateService.OrdenActual = ordenSeleccionada;
                _stateService.FechaInicio = DateTime.Now;

                // 2. Si la plantilla de secciones y preguntas no está cargada, la consultamos
                if(_stateService.PlantillaBaseSecciones == null || !_stateService.PlantillaBaseSecciones.Any()) {
                    string urlPlantilla = $"{_baseUrlApi}SupervisionMantenimeintoChecklist?id_rol={UserSession.IdRol}";
                    var plantilla = await _httpHelper.GetAsync<List<SeccionModel>>(urlPlantilla);

                    if(plantilla == null || !plantilla.Any()) {
                        await Shell.Current.DisplayAlert("Atención", "No se pudo obtener el catálogo de secciones y preguntas.", "OK");
                        return;
                    }

                    // Guardamos la plantilla base con sus preguntas anidadas en el StateService
                    _stateService.PlantillaBaseSecciones = plantilla;
                }

                // 3. Navegamos a la pantalla de selección de pisos
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