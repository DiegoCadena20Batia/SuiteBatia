using BatiaSuite.ViewModel.Popups;
using Mopups.Pages;

namespace BatiaSuite.Popups;

public partial class ObjectPicker : PopupPage {

    public ObjectPickerViewModel _viewModel;

    public ObjectPicker(object oldValue, List<object> list, double divisor, bool showSearching) {
        InitializeComponent();
        _viewModel = new ObjectPickerViewModel(oldValue, list, divisor, showSearching);
        BindingContext = _viewModel;
    }
}