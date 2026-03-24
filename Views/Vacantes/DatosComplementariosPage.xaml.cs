using BatiaSuite.Controls;
using BatiaSuite.ViewModel.Vacantes;

namespace BatiaSuite.Views.Vacantes;

public partial class DatosComplementariosPage : MasterPage {

    DatosComplementariosViewModel _viewModel;

    public DatosComplementariosPage() {
        InitializeComponent();
        _viewModel = new DatosComplementariosViewModel();
        BindingContext = _viewModel;
        MasterPageContent.BindingContext = _viewModel;
    }
}