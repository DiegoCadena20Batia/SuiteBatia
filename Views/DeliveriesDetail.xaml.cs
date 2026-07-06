using BatiaSuite.Models;
using BatiaSuite.ViewModel;
using System.Collections.ObjectModel;

namespace BatiaSuite.Views;

public partial class DeliveriesDetail : ContentPage
{
	public DeliveriesDetail(DeliveriesDetailViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

    protected override async void OnAppearing() {
        base.OnAppearing();
        if(BindingContext is DeliveriesDetailViewModel vm) {
            await vm.CargarSucursalesDeRuta();
        }
    }
}