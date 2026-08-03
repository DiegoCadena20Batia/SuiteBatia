using BatiaSuite.ViewModel.SupervisionMantenimiento.Operarios;

namespace BatiaSuite.Views.SupervisionMantenimiento.Operarios;

public partial class PreguntasSeccionPage : ContentPage
{
    public PreguntasSeccionPage(PreguntasSeccionViewModel viewModel) {
        InitializeComponent();
        BindingContext = viewModel;
    }
}