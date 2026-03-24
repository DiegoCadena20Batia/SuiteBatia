using BatiaSuite.Controls;
using BatiaSuite.ViewModel.Supervision;

namespace BatiaSuite.Views.Supervision;

public partial class EvaluacionPage : MasterPage {

    EvaluacionViewModel _viewModel;
    public EvaluacionPage() {
        InitializeComponent();
        _viewModel = new EvaluacionViewModel();
        BindingContext = _viewModel;
        MasterPageContent.BindingContext = _viewModel;
    }
}