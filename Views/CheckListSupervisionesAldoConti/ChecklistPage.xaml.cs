using BatiaSuite.ViewModel.CheckListSupervisionesAldoConti;

namespace BatiaSuite.Views.CheckListSupervisionesAldoConti;

public partial class ChecklistPage : ContentPage
{
	public ChecklistPage()
	{
		InitializeComponent();

        BindingContext = new ChecklistViewModel();
    }
}