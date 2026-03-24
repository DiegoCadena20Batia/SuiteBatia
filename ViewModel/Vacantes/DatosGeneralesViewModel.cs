using BatiaSuite.Models;
using BatiaSuite.Models.Vacantes;
using BatiaSuite.Utils;
using BatiaSuite.Views.Vacantes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BatiaSuite.ViewModel.Vacantes;

public partial class DatosGeneralesViewModel : ViewModelBase, IQueryAttributable {

    [ObservableProperty]
    bool _isBusy;

    [ObservableProperty]
    bool _isLoading;

    [ObservableProperty]
    string _loadingText;

    [ObservableProperty]
    VacanteModel _vacante;

    [ObservableProperty]
    EstadoModel _selectedEstado;

    [ObservableProperty]
    CatalogoModel _selectedGenero;

    [ObservableProperty]
    CatalogoModel _selectedEstadoCivil;

    [ObservableProperty]
    DateTime _selectedFechaNacimiento;

    public DatosGeneralesViewModel() {
        SelectedEstado = new EstadoModel();
        SelectedGenero = new CatalogoModel();
        SelectedEstadoCivil = new CatalogoModel();
        SelectedFechaNacimiento = new DateTime(1900, 1, 1);
        InitPageValues();
    }

    [RelayCommand]
    async Task SelectEstado() {
        IsBusy = true;
        SelectedEstado = await Constants.GetEstadoAsync(SelectedEstado);
        IsBusy = false;
    }

    [RelayCommand]
    async Task SelectGenero() {
        IsBusy = true;
        double size = Constants.IS_IOS ? Constants.IS_TABLET ? 5 : 10 : Constants.IS_TABLET ? 4 : 7.5;
        SelectedGenero = await Constants.GetCatalogoAsync(SelectedGenero, Opciones.GENEROS, size);
        IsBusy = false;
    }

    [RelayCommand]
    async Task SelectEstadoCivil() {
        IsBusy = true;
        double size = Constants.IS_IOS ? Constants.IS_TABLET ? 2 : 6 : Constants.IS_TABLET ? 2 : 4.5;
        SelectedEstadoCivil = await Constants.GetCatalogoAsync(SelectedEstadoCivil, Opciones.ESTADOS_CIVIL, size);
        IsBusy = false;
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
        await Constants.GoToAsync(nameof(DatosSueldoPage), data);
    }

    [RelayCommand]
    void CheckBoxIsChecked(bool isChecked) {
        if(isChecked) {
            Vacante.SeguroSocial = string.Empty;
        }
    }

    async void InitPageValues() {
        IsLoading = true;
        await Constants.LoadEstadosAsync();
        Constants.LoadGenerosAsync();
        Constants.LoadEstadosCivilAsync();
        IsLoading = false;
    }

    void SetVacanteValues() {
        Vacante.LugarNacimiento = SelectedEstado.abreviatura;
        Vacante.Genero = SelectedGenero.Id;
        Vacante.EstadoCivil = SelectedEstadoCivil.Id;
        Vacante.FechaNacimiento = SelectedFechaNacimiento.ToString("yyyy-MM-dd");
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query) {
        if(query.ContainsKey(Constants.VACANTES_LIST_DATA_KEY)) {
            VacanteListModel selectedVacante = (VacanteListModel)query[Constants.VACANTES_LIST_DATA_KEY];

            Vacante = new VacanteModel {
                IdVacante = selectedVacante.IdVacante,
                IdUsuario = UserSession.IdPersonal,
                SueldoVacante = selectedVacante.Sueldo
            };

            query.Remove(Constants.VACANTES_LIST_DATA_KEY);
        };
    }

    async Task<bool> ValidateDataAsync() {
        string msj = "";

        if(string.IsNullOrWhiteSpace(Vacante.ApellidoPaterno)) {
            msj = Constants.INGRESE + Constants.APELLIDO_PATERNO;
        } else if(string.IsNullOrWhiteSpace(Vacante.ApellidoMaterno)) {
            msj = Constants.INGRESE + Constants.APELLIDO_MATERNO;
        } else if(string.IsNullOrWhiteSpace(Vacante.Nombre)) {
            msj = Constants.INGRESE + Constants.NOMBRE;
        } else if(SelectedFechaNacimiento.ToShortDateString().Equals("1/1/1900")) {
            msj = Constants.INGRESE + Constants.FECHA_NACIMIENTO;
        } else if(string.IsNullOrWhiteSpace(Vacante.LugarNacimiento)) {
            msj = Constants.INGRESE + Constants.LUGAR_NACIMIENTO;
        } else if(Vacante.Genero == 0) {
            msj = Constants.INGRESE + Constants.GENERO;
        } else if(string.IsNullOrWhiteSpace(Vacante.Curp)) {
            msj = Constants.INGRESE + Constants.CURP;
        } else if(Vacante.Curp.Length < 13 || Vacante.Curp.Length > 18) {
            msj = "El CURP no tiene la longitud de caracteres correcta.";
        } else if(string.IsNullOrWhiteSpace(Vacante.Rfc)) {
            msj = Constants.INGRESE + Constants.RFC;
        } else if(Vacante.Rfc.Length < 10 || Vacante.Rfc.Length > 13) {
            msj = "El RFC no tiene la longitud de caracteres correcta.";
        } else if(!Vacante.Pensionado && string.IsNullOrWhiteSpace(Vacante.SeguroSocial)) {
            msj = Constants.INGRESE + Constants.NSS;
        } else if(!Vacante.Pensionado && Vacante.SeguroSocial.Length != 11) {
            msj = "El NSS no tiene la longitud de caracteres correcta.";
        } else if(string.IsNullOrWhiteSpace(Vacante.FuenteReclutamiento)) {
            msj = Constants.INGRESE + Constants.FUENTE_RECLUTAMIENTO;
        } else {
            IsLoading = true;
            return await ValidateRfcAsync();
        }

        await App.Current.MainPage.DisplayAlert(string.Empty, msj, Constants.ACEPTAR);
        return false;
    }

    async Task<bool> ValidateRfcAsync() {
        ValidateRfcModel rfcData = new ValidateRfcModel {
            PrimerApellido = Vacante.ApellidoPaterno,
            SegundoApellido = Vacante.ApellidoMaterno,
            Nombre = Vacante.Nombre,
            Fecha = SelectedFechaNacimiento.ToString("yyyy/MM/dd")
        };

        string msj = "";
        LoadingText = "Validando RFC";
        string rfc = await _httpHelper.PostBodyAsync<ValidateRfcModel, string>(Constants.VAC_POST_RFC, rfcData);

        if(string.IsNullOrWhiteSpace(rfc)) {
            msj = "Los datos para calcular el RFC no son correctos.";
        }

        string miRfc = Vacante.Rfc.Substring(0, 10);

        if(!rfc.Equals(miRfc)) {
            msj = "El RFC ó los  datos para calcular el RFC son incorrectos.";
        } else {
            return await ValidateCurpAsync();
        }

        IsLoading = false;
        await App.Current.MainPage.DisplayAlert(string.Empty, msj, Constants.ACEPTAR);
        return false;
    }

    async Task<bool> ValidateCurpAsync() {
        ValidateCurpModel curpData = new ValidateCurpModel {
            PrimerApellido = Vacante.ApellidoPaterno,
            SegundoApellido = Vacante.ApellidoMaterno,
            Nombre = Vacante.Nombre,
            Fecha = SelectedFechaNacimiento.ToString("yyyy/MM/dd"),
            Genero = SelectedGenero.Descripcion.Substring(0, 1),
            Entidad = SelectedEstado.abreviatura
        };

        string msj = "";

        LoadingText = "Validando CURP";
        string curp = await _httpHelper.PostBodyAsync<ValidateCurpModel, string>(Constants.VAC_POST_CURP, curpData);

        if(string.IsNullOrWhiteSpace(curp)) {
            msj = "Los datos para calcular el CURP no son correctos.";
        }

        curp = curp.Substring(0, 13);
        string miCurp = Vacante.Curp.Substring(0, 13);

        if(!curp.Equals(miCurp)) {
            msj = "El CURP ó los  datos para calcular el CURP son incorrectos.";
        } else {
            return await ValidateCurpRfcNssAsync();
        }

        IsLoading = false;
        await App.Current.MainPage.DisplayAlert(string.Empty, msj, Constants.ACEPTAR);
        return false;
    }

    async Task<bool> ValidateCurpRfcNssAsync() {

        LoadingText = "Validando datos";
        string nss = Vacante.SeguroSocial is null || Vacante.SeguroSocial.Length == 0 ? "0" : Vacante.SeguroSocial;
        string url = $"{Constants.VAC_GET_VALIDA_DATOS}/{Vacante.Curp}/{Vacante.Rfc}/{nss}";
        List<ValidateCurpRfcNss> validates = await _httpHelper.GetAsync<List<ValidateCurpRfcNss>>(url);

        foreach(ValidateCurpRfcNss validate in validates) {
            if(validate.StatusCode == 1) {
                IsLoading = false;
                await App.Current.MainPage.DisplayAlert(string.Empty, "El CURP, RFC o NSS ya están registrados con otro empleado.", Constants.ACEPTAR);
                return false;
            }
        }

        IsLoading = false;
        return true;
    }
}