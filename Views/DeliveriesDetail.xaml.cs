using BatiaSuite.Models;
using BatiaSuite.ViewModel;
using System.Collections.ObjectModel;

namespace BatiaSuite.Views;

public partial class DeliveriesDetail : ContentPage
{
	public DeliveriesDetail()
	{
		InitializeComponent();
		BindingContext = new DeliveriesDetailViewModel();
	}
}