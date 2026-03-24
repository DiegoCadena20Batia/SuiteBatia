using BatiaSuite.ViewModel;

namespace BatiaSuite.Views;

public partial class DeliveriesRoute : ContentPage
{
    public DeliveriesRoute(DeliveriesRouteViewModel viewModel) {
        InitializeComponent();
        BindingContext = viewModel;
    }
}