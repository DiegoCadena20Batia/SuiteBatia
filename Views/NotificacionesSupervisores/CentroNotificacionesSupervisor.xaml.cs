using BatiaSuite.ViewModel.NotificacionesSupervisores;

namespace BatiaSuite.Views.NotificacionesSupervisores;

public partial class CentroNotificacionesSupervisor : ContentPage {
    private readonly CentroNotificacionesSupervisorViewModel _viewModel;

    public CentroNotificacionesSupervisor(CentroNotificacionesSupervisorViewModel viewModel) {
        InitializeComponent();

        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}