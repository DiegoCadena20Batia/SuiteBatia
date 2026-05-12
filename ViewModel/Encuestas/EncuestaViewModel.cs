using BatiaSuite.Models.Encuestas;
using BatiaSuite.Models.OrdenesTrabajo;
using BatiaSuite.Utils;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BatiaSuite.ViewModel.Encuestas;

public partial class EncuestaViewModel : ViewModelBase, IQueryAttributable {
    private OrdenTrabajoEjecutadaModel _ordenTrabajo;
    private int _trabajosGral, _tecnicoUniforme, _trato, _calidad, _materiales;
    private DrawingView _drawingView;
    private string _signatureFilePath;

    [ObservableProperty]
    private bool _hayEncuesta;

    [ObservableProperty]
    private string _nombreEncuestado;

    [ObservableProperty]
    private bool _showCleaner;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _textLoading;

    public EncuestaViewModel(DrawingView drawingView) {
        _drawingView = drawingView;
    }

    [RelayCommand]
    private void ClearDrawingView() {
        _drawingView.Clear();
        ShowCleaner = false;
        _signatureFilePath = null;
    }

    [RelayCommand]
    private void Draw(IDrawingLine line) {
        ShowCleaner = true;
    }

    [RelayCommand]
    private void CheckedChanged(object parameters) {
        object[] parametros = parameters as object[];
        bool isChecked = (bool)parametros[0];

        if(isChecked) {
            string pregunta = (string)parametros[1];
            int value = int.Parse(parametros[2].ToString());

            switch(pregunta) {
                case "1":
                    _trabajosGral = value;
                    break;

                case "2":
                    _tecnicoUniforme = value;
                    break;

                case "3":
                    _trato = value;
                    break;

                case "4":
                    _calidad = value;
                    break;

                case "5":
                    _materiales = value;
                    break;
            }
        }
    }

    [RelayCommand]
    private async Task EnviarOrdenTrabajoEjecutada() {
        if(_ordenTrabajo is null) {
            return;
        }
        Reporte reporte = null;

        if(HayEncuesta) {
             reporte = new Reporte {
                IdOrden = _ordenTrabajo.Trabajo.IdOrden,
                TrabajosGeneral = _trabajosGral,
                TecnicosUniforme = _tecnicoUniforme,
                TratoTecnicos = _trato,
                TrabajosOrden = _calidad,
                MaterialesAdecuados = _materiales,
                Encuestado = NombreEncuestado,
            };

            if(!reporte.IsValid) {
                await Toast.Make(reporte.ErrorMessage, ToastDuration.Short).Show();
                return;
            }

            if(!await ValidarFirma()) {
                await Toast.Make(Constants.INGRESE_FIRMA, ToastDuration.Short).Show();
                return;
            }
        }
        _ordenTrabajo.Reporte = reporte;
        _ordenTrabajo.Trabajo.Fejecucion = (DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss");

        IsLoading = true;
        TextLoading = "Enviando orden...";
        if(!await UploadPhotosAsync()) {
            IsLoading = false;
            TextLoading = "";
            await App.Current.MainPage.DisplayAlert("", Constants.ERROR_API, Constants.ACEPTAR);
            return;
        }

        //TODO: en este punto se debe guardar en local la orden de trabajo 

        int result = await _httpHelper.PostBodyAsync<OrdenTrabajoEjecutadaModel, int>(Constants.OT_ENVIAR_ORDEN_EJECUTADA_API, _ordenTrabajo);

        IsLoading = false;
        TextLoading = "";
        if(result > 0) {
            await Shell.Current.GoToAsync("//MyMenu");
            await App.Current.MainPage.DisplayAlert("", Constants.ORDEN_TRABAJO_ENVIADA, Constants.ACEPTAR);
        } else {
            await App.Current.MainPage.DisplayAlert("", Constants.ERROR_API, Constants.ACEPTAR);
        }
    }

    private async Task<bool> UploadPhotosAsync() {
        using(MultipartFormDataContent multipartContent = new MultipartFormDataContent()) {
            if(HayEncuesta && !string.IsNullOrWhiteSpace(_signatureFilePath)) {
                byte[] fileBytesArray = File.ReadAllBytes(_signatureFilePath);
                ByteArrayContent byteArrayContent = new ByteArrayContent(fileBytesArray);
                multipartContent.Add(byteArrayContent, "files", Path.GetFileName(_signatureFilePath));
            }
            if(_ordenTrabajo.FotosList is not null && _ordenTrabajo.FotosList.Count() > 0) {
                foreach(string foto in _ordenTrabajo.FotosList) {
                    byte[] fileBytesArray = File.ReadAllBytes(foto);
                    byte[] resizedImage = await ImageResizerHelper.ResizeImage(fileBytesArray, 480, 640);
                    ByteArrayContent byteArrayContent = new ByteArrayContent(resizedImage);
                    multipartContent.Add(byteArrayContent, "files", Path.GetFileName(foto));
                }
            }
            if(_ordenTrabajo.FilesList is not null && _ordenTrabajo.FilesList.Count() > 0) {
                foreach(string file in _ordenTrabajo.FilesList) {
                    byte[] fileBytesArray = File.ReadAllBytes(file);
                    if(Path.GetFileName(file).EndsWith("jpg") || Path.GetFileName(file).EndsWith("jpeg") || Path.GetFileName(file).EndsWith("png")) {
                        byte[] resizedImage = await ImageResizerHelper.ResizeImage(fileBytesArray, 480, 640, false);
                        ByteArrayContent byteArrayCont = new ByteArrayContent(resizedImage);
                        multipartContent.Add(byteArrayCont, "files", Path.GetFileName(file));
                        continue;
                    }
                    ByteArrayContent byteArrayContent = new ByteArrayContent(fileBytesArray);
                    multipartContent.Add(byteArrayContent, "files", Path.GetFileName(file));
                }
            }
            string url = $"{Constants.OT_FILES_API}/CargaMul?folio={_ordenTrabajo.Trabajo.IdOrden}";

            string result = await _httpHelper.PostMultipartAsync<string>(url, multipartContent);
            return !string.IsNullOrWhiteSpace(result);
        }
    }

    private async Task<bool> ValidarFirma() {
        try {
            string localFilePath = Path.Combine(FileSystem.CacheDirectory, $"Firma_{Guid.NewGuid()}.png");

            using(Stream stream = await _drawingView.GetImageStream(512, 512)) {
                using FileStream localFileStream = File.OpenWrite(localFilePath);
                await stream.CopyToAsync(localFileStream);
            }

            _signatureFilePath = localFilePath;

            return true;
        } catch(Exception) {
            return false;
        }
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query) {
        try {
            _ordenTrabajo = (OrdenTrabajoEjecutadaModel)query[Constants.ORDEN_TRABAJO_KEY];
        } catch(Exception) { }
    }
}