using BatiaSuite.Data;
using BatiaSuite.Models;
using BatiaSuite.Models.Supervision;
using BatiaSuite.Models.SupervisionMantenimiento;
using BatiaSuite.Services;
using BatiaSuite.Utils;
using BatiaSuite.Views.Supervision;
using BatiaSuite.Views.SupervisionMantenimiento;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace BatiaSuite.ViewModel.Supervisionmantenimiento;

public partial class SupervisionMantenimientoInmuebleViewModel : ViewModelBase {

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
    //[NotifyCanExecuteChangedFor(nameof(ContinuarCommand), nameof(SelectClienteCommand), nameof(SelectEstadoCommand), nameof(SelectInmuebleCommand))]
    bool _isBusy;

    private readonly SupervisionMantenimientoService _supervisionMantenimientoService;

    public SupervisionMantenimientoInmuebleViewModel(SupervisionMantenimientoService supervisionMantenimientoService) {
        SelectedCliente = new ClientsModel();
        SelectedEstado = new EstadoModel();
        SelectedInmueble = new Inmueble();
        _supervisionMantenimientoService = supervisionMantenimientoService;
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
            _supervisionMantenimientoService.InitModel();
            _supervisionMantenimientoService.InicioSupervision(SelectedCliente.idCliente, SelectedInmueble.IdInmueble);
            if (!await ObtenerSeccionesPreguntasByIdCliente()) {
                return;
            }
                //await Constants.GoToAsync(nameof(SupervisionMantenimientoPreguntasPage));
                await Constants.GoToAsync(nameof(SupervisionMantenimientoSeccionesPage));
                IsLoading = false;
                IsBusy = false;
           // }
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
        _supervisionMantenimientoService.InitModel();
        IsLoading = false;
    }

    bool CanExecute() {
        return !IsBusy;
    }

    public async Task<bool> ObtenerSeccionesPreguntasByIdCliente() {
        IsBusy = true;
        IsLoading = true;
        if(SelectedInmueble.IdInmueble == 0) {
            await Toast.Make(Constants.SELECCIONE_PUNTO_ATENCION, ToastDuration.Short).Show();
            //IsBusy = false;
            //IsLoading = false;
            //return false;
        }
        try {
            Uri requestUri = new Uri(Constants.API_BASE_URL + "SupervisionMantenimientoPreguntas?idcliente=" + SelectedCliente.idCliente);
            var client = new HttpClient();
            var response = await client.GetAsync(requestUri);
            if(!response.IsSuccessStatusCode) {
                return false;
            }
            string jsonResponse = await response.Content.ReadAsStringAsync();
            var preguntas = JsonConvert.DeserializeObject<SupervisionMantenimientoSeccionPreguntaModel>(jsonResponse);

            if(preguntas != null && preguntas.Secciones!= null && preguntas.Secciones.Count > 0 && preguntas.Preguntas != null && preguntas.Preguntas.Count > 0) {
                _supervisionMantenimientoService.GuardarSeccionesPreguntas(preguntas);
            } else {
                await Toast.Make("No hay secciones disponibles para el cliente seleccionado", ToastDuration.Long).Show();
                return false;
                IsBusy = false;
                IsLoading = false;
            }
                return true;
        } catch(Exception ex) {
            await Toast.Make("Error" + ex.Message, ToastDuration.Long).Show();
            IsBusy = false;
            IsLoading = false;
            return false;
        } finally {
            IsBusy = false;
            IsLoading = false;
        }
    }
}

public class SupervisionMantenimientoSeccionPreguntaModel {
    public List<SupervisionMantenimientoSeccionesModel>? Secciones { get; set; }
    public List<SupervisionMantenimientoPreguntasModel>? Preguntas { get; set; }
}
