using BatiaSuite.Models;
using BatiaSuite.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BatiaSuite.ViewModel;

public partial class LogueoViewModel : ViewModelBase {

    [ObservableProperty]
    bool _isPassword = true;

    [ObservableProperty]
    string _iconPass = "eye_on.png";

    [ObservableProperty]
    string _userName;

    [ObservableProperty]
    string _password;

    [ObservableProperty]
    bool _isBusy;

    [RelayCommand]
    async Task Login() {
        if(!await ValidateData()) {
            return;
        }
        if(!Utils.InternetUtil.IsConnectedInternet()) {
            await App.Current.MainPage.DisplayAlert(string.Empty, Constants.ERROR_INTERNET, Constants.ACEPTAR);
            return;
        }

        IsBusy = true;
        await Task.Delay(200);
        string url = $"Login?usr={UserName}&pwd={Password}";

        LogueoModel response = await _httpHelper.GetAsync<LogueoModel>(url);

        if(response is not null) {

            if(!string.IsNullOrEmpty(response.per_Nombre)) {
                UserSession.SetData(response);
                App.Current.MainPage = new AppShell();
            } else {
                await App.Current.MainPage.DisplayAlert(string.Empty, Constants.USER_PASS_INCORRECTOS, Constants.ACEPTAR);
            }
        } else {
            await App.Current.MainPage.DisplayAlert(string.Empty, Constants.ERROR_API, Constants.ACEPTAR);
        }
        IsBusy = false;
    }

    [RelayCommand]
    void ShowPassword() {
        IsPassword = !IsPassword;
        IconPass = IsPassword ? "eye_on.png" : "eye_off.png";
    }

    async Task<bool> ValidateData() {
        if(string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password)) {
            await App.Current.MainPage.DisplayAlert("", "Ingrese Usuario y Contraseña", Constants.ACEPTAR);
            return false;
        }
        return true;
    }

    [RelayCommand]
    async Task OpenRegisterPage() {
        // enlace a abrir en el navegador del dispositivo:
        try {
            Launcher.Default.OpenAsync("https://www.singa.com.mx:8087/Registro.aspx");
        } catch(Exception ex) {
            await App.Current.MainPage.DisplayAlert("Error", ex.Message, Constants.ACEPTAR);
        }
    }
}