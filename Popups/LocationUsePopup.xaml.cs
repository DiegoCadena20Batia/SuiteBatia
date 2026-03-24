using Mopups.Pages;
using BatiaSuite.ViewModel.Popups;

namespace BatiaSuite.Popups;

public partial class LocationUse : PopupPage {

    public LocationUseViewModel _viewModel;

    public LocationUse() {
        InitializeComponent();
        _viewModel = new LocationUseViewModel();
        BindingContext = _viewModel;
    }
}