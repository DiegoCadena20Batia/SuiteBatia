using BatiaSuite.Controls;
using BatiaSuite.ViewModel.CheckListAparadores;

namespace BatiaSuite.Views.CheckListAparadores;

public partial class CheckListAparadoresPreguntasCincoPage : MasterPage {

    CheckListAparadoresPreguntasCincoViewModel _viewModel;

    public CheckListAparadoresPreguntasCincoPage( CheckListAparadoresPreguntasCincoViewModel vm) {
        InitializeComponent();
        BindingContext = this;
        MasterPageContent.BindingContext = vm;
    }
}