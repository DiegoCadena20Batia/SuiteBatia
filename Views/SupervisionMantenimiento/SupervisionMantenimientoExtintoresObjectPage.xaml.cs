using BatiaSuite.Controls;
using BatiaSuite.ViewModel.Supervisionmantenimiento;
using BatiaSuite.ViewModel.SupervisionMantenimiento;

namespace BatiaSuite.Views.SupervisionMantenimiento;

public partial class SupervisionMantenimientoExtintoresObjectPage : MasterPage
{
	public SupervisionMantenimientoExtintoresObjectPage(SupervisionMantenimientoExtintoresObjectViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
        MasterPageContent.BindingContext = vm;

        vm.ScrollPreguntasToTop += ScrollPreguntasToTop;
    }
    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e) {
        Frame selectedFrame = (Frame)sender;
        selectedFrame.BackgroundColor = Color.FromArgb("#FFC8C8C8");
        await Task.Delay(100);
        selectedFrame.BackgroundColor = Color.FromArgb("#FFFFFFFF");
    }

    private void ScrollPreguntasToTop() {
        MainThread.BeginInvokeOnMainThread(() => {
            if(PreguntasCollection.ItemsSource != null) {
                PreguntasCollection.ScrollTo(
                    0,
                    position: ScrollToPosition.Start,
                    animate: true
                );
            }
        });
    }
}