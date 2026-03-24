using BatiaSuite.Data;
using BatiaSuite.Models;
using BatiaSuite.Models.Supervision;
using BatiaSuite.Utils;
using BatiaSuite.Views.Supervision;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BatiaSuite.ViewModel.Supervision;

public partial class SupervisionInmuebleViewModel : ViewModelBase {

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

    DbContext _dbContext;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ContinuarCommand), nameof(SelectClienteCommand), nameof(SelectEstadoCommand), nameof(SelectInmuebleCommand))]
    bool _isBusy;

    public SupervisionInmuebleViewModel() {
        SelectedCliente = new ClientsModel();
        SelectedEstado = new EstadoModel();
        SelectedInmueble = new Inmueble();
        InitValues();
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
    async void SelectEstado() {

        IsBusy = true;
        IsLoading = true;
        if (!InternetUtil.IsConnectedInternet())
        {
            //OBTENER CLIENTES DE LOCAL
        }
        var estado = await Constants.GetEstadoAsync(SelectedEstado);
        if(estado == null) {
            await Toast.Make(Constants.ERROR_API_GET, ToastDuration.Short).Show();
            IsBusy = false;
            IsLoading = false;
            return;
        }
        IsLoading = false;
        if(SelectedEstado.Equals(estado)) {
            IsBusy = false;
            IsLoading = false;
            return;
        }

        SelectedEstado = estado;
        SelectedInmueble = new Inmueble();
        IsLoading = false;
        IsBusy = false;
    }

    [RelayCommand(CanExecute = nameof(CanExecute))]
    async void SelectInmueble() {
        IsLoading = true;
        IsBusy = true;
        if (!InternetUtil.IsConnectedInternet())
        {
            //OBTENER CLIENTES DE LOCAL
        }

        if (SelectedCliente.idCliente == 0) {
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

    [RelayCommand(CanExecute = nameof(CanExecute))]
    async Task Continuar() {
        try {


            IsBusy = true;
            IsLoading = true;
            if(SelectedInmueble.IdInmueble == 0) {
                await Toast.Make(Constants.SELECCIONE_PUNTO_ATENCION, ToastDuration.Short).Show();
                IsBusy = false;
                IsLoading = false;
                return;
            }

            TipoSucursal tipoInmueble = SelectedInmueble.Tipo == 0 ? (TipoSucursal)1 : (TipoSucursal)SelectedInmueble.Tipo;
            _dbContext = new DbContext();
            var tieneBanco = await _dbContext.VerificarBancoInmueble(SelectedInmueble.IdInmueble);

            SupervisionRequestDataModel supervisionRequestData = new SupervisionRequestDataModel {
                Id_Cliente = SelectedCliente.idCliente,
                Id_Inmueble = SelectedInmueble.IdInmueble,
                AreaBanco = tieneBanco,
                TipoSucursal = tipoInmueble,
                Fechaini = DateTime.Now,
                Usuario = UserSession.IdPersonal,
                Anio = DateTime.Now.Year,
                Mes = DateTime.Now.Month,
                Cliente = SelectedCliente.nombre,
                Inmueble = SelectedInmueble.Nombre
            };

            List<SeccionTipoSucursal> secciones = await SeccionTipoSucursal.ObtenerSeccionesPorTipoSucursal(tipoInmueble, supervisionRequestData.Id_Inmueble);
            if(secciones == null) {
                await Toast.Make(Constants.ERROR_API_GET, ToastDuration.Short).Show();
                IsLoading = false;
                IsBusy = false;
                return;
            } else {
                Dictionary<string, object> data = new Dictionary<string, object>{
            { Constants.SUPERVISION_REQUEST_DATA_KEY, supervisionRequestData },
            { Constants.SECCIONES_KEY, secciones },
            { Constants.INDICE_KEY, 0 }
        };

                await Constants.GoToAsync(nameof(PreguntasPage), data);
                IsLoading = false;
                IsBusy = false;
            }
        } catch(Exception ex) {
            await Toast.Make(ex.Message, ToastDuration.Long).Show();
            IsLoading = false;
            IsBusy = false;
        }

    }

    async void InitValues() {
        IsLoading = true;
        _dbContext = new DbContext();
        await Constants.LoadClientesAsync();
        await Constants.LoadEstadosAsync();
        Constants.LoadProcedimientosAsync();

        IsLoading = false;
    }

    bool CanExecute() {
        return !IsBusy;
    }
}
