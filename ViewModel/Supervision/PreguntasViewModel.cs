using BatiaSuite.Models.Supervision;
using BatiaSuite.Utils;
using BatiaSuite.Views.Supervision;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace BatiaSuite.ViewModel.Supervision;

public partial class PreguntasViewModel : ViewModelBase, IQueryAttributable {

    [ObservableProperty]
    ObservableCollection<SupervisionPregunta> _preguntas;

    [ObservableProperty]
    bool _isLoading;

    [ObservableProperty]
    string _textLoading;

    [ObservableProperty]
    SeccionTipoSucursal _seccion;

    [ObservableProperty]
    ObservableCollection<string> _photoList;

    SupervisionRequestDataModel _supervisionRequestData;
    List<SeccionTipoSucursal> _secciones;
    int _indice, _filesMaxNum = 5;
    CancellationTokenSource _cancelTokenSource;
    bool _isCheckingLocation;

    public PreguntasViewModel() {
        PhotoList = new ObservableCollection<string>();
    }

    [RelayCommand]
    async Task Continuar() {
        IsLoading = true;
        foreach(SupervisionPregunta pregunta in Preguntas) {
            if(pregunta.Valor is null) {
                IsLoading = false;
                int indice = Preguntas.IndexOf(pregunta) + 1;
                await App.Current.MainPage.DisplayAlert("", $"{Constants.CONTESTE_PREGUNTAS} {indice}", Constants.ACEPTAR);
                return;
            }
        }

        foreach(SupervisionPregunta pregunta in Preguntas) {
            if(pregunta.Valor == -0.5 && string.IsNullOrWhiteSpace(pregunta.Observaciones)) {
                IsLoading = false;
                int indice = Preguntas.IndexOf(pregunta) + 1;
                await App.Current.MainPage.DisplayAlert("", $"{Constants.INGRESE_OBSERVACIONES} {indice}", Constants.ACEPTAR);
                return;
            }
        }

        int seccionActual = _indice + 1;

        List<ArchivoModel> archivosActuales = ConvertertFotoList(PhotoList, seccionActual);
        List<SupervisionPregunta> preguntasActuales = new List<SupervisionPregunta>(Preguntas);

        switch(seccionActual) {
            case 1:
                _supervisionRequestData.PreguntasSeccion1 = preguntasActuales;
                _supervisionRequestData.FotosPantalla1 = archivosActuales;
                break;
            case 2:
                _supervisionRequestData.PreguntasSeccion2 = preguntasActuales;
                _supervisionRequestData.FotosPantalla2 = archivosActuales;
                break;
            case 3:
                _supervisionRequestData.PreguntasSeccion3 = preguntasActuales;
                _supervisionRequestData.FotosPantalla3 = archivosActuales;
                break;
            case 4:
                _supervisionRequestData.PreguntasSeccion4 = preguntasActuales;
                _supervisionRequestData.FotosPantalla6 = archivosActuales;
                break;
        }

        Dictionary<string, object> data = new Dictionary<string, object> {
             { Constants.SUPERVISION_REQUEST_DATA_KEY, _supervisionRequestData },
        };

        if(_secciones.Count > seccionActual) {
            IsLoading = false;
            data.Add(Constants.SECCIONES_KEY, _secciones);
            data.Add(Constants.INDICE_KEY, seccionActual);
            await Constants.GoToAsync(nameof(PreguntasPage), data);

            return;
        }
        
        await Constants.GoToAsync(nameof(EvaluacionPage), data);
        IsLoading = false;
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
    void RemovePhoto(string filePath)
        => PhotoList.Remove(filePath);

    public async void ApplyQueryAttributes(IDictionary<string, object> query) {
        IsLoading = true;
        TextLoading = Constants.CARGANDO;
        if(query.ContainsKey(Constants.INDICE_KEY)) {
            _indice = (int)query[Constants.INDICE_KEY];

            query.Remove(Constants.INDICE_KEY);
        }

        if(query.ContainsKey(Constants.SECCIONES_KEY)) {
            _secciones = (List<SeccionTipoSucursal>)query[Constants.SECCIONES_KEY];
            Seccion = _secciones[_indice];
            Preguntas = new ObservableCollection<SupervisionPregunta>(await SupervisionPregunta.ObtenerPreguntasPorIdSeccion(Seccion.Id));

            query.Remove(Constants.SECCIONES_KEY);
        };

        if(query.ContainsKey(Constants.SUPERVISION_REQUEST_DATA_KEY)) {
            _supervisionRequestData = (SupervisionRequestDataModel)query[Constants.SUPERVISION_REQUEST_DATA_KEY];
            GetCurrentLocation();

            query.Remove(Constants.SUPERVISION_REQUEST_DATA_KEY);
        }
        IsLoading = false;
        TextLoading = "";
    }

    async void GetCurrentLocation() {
        if(!string.IsNullOrWhiteSpace(_supervisionRequestData.Latitud) || !string.IsNullOrWhiteSpace(_supervisionRequestData.Longitud)) {
            return;
        }

        try {
            _isCheckingLocation = true;

            GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

            _cancelTokenSource = new CancellationTokenSource();

            Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

            if(location != null) {
                _supervisionRequestData.Latitud = location.Latitude.ToString();
                _supervisionRequestData.Longitud = location.Longitude.ToString();
                return;
            }

            _supervisionRequestData.Latitud = "0.0";
            _supervisionRequestData.Longitud = "0.0";

        } catch(Exception) {
            _supervisionRequestData.Latitud = "0.0";
            _supervisionRequestData.Longitud = "0.0";
        } finally {
            _isCheckingLocation = false;
        }
    }
}