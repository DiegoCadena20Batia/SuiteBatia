namespace BatiaSuite.ViewModel.SupervisionMantenimiento.Operarios;

public partial class PreguntasSeccionPage : ContentPage
{
	public PreguntasSeccionPage(PreguntasSeccionViewModel _viewModel)
	{
		InitializeComponent();
		BindingContext= _viewModel;
	}
}