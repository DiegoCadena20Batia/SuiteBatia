using BatiaSuite.ViewModel.SupervisionMantenimiento.Supervisores;

namespace BatiaSuite.Views.SupervisionMantenimiento.Supervisores;

public partial class SeleccionPisoSupervisorPage : ContentPage
{
    SeleccionPisoSupervisorViewModel _viewModel;
    public SeleccionPisoSupervisorPage(SeleccionPisoSupervisorViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
		BindingContext = _viewModel;
	}


    protected override void OnAppearing() {
        base.OnAppearing();

        // Invoca el método de actualización del ViewModel
        if(BindingContext is SeleccionPisoSupervisorViewModel vm) {
            vm.OnAppearing();
        }
    }
}