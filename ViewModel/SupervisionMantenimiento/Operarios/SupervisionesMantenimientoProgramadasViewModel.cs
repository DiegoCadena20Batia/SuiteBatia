using BatiaSuite.Models.OrdenesTrabajo;
using BatiaSuite.Utils;
using BatiaSuite.ViewModel.OrdenesTrabajo;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SlackAPI;
using System.Collections.ObjectModel;

namespace BatiaSuite.ViewModel {
    using BatiaSuite.Services.SupervisionesMantenimiento;
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using System.Collections.ObjectModel;

    public partial class SupervisionesMantenimientoProgramadasViewModel : ObservableObject {

        #region Variables
        private readonly HttpHelper _httpHelper;
        private readonly string _baseUrlApi = Constants.API_BASE_URL;
        private readonly SupervisionMantenimientoStateService _stateService;

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

        private List<int> _yearList = new();
        #endregion

        public SupervisionesMantenimientoProgramadasViewModel(HttpHelper httpHelper, SupervisionMantenimientoStateService stateService) {
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
                    .Where(x => x.sucursal != null && x.sucursal.Contains(value, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                SucursalesFiltradas = new ObservableCollection<OrdenTrabajoModel>(filtrados);
            }
        }

        [RelayCommand]
        private async Task GetYearAsync() {
            double size = Constants.IS_IOS ? (Constants.IS_TABLET ? 5 : 7) : (Constants.IS_TABLET ? 5 : 5);

            // PopupUtil espera la lista tipada o de objetos según su definición
            int value = (int)await PopupUtil.GetObjectAsync(FilterYear, _yearList.Cast<object>().ToList(), size);

            if(value == FilterYear || value == 0) return;

            FilterYear = value;
            await CargarOrdenesAsync();
        }

        [RelayCommand]
        private async Task GetMonthAsync() {
            double size = Constants.IS_IOS ? (Constants.IS_TABLET ? 5 : 5) : (Constants.IS_TABLET ? 3 : 3);
            string value = (string)await PopupUtil.GetObjectAsync(SelectedMonth, Constants.MonthList, size);

            if(string.IsNullOrEmpty(value) || value == SelectedMonth) return;

            SelectedMonth = value;
            _filterMonth = Constants.GetMonthNumber(SelectedMonth);

            await CargarOrdenesAsync();
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
                    var filtradas = resultado.Where(x =>
                    {
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
            if(ordenSeleccionada == null) return;


            _stateService.FechaInicio = DateTime.Now;
            // Preparamos el diccionario de parámetros con el objeto completo
            var navigationParameters = new Dictionary<string, object>
            {
        { "OrdenSeleccionada", ordenSeleccionada }
    };

            // Navegamos hacia la nueva ruta (ejemplo: "SeccionesFormularioPage") pasando los parámetros
            await Shell.Current.GoToAsync("SupervisionMantenimientoOperarioPage", navigationParameters);
        }
    }
}