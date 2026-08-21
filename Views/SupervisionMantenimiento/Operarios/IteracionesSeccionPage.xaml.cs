using BatiaSuite.ViewModel.SupervisionMantenimiento.Operarios;

namespace BatiaSuite.Views.SupervisionMantenimiento.Operarios;

public partial class IteracionesSeccionPage : ContentPage
{
    public IteracionesSeccionPage(IteracionesSeccionViewModel _viewModel)
	{
		InitializeComponent();
		BindingContext= _viewModel;
	}

    protected override void OnAppearing() {
        if(BindingContext is IteracionesSeccionViewModel vm) {
            vm.CargarIteraciones();
        }
    }
}