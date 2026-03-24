using BatiaSuite.Models.OrdenesTrabajo;
using BatiaSuite.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using System.Collections.ObjectModel;

namespace BatiaSuite.ViewModel.Popups;

public partial class MatSumClienteViewModel : ViewModelBase {

    [ObservableProperty]
    bool _isLoading;

    [ObservableProperty]
    ObservableCollection<UnidadMedidaModel> _unidadMedidaList;

    [ObservableProperty]
    string _descripcion;

    [ObservableProperty]
    int? _cantidad;

    [ObservableProperty]
    UnidadMedidaModel _selectedUnidadMedida;

    [ObservableProperty]
    string _errorMesage;

    [ObservableProperty]
    bool _showError;

    [ObservableProperty]
    Color _borderColor = Color.FromArgb("#00FFFFFF");

    public event EventHandler<MaterialRequest> SendMaterial;
    protected void OnSendMaterial(MaterialRequest material) {
        SendMaterial?.Invoke(this, material);
    }

    public MatSumClienteViewModel() {
        InitValues();
    }

    [RelayCommand]
    async void Cancel() {
        SendResponse();
    }

    [RelayCommand]
    async Task Accept() {
        if(string.IsNullOrWhiteSpace(Descripcion)) {
            ErrorMesage = Constants.DESCRIPCION;
            ShowError = true;
            BorderColor = Color.FromArgb("#FF0000");
            return;
        }
        if(Cantidad is null || Cantidad.Value < 1) {
            ErrorMesage = Constants.INGRESE_CANTIDAD;
            ShowError = true;
            BorderColor = Color.FromArgb("#FF0000");
            return;
        }
        if(SelectedUnidadMedida is null) {
            ErrorMesage = Constants.SELECCIONE_UNIDAD;
            ShowError = true;
            BorderColor = Color.FromArgb("#FF0000");
            return;
        }

        MaterialRequest material = new MaterialRequest {
            Descripcion = Descripcion,
            Cantidad = Cantidad.Value,
            IdUnidad = SelectedUnidadMedida.IdUnidad,
            Unidad = SelectedUnidadMedida
        };

        SendResponse(material);
    }

    public async void SendResponse(MaterialRequest material = null) {
        material = material is null ? new MaterialRequest() : material;

        OnSendMaterial(material);
        await ClosePopup();
    }

    async void InitValues() {
        await InitUnidadMedidaList();
    }

    async Task InitUnidadMedidaList() {
        IsLoading = true;
        string url = Constants.OT_UNIDAD_MEDIDA_API;
        UnidadMedidaList = await _httpHelper.GetAsync<ObservableCollection<UnidadMedidaModel>>(url);
        IsLoading = false;
    }

    [RelayCommand]
    void CloseError() {
        ShowError = false;
        BorderColor = Color.FromArgb("#00FFFFFF");
    }

    async Task ClosePopup() {
        await MopupService.Instance.PopAsync();
    }
}