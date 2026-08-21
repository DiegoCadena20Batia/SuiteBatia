
using BatiaSuite.ViewModel.SupervisionMantenimiento.Supervisores;

namespace BatiaSuite.Views.SupervisionMantenimiento.Supervisores;

public partial class SupervisionMantenimientoSupervisorPage : ContentPage
{

    public SupervisionMantenimientoSupervisorPage(SupervisionMantenimientoSupervisorViewModel viewModel)
	{
		InitializeComponent();
       BindingContext= viewModel;
    }
}