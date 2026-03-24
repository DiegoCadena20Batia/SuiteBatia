using BatiaSuite.Models.OrdenesTrabajo;
using BatiaSuite.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using System.Collections.ObjectModel;

namespace BatiaSuite.ViewModel.Popups;

public partial class LocationUseViewModel : ViewModelBase {

    public LocationUseViewModel() {
    }

    [RelayCommand]
    async Task Aceptar() {
        UserSession.ShowAcceptTracking = true;
        await MopupService.Instance.PopAsync();
    }

    [RelayCommand]
    async Task Cancelar() {
        UserSession.ShowAcceptTracking = false;
        await MopupService.Instance.PopAsync();
        await Shell.Current.GoToAsync("//MyMenu");
    }
}