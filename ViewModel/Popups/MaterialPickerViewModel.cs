using BatiaSuite.Models.OrdenesTrabajo;
using BatiaSuite.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using System.Collections.ObjectModel;

namespace BatiaSuite.ViewModel.Popups;

public partial class MaterialPickerViewModel : ViewModelBase {

    CancellationTokenSource _cancellationTokenSource;
    int _idCliente;

    [ObservableProperty]
    int _idAlmacen;

    [ObservableProperty]
    bool _isLoading;

    [ObservableProperty]
    string _emptyView;

    [ObservableProperty]
    ObservableCollection<MaterialResponse> _materialList;

    public event EventHandler<MaterialResponse> SelectedMaterial;
    protected void OnSelectedMaterial(MaterialResponse material) {
        SelectedMaterial?.Invoke(this, material);
    }

    public MaterialPickerViewModel(int idAlmacen, int idCliente) {
        IdAlmacen = idAlmacen;
        _idCliente = idCliente;
    }

    [RelayCommand]
    async Task GetMaterialList(string query) {
        if(_cancellationTokenSource is not null) {
            _cancellationTokenSource.Cancel();
        }
        _cancellationTokenSource = new CancellationTokenSource();

        IsLoading = true;
        CancellationToken cancellationToken = _cancellationTokenSource.Token;

        if(!cancellationToken.IsCancellationRequested) {

            MaterialList = await _httpHelper.GetAsync<ObservableCollection<MaterialResponse>>(GetUrl(query));

            if(IdAlmacen == 0 && _idCliente == 0) {// es compra directa
                foreach(MaterialResponse material in MaterialList) {
                    material.IsCompra = true;
                }
            }
        }

        if(MaterialList is null || MaterialList.Count == 0) {
            EmptyView = Constants.NO_HAY_REGISTROS;
        }

        IsLoading = false;
    }

    [RelayCommand]
    async Task SelectMaterial(MaterialResponse materialResponse) {
        SendResponse(materialResponse);
    }

    [RelayCommand]
    async void Cancel() {
        SendResponse();
    }

    string GetUrl(string query) {
        if(IdAlmacen == 0) {
            if(_idCliente == 0) {
                return $"{Constants.OT_PRODUCTOS_API}?nombre={query}"; // es compra directa MANTENIMIENTO
            } else {
                return $"{Constants.SUP_PRODUCTOS_CLIENTE_API}?idcliente={_idCliente}&nombre={query}"; // Es material autorizado SUPERVISIÓN
            }
        } else {
            return $"{Constants.OT_MATERIAL_API}?idalmacen={IdAlmacen}&nombre={query}"; // es almacén MANTENIMIENTO
        }
    }

    async void SendResponse(MaterialResponse material = null) {
        if(material is null) {
            material = new MaterialResponse();
        }
        OnSelectedMaterial(material);
        await ClosePopup();
    }

    async Task ClosePopup() {
        await MopupService.Instance.PopAsync();
    }
}