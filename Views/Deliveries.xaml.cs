using BatiaSuite.ViewModel;

namespace BatiaSuite.Views;

public partial class Deliveries : ContentPage
{
    public Deliveries(DeliveriesViewModel viewModel) {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing() {
        base.OnAppearing();

        // Si en el futuro necesitas forzar alguna recarga visual o de red 
        // CADA VEZ que la pantalla vuelve al frente, se hace aquí.
        // Por ahora, la inicialización de GetMes() y GetRutas() ya vive 
        // en el constructor de tu ViewModel.
    }

    protected override void OnDisappearing() {
        base.OnDisappearing();
    }

  
}