using BatiaSuite.Data;
using BatiaSuite.Models;
using BatiaSuite.Models.Supervision;
using BatiaSuite.Models.SupervisionMantenimiento;
using BatiaSuite.Services;
using BatiaSuite.Utils;
using BatiaSuite.Views.Supervision;
using BatiaSuite.Views.SupervisionMantenimiento;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace BatiaSuite.ViewModel.Supervisionmantenimiento;

public partial class SupervisionMantenimientoSeccionesViewModel : ViewModelBase {

    [ObservableProperty]
    bool _isLoading;

    [ObservableProperty]
    string _textLoading;

    [ObservableProperty]
    int _idSeccion;

    DbContext _dbContext;

    public ObservableCollection<SupervisionMantenimientoSeccionesModel> SeccionesList { get; set; }
     = new();

    private readonly SupervisionMantenimientoService _supervisionMantenimientoService;

    public SupervisionMantenimientoSeccionesViewModel(SupervisionMantenimientoService supervisionMantenimientoService) {
        _supervisionMantenimientoService = supervisionMantenimientoService;
        InitValues();
    }


    public async Task ObtenerSecciones() {
               IsLoading = true;
        var sect = _supervisionMantenimientoService.ObtenerSecciones();

        
        if(SeccionesList == null) {
            await Toast.Make(Constants.ERROR_API_GET, ToastDuration.Short).Show();
            IsLoading = false;
            return;
        }
        SeccionesList.Clear();

        foreach(var item in sect) {
            SeccionesList.Add(item);
        }
        IsLoading = false;
    }


    async void InitValues() {
        IsLoading = true;
        //_supervisionMantenimientoService.InitModel();
        await ObtenerSecciones();
        IsLoading = false;
    }

    [RelayCommand]
    public async Task<bool> SelectedSection(SupervisionMantenimientoSeccionesModel seccion) {
        try {
            Dictionary<string, object> data = new Dictionary<string, object>{
            { "idseccion", seccion.IdSeccion },
            { "seccion", seccion.Seccion }
        };
            if (seccion.IdSeccion == 7) {
                await Constants.GoToAsync(nameof(SupervisionMantenimientoHidrantesObjectPage), data);
            } else if (seccion.IdSeccion == 10) {
                await Constants.GoToAsync(nameof(SupervisionMantenimientoExtintoresObjectPage), data);
            } else {
                await Constants.GoToAsync(nameof(SupervisionMantenimientoSeccionPage), data);
            }
                return true;
        }
        catch(Exception ex) {
            await Toast.Make(ex.Message, ToastDuration.Long).Show();
            return false;
        }
    }

    public void RefreshSecciones() {
        var sect = _supervisionMantenimientoService.ObtenerSecciones();

        SeccionesList.Clear();
        SeccionesList = new ObservableCollection<SupervisionMantenimientoSeccionesModel>();

        foreach(var item in sect) {
            SeccionesList.Add(item);
        }
    }
    [RelayCommand]
    public async Task Continuar() {
        try {
            if (SeccionesList != null && SeccionesList.Count > 0) {
                bool incompleteSections = false;
                foreach (var sec in SeccionesList) {
                    if (sec.Terminada == false) {
                        incompleteSections = true;
                        break;
                    }
                }
                if(incompleteSections) {
                    bool result = await App.Current.MainPage.DisplayAlert("Alerta", "Algunas secciones aun no han sido completadas \n\n ¿Desea continuar?", "Sí", "No");
                    if(!result) {
                        return;
                    }
                }
                //si todas estan termiandas continuar a las firmas
                await Constants.GoToAsync(nameof( SupervisionMantenimientoFirmasPage));
            }
        }catch(Exception ex) {
            await Toast.Make(ex.Message, ToastDuration.Short).Show();
        }
    }

}

