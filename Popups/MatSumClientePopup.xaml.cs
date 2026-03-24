using BatiaSuite.ViewModel.Popups;
using Mopups.Pages;

namespace BatiaSuite.Popups;

public partial class MatSumClientePopup : PopupPage {

    public MatSumClienteViewModel viewModel;

    public MatSumClientePopup() {
        InitializeComponent();
        viewModel = new MatSumClienteViewModel();
        BindingContext = viewModel;
    }
}