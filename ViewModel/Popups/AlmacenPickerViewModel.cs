using BatiaSuite.Models.OrdenesTrabajo;
using BatiaSuite.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using System.Collections.ObjectModel;

namespace BatiaSuite.ViewModel.Popups;

public partial class AlmacenPickerViewModel : ViewModelBase {

    CancellationTokenSource _cancellationTokenSource;
    AlmacenModel _oldAlmacen;

    [ObservableProperty]
    string _emptyView;

    [ObservableProperty]
    ObservableCollection<AlmacenModel> _almacenList;

    public event EventHandler<AlmacenModel> SelectedAlmacen;
    protected void OnSelectedAlmacen(AlmacenModel almacen) {
        SelectedAlmacen?.Invoke(this, almacen);
    }

    public AlmacenPickerViewModel(AlmacenModel oldAlmacen) {
        _oldAlmacen = oldAlmacen;
        AlmacenList = new ObservableCollection<AlmacenModel>();
    }

    [RelayCommand]
    async Task GetAlmacenList(string query) {
        if(_cancellationTokenSource is not null) {
            _cancellationTokenSource.Cancel();
        }
        _cancellationTokenSource = new CancellationTokenSource();

        CancellationToken cancellationToken = _cancellationTokenSource.Token;

        if(!cancellationToken.IsCancellationRequested) {
            string url = $"{Constants.OT_ALMACEN_API}?nombre={query}";

            AlmacenList = await _httpHelper.GetAsync<ObservableCollection<AlmacenModel>>(url, cancellationToken);

            if(AlmacenList is null || AlmacenList.Count == 0) {
                EmptyView =Constants.NO_HAY_REGISTROS;
            }
        }
    }

    [RelayCommand]
    void SelectAlmacen(AlmacenModel selectedAlmacen) {
        SendResponse(selectedAlmacen);
    }

    [RelayCommand]
    async void Cancel() {
        if(_oldAlmacen is null) {
            _oldAlmacen = new AlmacenModel();
        }
        SendResponse(_oldAlmacen);
    }

    void SendResponse(AlmacenModel almacen) {
        if(almacen is null) {
            almacen = new AlmacenModel();
        }
        OnSelectedAlmacen(almacen);
        ClosePopup();
    }

    async void ClosePopup() {
        await MopupService.Instance.PopAsync();
    }
}