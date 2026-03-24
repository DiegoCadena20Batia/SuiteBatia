using BatiaSuite.Models.Supervision;
using BatiaSuite.Utils;
using BatiaSuite.Views.Supervision;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace BatiaSuite.ViewModel.Supervision;

public partial class ChecklistOperadorViewModel : ViewModelBase, IQueryAttributable {

    [ObservableProperty]
    ObservableCollection<ChecklistPregunta> _preguntas;

    [ObservableProperty]
    ObservableCollection<string> _photoList;

    [ObservableProperty]
    bool _showCleaner;

    [ObservableProperty]
    string _nombreOperador;

    SupervisionRequestDataModel _supervisionRequestData;
    int _filesMaxNum = 5;
    DrawingView _drawingView;
    string _pathFirmaOperador;


    public ChecklistOperadorViewModel(DrawingView drawingView) {
        _drawingView = drawingView;
        PhotoList = new ObservableCollection<string>();
    }
    [ObservableProperty]
    ObservableCollection<ListadoMaterial> _materialList;


    [ObservableProperty]
    bool _mostrarElemento;
    [ObservableProperty]
    bool _isLoading;

    [ObservableProperty]
    string _textLoading;
    [RelayCommand]
    async Task Continuar() {

        if(Preguntas[9].Valor == true) {
            if(string.IsNullOrWhiteSpace(NombreOperador)) {
                await Toast.Make($"{Constants.INGRESE} {Constants.PERSONAL_QUE_RECIBIO_CAPACITACION}", ToastDuration.Short).Show();
                return;
            }
            if (_drawingView.Lines.Count == 0) {
                await Toast.Make($"{Constants.INGRESE_FIRMA}", ToastDuration.Short).Show();
                return;
            }
        }

        await CargarFirma();

        _supervisionRequestData.FotosPantalla5 = ConvertertFotoList(PhotoList, 5);
        _supervisionRequestData.PathFirmaOperador = _pathFirmaOperador;
        _supervisionRequestData.ChecklistPreguntas = new List<ChecklistPregunta>(Preguntas);
        _supervisionRequestData.NombreOperador = NombreOperador;
        _supervisionRequestData.ListadoMateriales = new List<ListadoMaterial>();
        Dictionary<string, object> data = new Dictionary<string, object> {
             { Constants.SUPERVISION_REQUEST_DATA_KEY, _supervisionRequestData },
            };

        string url = $"{Constants.SUP_GET_MATERIALES_API}?idcliente={_supervisionRequestData.Id_Cliente}&idinmueble={_supervisionRequestData.Id_Inmueble}&anio={_supervisionRequestData.Anio}&mes={_supervisionRequestData.Mes}";

        if (!Utils.InternetUtil.IsConnectedInternet())
        {
            await Shell.Current.GoToAsync(nameof(VideoPage), true, data);
            return;
        }
        var materialList = await _httpHelper.GetAsync<ObservableCollection<ListadoMaterial>>(url);

        if(materialList is null) {
            materialList = new ObservableCollection<ListadoMaterial>();

            //_supervisionRequestData.ListadoMateriales = new List<ListadoMaterial>();
            //Dictionary<string, object> data = new Dictionary<string, object> {
            // { Constants.SUPERVISION_REQUEST_DATA_KEY, _supervisionRequestData },
            //};
            await Shell.Current.GoToAsync(nameof(VideoPage), true, data);
            //await Shell.Current.GoToAsync(nameof(EncuestaSupervisionPage), true, data);

        } else {
        //    Dictionary<string, object> data = new Dictionary<string, object> {
        //     { Constants.SUPERVISION_REQUEST_DATA_KEY, _supervisionRequestData },
        //};
            await Constants.GoToAsync(nameof(MaterialesPage), data);
        }
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

    [RelayCommand]
    void ClearDrawingView() {
        _drawingView.Clear();
        ShowCleaner = false;
    }

    [RelayCommand]
    void Draw(IDrawingLine line) =>
        ShowCleaner = true;

    async Task<bool> CargarFirma() {
        try {
            _pathFirmaOperador = Path.Combine(FileSystem.CacheDirectory, $"Firma_{Guid.NewGuid()}.png");

            using(Stream stream = await _drawingView.GetImageStream(512, 512)) {
                using FileStream localFileStream = File.OpenWrite(_pathFirmaOperador);
                await stream.CopyToAsync(localFileStream);
            }

            return true;
        } catch(Exception) {
            _pathFirmaOperador = null;
            return false;
        }
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query) {
        IsLoading = true;
        TextLoading = "Obteniendo datos";
        if(query.ContainsKey(Constants.SUPERVISION_REQUEST_DATA_KEY)) {
            _supervisionRequestData = (SupervisionRequestDataModel)query[Constants.SUPERVISION_REQUEST_DATA_KEY];
            List<ChecklistPregunta> list = await ChecklistPregunta.ObtenerPreguntas();
            Preguntas = new ObservableCollection<ChecklistPregunta>(list);
            // Suscribir al evento de cambio de propiedad en cada pregunta
            foreach(var pregunta in Preguntas) {
                pregunta.PropertyChanged += Pregunta_PropertyChanged;
            }

            query.Remove(Constants.SUPERVISION_REQUEST_DATA_KEY);
        }
        IsLoading = false;
        TextLoading = "";
    }

    private void Pregunta_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
        if(e.PropertyName == nameof(ChecklistPregunta.Valor)) {
            var pregunta = sender as ChecklistPregunta;
            if(pregunta != null && Preguntas.IndexOf(pregunta) == 9) // Índice 8 es la posición 9
            {
                MostrarElemento = pregunta.Valor;
            }
        }
    }
}
