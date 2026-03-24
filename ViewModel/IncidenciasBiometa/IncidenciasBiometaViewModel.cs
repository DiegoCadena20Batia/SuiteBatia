using BatiaSuite.Models.IncidenciasBiometa;
using BatiaSuite.Services;
using BatiaSuite.Utils;

using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using System.Windows.Input;

namespace BatiaSuite.ViewModel.IncidenciasBiometa;

public partial class IncidenciasBiometaViewModel : ViewModelBase {

    [ObservableProperty]
    List<IncidenciaBiometa> _incidencias;

    [ObservableProperty]
    DateTime _fechaFormat;

    [ObservableProperty]
    bool _isLoading;

    [ObservableProperty]
    string _textLoading;

    [ObservableProperty]
    bool isRefreshing;
    public ICommand RefreshCommand { get; }


    private readonly CheckListService _checkListService;


    public IncidenciasBiometaViewModel() {
        InitValues();
        FechaFormat = DateTime.Now;
        UserSession.IdClienteCheckList = 0;
        UserSession.IdInmuebleCheckList = 0;
    RefreshCommand = new Command(async () =>
{
        try {
            IsRefreshing = true;
            await ObtenerIncidenciasBiometa();
        } finally {
            IsRefreshing = false;
        }
    });
    }


    async void InitValues() {
        try {
            IsLoading = true;
            await ObtenerIncidenciasBiometa();
            IsLoading = false;
        }
        catch(Exception ex) {
            await Toast.Make(ex.Message, ToastDuration.Long).Show();
        }
    }

    public async Task<bool> ObtenerIncidenciasBiometa() {
        try {
            Uri requestUri = new Uri(Constants.API_BASE_URL + "IncidenciasBiometa" + $"?idsupervisor={UserSession.IdEmpleado.ToString()}");
            var client = new HttpClient();
            var response = await client.GetAsync(requestUri);
            if(!response.IsSuccessStatusCode) {
                // Manejar error
                await Toast.Make("Error al consultar las incidencias.", ToastDuration.Short).Show();
                return false;
            }
            string jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<IncidenciaBiometa>>(jsonResponse);
            
            if (result != null && result.Count > 0) {
                Incidencias = result;
                foreach (var incidencia in Incidencias)
                {
                    incidencia.FechaFormat = DateTime.Now;
                }
            } else {
                await Toast.Make("No hay incidencias registradas en el inmueble especificado.", ToastDuration.Short).Show();
                Incidencias = new List<IncidenciaBiometa>();
            }
                return true;
        }
        catch(Exception ex) {
            await Toast.Make(ex.Message, ToastDuration.Long).Show();
            return false;
        }
    }

    public ICommand GoBackCommand => new Command(async () =>
    {
        await Shell.Current.GoToAsync("..");
    });

}
