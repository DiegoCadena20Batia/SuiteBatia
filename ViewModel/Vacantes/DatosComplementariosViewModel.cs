using BatiaSuite.Models.Vacantes;
using BatiaSuite.Utils;
using BatiaSuite.Views.Vacantes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BatiaSuite.ViewModel.Vacantes;

public partial class DatosComplementariosViewModel : ViewModelBase, IQueryAttributable {

    [ObservableProperty]
    bool _isLoading;

    [ObservableProperty]
    bool _isBusy;

    [ObservableProperty]
    CatalogoModel _selectedGradoEstudios;

    [ObservableProperty]
    CatalogoModel _selectedMetodoComunica;

    [ObservableProperty]
    CatalogoModel _selectedTransporte;

    [ObservableProperty]
    CatalogoModel _selectedCantidadTransporte;

    [ObservableProperty]
    VacanteModel _vacante;

    public DatosComplementariosViewModel() {
        SelectedGradoEstudios = new CatalogoModel();
        SelectedMetodoComunica = new CatalogoModel();
        SelectedTransporte = new CatalogoModel();
        SelectedCantidadTransporte = new CatalogoModel();
        InitValues();
    }

    [RelayCommand]
    async Task SelectGradoEstudios() {
        SelectedGradoEstudios = await Constants.GetCatalogoAsync(SelectedGradoEstudios, Opciones.GRADO_ESTUDIOS, DeviceInfo.Current.Idiom == DeviceIdiom.Tablet ? 2 : 4);
    }

    [RelayCommand]
    async Task SelectMetodoComunica() {
        SelectedMetodoComunica = await Constants.GetCatalogoAsync(SelectedMetodoComunica, Opciones.METODO_COMUNICA, DeviceInfo.Current.Idiom == DeviceIdiom.Tablet ? 3 : 6);
        if(SelectedMetodoComunica.Descripcion is not null && !SelectedMetodoComunica.Descripcion.Equals(Constants.OTRO)) {
            Vacante.FormComunDesc = string.Empty;
        }
    }

    [RelayCommand]
    async Task SelectTransporte() {
        SelectedTransporte = await Constants.GetCatalogoAsync(SelectedTransporte, Opciones.TRANSPORTE, DeviceInfo.Current.Idiom == DeviceIdiom.Tablet ? 2 : 4);
        if(SelectedTransporte.Descripcion is not null && !SelectedTransporte.Descripcion.Equals(Constants.OTRO)) {
            Vacante.TransporteDesc = string.Empty;
        }
    }

    [RelayCommand]
    async Task SelectCantidadTransporte() {
        SelectedCantidadTransporte = await Constants.GetCatalogoAsync(SelectedCantidadTransporte, Opciones.CANTIDAD_TRANSPORTE, DeviceInfo.Current.Idiom == DeviceIdiom.Tablet ? 2.5 : 5);
    }

    [RelayCommand]
    async Task Continuar() {
        SetVacanteValues();

        if(!await ValidateDataAsync()) {
            return;
        }

        Dictionary<string, object> data = new Dictionary<string, object> {
            {Constants.VACANTE_DATA_KEY, Vacante }
        };

        await Constants.GoToAsync(nameof(DocumentosPage), data);
    }

    [RelayCommand]
    void TieneHijosChanged(bool value) {
        if(!value) {
            Vacante.CantHijos = null;
            Vacante.DependeEconomico = false;
        }
    }

    [RelayCommand]
    void TieneTelefonoChanged(bool value) {
        if(value) {
            SelectedMetodoComunica = new CatalogoModel();
            Vacante.FormComunDesc = null;
        }
    }
    void InitValues() {
        Constants.LoadGradoEstudiosAsync();
        Constants.LoadMetodosComunicaAsync();
        Constants.LoadTransportesAsync();
        Constants.LoadCantidadTransporteAsync();
    }

    void SetVacanteValues() {
        Vacante.GradoEstudio = SelectedGradoEstudios.Id;
        Vacante.FormComunicacion = SelectedMetodoComunica.Id;
        Vacante.Transporte = SelectedTransporte.Id;
        Vacante.TransporteUnidad = SelectedCantidadTransporte.Id;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query) {
        if(query.ContainsKey(Constants.VACANTE_DATA_KEY)) {
            Vacante = (VacanteModel)query[Constants.VACANTE_DATA_KEY];
            query.Remove(Constants.VACANTE_DATA_KEY);
        };
    }

    async Task<bool> ValidateDataAsync() {
        string msj = "";

        if(Vacante.GradoEstudio == 0) {
            msj = Constants.INGRESE + Constants.ULTIMO_GRADO_ESTUDIOS;
        } else if(Vacante.TieneHijos && (Vacante.CantHijos is null || Vacante.CantHijos == 0)) {
            msj = Constants.INGRESE + Constants.CUANTOS_HIJOS;
        } else if(!Vacante.TelIntel && Vacante.FormComunicacion == 0) {
            msj = Constants.INGRESE + Constants.COMO_COMUNICA;
        } else if(!Vacante.TelIntel && Vacante.FormComunicacion == 3 && string.IsNullOrWhiteSpace(Vacante.FormComunDesc)) {
            msj = Constants.ESPECIFIQUE_COMUNICA;
        } else if(Vacante.Transporte == 0) {
            msj = Constants.INGRESE + Constants.QUE_TRANSPORTE;
        } else if(Vacante.Transporte == 12 && string.IsNullOrWhiteSpace(Vacante.TransporteDesc)) {
            msj = Constants.ESPECIFIQUE_TRANSPORTE;
        } else if(Vacante.TransporteUnidad == 0) {
            msj = Constants.INGRESE + Constants.CUANTO_UNIDADES_TRANSPORTE;
        } else if(string.IsNullOrWhiteSpace(Vacante.TransporteGasto)) {
            msj = Constants.INGRESE + Constants.CUANTO_GASTA;
        } else {
            return true;
        }

        await App.Current.MainPage.DisplayAlert(string.Empty, msj, Constants.ACEPTAR);
        return false;
    }
}