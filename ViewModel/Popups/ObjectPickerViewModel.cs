using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using System.Collections.ObjectModel;

namespace BatiaSuite.ViewModel.Popups;

public partial class ObjectPickerViewModel : ViewModelBase {

    List<object> _totalList;
    object _oldValue;

    [ObservableProperty]
    ObservableCollection<object> _partialList;

    [ObservableProperty]
    double _popupHeight;

    [ObservableProperty]
    bool _showSearching;

    public event EventHandler<object> SendValue;
    protected void OnSendValue(object value) {
        SendValue?.Invoke(this, value);
    }

    public ObjectPickerViewModel(object oldValue, List<object> list, double divisor, bool showSearching) {
        _totalList = list;
        PartialList = new ObservableCollection<object>(list);
        _oldValue = oldValue;
        PopupHeight = DeviceDisplay.Current.MainDisplayInfo.Height / divisor;
        ShowSearching = showSearching;
    }

    [RelayCommand]
    async Task FiltrarLista(string query) {
        PartialList.Clear();
        foreach(object item in _totalList) {
            if(((string)item).Contains(query, StringComparison.OrdinalIgnoreCase)) {
                PartialList.Add(item);
            }
        }
    }

    [RelayCommand]
    void SelectValue(object value) {
        SendResponse(value);
    }

    [RelayCommand]
    async void Cancel() {
        SendResponse(_oldValue);
    }

    void SendResponse(object value) {
        if(value is null) {
            value = new object();
        }
        OnSendValue(value);
        ClosePopup();
    }

    async void ClosePopup() {
        await MopupService.Instance.PopAsync();
    }
}