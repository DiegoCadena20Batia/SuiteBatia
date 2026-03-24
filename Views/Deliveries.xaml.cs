using BatiaSuite.ViewModel;

namespace BatiaSuite.Views;

public partial class Deliveries : ContentPage
{
    public Deliveries(DeliveriesViewModel viewModel) {
        InitializeComponent();
        BindingContext = viewModel;
    }
    private async void Frame_Tapped(object sender, TappedEventArgs e) {
        Frame selectedFrame = (Frame)sender;
        selectedFrame.BackgroundColor = Color.FromArgb("#FFC8C8C8");
        await Task.Delay(100);
        selectedFrame.BackgroundColor = Color.FromArgb("#FFFFFFFF");
    }
}