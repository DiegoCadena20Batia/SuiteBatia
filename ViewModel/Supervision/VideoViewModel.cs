using BatiaSuite.Models.OrdenesTrabajo;
using BatiaSuite.Models.Supervision;
using BatiaSuite.Utils;
using BatiaSuite.Views.Supervision;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Camera.MAUI;
using static Microsoft.Maui.ApplicationModel.Permissions;
using System.Diagnostics;
namespace BatiaSuite.ViewModel.Supervision;

public partial class VideoViewModel : ViewModelBase, IQueryAttributable {

    [ObservableProperty]
    bool _isLoading;

    [ObservableProperty]
    string _loadingText;

    [ObservableProperty]
    string _pageTitle;

    [ObservableProperty]
    bool _cameraVisible;

    [ObservableProperty]
    MediaSource _videoMediaSource;

    public event EventHandler DisplaceScroll;
    SupervisionRequestDataModel _supervisionRequestData;
    string _pathVideo;
    MediaElement _mediaElement;

    private CameraView? _currentCameraView;
    private string _fileName = string.Empty;
    private bool _isRecording;
    private CancellationTokenSource _grabacionCts;

    public bool IsRecording
    {
        get => _isRecording;
        set => SetProperty(ref _isRecording, value);
    }

    protected void OnDisplaceScroll() {
        DisplaceScroll?.Invoke(this, new EventArgs());
    }

    public VideoViewModel(MediaElement mediaElement) {
        _mediaElement = mediaElement;
    }

    
    [RelayCommand]
    async Task Continuar() {
        if(string.IsNullOrWhiteSpace(_pathVideo)) {
            await App.Current.MainPage.DisplayAlert("", Constants.AGREGUE_VIDEO, Constants.ACEPTAR);
            return;
        }

        _supervisionRequestData.PathVideo = _pathVideo;

        Dictionary<string, object> data = new Dictionary<string, object> {
            { Constants.SUPERVISION_REQUEST_DATA_KEY, _supervisionRequestData}
        };

        await Shell.Current.GoToAsync(nameof(EncuestaSupervisionPage), true, data);
    }


    [RelayCommand]
    async Task CapturarVideo(CameraView cameraView)
    {
#if IOS
            await CapturarVideoIos();
            return;
#endif
#if ANDROID
_grabacionCts = new CancellationTokenSource();
        try
        {
            IsLoading = true;
            LoadingText = "Preparando camara";
            if (!MediaPicker.Default.IsCaptureSupported)
                return;

            if (!await PopupUtil.HasCameraPermissions())
                return;

            if (cameraView.Cameras is null || cameraView.Cameras.Count == 0)
                throw new Exception("No se encontraron cámaras disponibles.");
            await ReiniciarCamara(cameraView);
            await Task.Delay(1000);
            
            if (_currentCameraView != null && IsRecording)
            {
                try
                {
                    await _currentCameraView.StopRecordingAsync();
                }
                catch (ObjectDisposedException)
                {
                    Debug.WriteLine("Camera session ya estaba liberada. Ignorando StopRecordingAsync.");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error al intentar detener grabación previa: {ex.Message}");
                }
            }

            var backCamera = cameraView.Cameras[0];
            cameraView.Camera = backCamera;
            CameraVisible = true;
            await cameraView.StartCameraAsync();
            await Task.Delay(1000);
            string fileName = Path.Combine(FileSystem.CacheDirectory, $"video_{DateTime.Now:yyyyMMdd_HHmmss}.mp4");
            _fileName = fileName;
            _currentCameraView = cameraView;
            IsRecording = true;
            IsLoading = false;
            LoadingText = "";
            // Inicia la grabación
            await cameraView.StartRecordingAsync(fileName, new Size(1280, 720));

            // Graba por 15 segundos si no hay botón de detener
            await Task.Delay(15000, _grabacionCts.Token);
            CameraVisible = false;
            IsLoading = true;
            LoadingText = "Guardando video";
            // Detiene la grabación al finalizar
            await DetenerGrabacion();
            IsLoading = false;
            LoadingText = "";
        }
        catch(TaskCanceledException ex) {
            Console.WriteLine("Grabación cancelada antes de tiempo: " + ex.Message);
        }

        catch (Exception ex)
        {
            _pathVideo = "";
            VideoMediaSource = null;
            Debug.WriteLine($"Error al grabar video: {ex.Message}");
            IsLoading = false;
            LoadingText = "";
            await CapturarVideoIos();
        }
        finally
        {
            IsLoading = false;
            LoadingText = "";
        }
#endif
    }

    [RelayCommand]
    async Task DetenerGrabacion()
    {
        IsLoading = true;
        LoadingText = "Guardando ...";
        try
        {
            if (_currentCameraView == null || !IsRecording)
                return;

            var recordedFile = await _currentCameraView.StopRecordingAsync();
            await Task.Delay(500);
            _grabacionCts?.Cancel();
            _pathVideo = _fileName;
            CameraVisible = false;
            if (File.Exists(_pathVideo))
            {
                Console.WriteLine("El video existe en el dispositivo");
            }

            VideoMediaSource = FileMediaSource.FromFile(_pathVideo);
            IsLoading = false;
            LoadingText = "";
            _grabacionCts?.Dispose();
            _grabacionCts = null;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error al detener grabación: {ex.Message}");
            _pathVideo = "";
            VideoMediaSource = null;
            IsLoading = false;
            LoadingText = "";
        }
        finally
        {
            IsRecording = false;

            // Muy importante: detener la cámara después de grabar
            if (_currentCameraView != null)
            {
                await _currentCameraView.StopCameraAsync();
                _currentCameraView = null;
            }

            IsLoading = false;
            LoadingText = "";
        }
        IsLoading = false;
        LoadingText = "";
    }

    private async Task ReiniciarCamara(CameraView cameraView)
    {
        if (_currentCameraView != null)
        {
            try { await _currentCameraView.StopRecordingAsync(); } catch { }
            try { await _currentCameraView.StopCameraAsync(); } catch { }
        }

        _currentCameraView = cameraView;
        await _currentCameraView.StartCameraAsync();
    }


    [RelayCommand]
    async Task CapturarVideoIos()
    {
        try
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                if (await PopupUtil.HasCameraPermissions())
                {
                    FileResult? fileResult = await MediaPicker.CaptureVideoAsync();

                    if (fileResult != null)
                    {
                        _pathVideo = Path.Combine(FileSystem.CacheDirectory, fileResult.FileName);

                        using (Stream stream = await fileResult.OpenReadAsync())
                        {
                            using FileStream localFileStream = File.OpenWrite(_pathVideo);
                            await stream.CopyToAsync(localFileStream);
                        }
                        VideoMediaSource = FileMediaSource.FromFile(_pathVideo);

                        IsLoading = true;
                        LoadingText = Constants.VALIDAR_VIDEO;
                        await Task.Delay(2000);
                        LoadingText = "";
                        IsLoading = false;

                        int seconds = 16;
                        if (_mediaElement.Duration > TimeSpan.FromSeconds(seconds))
                        {
                            await App.Current.MainPage.DisplayAlert("", $"La duración del video debe ser menor de {seconds} segundos", Constants.ACEPTAR);
                            _pathVideo = null;
                            VideoMediaSource = null;
                            return;
                        }
                    }
                }
            }
        }
        catch (Exception)
        {
            VideoMediaSource = null;
        }
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query) {
        if(query.ContainsKey(Constants.SUPERVISION_REQUEST_DATA_KEY)) {
            _supervisionRequestData = (SupervisionRequestDataModel)query[Constants.SUPERVISION_REQUEST_DATA_KEY];
            PageTitle = $"Capturar Evidencia";
            InitValues();

            query.Remove(Constants.SUPERVISION_REQUEST_DATA_KEY);
        }
    }

    async void InitValues() {
        VideoMediaSource = null;
    }
    [RelayCommand]
    void RemoveVideo() {
        _pathVideo = null;
        VideoMediaSource = null;
    }
}