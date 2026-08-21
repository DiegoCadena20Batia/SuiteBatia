using BatiaSuite.ViewModel.SupervisionMantenimiento.Operarios;
using Shiny;

namespace BatiaSuite.Views.SupervisionMantenimiento.Operarios;

public partial class SeleccionPisosPage : ContentPage {

    public SeleccionPisosPage(SeleccionPisosViewModel viewModel) {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing() {
        base.OnAppearing();

        // Invoca el método de actualización del ViewModel
        if(BindingContext is SeleccionPisosViewModel vm) {
            vm.OnAppearing();
        }
    }
}