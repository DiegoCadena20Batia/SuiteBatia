using BatiaSuite.ViewModel.SupervisionMantenimiento.Supervisores;

namespace BatiaSuite.Views.SupervisionMantenimiento.Supervisores;

public partial class SeccionesFormularioSupervisorPage : ContentPage
{
	public SeccionesFormularioSupervisorPage(SeccionesFormularioSupervisorViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }

    protected override void OnAppearing() {
		if(BindingContext is SeccionesFormularioSupervisorViewModel vm) {
			vm.CargarSecciones();
		}
    }
}