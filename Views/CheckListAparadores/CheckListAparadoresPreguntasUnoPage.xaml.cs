using BatiaSuite.Controls;
using BatiaSuite.ViewModel.CheckListAparadores;

namespace BatiaSuite.Views.CheckListAparadores;

public partial class CheckListAparadoresPreguntasUnoPage : MasterPage {
    public CheckListAparadoresPreguntasUnoPage(CheckListAparadoresPreguntasUnoViewModel vm) {
        InitializeComponent();

        BindingContext = this;
        MasterPageContent.BindingContext = vm;
    }
}

