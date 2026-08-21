using BatiaSuite.Controls;
using BatiaSuite.ViewModel.SupervisionMantenimiento;
using CommunityToolkit.Maui.Core;

namespace BatiaSuite.Views.SupervisionMantenimiento;

public partial class SupervisionMantenimientoPreguntasPage : MasterPage {
    private SupervisionMantenimientoPreguntasViewModel _viewModel;

    public SupervisionMantenimientoPreguntasPage(SupervisionMantenimientoPreguntasViewModel vm) {
        InitializeComponent();
        _viewModel = vm;

        // Configurar el BindingContext correctamente
        BindingContext = _viewModel;

        // Si tu MasterPage necesita BindingContext separado para el contenido
        if(MasterPageContent != null) {
            MasterPageContent.BindingContext = _viewModel;
        }
    }

    protected override async void OnAppearing() {
        base.OnAppearing();

        Console.WriteLine("=== OnAppearing de SupervisionMantenimientoPreguntasPage ===");

        if(_viewModel != null) {
            // Obtener parámetros de la navegación
            // Necesitas recibir estos parámetros de la página anterior
            var parameters = GetNavigationParameters();

            int clienteId = parameters.clienteId;
            int inmuebleId = parameters.inmuebleId;

            Console.WriteLine($"Parámetros recibidos: clienteId={clienteId}, inmuebleId={inmuebleId}");

            if(clienteId > 0 && inmuebleId > 0) {
                await _viewModel.InitializeAsync(clienteId, inmuebleId);
            } else {
                Console.WriteLine("ADVERTENCIA: Parámetros inválidos, usando valores por defecto");
                // Valores temporales para pruebas
                await _viewModel.InitializeAsync(1, 1);
            }
        } else {
            Console.WriteLine("ERROR: _viewModel es null");
        }
    }

    private (int clienteId, int inmuebleId) GetNavigationParameters() {
        try {
            // Método 1: Si usas Shell Navigation con query parameters
            if(Shell.Current?.CurrentState?.Location != null) {
                var route = Shell.Current.CurrentState.Location.ToString();
                Console.WriteLine($"Ruta actual: {route}");

                // Extraer parámetros de la query string
                if(route.Contains("?")) {
                    var queryString = route.Split('?')[1];
                    var parameters = System.Web.HttpUtility.ParseQueryString(queryString);

                    int clienteId = Convert.ToInt32(parameters["clienteId"] ?? "0");
                    int inmuebleId = Convert.ToInt32(parameters["inmuebleId"] ?? "0");

                    return (clienteId, inmuebleId);
                }
            }

            // Método 2: Si pasas parámetros vía Navigation
            if(Navigation.NavigationStack.Count > 0) {
                // Verifica si hay parámetros en la navegación anterior
            }
        } catch(Exception ex) {
            Console.WriteLine($"Error obteniendo parámetros: {ex.Message}");
        }

        return (0, 0);
    }

    // Método para hacer scroll al inicio (llamado desde el ViewModel)
    public async Task ScrollToTop() {
        try {
            // Buscar el ScrollView en el contenido
            //if(MasterPageContent?.Content is ScrollView scrollView) {
            //    await scrollView.ScrollToAsync(0, 0, true);
            //} else {
            //    // Intentar encontrar el ScrollView de otra manera
            //    var scrollViewByName = this.FindByName<ScrollView>("scrollView");
            //    if(scrollViewByName != null) {
            //        await scrollViewByName.ScrollToAsync(0, 0, true);
            //    }
            //}
        } catch(Exception ex) {
            Console.WriteLine($"Error en ScrollToTop: {ex.Message}");
        }
    }

    // Opcional: Manejar el evento de retroceso físico (Android)
    protected override bool OnBackButtonPressed() {
        if(_viewModel?.CanGoBack == true) {
            // Navegar a la sección anterior
            _ = _viewModel.Anterior();
            return true; // Indicar que manejamos el evento
        }

        return base.OnBackButtonPressed();
    }
}