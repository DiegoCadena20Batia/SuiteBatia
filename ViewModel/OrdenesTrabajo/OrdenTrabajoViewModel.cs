using BatiaSuite.Data;
using BatiaSuite.Models.OrdenesTrabajo;
using BatiaSuite.Utils;
using BatiaSuite.Views.OrdenesTrabajo;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace BatiaSuite.ViewModel.OrdenesTrabajo;

public partial class OrdenTrabajoViewModel : ViewModelBase {

    [ObservableProperty]
    private ObservableCollection<OrdenTrabajoModel> _listOrdenes;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _textLoading;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SelectedOrdenCommand))]
    private bool _isBusy;

    private int _idEmpleado = 0;

    private DbContext _dbContext;

    public OrdenTrabajoViewModel() {
        _dbContext = new DbContext();
        _idEmpleado = UserSession.IdEmpleado;
        InitValues();
    }

    private async void InitValues() {
        //aqui validar si el usuario es de tipo cliente o es el que solo va a ver las ordenes de trabajo programadas
        await GetOrdersAsync();
    }

    private async Task GetOrdersAsync() {
        if(!Utils.InternetUtil.IsConnectedInternet()) {
            var ordenesLoclaes = await _dbContext.ObtenerOrdenesTrabajoLocales();
            ListOrdenes = new ObservableCollection<OrdenTrabajoModel>(ordenesLoclaes);
        } else {
            IsLoading = true;
            TextLoading = "Cargando órdenes de trabajo...";
            string url = $"{Constants.ORDENES_TRABAJO_API}?idtecnico={_idEmpleado}";
            ListOrdenes = await _httpHelper.GetAsync<ObservableCollection<OrdenTrabajoModel>>(url);
            TextLoading = "";
            IsLoading = false;
        }
    }

    [RelayCommand(CanExecute = nameof(CanSelectedOrden))]
    private async Task SelectedOrden(OrdenTrabajoModel ordenSeleccionada) {
        IsBusy = true;
        IsLoading = true;
        TextLoading = "Cargando...";
        Trabajo trabajo = new Trabajo {
            IdOrden = ordenSeleccionada.idOrden,
            IdCliente = ordenSeleccionada.idCliente,
            Cliente = ordenSeleccionada.cliente,
            Inmueble = ordenSeleccionada.sucursal,
            Descripcion = ordenSeleccionada.descripcion,
            Falta = ordenSeleccionada.falta,
            Tipo = ordenSeleccionada.tipomanto
        };

        OrdenTrabajoEjecutadaModel orden = new OrdenTrabajoEjecutadaModel {
            Trabajo = trabajo
        };

        Dictionary<string, object> data = new Dictionary<string, object>            {
            {Constants.ORDEN_TRABAJO_KEY, orden}
        };

        await Shell.Current.GoToAsync($"{nameof(ManoObra)}", true, data);

        IsBusy = false;
        IsLoading = false;
        TextLoading = "";
    }

    private bool CanSelectedOrden() {
        return !IsBusy;
    }
}