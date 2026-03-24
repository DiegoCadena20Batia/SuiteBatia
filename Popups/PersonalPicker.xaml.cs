using BatiaSuite.ViewModel.Popups;
using Mopups.Pages;

namespace BatiaSuite.Popups;

public partial class PersonalPicker : PopupPage {

    public PersonalPickerViewModel _viewModel;

    public PersonalPicker() {
        InitializeComponent();
        _viewModel = new PersonalPickerViewModel();
        BindingContext = _viewModel;
    }
}