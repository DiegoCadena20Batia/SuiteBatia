using BatiaSuite.ViewModel.SupervisionMantenimiento.Operarios;

namespace BatiaSuite.Views.SupervisionMantenimiento.Operarios;

public partial class PreguntasSeccionPage : ContentPage
{
    public PreguntasSeccionPage(PreguntasSeccionViewModel viewModel) {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing() {
        base.OnAppearing();
        await Task.Yield();
    }
}