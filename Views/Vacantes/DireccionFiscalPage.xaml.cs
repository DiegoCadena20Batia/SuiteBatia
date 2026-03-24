using BatiaSuite.Controls;
using BatiaSuite.ViewModel.Vacantes;

namespace BatiaSuite.Views.Vacantes;

public partial class DireccionFiscalPage : MasterPage {

    DireccionFiscalViewModel _viewModel;

    public DireccionFiscalPage() {
        InitializeComponent();
        _viewModel = new DireccionFiscalViewModel();
        BindingContext = _viewModel;
        MasterPageContent.BindingContext = _viewModel;
    }
}