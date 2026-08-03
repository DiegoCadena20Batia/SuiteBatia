using BatiaSuite.ViewModel.SupervisionMantenimiento.Operarios;

namespace BatiaSuite.Views.SupervisionMantenimiento.Operarios;

public partial class SupervisionMantenimientoOperarioPage : ContentPage
{
    public SupervisionMantenimientoOperarioPage(SupervisionMantenimientoOperarioViewModel viewModel)
	{
		InitializeComponent();
       BindingContext=viewModel;
    }
}