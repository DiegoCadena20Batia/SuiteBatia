using BatiaSuite.ViewModel;

namespace BatiaSuite.Views.SupervisionMantenimiento.Operarios;

public partial class SupervisionesMantenimientoProgramadasPage : ContentPage
{
	private readonly SupervisionesMantenimientoProgramadasViewModel _viewModel;
    public SupervisionesMantenimientoProgramadasPage(SupervisionesMantenimientoProgramadasViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
	}
}