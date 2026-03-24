using BatiaSuite.Controls;
using BatiaSuite.ViewModel.CheckListAparadores;

namespace BatiaSuite.Views.CheckListAparadores;

public partial class CheckListAparadoresPreguntasTresPage : MasterPage {

public CheckListAparadoresPreguntasTresPage(CheckListAparadoresPreguntasTresViewModel vm) {
        InitializeComponent();

        BindingContext = this;
        MasterPageContent.BindingContext = vm;
    }
}