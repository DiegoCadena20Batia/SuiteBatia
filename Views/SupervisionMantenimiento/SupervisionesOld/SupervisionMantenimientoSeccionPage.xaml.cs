using BatiaSuite.Controls;
using BatiaSuite.ViewModel.Supervisionmantenimiento;
using BatiaSuite.ViewModel.SupervisionMantenimiento;

namespace BatiaSuite.Views.SupervisionMantenimiento;

public partial class SupervisionMantenimientoSeccionPage : MasterPage
{
	public SupervisionMantenimientoSeccionPage(SupervisionMantenimientoSeccionViewModel vm)
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
}