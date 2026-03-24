using BatiaSuite.Controls;
using BatiaSuite.ViewModel.Supervisionmantenimiento;

namespace BatiaSuite.Views.SupervisionMantenimiento;

public partial class SupervisionMantenimientoInmueblePage : MasterPage
{
	public SupervisionMantenimientoInmueblePage( SupervisionMantenimientoInmuebleViewModel vm)
	{
		InitializeComponent();
		BindingContext = this;
		MasterPageContent.BindingContext = vm;
    }
}