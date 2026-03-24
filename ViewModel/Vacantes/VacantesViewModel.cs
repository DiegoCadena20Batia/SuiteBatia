using BatiaSuite.Models.Vacantes;
using BatiaSuite.Utils;
using BatiaSuite.Views.Vacantes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace BatiaSuite.ViewModel.Vacantes;

public partial class VacantesViewModel : ViewModelBase {

    [ObservableProperty]
    ObservableCollection<VacanteListModel> _vacanteList;

    [ObservableProperty]
    bool _isLoading;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SelectedVacanteCommand))]
    bool _isBusy;

    public VacantesViewModel() {
        InitValues();
    }

    [RelayCommand(CanExecute = nameof(CanExecuteSelectedVacante))]
    public async Task SelectedVacante(VacanteListModel vacante) {
        IsBusy = true;

        await Task.Delay(10);

        Dictionary<string, object> data = new Dictionary<string, object>{
            { Constants.VACANTES_LIST_DATA_KEY, vacante }
        };

        await Constants.GoToAsync(nameof(DatosGeneralesPage), data);
        IsBusy = false;
    }

    async void InitValues() {
        IsLoading = true;
        await LoadVacanteList();
        IsLoading = false;
    }

    async Task LoadVacanteList() {
        VacanteList = await _httpHelper.GetAsync<ObservableCollection<VacanteListModel>>($"{Constants.VAC_GET_VACANTES}?idreclutador={UserSession.IdEmpleado}");
    }

    bool CanExecuteSelectedVacante() {
        return !IsBusy;
    }
}