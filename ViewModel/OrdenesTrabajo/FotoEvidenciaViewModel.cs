using BatiaSuite.Models.OrdenesTrabajo;
using BatiaSuite.Utils;
using BatiaSuite.Views.Encuestas;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Storage;
using System.Collections.ObjectModel;

namespace BatiaSuite.ViewModel.OrdenesTrabajo;

public partial class FotoEvidenciaViewModel : ViewModelBase, IQueryAttributable {

    OrdenTrabajoEjecutadaModel _ordenTrabajo;
    int _filesMaxNum = 5;

    [ObservableProperty]
    ObservableCollection<string> _photoList;

    [ObservableProperty]
    ObservableCollection<string> _fileList;

    public FotoEvidenciaViewModel() {
        PhotoList = new ObservableCollection<string>();
        FileList = new ObservableCollection<string>();
    }
    [ObservableProperty]
    bool _isLoading;

    [ObservableProperty]
    string _textLoading;

    [RelayCommand]
    async Task TakePhoto() {
        if(PhotoList.Count >= _filesMaxNum) {
            await Toast.Make($"Número máximo de imágenes : {_filesMaxNum}", ToastDuration.Short).Show();
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
    async Task OpenGallery() {

        if(PhotoList.Count >= _filesMaxNum) {
            await Toast.Make($"Número máximo de imágenes : {_filesMaxNum}", ToastDuration.Short).Show();
            return;
        }

        try {
            FileResult selectedPhoto = await MediaPicker.Default.PickPhotoAsync();

            if(selectedPhoto != null) {
                string localFilePath = Path.Combine(FileSystem.CacheDirectory, selectedPhoto.FileName);

                using(Stream stream = await selectedPhoto.OpenReadAsync()) {
                    using FileStream localFileStream = File.OpenWrite(localFilePath);
                    await stream.CopyToAsync(localFileStream);
                }

                PhotoList.Add(localFilePath);

                if(PhotoList.Count >= _filesMaxNum) {
                    await Toast.Make($"Número máximo de imágenes : {_filesMaxNum}", ToastDuration.Short).Show();
                    return;
                }
            }
        } catch(Exception) { }
    }

    [RelayCommand]
    void RemovePhoto(string filePath) =>
        PhotoList.Remove(filePath);

    [RelayCommand]
    async Task OpenFilesPicker() {
        if(FileList.Count >= _filesMaxNum) {
            await Toast.Make($"Número máximo de archivos : {_filesMaxNum}", ToastDuration.Short).Show();
            return;
        }

        try {
            IEnumerable<FileResult> fileResultList = await FilePicker.Default.PickMultipleAsync(Constants.GetPickOptions(true, true));
            if(fileResultList is not null && fileResultList.Count() > 0) {
                foreach(FileResult fileResult in fileResultList) {
                    string localFilePath = Path.Combine(FileSystem.CacheDirectory, fileResult.FileName);

                    using(Stream stream = await fileResult.OpenReadAsync()) {
                        using FileStream localFileStream = File.OpenWrite(localFilePath);
                        await stream.CopyToAsync(localFileStream);
                    }

                    FileList.Add(localFilePath);

                    if(FileList.Count >= _filesMaxNum) {
                        await Toast.Make($"Número máximo de archivos : {_filesMaxNum}", ToastDuration.Short).Show();
                        return;
                    }
                }
            }
        } catch(Exception) { }
    }

    [RelayCommand]
    void RemoveFilePath(string filePath) =>
        FileList.Remove(filePath);

    [RelayCommand]
    async Task NextPage() {
        if((FileList is null || FileList.Count == 0)
            && (PhotoList is null || PhotoList.Count == 0)) {
            await Toast.Make(Constants.SELECCIONE_ARCHIVOS, ToastDuration.Short).Show();
            return;
        }
        IsLoading = true;
        TextLoading = "Cargando...";
        _ordenTrabajo.FilesList = FileList;
        _ordenTrabajo.FotosList = PhotoList;

        Dictionary<string, object> datos = new Dictionary<string, object>{
           { Constants.ORDEN_TRABAJO_KEY, _ordenTrabajo }
        };

        await Shell.Current.GoToAsync($"{nameof(EncuestaPage)}", true, datos);
        TextLoading = "";
        IsLoading = false;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query) {
        try {
            _ordenTrabajo = (OrdenTrabajoEjecutadaModel)query[Constants.ORDEN_TRABAJO_KEY];
        } catch(Exception) { }
    }   
}