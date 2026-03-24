using BatiaSuite.Controls;
using BatiaSuite.ViewModel.Vacantes;

namespace BatiaSuite.Views.Vacantes;

public partial class DocumentosPage : MasterPage {

    DocumentosViewModel _viewModel;

    public DocumentosPage() {
        InitializeComponent();
        _viewModel = new DocumentosViewModel();
        BindingContext = _viewModel;
        MasterPageContent.BindingContext = _viewModel;
    }
}