using BatiaSuite.Controls;
using BatiaSuite.ViewModel.Vacantes;

namespace BatiaSuite.Views.Vacantes;

public partial class DatosGeneralesPage : MasterPage {

    DatosGeneralesViewModel _viewModel;

    public DatosGeneralesPage() {
        InitializeComponent();
        _viewModel = new DatosGeneralesViewModel();
        BindingContext = _viewModel;
        MasterPageContent.BindingContext = _viewModel;
    }
}