using BatiaSuite.ViewModel.SupervisionMantenimiento.Operarios;
using BatiaSuite.ViewModel.SupervisionMantenimiento.Supervisores;

namespace BatiaSuite.Views.SupervisionMantenimiento.Operarios;

public partial class SeccionesFormularioPage : ContentPage
{
	public SeccionesFormularioPage(SeccionesFormularioViewModel _viewModel)
	{
		InitializeComponent();
		BindingContext = _viewModel;
	}

    protected override void OnAppearing() {
        base.OnAppearing();

        // Invoca el método de actualización del ViewModel
        if(BindingContext is SeccionesFormularioViewModel vm) {
            vm.CargarSecciones();
        }
    }
}