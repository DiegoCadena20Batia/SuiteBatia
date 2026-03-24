using BatiaSuite.ViewModel.Encuestas;

namespace BatiaSuite.Views.Encuestas;

public partial class EncuestaPage : ContentPage {

    EncuestaViewModel _viewModel;

    public EncuestaPage() {
        InitializeComponent();
        _viewModel = new EncuestaViewModel(drawingView);
        BindingContext = _viewModel;
    }
}