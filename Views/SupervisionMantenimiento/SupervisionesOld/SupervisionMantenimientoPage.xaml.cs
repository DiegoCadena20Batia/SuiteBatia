using BatiaSuite.ViewModel.SupervisionMantenimiento;

namespace BatiaSuite.Views.SupervisionMantenimiento;

public partial class SupervisionMantenimientoPage : ContentPage {

    public SupervisionMantenimientoPage() {
        InitializeComponent();
        BindingContext = new SupervisionMantenimientoViewModel();
    }

    private async void Frame_Tapped(object sender, TappedEventArgs e) {
        Frame selectedFrame = (Frame)sender;
        selectedFrame.BackgroundColor = Color.FromArgb("#FFC8C8C8");
        await Task.Delay(100);
        selectedFrame.BackgroundColor = Color.FromArgb("#FFFFFFFF");
    }
}