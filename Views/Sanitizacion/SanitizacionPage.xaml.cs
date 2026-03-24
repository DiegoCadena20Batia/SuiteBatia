using BatiaSuite.Controls;
using BatiaSuite.ViewModel.Sanitizacion;

namespace BatiaSuite.Views.Sanitizacion;

public partial class SanitizacionPage : MasterPage {

    SanitizacionViewModel _viewModel;
    public SanitizacionPage() {
        InitializeComponent();
        _viewModel = new SanitizacionViewModel();
        BindingContext = _viewModel;
        MasterPageContent.BindingContext = _viewModel;
    }
}