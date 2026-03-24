using BatiaSuite.Models;
using BatiaSuite.ViewModel;
using BatiaSuite.ViewModel.SupplierDeliveries;

namespace BatiaSuite.Views.SupplierDeliveries;

public partial class SupplierListadoMateriales : ContentPage
{
	public SupplierListadoMateriales()
	{
		InitializeComponent();
		BindingContext = new SupplierLiatadoMaterialesViewModel();
	}

    private void txt_Entregado_Completed(object sender, EventArgs e)
    {
        var entry = sender as Entry; //Obtienes el Entry que disparó el evento
        var valorEntry = entry.Text; //Obtienes el valor del Entry
        var material = entry.BindingContext as ListadoMaterialesModel; //Obtienes el objeto Material al que pertenece el Entry
        material.ModificarEntregado(int.Parse(valorEntry)); //Llamas al método del modelo que modifica la propiedad entregado
    }
}