using BatiaSuite.Models;
using BatiaSuite.ViewModel;
using BatiaSuite.ViewModel.SupplierDeliveries;
using System.Collections.ObjectModel;

namespace BatiaSuite.Views.SupplierDeliveries;

public partial class SupplierDeliveriesDetail : ContentPage
{
	public SupplierDeliveriesDetail()
	{
		InitializeComponent();
		BindingContext = new SupplierDeliveriesDetailViewModel();
	}
}