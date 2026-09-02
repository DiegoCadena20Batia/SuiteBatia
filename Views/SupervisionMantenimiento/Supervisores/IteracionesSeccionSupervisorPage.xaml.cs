using BatiaSuite.ViewModel.SupervisionMantenimiento.Supervisores;

namespace BatiaSuite.Views.SupervisionMantenimiento.Supervisores;

public partial class IteracionesSeccionSupervisorPage : ContentPage
{
	public IteracionesSeccionSupervisorPage(IteracionesSeccionSupervisorViewModel viewModel)
	{
		InitializeComponent();
		BindingContext= viewModel;
    }
}