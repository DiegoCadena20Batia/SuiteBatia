using BatiaSuite.Controls;
using BatiaSuite.ViewModel.CheckListAparadores;

namespace BatiaSuite.Views.CheckListAparadores;

public partial class CheckListAparadoresPreguntasDosPage : MasterPage {

    public CheckListAparadoresPreguntasDosPage(CheckListAparadoresPreguntasDosViewModel vm) {
        InitializeComponent();
        BindingContext = this;
        MasterPageContent.BindingContext = vm;
    }
}