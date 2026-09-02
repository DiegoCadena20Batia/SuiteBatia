using BatiaSuite.ViewModel.SupervisionMantenimiento.Supervisores;

namespace BatiaSuite.Views.SupervisionMantenimiento.Supervisores;

public partial class ResumenSupervisionSupervisorPage : ContentPage
{
	private readonly ResumenSupervisionSupervisorViewModel _viewModel;
    public ResumenSupervisionSupervisorPage(ResumenSupervisionSupervisorViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
    }

    private void OnLimpiarFirmaTecnicoClicked(object sender, EventArgs e) {
        tecnicoDrawingView.Lines.Clear();
        _viewModel.LineasTecnico.Clear();
    }

    private void OnLimpiarFirmaClienteClicked(object sender, EventArgs e) {
        clienteDrawingView.Lines.Clear();
        _viewModel.LineasCliente.Clear();
    }
}