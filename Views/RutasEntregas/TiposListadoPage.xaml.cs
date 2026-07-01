using BatiaSuite.ViewModel.RutasEntregas;

namespace BatiaSuite.Views.RutasEntregas;

public partial class TiposListadoPage : ContentPage
{
    public TiposListadoPage(TiposListadoViewModel viewModel) {
        InitializeComponent();
        BindingContext = viewModel;
    }
}