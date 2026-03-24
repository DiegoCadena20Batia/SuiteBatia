using BatiaSuite.Models.OrdenesTrabajo;
using BatiaSuite.ViewModel.Popups;
using Mopups.Pages;

namespace BatiaSuite.Popups;

public partial class AlmacenPicker : PopupPage {

    public AlmacenPickerViewModel _viewModel;

    public AlmacenPicker(AlmacenModel oldAlmacen) {
        InitializeComponent();
        _viewModel = new AlmacenPickerViewModel(oldAlmacen);
        BindingContext = _viewModel;
    }
}