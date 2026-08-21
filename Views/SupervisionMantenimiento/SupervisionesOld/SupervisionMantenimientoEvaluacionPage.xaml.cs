using BatiaSuite.Controls;
using BatiaSuite.ViewModel.SupervisionMantenimiento;

namespace BatiaSuite.Views.SupervisionMantenimiento;

public partial class SupervisionMantenimientoEvaluacionPage : MasterPage {

    SupervisionMantenimientoEvaluacionViewModel _viewModel;
    public SupervisionMantenimientoEvaluacionPage() {
        InitializeComponent();
        _viewModel = new SupervisionMantenimientoEvaluacionViewModel();
        BindingContext = _viewModel;
        MasterPageContent.BindingContext = _viewModel;
    }
}