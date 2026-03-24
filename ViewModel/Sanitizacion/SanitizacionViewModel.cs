using BatiaSuite.Models;
using BatiaSuite.Models.Sanitizacion;
using BatiaSuite.Models.Supervision;
using BatiaSuite.Models.Vacantes;
using BatiaSuite.Utils;
using BatiaSuite.Views.Sanitizacion;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json.Serialization;

namespace BatiaSuite.ViewModel.Sanitizacion;

public partial class SanitizacionViewModel : ViewModelBase {

    [ObservableProperty]
    ClientsModel _selectedCliente;

    [ObservableProperty]
    EstadoModel _selectedEstado;

    [ObservableProperty]
    Inmueble _selectedInmueble = new Inmueble();

    [ObservableProperty]
    string _area;

    [ObservableProperty]
    CatalogoModel _selectedProcedimiento;

    [ObservableProperty]
    bool _isLoading;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SelectClienteCommand), nameof(SelectEstadoCommand), nameof(SelectInmuebleCommand), nameof(SelectProcedimientoCommand), nameof(ContinuarCommand))]
    bool _isBusy;

    public SanitizacionViewModel() {
        SelectedCliente = new ClientsModel();
        SelectedEstado = new EstadoModel();
        SelectedInmueble = new Inmueble();
        SelectedProcedimiento = new CatalogoModel();
        InitValues();
    }

    [RelayCommand(CanExecute = nameof(CanExecute))]
    async Task SelectCliente() {
        IsBusy = true;

        ClientsModel cliente = await Constants.GetClienteAsync(SelectedCliente);

        if(cliente.Equals(SelectedCliente)) {
            IsBusy = false;
            return;
        }

        SelectedCliente = cliente;
        SelectedEstado = new EstadoModel();
        SelectedInmueble = new Inmueble();

        IsBusy = false;
    }

    [RelayCommand(CanExecute = nameof(CanExecute))]
    async Task SelectEstado() {
        IsBusy = true;

        if(SelectedCliente.idCliente == 0) {
            await Toast.Make(Constants.SELECCIONE_CLIENTE, ToastDuration.Short).Show();
            IsBusy = false;
            return;
        }

        EstadoModel estado = await Constants.GetEstadoAsync(SelectedEstado);

        if(estado.Equals(SelectedEstado)) {
            IsBusy = false;
            return;
        }

        SelectedEstado = estado;
        SelectedInmueble = new Inmueble();

        IsBusy = false;
    }

    [RelayCommand(CanExecute = nameof(CanExecute))]
    async Task SelectInmueble() {
        IsBusy = true;

        if(SelectedCliente.idCliente == 0) {
            await Toast.Make(Constants.SELECCIONE_CLIENTE, ToastDuration.Short).Show();
            IsBusy = false;
            return;
        }

        Inmueble inmueble = await Constants.GetInmuebleAsync(SelectedCliente.idCliente, SelectedEstado.id_estado, SelectedInmueble);

        if(inmueble.Equals(SelectedInmueble)) {
            IsBusy = false;
            return;
        }

        SelectedInmueble = inmueble;

        IsBusy = false;
    }

    [RelayCommand(CanExecute = nameof(CanExecute))]
    async Task SelectProcedimiento() {
        IsBusy = true;
        double size = Constants.IS_IOS ? Constants.IS_TABLET ? 4.5 : 8 : Constants.IS_TABLET ? 2.5 : 4;
        SelectedProcedimiento = await Constants.GetCatalogoAsync(SelectedProcedimiento, Opciones.PROCEDIMIENTOS, size);
        IsBusy = false;
    }

    [RelayCommand(CanExecute = nameof(CanExecute))]
    async Task Continuar() {
        IsBusy = true;

        if(SelectedInmueble.IdInmueble == 0) {
            await Toast.Make(Constants.SELECCIONE_PUNTO_ATENCION, ToastDuration.Short).Show();
            IsBusy = false;
            return;
        }

        if(SelectedProcedimiento.Id == 0) {
            await Toast.Make($"Seleccione {Constants.PROCEDIMIENTO}", ToastDuration.Short).Show();
            IsBusy = false;
            return;
        }

        SanitizacionModel sanitizacion = new SanitizacionModel {
            IdCliente = SelectedCliente.idCliente,
            IdInmueble = SelectedInmueble.IdInmueble,
            Area = Area,
            Procedimiento = SelectedProcedimiento.Id
        };

        Dictionary<string, object> data = new Dictionary<string, object> {
            {Constants.SANITIZACION_DATA_KEY, sanitizacion }
        };

        await Constants.GoToAsync(nameof(EvidenciasPage), data);

        IsBusy = false;
    }

    async void InitValues() {
        IsLoading = true;

        await Constants.LoadClientesAsync();
        await Constants.LoadEstadosAsync();
        Constants.LoadProcedimientosAsync();

        IsLoading = false;
    }

    bool CanExecute() {
        return !IsBusy;
    }
}
