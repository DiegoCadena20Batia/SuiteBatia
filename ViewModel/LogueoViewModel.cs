using BatiaSuite.Data;
using BatiaSuite.Models;
using BatiaSuite.Models.Apps;
using BatiaSuite.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BatiaSuite.ViewModel;

public partial class LogueoViewModel : ViewModelBase {
    private DbContext _dbContext;

    [ObservableProperty]
    private bool _isPassword = true;

    [ObservableProperty]
    private string _iconPass = "eye_on.png";

    [ObservableProperty]
    private string _userName;

    [ObservableProperty]
    private string _password;

    [ObservableProperty]
    private bool _isBusy;

    public LogueoViewModel() {
        _dbContext = new DbContext();
        _ = InitAsync();
    }

    private async Task InitAsync() {
        if(await ValidarVersion()) {

        } else {

        }
        
    }

    public async Task<bool> ValidarVersion() {
        try {
            string version = AppInfo.Current.VersionString;
            string buildVersion = AppInfo.Current.BuildString;
            string plataforma = "";

#if ANDROID
            plataforma = "1";
#endif
#if IOS
plataforma = "2";
#endif

            string url = Constants.API_BASE_URL + $"VersionesApp?app=1$plataforma={plataforma}";

            var _httpClient = new HttpClient();
            var response = await _httpClient.GetAsync(url);
            if(!response.IsSuccessStatusCode) {
                Console.WriteLine("No se pudo obtener la versión de la app desde el server");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = System.Text.Json.JsonSerializer.Deserialize<List<VersionApp>>(jsonResponse);

            if(result != null && result[0] != null) {
                if(result[0].nomversion == version) {
                    return true;
                } else {
                    Console.WriteLine($"version incorrecta: {result[0].nomversion} (num: {result[0].numversion}), se esperaba {version}");
                }
            }

            return true;
        } catch(Exception ex) {
            Console.WriteLine($"Error al validar la versión de la app: {ex.Message}");
            return true;
        }
    }

    [RelayCommand]
    private async Task Login() {
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
                if(Utils.InternetUtil.IsConnectedInternet()) {
                    await _dbContext.GuardarDataOrdenesTrabajoLocal();
                }

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
    private void ShowPassword() {
        IsPassword = !IsPassword;
        IconPass = IsPassword ? "eye_on.png" : "eye_off.png";
    }

    private async Task<bool> ValidateData() {
        if(string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password)) {
            await App.Current.MainPage.DisplayAlert("", "Ingrese Usuario y Contraseña", Constants.ACEPTAR);
            return false;
        }
        return true;
    }

    [RelayCommand]
    private async Task OpenRegisterPage() {
        // enlace a abrir en el navegador del dispositivo:
        try {
            Launcher.Default.OpenAsync("https://www.singa.com.mx:8087/Registro.aspx");
        } catch(Exception ex) {
            await App.Current.MainPage.DisplayAlert("Error", ex.Message, Constants.ACEPTAR);
        }
    }
}