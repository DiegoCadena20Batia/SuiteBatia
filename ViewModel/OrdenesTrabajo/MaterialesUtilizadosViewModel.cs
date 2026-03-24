using BatiaSuite.Models.OrdenesTrabajo;
using BatiaSuite.Utils;
using BatiaSuite.Views.OrdenesTrabajo;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace BatiaSuite.ViewModel.OrdenesTrabajo;

public partial class MaterialesUtilizadosViewModel : ViewModelBase, IQueryAttributable {

    OrdenTrabajoEjecutadaModel _ordenTrabajo;

    [ObservableProperty]
    ObservableCollection<MaterialRequest> _matSumClienteList;

    [ObservableProperty]
    AlmacenModel _almacen;

    [ObservableProperty]
    ObservableCollection<MaterialResponse> _matSumAlmacenList;

    [ObservableProperty]
    ObservableCollection<MaterialResponse> _matSumCompraList;
    [ObservableProperty]
    bool _isLoading;

    [ObservableProperty]
    string _textLoading;
    public MaterialesUtilizadosViewModel() {
        MatSumClienteList = new ObservableCollection<MaterialRequest>();
        MatSumAlmacenList = new ObservableCollection<MaterialResponse>();
        MatSumCompraList = new ObservableCollection<MaterialResponse>();
        _ordenTrabajo = new OrdenTrabajoEjecutadaModel();
    }

    [RelayCommand]
    async Task AddMatSumCliente() {
        MaterialRequest matSumCliente = await PopupUtil.GetMaterialesAsync();

        if(string.IsNullOrWhiteSpace(matSumCliente.Descripcion)) {
            return;
        }
        MatSumClienteList.Add(matSumCliente);
    }

    [RelayCommand]
    void RemoveMatSumCliente(MaterialRequest matSumCliente)
        => MatSumClienteList.Remove(matSumCliente);

    [RelayCommand]
    async Task GetAlmacen() {
        AlmacenModel almacen = await PopupUtil.GetAlmacenAsync(Almacen);
        if(string.IsNullOrWhiteSpace(almacen.Nombre) || almacen.IdAlmacen == Almacen?.IdAlmacen) {
            return;
        }
        Almacen = almacen;

        MatSumAlmacenList.Clear();
        MatSumCompraList.Clear();
    }

    [RelayCommand]
    async Task AddMatSumAlmacen() {

        if(Almacen is null) {
            await GetAlmacen();

            if(Almacen is null) {
                return;
            }
        }

        MaterialResponse material = await PopupUtil.GetMaterialAsync(idAlmacen: Almacen.IdAlmacen, idCliente: 0);

        if(string.IsNullOrWhiteSpace(material.Descripcion)) {
            return;
        }

        if(MatSumAlmacenList.Any(m => m.Clave == material.Clave)) {
            await App.Current.MainPage.DisplayAlert("", Constants.MATERIAL_AGREGADO, Constants.ACEPTAR);
            return;
        }

        //Cantidad usada
        string cantidadString = await App.Current.MainPage
            .DisplayPromptAsync("", Constants.INGRESE_CANTIDAD, Constants.ACEPTAR, Constants.CANCELAR, keyboard: Keyboard.Numeric);

        if(cantidadString is null) return;

        int.TryParse(cantidadString, out int cantidadUtilizada);

        material.CantidadUsada = cantidadUtilizada;

        if(material.ExistsError) {
            await App.Current.MainPage.DisplayAlert("", material.ErrorMessage, Constants.ACEPTAR);
            return;
        }

        //Cantidad a cobrar 
        string cantidadXCobrarString = await App.Current.MainPage
            .DisplayPromptAsync("", Constants.INGRESE_CANTIDAD_COBRAR, Constants.ACEPTAR, Constants.CANCELAR, keyboard: Keyboard.Numeric);

        if(cantidadXCobrarString is null) return;

        int.TryParse(cantidadXCobrarString, out int cantidadXCobrar);

        material.CantidadXCobrar = cantidadXCobrar;

        if(material.ExistsError) {
            await App.Current.MainPage.DisplayAlert("", material.ErrorMessage, Constants.ACEPTAR);
            material.CantidadXCobrar = 0;
            return;
        }

        material.IdAlmacen = Almacen.IdAlmacen;
        MatSumAlmacenList.Add(material);
    }

    [RelayCommand]
    void RemoveMatSumAlmacen(MaterialResponse matSumAlmacen)
        => MatSumAlmacenList.Remove(matSumAlmacen);

    [RelayCommand]
    async Task AddMatSumCompra() {
        if(Almacen is null) {
            await GetAlmacen();

            if(Almacen is null) {
                return;
            }
        }

        MaterialResponse matSumCompra = await PopupUtil.GetMaterialAsync();

        if(string.IsNullOrWhiteSpace(matSumCompra.Descripcion)) {
            return;
        }

        if(MatSumCompraList.Any(m => m.Clave == matSumCompra.Clave)) {
            await App.Current.MainPage.DisplayAlert("", Constants.MATERIAL_AGREGADO, Constants.ACEPTAR);
            return;
        }

        //Cantidad comprada
        string cantidadString = await App.Current.MainPage
            .DisplayPromptAsync("", Constants.INGRESE_CANTIDAD_COMPRADA, Constants.ACEPTAR, Constants.CANCELAR, keyboard: Keyboard.Numeric);

        if(cantidadString is null) return;

        int.TryParse(cantidadString, out int cantidadComprada);

        matSumCompra.CantidadUsada = cantidadComprada;

        if(matSumCompra.ExistsError) {
            await App.Current.MainPage.DisplayAlert("", matSumCompra.ErrorMessage, Constants.ACEPTAR);
            return;
        }

        //Precio de compra 
        string precioCompra = await App.Current.MainPage
            .DisplayPromptAsync("", Constants.INGRESE_COSTO_UNITARIO, Constants.ACEPTAR, Constants.CANCELAR, keyboard: Keyboard.Numeric);

        if(precioCompra is null) return;

        float.TryParse(precioCompra, out float precio);

        matSumCompra.CostoUnitario = precio;

        if(matSumCompra.ExistsError) {
            await App.Current.MainPage.DisplayAlert("", matSumCompra.ErrorMessage, Constants.ACEPTAR);
            return;
        }

        //Cantidad utilizada
        string cantUtilizada = await App.Current.MainPage
            .DisplayPromptAsync("", Constants.INGRESE_CANTIDAD_UTILIZADA, Constants.ACEPTAR, Constants.CANCELAR, keyboard: Keyboard.Numeric, initialValue: cantidadComprada.ToString());

        if(cantUtilizada is null) return;

        int.TryParse(cantUtilizada, out int cantidadUtilizada);

        matSumCompra.CantUtilizada = cantidadUtilizada;

        if(matSumCompra.ExistsError) {
            await App.Current.MainPage.DisplayAlert("", matSumCompra.ErrorMessage, Constants.ACEPTAR);
            matSumCompra.CantUtilizada = 0;
            return;
        }

        matSumCompra.IdAlmacen = Almacen.IdAlmacen;
        MatSumCompraList.Add(matSumCompra);
    }

    [RelayCommand]
    void RemoveMatSumCompra(MaterialResponse matSumCompra)
        => MatSumCompraList.Remove(matSumCompra);

    [RelayCommand]
    async Task NextPage() {
        List<MaterialRequest> materialsList = new List<MaterialRequest>();

        if(!(MatSumClienteList is null || MatSumClienteList.Count == 0)) {
            foreach(MaterialRequest material in MatSumClienteList) {
                materialsList.Add(material);
            }
        }

        if(!(MatSumAlmacenList is null || MatSumAlmacenList.Count == 0)) {
            foreach(MaterialResponse material in MatSumAlmacenList) {
                MaterialRequest materialRequest = MaterialResponse.MaterialConvert(material);
                materialRequest.BtAlmacen = 1;
                materialsList.Add(materialRequest);
            }
        }

        if(!(MatSumCompraList is null || MatSumCompraList.Count == 0)) {
            foreach(MaterialResponse material in MatSumCompraList) {
                MaterialRequest materialRequest = MaterialResponse.MaterialConvert(material);
                materialRequest.BtAlmacen = 2;
                materialsList.Add(materialRequest);
            }
        }
        IsLoading = true;
        TextLoading = "Cargando...";

        _ordenTrabajo.Material = materialsList;

        Dictionary<string, object> datos = new Dictionary<string, object>{
           { Constants.ORDEN_TRABAJO_KEY, _ordenTrabajo }
        };

        await Shell.Current.GoToAsync($"{nameof(FotoEvidenciaPage)}", true, datos);
        IsLoading = false;
        TextLoading = "";
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query) {
        try {
            _ordenTrabajo = (OrdenTrabajoEjecutadaModel)query[Constants.ORDEN_TRABAJO_KEY];
        } catch(Exception) { }
    }
}