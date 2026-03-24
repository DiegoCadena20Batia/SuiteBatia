using BatiaSuite.Controls;
using BatiaSuite.ViewModel.CheckListAparadores;

namespace BatiaSuite.Views.CheckListAparadores;

public partial class CheckListAparadoresPreguntasResumenPage : MasterPage {
    public CheckListAparadoresPreguntasResumenPage(
        CheckListAparadoresPreguntasResumenViewModel vm) {
        InitializeComponent();

        // Pasa los DrawingViews al ViewModel
        vm.SetDrawingViews(gerenteDrawingView, aparadoristaDrawingView,encargadoDrawingView,auditorDrawingView);

        BindingContext = this;
        MasterPageContent.BindingContext = vm;
    }
}
