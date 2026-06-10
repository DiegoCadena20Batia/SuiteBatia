using BatiaSuite.ViewModel.VersionApp;
using Mopups.Pages;
using Mopups.Services;


namespace BatiaSuite.Popups.VersionApp;

public partial class VersionAppPopup : PopupPage {

    public VersionAppViewModel _viewModel;

    public VersionAppPopup() {
        InitializeComponent();
        _viewModel = new VersionAppViewModel();
        BindingContext = _viewModel;
    }


    private void buttonAceptar_Clicked(object sender, EventArgs e) {
        buttonCancel_Clicked(null, null);
    }

    private void buttonCancel_Clicked(object sender, EventArgs e) {
        MopupService.Instance.PopAsync();
    }
}