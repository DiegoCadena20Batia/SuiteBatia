using BatiaSuite.Data;
using BatiaSuite.Models;
using BatiaSuite.Models.CheckListAparadores;
using BatiaSuite.Models.Supervision;
using BatiaSuite.Services;
using BatiaSuite.Utils;
using BatiaSuite.Views.CheckListAparadores;
using BatiaSuite.Views.Supervision;
using BatiaSuite.Views.SupervisionMantenimiento;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using System.Text;
using System.Windows.Input;

namespace BatiaSuite.ViewModel.CheckListAparadores;

public partial class CheckListAparadoresInmuebleViewModel : ViewModelBase {

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

    private readonly CheckListService _checkListService;


    public CheckListAparadoresInmuebleViewModel(CheckListService checkListService) {
        SelectedCliente = new ClientsModel();
        SelectedEstado = new EstadoModel();
        SelectedInmueble = new Inmueble();
        _checkListService = checkListService;
        
        InitValues();
        UserSession.IdClienteCheckList = 0;
        UserSession.IdInmuebleCheckList = 0;
    }
    public async Task LoadDataAsync() {
         //InitValues();
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
            UserSession.IdClienteCheckList = SelectedCliente.idCliente;
            UserSession.IdInmuebleCheckList = SelectedInmueble.IdInmueble;
            _checkListService.LimpiarDatos();

            //await Constants.GoToAsync(nameof(CheckListAparadoresPreguntasCuatroPage));
            //REDIRECCIONAR A PREGUNTAS CINCO DIRECTO PARA EFECTOS DE TESTEO
            await Constants.GoToAsync(nameof(CheckListAparadoresPreguntasUnoPage));
                IsLoading = false;
                IsBusy = false;
            
        } catch(Exception ex) {
            await Toast.Make(ex.Message, ToastDuration.Long).Show();
            IsLoading = false;
            IsBusy = false;
        }

    }

    async void InitValues() {
        try {
            IsLoading = true;
            _dbContext = new DbContext();
            await Constants.LoadClientesAsync();
            //await Constants.LoadEstadosAsync();
            // Constants.LoadProcedimientosAsync();
            await ObtenerSeccionesPreguntas();
            IsLoading = false;
        }
        catch(Exception ex) {
            await Toast.Make(ex.Message, ToastDuration.Long).Show();
        }
        
    }

    bool CanExecute() {
        return !IsBusy;
    }

    public async Task<bool> ObtenerSeccionesPreguntas() {
        try {
            Uri requestUri = new Uri(Constants.API_BASE_URL + "CheklistPreguntas");
            var client = new HttpClient();
            var response = await client.GetAsync(requestUri);
            if(!response.IsSuccessStatusCode) {
                // Manejar error


                return false;
            } 
                string jsonResponse = await response.Content.ReadAsStringAsync();
            var preguntas = JsonConvert.DeserializeObject<List<CheckListPreguntasModel>>(jsonResponse);
            
            if (preguntas != null && preguntas.Count > 0) {
                _checkListService.SetPreguntas(preguntas);
            }
            return true;
        }
        catch(Exception ex) {
            await Toast.Make(ex.Message, ToastDuration.Long).Show();
            return false;
        }
    }
    

    //}
    public ICommand GoBackCommand => new Command(async () =>
    {
        await Shell.Current.GoToAsync("..");
    });

}
