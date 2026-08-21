using BatiaSuite.Controls;
using BatiaSuite.ViewModel.Supervisionmantenimiento;

namespace BatiaSuite.Views.SupervisionMantenimiento;

public partial class SupervisionMantenimientoSeccionesPage : MasterPage
{
	public SupervisionMantenimientoSeccionesPage(SupervisionMantenimientoSeccionesViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
        MasterPageContent.BindingContext = vm;
    }
    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e) {
        Frame selectedFrame = (Frame)sender;
        selectedFrame.BackgroundColor = Color.FromArgb("#FFC8C8C8");
        await Task.Delay(100);
        selectedFrame.BackgroundColor = Color.FromArgb("#FFFFFFFF");
    }
    protected override void OnAppearing() {
        base.OnAppearing();
        Console.WriteLine($"BindingContext: {BindingContext?.GetType().FullName}");
        if(BindingContext is SupervisionMantenimientoSeccionesViewModel vm) {
            vm.RefreshSecciones();
        }
    }

}