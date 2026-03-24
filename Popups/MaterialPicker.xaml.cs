using BatiaSuite.ViewModel.Popups;
using Mopups.Pages;

namespace BatiaSuite.Popups;

public partial class MaterialPicker : PopupPage {

    public MaterialPickerViewModel _viewModel;

    public MaterialPicker(int idAlmacen, int idCliente) {
        InitializeComponent();
        _viewModel = new MaterialPickerViewModel(idAlmacen, idCliente);
        BindingContext = _viewModel;
    }
}