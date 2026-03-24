using BatiaSuite.Data;
using BatiaSuite.Models;
using BatiaSuite.Models.SolicitudCotizacion;
using BatiaSuite.Models.Supervision;
using BatiaSuite.Utils;
using BatiaSuite.ViewModel.Popups;
using BatiaSuite.Views.SolicitudCotizacion;
using BatiaSuite.Views.Supervision;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO.Compression;
using System.Text;

namespace BatiaSuite.ViewModel.SolicitudCotizacion;

public partial class SolicitudCotizacionViewModel : ViewModelBase
{

    [ObservableProperty]
    ClientsModel _selectedCliente;

    [ObservableProperty]
    EstadoModel _selectedEstado;

    [ObservableProperty]
    Inmueble _selectedInmueble;

   
    [ObservableProperty]
    bool _isLoading;

    [ObservableProperty]
    string _textLoading;

    [ObservableProperty]
    //[NotifyCanExecuteChangedFor(nameof(GetYearCommand), nameof(GetMonthCommand))]
    bool _isBusy;
    [ObservableProperty]
    private ObservableCollection<SolicitudCotizacionProductos> _productos;
    [ObservableProperty]
    private SolicitudCotizacionProductos _productoSeleccionado;
    public SolicitudCotizacionViewModel()
    {
        SelectedCliente = new ClientsModel();
        SelectedEstado = new EstadoModel();
        SelectedInmueble = new Inmueble();
        Productos = new ObservableCollection<SolicitudCotizacionProductos>();

        InitValues();
    }

    async void InitValues() {
        IsLoading = true;
        await Constants.LoadClientesAsync();
        await Constants.LoadEstadosAsync();
        Constants.LoadProcedimientosAsync();

        IsLoading = false;
    }
    [RelayCommand(CanExecute = nameof(CanExecute))]
    async void SelectCliente() {
        IsBusy = true;
        IsLoading = true;



        ClientsModel cliente = await Constants.GetClienteAsync(SelectedCliente);
        if(cliente == null) {

            //SI EL ENDPOINT NO RESPONDE OBTENER DEL LOCAL
            await Toast.Make(Constants.ERROR_API_GET, ToastDuration.Short).Show();
            IsBusy = false;
            IsLoading = false;
            return;
        }
        IsLoading = false;
        if(cliente.Equals(SelectedCliente)) {
            IsBusy = false;
            IsLoading = false;
            return;
        }

        SelectedCliente = cliente;

        SelectedEstado = new EstadoModel();
        SelectedInmueble = new Inmueble();
        IsLoading = false;
        IsBusy = false;
    }
    [RelayCommand(CanExecute = nameof(CanExecute))]
    async void SelectInmueble() {
        IsLoading = true;
        IsBusy = true;
        if(!InternetUtil.IsConnectedInternet()) {
            //OBTENER CLIENTES DE LOCAL
        }

        if(SelectedCliente.idCliente == 0) {
            await App.Current.MainPage.DisplayAlert("", Constants.SELECCIONE_CLIENTE, Constants.ACEPTAR);
            IsBusy = false;
            IsLoading = false;
            return;
        }

        Inmueble inmueble = await Constants.GetInmuebleAsync(SelectedCliente.idCliente, SelectedEstado.id_estado, SelectedInmueble);
        if(inmueble == null) {
            await Toast.Make(Constants.ERROR_API_GET, ToastDuration.Short).Show();
            IsBusy = false;
            IsLoading = false;
            return;
        }
        IsLoading = false;
        if(inmueble.Equals(SelectedInmueble)) {
            IsBusy = false;
            IsLoading = false;
            return;
        }

        SelectedInmueble = inmueble;
        IsLoading = false;
        IsBusy = false;
    }

    bool CanExecute()
    {
        return !IsBusy;
    }

    public void IniciarCarga(string mensaje)
    {
        IsLoading = true;
        TextLoading = mensaje;
    }

    public void DetenerCarga()
    {
        IsLoading = false;
        TextLoading = "";
    }

    [RelayCommand]
    private async Task AgregarProducto() {
        var popup = new AgregarProductoPopup();
        var viewModel = (AgregarProductoViewModel)popup.BindingContext;

        viewModel.InicializarParaAgregar((producto) =>
        {
            // Callback cuando se guarda el producto
            Productos.Add(producto);
        });

        await MopupService.Instance.PushAsync(popup);
    }

    [RelayCommand]
    private async Task EditarProducto(SolicitudCotizacionProductos producto) {
        var popup = new AgregarProductoPopup();
        var viewModel = (AgregarProductoViewModel)popup.BindingContext;

        viewModel.InicializarParaEditar(producto, (productoEditado) =>
        {
            // Remover el producto original y agregar el editado
            Productos.Remove(producto);

            Productos.Add(productoEditado);
        });

        await MopupService.Instance.PushAsync(popup);
    }

    [RelayCommand]

    private async Task EliminarProducto(SolicitudCotizacionProductos producto) {
        bool confirmar = await App.Current.MainPage.DisplayAlert("Confirmar", "¿Estás seguro de que deseas eliminar este producto?", "Sí", "No");
        if(confirmar) {
            Productos.Remove(producto);
        }
    }

}