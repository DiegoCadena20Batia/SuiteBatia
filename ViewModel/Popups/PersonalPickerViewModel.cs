using BatiaSuite.Models.OrdenesTrabajo;
using BatiaSuite.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using System.Collections.ObjectModel;

namespace BatiaSuite.ViewModel.Popups;

public partial class PersonalPickerViewModel : ViewModelBase {

    [ObservableProperty]
    bool _isLoading;

    [ObservableProperty]
    string _emptyView;

    [ObservableProperty]
    ObservableCollection<PersonalOrdenTrabajoResponse> _personalList;

    public event EventHandler<PersonalOrdenTrabajoResponse> SendPersonal;
    protected void OnSendPersonal(PersonalOrdenTrabajoResponse personal) {
        SendPersonal?.Invoke(this, personal);
    }

    [RelayCommand]
    async Task GetPersonalList(string query) {
        IsLoading = true;
        try {
            string url = $"{Constants.OT_TECNICO_API}/?nombre={query}";
            PersonalList = await _httpHelper.GetAsync<ObservableCollection<PersonalOrdenTrabajoResponse>>(url);
        } catch(Exception) { }

        if(PersonalList is null || PersonalList.Count == 0) {
            EmptyView = Constants.NO_HAY_REGISTROS;
        }
        IsLoading = false;
    }

    [RelayCommand]
    void SelectPersonal(PersonalOrdenTrabajoResponse selectedPersonal) {
        SendResponse(selectedPersonal);
    }

    [RelayCommand]
    async void Cancel() {
        SendResponse();
    }

    void SendResponse(PersonalOrdenTrabajoResponse selectedPersonal = null) {
        if(selectedPersonal is null) {
            selectedPersonal = new PersonalOrdenTrabajoResponse();
        }
        OnSendPersonal(selectedPersonal);
        ClosePopup();
    }

    async void ClosePopup() {
        await MopupService.Instance.PopAsync();
    }
}