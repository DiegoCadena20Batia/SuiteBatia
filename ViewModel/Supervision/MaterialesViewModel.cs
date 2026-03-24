using BatiaSuite.Models.OrdenesTrabajo;
using BatiaSuite.Models.Supervision;
using BatiaSuite.Utils;
using BatiaSuite.Views.Supervision;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace BatiaSuite.ViewModel.Supervision;

public partial class MaterialesViewModel : ViewModelBase, IQueryAttributable {

    [ObservableProperty]
    ObservableCollection<ListadoMaterial> _materialList;

    [ObservableProperty]
    ObservableCollection<ListadoMaterial> _extraMaterialList;

    [ObservableProperty]
    bool _isLoading;

    [ObservableProperty]
    string _loadingText;

    [ObservableProperty]
    string _pageTitle;

    [ObservableProperty]
    MediaSource _videoMediaSource;

    public event EventHandler DisplaceScroll;
    SupervisionRequestDataModel _supervisionRequestData;
    string _pathVideo;
    MediaElement _mediaElement;

    protected void OnDisplaceScroll() {
        DisplaceScroll?.Invoke(this, new EventArgs());
    }

    public MaterialesViewModel() {
        ExtraMaterialList = new ObservableCollection<ListadoMaterial>();
    }

    [RelayCommand]
    async Task AddExtraMaterial() {
        MaterialResponse material = await PopupUtil.GetMaterialAsync(idAlmacen: 0, _supervisionRequestData.Id_Cliente);

        if(string.IsNullOrWhiteSpace(material.Descripcion)) {
            return;
        }

        if(!ValidarExistenciaMaterial(material.Clave)) {
            await App.Current.MainPage.DisplayAlert("", Constants.MATERIAL_AGREGADO, Constants.ACEPTAR);
            return;
        }

        string cantidadSugeridaString = await App.Current.MainPage
           .DisplayPromptAsync("", Constants.INGRESE_CANTIDAD_SUGERIDA, Constants.ACEPTAR, Constants.CANCELAR, keyboard: Keyboard.Numeric);

        if(cantidadSugeridaString is null) return;

        int.TryParse(cantidadSugeridaString, out int cantidadSugerida);

        ExtraMaterialList.Add(new ListadoMaterial {
            IdListado = MaterialList.Count > 0 ? MaterialList[0].IdListado : 0,
            Clave = material.Clave,
            Descripcion = material.Descripcion,
            Cantidad = "0",
            Sugerido = cantidadSugerida
        });

        await Task.Delay(50);
        OnDisplaceScroll();
    }

    [RelayCommand]
    void EliminarExtraMaterial(ListadoMaterial material) =>
        ExtraMaterialList.Remove(material);

    [RelayCommand]
    async Task Continuar() {
        //if(string.IsNullOrWhiteSpace(_pathVideo)) {
        //    await App.Current.MainPage.DisplayAlert("", Constants.AGREGUE_VIDEO, Constants.ACEPTAR);
        //    return;
        //}

        //_supervisionRequestData.PathVideo = _pathVideo;
        _supervisionRequestData.ListadoMateriales = (MaterialList.Union(ExtraMaterialList)).ToList();

        Dictionary<string, object> data = new Dictionary<string, object> {
            { Constants.SUPERVISION_REQUEST_DATA_KEY, _supervisionRequestData}
        };

        //await Shell.Current.GoToAsync(nameof(EncuestaSupervisionPage), true, data);
        await Shell.Current.GoToAsync(nameof(VideoPage), true, data);
    }

    [RelayCommand]
    async Task CapturarVideo() {
        try {
            if(MediaPicker.Default.IsCaptureSupported) {
                if(await PopupUtil.HasCameraPermissions()) {
                    FileResult? fileResult = await MediaPicker.CaptureVideoAsync();
                    
                    if(fileResult != null) {
                        _pathVideo = Path.Combine(FileSystem.CacheDirectory, fileResult.FileName);

                        using(Stream stream = await fileResult.OpenReadAsync()) {
                            using FileStream localFileStream = File.OpenWrite(_pathVideo);
                            await stream.CopyToAsync(localFileStream);
                        }
                        VideoMediaSource = FileMediaSource.FromFile(_pathVideo);

                        IsLoading = true;
                        LoadingText = Constants.VALIDAR_VIDEO;
                        await Task.Delay(2000);
                        LoadingText = "";
                        IsLoading = false;

                        int seconds = 15;
                        if(_mediaElement.Duration > TimeSpan.FromSeconds(seconds)) {
                            await App.Current.MainPage.DisplayAlert("", $"La duración del video debe ser menor de {seconds} segundos", Constants.ACEPTAR);
                            _pathVideo = null;
                            VideoMediaSource = null;
                            return;
                        }
                    }
                }
            }
        } catch(Exception) {
            VideoMediaSource = null;
        }
    }

    bool ValidarExistenciaMaterial(string clave) {
        foreach(ListadoMaterial material in MaterialList) {
            if(material.Clave.Equals(clave)) {
                return false;
            }
        }

        foreach(ListadoMaterial material in ExtraMaterialList) {
            if(material.Clave.Equals(clave)) {
                return false;
            }
        }

        return true;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query) {
        if(query.ContainsKey(Constants.SUPERVISION_REQUEST_DATA_KEY)) {
            _supervisionRequestData = (SupervisionRequestDataModel)query[Constants.SUPERVISION_REQUEST_DATA_KEY];
            PageTitle = $"Inventario  {Constants.GetMonthName(_supervisionRequestData.Mes)}  {_supervisionRequestData.Anio}";
            InitValues();

            query.Remove(Constants.SUPERVISION_REQUEST_DATA_KEY);
        }
    }

    async void InitValues() {
        IsLoading = true;
        await LoadMaterialListAsync();
        VideoMediaSource = null;
        IsLoading = false;
    }

    async Task LoadMaterialListAsync() {
        string url = $"{Constants.SUP_GET_MATERIALES_API}?idcliente={_supervisionRequestData.Id_Cliente}&idinmueble={_supervisionRequestData.Id_Inmueble}&anio={_supervisionRequestData.Anio}&mes={_supervisionRequestData.Mes}";

        MaterialList = await _httpHelper.GetAsync<ObservableCollection<ListadoMaterial>>(url);

        if(MaterialList is null) {
            MaterialList = new ObservableCollection<ListadoMaterial>();
        }
    }
}