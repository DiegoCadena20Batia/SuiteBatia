
using BatiaSuite.ViewModel.SupervisionMantenimiento.Supervisores;

namespace BatiaSuite.Views.SupervisionMantenimiento.Supervisores;

public partial class SupervisionMantenimientoSupervisorPage : ContentPage {
    private readonly SupervisionMantenimientoSupervisorViewModel _viewModel;

    public SupervisionMantenimientoSupervisorPage(SupervisionMantenimientoSupervisorViewModel viewModel) {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing() {
        base.OnAppearing();

        if(_viewModel != null) {
            await _viewModel.CargarDatosInicialesCommand.ExecuteAsync(null);
        }
    }
}