namespace BatiaSuite.Views.SupplierDeliveries;

public partial class SupplierDeliveries : ContentPage
{
	public SupplierDeliveries()
	{
		InitializeComponent();
	}
    private async void Frame_Tapped(object sender, TappedEventArgs e) {
        Frame selectedFrame = (Frame)sender;
        selectedFrame.BackgroundColor = Color.FromArgb("#FFC8C8C8");
        await Task.Delay(100);
        selectedFrame.BackgroundColor = Color.FromArgb("#FFFFFFFF");
    }
}