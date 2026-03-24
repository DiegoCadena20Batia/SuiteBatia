using BatiaSuite.Models.Sanitizacion;
using BatiaSuite.Models.Supervision;
using BatiaSuite.Utils;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Text;

namespace BatiaSuite.ViewModel.Sanitizacion;

public partial class EvidenciasViewModel : ViewModelBase, IQueryAttributable {

    DrawingView _drawingView;
    string _signaturePath;
    int _filesMaxNum = 5;
    SanitizacionModel _sanitizacionData;

    [ObservableProperty]
    ObservableCollection<string> _photoList;

    [ObservableProperty]
    string _recibe;

    [ObservableProperty]
    bool _isLoading;

    [ObservableProperty]
    string _loadingText;

    [ObservableProperty]
    bool _showCleaner;

    public EvidenciasViewModel(DrawingView drawingView) {
        _drawingView = drawingView;
        PhotoList = new ObservableCollection<string>();
    }

    [RelayCommand]
    async Task TakePhoto() {
        if(PhotoList.Count >= _filesMaxNum) {
            await Toast.Make($"Número máximo de fotos : {_filesMaxNum}", ToastDuration.Short).Show();
            return;
        }

        try {
            if(MediaPicker.Default.IsCaptureSupported) {
                if(await PopupUtil.HasCameraPermissions()) {
                    FileResult? fileResult = await MediaPicker.CapturePhotoAsync();

                    if(fileResult != null) {
                        string localFilePath = Path.Combine(FileSystem.CacheDirectory, fileResult.FileName);

                        using(Stream stream = await fileResult.OpenReadAsync()) {
                            using FileStream localFileStream = File.OpenWrite(localFilePath);
                            await stream.CopyToAsync(localFileStream);
                        }

                        PhotoList.Add(localFilePath);
                    }
                }
            }
        } catch(Exception) { }
    }

    [RelayCommand]
    void RemovePhoto(string filePath) =>
       PhotoList.Remove(filePath);

    [RelayCommand]
    void ClearDrawingView() {
        _drawingView.Clear();
        ShowCleaner = false;
    }

    [RelayCommand]
    void Draw(IDrawingLine line) {
        ShowCleaner = true;
    }

    [RelayCommand]
    async Task SendData() {

        if(PhotoList is null || PhotoList.Count == 0) {
            await Toast.Make("Ingrese al menos una fotografía ", ToastDuration.Short).Show();
            return;
        }

        if(string.IsNullOrWhiteSpace(Recibe)) {
            await Toast.Make("Ingrese quién recibe", ToastDuration.Short).Show();
            return;
        }

        if(!await ValidarFirma()) {
            await Toast.Make("Ingrese la firma de quién recibe", ToastDuration.Short).Show();
            return;
        }

        LoadingText = Constants.ENVIANDO_DATOS;
        IsLoading = true;

        Location currentLocation = await LocationUtil.GetCurrentLocationAsync();
        if(currentLocation != null) {
            _sanitizacionData.Latitud = currentLocation.Latitude.ToString();
            _sanitizacionData.Longitud = currentLocation.Longitude.ToString();
        }

        _sanitizacionData.Recibe = Recibe;
        _sanitizacionData.IdUsuario = UserSession.IdPersonal;

        _sanitizacionData.Imagenes = new List<ArchivoModel>();

        foreach(string filePath in PhotoList) {
            byte[] fileBytesArray = File.ReadAllBytes(filePath);
            byte[] resizedImage = await ImageResizerHelper.ResizeImage(fileBytesArray, 480, 640);

            _sanitizacionData.Imagenes.Add(new ArchivoModel {
                Path = filePath,
                Tamano = resizedImage.Length,
                Seccion = 1
            });
        }

        byte[] bytesArray = File.ReadAllBytes(_signaturePath);
        _sanitizacionData.Imagenes.Add(new ArchivoModel {
            Path = _signaturePath,
            Tamano = bytesArray.Length,
            Seccion = 2
        });

        if(!await UploadPhotosAsync()) {
            await App.Current.MainPage.DisplayAlert("", Constants.ERROR_API, Constants.ACEPTAR);
            IsLoading = false;
            LoadingText = string.Empty;
            return;
        }

        int idSanitizacion = await _httpHelper.PostBodyAsync<SanitizacionModel, int>(Constants.SAN_POST_SANITIZACION_DATA, _sanitizacionData);

        IsLoading = false;
        LoadingText = string.Empty;

        if(idSanitizacion > 0) {
            await Shell.Current.GoToAsync("//MyMenu");
            await App.Current.MainPage.DisplayAlert("", Constants.DATOS_ENVIADOS_CORRECTAMENTE, Constants.ACEPTAR);
        } else {
            await App.Current.MainPage.DisplayAlert("", Constants.ERROR_API, Constants.ACEPTAR);
        }
    }

    async Task<bool> ValidarFirma() {
        try {
            string localFilePath = Path.Combine(FileSystem.CacheDirectory, $"Firma_{Guid.NewGuid()}.png");

            using(Stream stream = await _drawingView.GetImageStream(512, 512)) {
                using FileStream localFileStream = File.OpenWrite(localFilePath);
                await stream.CopyToAsync(localFileStream);
            }

            _signaturePath = localFilePath;

            return true;
        } catch(Exception) {
            _signaturePath = string.Empty;
            return false;
        }
    }

    async Task<bool> UploadPhotosAsync() {
        using(MultipartFormDataContent multipartContent = new MultipartFormDataContent()) {

            multipartContent.Headers.ContentType.MediaType = "multipart/form-data";
            multipartContent.Headers.Add("folder", "sanitiza");

            foreach(ArchivoModel archivo in _sanitizacionData.Imagenes) {
                byte[] fileBytesArray = File.ReadAllBytes(archivo.Path);
                byte[] resizedImage = await ImageResizerHelper.ResizeImage(fileBytesArray, 480, 640);
                Stream stream = new MemoryStream(archivo.Seccion == 1 ? resizedImage : fileBytesArray);
                multipartContent.Add(new StreamContent(stream), "files", archivo.Nombre);
            }

            List<ArchivoModel> result = await _httpHelper.PostMultipartAsync<List<ArchivoModel>>(Constants.SUP_POST_FOTOS, multipartContent, false);
            return result is not null;
        }
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query) {
        if(query.ContainsKey(Constants.SANITIZACION_DATA_KEY)) {
            _sanitizacionData = (SanitizacionModel)query[Constants.SANITIZACION_DATA_KEY];
            query.Remove(Constants.SANITIZACION_DATA_KEY);
        }
    }
}