namespace BatiaSuite.Views;

public partial class MenuSuite : ContentPage {
    public MenuSuite() {
        InitializeComponent();
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e) {
        Frame selectedFrame = (Frame)sender;

        selectedFrame.BackgroundColor = Color.FromArgb("#FFC8C8C8");
        await Task.Delay(100);
        selectedFrame.BackgroundColor = Color.FromArgb("#FFFFFF");
    }
}