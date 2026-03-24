using BatiaSuite.Controls;
using BatiaSuite.ViewModel.CheckListAparadores;

namespace BatiaSuite.Views.CheckListAparadores;

public partial class CheckListAparadoresInmueblePage : MasterPage {
    public CheckListAparadoresInmueblePage(CheckListAparadoresInmuebleViewModel vm) {
        InitializeComponent();
        BindingContext = vm;
    }

}