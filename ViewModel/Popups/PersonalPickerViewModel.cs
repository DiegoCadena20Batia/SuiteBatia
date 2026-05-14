using BatiaSuite.Data;
using BatiaSuite.Models.OrdenesTrabajo;
using BatiaSuite.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using System.Collections.ObjectModel;

namespace BatiaSuite.ViewModel.Popups;

public partial class PersonalPickerViewModel : ViewModelBase {
    private DbContext _dbContext;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _emptyView;

    [ObservableProperty]
    private ObservableCollection<PersonalOrdenTrabajoResponse> _personalList;

    public event EventHandler<PersonalOrdenTrabajoResponse> SendPersonal;

    protected void OnSendPersonal(PersonalOrdenTrabajoResponse personal) {
        SendPersonal?.Invoke(this, personal);
    }

    public PersonalPickerViewModel() {
        _dbContext = new DbContext();
    }

    [RelayCommand]
    private async Task GetPersonalList(string query) {
        IsLoading = true;
        try {
            if(!Utils.InternetUtil.IsConnectedInternet()) {
                var personalList = await _dbContext.ObtenerPersonalLocales();
              
                PersonalList = new ObservableCollection<PersonalOrdenTrabajoResponse>(personalList.Where(x => x.nombre.Contains($"{query.ToUpper()}")).ToList());
            } else {
                string url = $"{Constants.OT_TECNICO_API}/?nombre={query}";
                PersonalList = await _httpHelper.GetAsync<ObservableCollection<PersonalOrdenTrabajoResponse>>(url);
            }
        } catch(Exception) { }

        if(PersonalList is null || PersonalList.Count == 0) {
            EmptyView = Constants.NO_HAY_REGISTROS;
        }
        IsLoading = false;
    }

    [RelayCommand]
    private void SelectPersonal(PersonalOrdenTrabajoResponse selectedPersonal) {
        SendResponse(selectedPersonal);
    }

    [RelayCommand]
    private async void Cancel() {
        SendResponse();
    }

    private void SendResponse(PersonalOrdenTrabajoResponse selectedPersonal = null) {
        if(selectedPersonal is null) {
            selectedPersonal = new PersonalOrdenTrabajoResponse();
        }
        OnSendPersonal(selectedPersonal);
        ClosePopup();
    }

    private async void ClosePopup() {
        await MopupService.Instance.PopAsync();
    }
}