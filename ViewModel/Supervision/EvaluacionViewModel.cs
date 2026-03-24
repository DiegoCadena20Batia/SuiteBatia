using BatiaSuite.Models.Supervision;
using BatiaSuite.Utils;
using BatiaSuite.Views.Supervision;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace BatiaSuite.ViewModel.Supervision;

public partial class EvaluacionViewModel : ViewModelBase, IQueryAttributable {

    [ObservableProperty]
    ObservableCollection<Evaluacion> _preguntas;

    [ObservableProperty]
    ObservableCollection<string> _photoList;

    SupervisionRequestDataModel _supervisionRequestData;
    int _filesMaxNum = 5;
    [ObservableProperty]
    bool _isLoading;

    [ObservableProperty]
    string _textLoading;
    public EvaluacionViewModel() {
        PhotoList = new ObservableCollection<string>();
    }

    [RelayCommand]
    async Task Continuar() {
        foreach(Evaluacion pregunta in Preguntas) {
            if(pregunta.Valor is null) {
                int numPregunta = Preguntas.IndexOf(pregunta) + 1;
                await App.Current.MainPage.DisplayAlert("", $"{Constants.CONTESTE_PREGUNTAS} {numPregunta}", Constants.ACEPTAR);
                return;
            }
        }

        foreach(Evaluacion pregunta in Preguntas) {
            if(pregunta.Valor == -0.5 && string.IsNullOrWhiteSpace(pregunta.Observaciones)) {
                int numPregunta = Preguntas.IndexOf(pregunta) + 1;
                await App.Current.MainPage.DisplayAlert("", $"{Constants.INGRESE_OBSERVACIONES} {numPregunta}", Constants.ACEPTAR);
                return;
            }
        }

        _supervisionRequestData.FotosPantalla4 = ConvertertFotoList(PhotoList, 4);
        _supervisionRequestData.PreguntasEvaluacion = new List<Evaluacion>(Preguntas);

        Dictionary<string, object> data = new Dictionary<string, object> {
             { Constants.SUPERVISION_REQUEST_DATA_KEY, _supervisionRequestData },
        };

        await Constants.GoToAsync(nameof(ChecklistOperadorPage), data);
    }

    [RelayCommand]
    async Task TakePhoto() {
        if(PhotoList.Count >= _filesMaxNum) {
            await Toast.Make($"{Constants.NUMERO_MAXIMO} {_filesMaxNum}", ToastDuration.Short).Show();
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

    public async void ApplyQueryAttributes(IDictionary<string, object> query) {
        IsLoading = true;
        TextLoading = Constants.CARGANDO;
        if(query.ContainsKey(Constants.SUPERVISION_REQUEST_DATA_KEY)) {
            _supervisionRequestData = (SupervisionRequestDataModel)query[Constants.SUPERVISION_REQUEST_DATA_KEY];
            Preguntas = new ObservableCollection<Evaluacion>(await Evaluacion.ObtenerPreguntas());

            query.Remove(Constants.SUPERVISION_REQUEST_DATA_KEY);
        }
        IsLoading = false;
        TextLoading = "";
    }
}