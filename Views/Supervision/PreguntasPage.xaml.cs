using BatiaSuite.Controls;
using BatiaSuite.ViewModel.Supervision;

namespace BatiaSuite.Views.Supervision;

public partial class PreguntasPage : MasterPage {

    PreguntasViewModel _viewModel;

    public PreguntasPage() {
        InitializeComponent();
        _viewModel = new PreguntasViewModel();
        BindingContext = _viewModel;
        MasterPageContent.BindingContext = _viewModel;
    }
}