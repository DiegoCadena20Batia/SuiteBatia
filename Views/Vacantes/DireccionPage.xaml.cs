using BatiaSuite.Controls;
using BatiaSuite.ViewModel.Vacantes;

namespace BatiaSuite.Views.Vacantes;

public partial class DireccionPage : MasterPage {

    DireccionViewModel _viewModel;

    public DireccionPage() {
        InitializeComponent();
        _viewModel = new DireccionViewModel();
        BindingContext = _viewModel;
        MasterPageContent.BindingContext = _viewModel;
    }
}