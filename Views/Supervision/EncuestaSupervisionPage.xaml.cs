using BatiaSuite.Controls;
using BatiaSuite.ViewModel.Supervision;

namespace BatiaSuite.Views.Supervision;

public partial class EncuestaSupervisionPage : MasterPage {

    EncuestaSupervisionViewModel _viewModel;

    public EncuestaSupervisionPage() {
        InitializeComponent();
        _viewModel = new EncuestaSupervisionViewModel(drawingView);
        BindingContext = _viewModel;
        MasterPageContent.BindingContext = _viewModel;
    }

}