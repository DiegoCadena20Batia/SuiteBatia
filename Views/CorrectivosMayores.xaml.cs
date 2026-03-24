using BatiaSuite.ViewModel;

namespace BatiaSuite.Views;

public partial class CorrectivosMayores : ContentPage
{
    public CorrectivosMayores()
	{
		InitializeComponent();
        BindingContext = new CorrectivosMayoresViewModel();
    }
}