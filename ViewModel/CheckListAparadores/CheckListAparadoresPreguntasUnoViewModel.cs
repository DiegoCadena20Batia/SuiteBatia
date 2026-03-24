
using BatiaSuite.Models.CheckListAparadores;
using BatiaSuite.Services;
using BatiaSuite.Utils;
using BatiaSuite.Views.CheckListAparadores;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace BatiaSuite.ViewModel.CheckListAparadores;

public partial class CheckListAparadoresPreguntasUnoViewModel : ViewModelBase, IQueryAttributable {


    [ObservableProperty]
    bool _isLoading;

    [ObservableProperty]
    string _textLoading;

    private readonly CheckListService _checkListService;

    [ObservableProperty]
    ObservableCollection<string> _photoList;

    int _indice, _filesMaxNum = 5;
    public ObservableCollection<CheckListPreguntasModel> Preguntas { get; set; }
        = new ObservableCollection<CheckListPreguntasModel>();

    public CheckListAparadoresPreguntasUnoViewModel(CheckListService checkListService) {
        _checkListService = checkListService;
        var items = _checkListService.GetPreguntasBySeccion(1);
        foreach(var item in items) {
            Preguntas.Add(item);
        }
        PhotoList = new ObservableCollection<string>();
        CargarFotosGuardadasPorSeccion();
        

    }
    [RelayCommand]
    async Task Continuar() {
        IsLoading = true;
        // Aquí no hacemos nada más porque las respuestas YA quedaron guardadas
        // dentro del mismo objeto en CheckListService
        
        await Constants.GoToAsync(nameof(CheckListAparadoresPreguntasDosPage));
        IsLoading = false;
    }


    public async void ApplyQueryAttributes(IDictionary<string, object> query) {
        IsLoading = true;
        //_checkListService.LimpiarDatos();
        IsLoading = false;
        TextLoading = "";
    }
    public ICommand GoBackCommand => new Command(async () => {
        var result = await Shell.Current.DisplayAlert(
            "¿Salir del checklist?",
            "Al salir perderás toda la información ingresada.\n\n¿Confirmas que quieres salir?",
            "Salir",
            "Cancelar"
        ); if(result) {
            await Shell.Current.GoToAsync("..");
        }
    });

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
                        var foto = new CheckListFoto {
                           IdSeccion = 1,
                            Path = localFilePath
                        };
                        _checkListService.GuardarFotoSeccion(foto);
                        CargarFotosGuardadasPorSeccion();
                    }
                }
            }
        } catch(Exception ex) {
            Console.WriteLine("Error:" + ex.Message);
        }
    }
    public void CargarFotosGuardadasPorSeccion() {
        PhotoList.Clear();
        PhotoList = new ObservableCollection<string>(
            _checkListService.ObtenerFotosPorSeccion(1).Select(x => x.Path));
    }

    [RelayCommand]
    public void RemovePhoto(string filePath) {
        var foto = new CheckListFoto {
            IdSeccion = 1,
            Path = filePath
        };
        _checkListService.EliminarFotoSeccion(foto);
        CargarFotosGuardadasPorSeccion();

    }
}