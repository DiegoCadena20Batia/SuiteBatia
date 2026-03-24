using BatiaSuite.ViewModel.Vacantes;
using BatiaSuite.Controls;

namespace BatiaSuite.Views.Vacantes;

public partial class VacantesPage : MasterPage {

    VacantesViewModel _viewModel;

    public VacantesPage() {
        InitializeComponent();
        _viewModel = new VacantesViewModel();
        BindingContext = _viewModel;
        MasterPageContent.BindingContext = _viewModel;
    }

    private async void Frame_Tapped(object sender, TappedEventArgs e) {
        Frame selectedFrame = (Frame)sender;
        selectedFrame.BackgroundColor = Color.FromArgb("#FFC8C8C8");
        await Task.Delay(100);
        selectedFrame.BackgroundColor = Color.FromArgb("#FFFFFFFF");
    }
}