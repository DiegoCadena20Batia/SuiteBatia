using BatiaSuite.Views.Vacantes;
using BatiaSuite.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BatiaSuite.Models;
using BatiaSuite.Models.Vacantes;

namespace BatiaSuite.ViewModel.Vacantes;

public partial class DireccionFiscalViewModel : ViewModelBase, IQueryAttributable {

    [ObservableProperty]
    bool _isLoading;

    [ObservableProperty]
    bool _isBusy;

    [ObservableProperty]
    EstadoModel _selectedEstado;

    [ObservableProperty]
    VacanteModel _vacante;

    public DireccionFiscalViewModel() {
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
        await Constants.GoToAsync(nameof(DatosComplementariosPage), data);
    }

    async void InitValues() {
        IsLoading = true;
        await Constants.LoadEstadosAsync();
        IsLoading = false;
    }

    void SetVacanteValues() {
        Vacante.IdEstadof = SelectedEstado.id_estado;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query) {
        if(query.ContainsKey(Constants.VACANTE_DATA_KEY)) {
            Vacante = (VacanteModel)query[Constants.VACANTE_DATA_KEY];
            query.Remove(Constants.VACANTE_DATA_KEY);
        };
    }

    async Task<bool> ValidateDataAsync() {
        string msj = "";

        if(string.IsNullOrWhiteSpace(Vacante.Callef)) {
            msj = Constants.INGRESE + Constants.CALLE_NUMERO;
        } else if(string.IsNullOrWhiteSpace(Vacante.Coloniaf)) {
            msj = Constants.INGRESE + Constants.COLONIA;
        } else if(Vacante.Cpf is null || Vacante.Cpf == 0) {
            msj = Constants.INGRESE + Constants.CODIGO_POSTAL;
        } else if(string.IsNullOrWhiteSpace(Vacante.Municipiof)) {
            msj = Constants.INGRESE + Constants.MUNICIPIO;
        } else if(Vacante.IdEstadof == 0) {
            msj = Constants.INGRESE + Constants.ESTADO;
        } else {
            return true;
        }

        await App.Current.MainPage.DisplayAlert(string.Empty, msj, Constants.ACEPTAR);
        return false;
    }
}