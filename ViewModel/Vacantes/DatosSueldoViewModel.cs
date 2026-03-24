using BatiaSuite.Models.Vacantes;
using BatiaSuite.Utils;
using BatiaSuite.Views.Vacantes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BatiaSuite.ViewModel.Vacantes;

public partial class DatosSueldoViewModel : ViewModelBase, IQueryAttributable {

    [ObservableProperty]
    bool _isLoading;

    [ObservableProperty]
    string _loadingText;

    [ObservableProperty]
    bool _isBusy;

    [ObservableProperty]
    VacanteModel _vacante;

    [ObservableProperty]
    BancoModel _selectedBanco;

    [ObservableProperty]
    DateTime _selectedfechaIngreso;

    [ObservableProperty]
    int _sueldoMensual;

    public DatosSueldoViewModel() {
        SelectedBanco = new BancoModel();
        SelectedfechaIngreso = DateTime.Now;
        InitValues();
    }

    [RelayCommand]
    async Task SelectBanco() {
        SelectedBanco = await Constants.GetBancoAsync(SelectedBanco);
    }

    [RelayCommand]
    async Task Continuar() {
        SetVacanteValues();

        if(!await ValidateDataAsync()) {
            return;
        }

        Vacante.SalarioMensual = SueldoMensual;

        Dictionary<string, object> data = new Dictionary<string, object> {
            {Constants.VACANTE_DATA_KEY, Vacante }
        };

        await Constants.GoToAsync(nameof(DireccionPage), data);
    }

    void SetVacanteValues() {
        Vacante.FechaIngreso = SelectedfechaIngreso.ToString("yyyy-MM-dd");
        Vacante.Banco = SelectedBanco.IdBanco;
    }

    async void InitValues() {
        IsLoading = true;
        await Constants.LoadBancosAsync();
        IsLoading = false;
    }

    async Task<bool> ValidateDataAsync() {
        string msj = string.Empty;

        if(SueldoMensual > Vacante.SueldoVacante) {
            msj = $"El sueldo mensual no puedo ser mayor a ${Vacante.SueldoVacante}";
        } else if(SelectedfechaIngreso.ToShortDateString().Equals("1/1/1900")) {
            msj = Constants.INGRESE + Constants.FECHA_INGRESO;
        } else if(Vacante.Banco == 0) {
            msj = Constants.INGRESE + Constants.BANCO;
        } else if(string.IsNullOrWhiteSpace(Vacante.Clabe) && string.IsNullOrWhiteSpace(Vacante.Cuenta) && string.IsNullOrWhiteSpace(Vacante.Tarjeta)) {
            msj = Constants.INGRESE + Constants.CLABE + ", " + Constants.NO_CUENTA + " o " + Constants.NO_TARJETA;
        } else {
            return await ValidateClabeNoCuentaNoTarjeta();
        }

        await App.Current.MainPage.DisplayAlert(string.Empty, msj, Constants.ACEPTAR);
        return false;
    }

    async Task<bool> ValidateClabeNoCuentaNoTarjeta() {
        LoadingText = "Validando datos";
        IsLoading = true;
        int resp = await _httpHelper.GetAsync<int>($"{Constants.VAC_GET_VALIDA_DATOS_BANCO}?cuenta={Vacante.Cuenta}&clabe={Vacante.Clabe}&tarjeta={Vacante.Tarjeta}");
        IsLoading = false;
        LoadingText = "";
        bool existenCuentas = resp != 0;

        if(existenCuentas) {
            await App.Current.MainPage.DisplayAlert(string.Empty, "La CLABE, el n° de Cuenta o el n° de Tarjeta ya están registrados con otro empleado.", Constants.ACEPTAR);
        }

        return !existenCuentas;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query) {
        if(query.ContainsKey(Constants.VACANTE_DATA_KEY)) {
            Vacante = (VacanteModel)query[Constants.VACANTE_DATA_KEY];
            SueldoMensual = Vacante.SueldoVacante;
            query.Remove(Constants.VACANTE_DATA_KEY);
        };
    }
}