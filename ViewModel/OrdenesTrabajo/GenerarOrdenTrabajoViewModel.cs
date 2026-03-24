using BatiaSuite.Data;
using BatiaSuite.Models;
using BatiaSuite.Models.OrdenesTrabajo;
using BatiaSuite.Models.Supervision;
using BatiaSuite.Utils;
using BatiaSuite.Views.OrdenesTrabajo;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Text;

namespace BatiaSuite.ViewModel.OrdenesTrabajo;

public partial class GenerarOrdenTrabajoViewModel : ViewModelBase {
    [ObservableProperty]
    ClientsModel _selectedCliente;

    [ObservableProperty]
    Inmueble _selectedInmueble;

    [ObservableProperty]
    TipoOrdenTrabajoModel _selectedTipoOrden;

    [ObservableProperty]
    TecnicoModel _selectedTecnico;

    [ObservableProperty]
    string _reporteCliente;

    [ObservableProperty]
    string _edificio;

    [ObservableProperty]
    string _piso;

    [ObservableProperty]
    string _area;

    [ObservableProperty]
    string _subArea;

    [ObservableProperty]
    string _trabajosEjecutados;


    [ObservableProperty]
    bool _isLoading;

    [ObservableProperty]
    string _textLoading;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor( nameof(SelectClienteCommand), nameof(SelectInmuebleCommand))]
    bool _isBusy;

    int _idEmpleado = 0;

    DbContext _dbContext;

    [ObservableProperty]
    MediaSource _imageMediaSource;

    [ObservableProperty]
    string? imagePath;




    public GenerarOrdenTrabajoViewModel() {
        SelectedCliente = new ClientsModel();
        SelectedInmueble = new Inmueble();
        SelectedTipoOrden = new TipoOrdenTrabajoModel();
        _idEmpleado = UserSession.IdEmpleado;
        InitValues();
    }

    async void InitValues() {
        IsLoading = true;
        _dbContext = new DbContext();
        await Constants.LoadClientesAsync();
        await Constants.LoadEstadosAsync();
        await Constants.LoadTipoOrdenAsync();
        //await Constants.LoadTecnicosAsync();
        //Constants.LoadProcedimientosAsync();
        await SetCliente();
        SelectedTecnico = new TecnicoModel();

        IsLoading = false;
    }

    async Task SetCliente() {
        var clienteFiltrado = await Constants.SetCliente(UserSession.Cliente);
        if (clienteFiltrado != null) {
            SelectedCliente = clienteFiltrado;
        }

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

        Inmueble inmueble = await Constants.GetInmuebleAsync(SelectedCliente.idCliente, 0, SelectedInmueble);
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
    async void SelectTipoOrden() {
        IsBusy = true;
        IsLoading = true;

        TipoOrdenTrabajoModel tipoOrden = await Constants.GetTipoOrdenAsync(SelectedTipoOrden);
        if(tipoOrden == null) {

            //SI EL ENDPOINT NO RESPONDE OBTENER DEL LOCAL
            await Toast.Make(Constants.ERROR_API_GET, ToastDuration.Short).Show();
            IsBusy = false;
            IsLoading = false;
            return;
        }
        IsLoading = false;
        if(tipoOrden.Equals(SelectedTipoOrden)) {
            IsBusy = false;
            IsLoading = false;
            return;
        }

        SelectedTipoOrden = tipoOrden;
        //SelectedInmueble = new Inmueble();
        IsLoading = false;
        IsBusy = false;
    }

    [RelayCommand(CanExecute = nameof(CanExecute))]
    async void SelectTecnico() {
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

        TecnicoModel tecnico = await Constants.GetTecnicoAsync(SelectedCliente.idCliente, SelectedTecnico);
        if(tecnico == null) {
            await Toast.Make(Constants.ERROR_API_GET, ToastDuration.Short).Show();
            IsBusy = false;
            IsLoading = false;
            return;
        }
        IsLoading = false;
        if(tecnico.Equals(SelectedInmueble)) {
            IsBusy = false;
            IsLoading = false;
            return;
        }

        SelectedTecnico = tecnico;
        IsLoading = false;
        IsBusy = false;
    }

    [RelayCommand]
    async Task TakePhoto() {

        try {
            if(MediaPicker.Default.IsCaptureSupported) {
                if(await PopupUtil.HasCameraPermissions()) {
                    FileResult? fileResult = await MediaPicker.CapturePhotoAsync();

                    if(fileResult != null) {
                        string localFilePath = Path.Combine(FileSystem.CacheDirectory, fileResult.FileName);

                        using(Stream stream = await fileResult.OpenReadAsync()) {
                            using FileStream localFileStream = File.OpenWrite(localFilePath);
                            await stream.CopyToAsync(localFileStream);
                        }
                        ImageMediaSource = localFilePath;
                        ImagePath = localFilePath;
                        //asignar a variable que muestra la foto
                    }
                }
            }
        } catch(Exception) { }
    }


    bool CanSelectedOrden() {
        return !IsBusy;
    }
    bool CanExecute() {
        return !IsBusy;
    }

    [RelayCommand]
    async Task ValidaryGenerarOrden() {
        IsLoading = true;
        IsBusy = true;
        if(await ValidaFormulario()) {
            //PRIMERO VALIDAR EL FORMULARIO
            //GENERAR ORDEN
            var orden = new GenerarOrdenTrabajoModel {
                IdCliente = SelectedCliente.idCliente,
                IdInmueble = SelectedInmueble.IdInmueble,
                IdTipo = SelectedTipoOrden.IdServicio,
                IdTecnico = SelectedTecnico.IdEmpleado,
                IdReporte = ReporteCliente,
                //Falta = DateTime.Now,
                IdStatus = 1,
                Edificio = Edificio,
                Piso = Piso,
                Area = Area,
                Subarea = SubArea,
                Trabajos = TrabajosEjecutados,
                //Trabejecutados = TrabajosEjecutados
            };




            byte[]? imagenBytes = null;
            if(!string.IsNullOrEmpty(ImagePath) && File.Exists(ImagePath)) {
                byte[] fileBytes = await File.ReadAllBytesAsync(ImagePath);
                byte[] resizedImage = await ImageResizerHelper.ResizeImage(fileBytes, 480, 640, true);
                orden.Imagen = resizedImage;
            } else
                orden.Imagen = null;
                await EnviarOrden(orden);
        }
        IsLoading = false;
        IsBusy = false;
    }
    public class OrdenWrapper {
        public GenerarOrdenTrabajoModel Orden { get; set; }
    }
    public async Task<bool> EnviarOrden(GenerarOrdenTrabajoModel orden) {
        try {
            var wrapped = new OrdenWrapper { Orden = orden };
             int result = await _httpHelper.PostBodyAsync<OrdenWrapper, int>(Constants.ORDEN_TRABAJO_B, wrapped);
            if (result > 0) {

                await App.Current.MainPage.DisplayAlert("Registro exitoso", "Orden de trabajo generada (Folio " + result + "). El técnico asignado ha sido notificado.", Constants.ACEPTAR);
                await Shell.Current.GoToAsync("//MyMenu");
                return true;
            } else {
                await App.Current.MainPage.DisplayAlert("", "Error al generar la orden, intentalo nuevamente : ", Constants.ACEPTAR);
                return false;
            }
        } catch(Exception ex) {
            await App.Current.MainPage.DisplayAlert("", "Error al generar la orden, intentalo nuevamente : " + ex.Message, Constants.ACEPTAR);
            return false;
        }
    }

    public async Task<bool> ValidaFormulario() {
        if(SelectedCliente.idCliente == 0) {
            await App.Current.MainPage.DisplayAlert("", Constants.SELECCIONE_CLIENTE, Constants.ACEPTAR);
            IsBusy = false;
            IsLoading = false;
            return false;
        }
        if(SelectedInmueble.IdInmueble == 0) {
            await App.Current.MainPage.DisplayAlert("", Constants.SELECCIONE_PUNTO_ATENCION, Constants.ACEPTAR);
            IsBusy = false;
            IsLoading = false;
            return false;
        }
        if(SelectedTipoOrden.IdServicio == 0) {
            await App.Current.MainPage.DisplayAlert("", Constants.SELECCIONE_TIPO_ORDEN, Constants.ACEPTAR);
            IsBusy = false;
            IsLoading = false;
            return false;
        }
        if(SelectedTecnico.IdEmpleado == 0) {
            await App.Current.MainPage.DisplayAlert("", Constants.SELECCIONE_TECNICO, Constants.ACEPTAR);
            IsBusy = false;
            IsLoading = false;
            return false;
        }
        if (ReporteCliente == null|| ReporteCliente == "") {
            await App.Current.MainPage.DisplayAlert("", Constants.INGRESE_REPORTE_CLIENTE, Constants.ACEPTAR);
            IsBusy = false;
            IsLoading = false;
            return false;
        }
        if(TrabajosEjecutados == null || TrabajosEjecutados == "") {
            await App.Current.MainPage.DisplayAlert("", Constants.INGRESE_TRABAJOS_A_EJECUTAR, Constants.ACEPTAR);
            IsBusy = false;
            IsLoading = false;
            return false;
        }
        
        //if(string.IsNullOrEmpty(ImagePath) && !File.Exists(ImagePath)) {
        //    await App.Current.MainPage.DisplayAlert("", Constants.CAPTURE_FOTOGRAFIA, Constants.ACEPTAR);
        //    IsBusy = false;
        //    IsLoading = false;
        //    return false;
        //}

                return true;
    }
    [RelayCommand]
    void RemoveImage() {
        ImageMediaSource = null;
        ImagePath = null;
    }
}