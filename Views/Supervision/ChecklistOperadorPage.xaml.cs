using BatiaSuite.Controls;
using BatiaSuite.ViewModel.Supervision;

namespace BatiaSuite.Views.Supervision;

public partial class ChecklistOperadorPage : MasterPage {

    ChecklistOperadorViewModel _viewModel;

    public ChecklistOperadorPage() {
        InitializeComponent();
        _viewModel = new ChecklistOperadorViewModel(drawingView);
        BindingContext = _viewModel;
        MasterPageContent.BindingContext = _viewModel;
    }
}