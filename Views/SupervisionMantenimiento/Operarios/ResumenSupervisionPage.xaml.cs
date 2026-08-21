using BatiaSuite.ViewModel.SupervisionMantenimiento.Operarios;
using Shiny;

namespace BatiaSuite.Views.SupervisionMantenimiento.Operarios;

public partial class ResumenSupervisionPage : ContentPage
{
    private readonly ResumenSupervisionViewModel _viewModel;
    public ResumenSupervisionPage(ResumenSupervisionViewModel viewModel)
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