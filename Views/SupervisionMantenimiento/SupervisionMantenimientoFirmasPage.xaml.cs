using BatiaSuite.Controls;
using BatiaSuite.ViewModel.SupervisionMantenimiento;

namespace BatiaSuite.Views.SupervisionMantenimiento;

public partial class SupervisionMantenimientoFirmasPage : MasterPage {
    public SupervisionMantenimientoFirmasPage(
        SupervisionMantenimientoFirmasViewModel vm) {
        InitializeComponent();

        // Pasa los DrawingViews al ViewModel
        vm.SetDrawingViews(administracionEntranteDrawingView, administracionSalienteDrawingView,testigoUnoDrawingView,testigoDosDrawingView, testigoTresDrawingView, testigoCuatroDrawingView);

        BindingContext = this;
        MasterPageContent.BindingContext = vm;
    }
}
