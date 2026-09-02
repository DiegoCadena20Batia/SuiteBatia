using BatiaSuite.ViewModel.SupervisionMantenimiento.Supervisores;

namespace BatiaSuite.Views.SupervisionMantenimiento.Supervisores;

public partial class PreguntasSeccionSupervisorPage : ContentPage
{
	public PreguntasSeccionSupervisorPage(PreguntasSeccionSupervisorViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }
}