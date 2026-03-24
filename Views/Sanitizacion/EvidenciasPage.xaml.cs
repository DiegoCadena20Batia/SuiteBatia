using BatiaSuite.Controls;
using BatiaSuite.ViewModel.Sanitizacion;

namespace BatiaSuite.Views.Sanitizacion;

public partial class EvidenciasPage : MasterPage {

    EvidenciasViewModel _viewModel;

    public EvidenciasPage() {
        InitializeComponent();
        _viewModel = new EvidenciasViewModel(drawingView);
        BindingContext = _viewModel;
        MasterPageContent.BindingContext = _viewModel;
    }
}