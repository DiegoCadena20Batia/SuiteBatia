using BatiaSuite.ViewModel;

namespace BatiaSuite.Views;

public partial class CorrectivosMayores : ContentPage {
    public CorrectivosMayores(CorrectivosMayoresViewModel vm) {
        InitializeComponent();
        BindingContext = vm;
    }
}