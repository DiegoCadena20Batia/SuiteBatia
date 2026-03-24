using BatiaSuite.ViewModel;

namespace BatiaSuite.Views;

public partial class Logueo : ContentPage {

    public Logueo() {
        InitializeComponent();
        BindingContext = new LogueoViewModel();
    }
}
