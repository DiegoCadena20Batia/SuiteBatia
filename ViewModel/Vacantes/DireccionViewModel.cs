using BatiaSuite.Models;
using BatiaSuite.Models.Vacantes;
using BatiaSuite.Utils;
using BatiaSuite.Views.Vacantes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BatiaSuite.ViewModel.Vacantes;

public partial class DireccionViewModel : ViewModelBase, IQueryAttributable {

    [ObservableProperty]
    bool _isLoading;

    [ObservableProperty]
    bool _isBusy;

    [ObservableProperty]
    EstadoModel _selectedEstado;

    [ObservableProperty]
    VacanteModel _vacante;

    public DireccionViewModel() {
        SelectedEstado = new EstadoModel();
        InitValues();
    }

    [RelayCommand]
    async Task SelectEstado() {
        SelectedEstado = await Constants.GetEstadoAsync(SelectedEstado);
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

        await Constants.GoToAsync(nameof(DireccionFiscalPage), data);
    }

    async void InitValues() {
        IsLoading = true;
        await Constants.LoadEstadosAsync();
        IsLoading = false;
    }

    void SetVacanteValues() {
        Vacante.IdEstado = SelectedEstado.id_estado;
    }

    async Task<bool> ValidateDataAsync() {
        string msj = "";

        if(string.IsNullOrWhiteSpace(Vacante.Calle)) {
            msj = Constants.INGRESE + Constants.CALLE;
        } else if(string.IsNullOrWhiteSpace(Vacante.NumeroExterior)) {
            msj = Constants.INGRESE + Constants.NO_EXTERIOR;
        } else if(string.IsNullOrWhiteSpace(Vacante.Colonia)) {
            msj = Constants.INGRESE + Constants.COLONIA;
        } else if(string.IsNullOrWhiteSpace(Vacante.CodigoPostal)) {
            msj = Constants.INGRESE + Constants.CODIGO_POSTAL;
        } else if(Vacante.CodigoPostal.Length != 5) {
            msj = "El código postal debe contener 5 dígitos";
        } else if(string.IsNullOrWhiteSpace(Vacante.Municipio)) {
            msj = Constants.INGRESE + Constants.MUNICIPIO;
        } else if(Vacante.IdEstado == 0) {
            msj = Constants.INGRESE + Constants.ESTADO;
        } else if(string.IsNullOrWhiteSpace(Vacante.Telefono)) {
            msj = Constants.INGRESE + Constants.TELEFONO;
        } else if(!string.IsNullOrWhiteSpace(Vacante.CorreoPersonal) && !Constants.IsValidEmail(Vacante.CorreoPersonal)) {
            msj = "El correo electrónico no tiene la estructura correcta.";
        } else {
            return true;
        }

        await App.Current.MainPage.DisplayAlert(string.Empty, msj, Constants.ACEPTAR);
        return false;
    }


    public void ApplyQueryAttributes(IDictionary<string, object> query) {
        if(query.ContainsKey(Constants.VACANTE_DATA_KEY)) {
            Vacante = (VacanteModel)query[Constants.VACANTE_DATA_KEY];
            query.Remove(Constants.VACANTE_DATA_KEY);
        };
    }
}