using BatiaSuite.Controls;
using BatiaSuite.ViewModel.Vacantes;

namespace BatiaSuite.Views.Vacantes;

public partial class DatosSueldoPage : MasterPage {

    DatosSueldoViewModel _viewModel;

    public DatosSueldoPage() {
        InitializeComponent();
        _viewModel = new DatosSueldoViewModel();
        BindingContext = _viewModel;
        MasterPageContent.BindingContext = _viewModel;
    }
}