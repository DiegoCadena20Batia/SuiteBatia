using BatiaSuite.Controls;
using BatiaSuite.ViewModel.CheckListAparadores;

namespace BatiaSuite.Views.CheckListAparadores;

public partial class CheckListAparadoresPreguntasCuatroPage : MasterPage {
    public CheckListAparadoresPreguntasCuatroPage(CheckListAparadoresPreguntasCuatroViewModel vm) {
        InitializeComponent();
        BindingContext = this;
        MasterPageContent.BindingContext = vm;
    }
}