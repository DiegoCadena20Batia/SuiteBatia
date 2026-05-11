using BatiaSuite.ViewModel;

namespace BatiaSuite.Views;

public partial class CorrectivosMayores : ContentPage {
    public CorrectivosMayores(CorrectivosMayoresViewModel vm) {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing() {
        base.OnAppearing();

        if(BindingContext is CorrectivosMayoresViewModel vm) {
            await vm.VerificarCorrectivosPendientes();
        }
    }
}