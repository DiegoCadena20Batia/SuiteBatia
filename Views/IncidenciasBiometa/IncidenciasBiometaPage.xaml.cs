using BatiaSuite.Controls;
using BatiaSuite.ViewModel.IncidenciasBiometa;

namespace BatiaSuite.Views.IncidenciasBiometa;

public partial class IncidenciasBiometaPage : MasterPage {

    public IncidenciasBiometaPage() {
        InitializeComponent();
        BindingContext = new IncidenciasBiometaViewModel();
    }

}