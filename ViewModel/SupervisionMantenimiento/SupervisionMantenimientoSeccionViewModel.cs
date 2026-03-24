using BatiaSuite.Models.SupervisionMantenimiento;
using BatiaSuite.Services;
using BatiaSuite.Utils;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;

namespace BatiaSuite.ViewModel.SupervisionMantenimiento;

public partial class SupervisionMantenimientoSeccionViewModel : ViewModelBase, IQueryAttributable {
    private readonly SupervisionMantenimientoService _supervisionService;
    private SupervisionMantenimientoSeccionesModel _currentSection;

    [ObservableProperty]
    private string _tituloSeccion;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _textLoading = "Cargando...";

    [ObservableProperty]
    public int _idSeccion;


    [ObservableProperty]
    public string _seccion;

    public ObservableCollection<SupervisionMantenimientoPreguntasModel> Preguntas { get; set; }

    public ICommand SelectAplicaCommand { get; }
    public ICommand SelectNoAplicaCommand { get; }
    public ICommand SelectNACommand { get; }

    [ObservableProperty]
    ObservableCollection<string> _photoList;

    int _indice, _filesMaxNum = 5;
    public SupervisionMantenimientoSeccionViewModel(SupervisionMantenimientoService supervisionService) {
        _supervisionService = supervisionService;
        Preguntas = new ObservableCollection<SupervisionMantenimientoPreguntasModel>();

        SelectAplicaCommand = new Command<SupervisionMantenimientoPreguntasModel>(
            (pregunta) => SelectOption(pregunta, 1));

        SelectNoAplicaCommand = new Command<SupervisionMantenimientoPreguntasModel>(
            (pregunta) => SelectOption(pregunta, 2));

        SelectNACommand = new Command<SupervisionMantenimientoPreguntasModel>(
            (pregunta) => SelectOption(pregunta, 3));
        PhotoList = new ObservableCollection<string>();
        
    }

    private void SelectOption(SupervisionMantenimientoPreguntasModel pregunta, int valor) {

        pregunta.Estado = valor;
        //Console.WriteLine($"=== SelectOption ===");
        //Console.WriteLine($"Pregunta ID: {pregunta.IdPregunta}");
        //Console.WriteLine($"Valor seleccionado: {valor}");
        //Console.WriteLine($"Estado anterior: {pregunta.Estado}");

        //var index = Preguntas.IndexOf(pregunta);
        //if(index >= 0) {
        //    // Método que SI funciona para forzar DataTriggers
        //    // 1. Crear una COPIA completa con el nuevo estado
        //    var nuevaPregunta = new SupervisionMantenimientoPreguntasModel {
        //        IdSeccion = pregunta.IdSeccion,
        //        IdPregunta = pregunta.IdPregunta,
        //        Pregunta = pregunta.Pregunta,
        //        Estado = valor,  // NUEVO valor
        //        Comentarios = pregunta.Comentarios
        //    };

        //    // 2. Reemplazar en la colección usando Remove/Insert
        //    Preguntas.RemoveAt(index);
        //    Preguntas.Insert(index, nuevaPregunta);

        //    Console.WriteLine($"UI actualizada - Nuevo objeto insertado en índice {index}");
        //    Console.WriteLine($"Nuevo estado verificado: {Preguntas[index].Estado}");
        //} else {
        //    Console.WriteLine($"ERROR: Pregunta no encontrada");
        //}

        //Console.WriteLine($"=== SelectOption completado ===");
    }

    public bool GetPreguntasBySeccion() {
        try {
            var preguntasList = _supervisionService.GetPreguntasBySeccion(IdSeccion);
            Preguntas = new ObservableCollection<SupervisionMantenimientoPreguntasModel>(preguntasList);
            return true;
        } catch(Exception ex) {
            return false;
        }
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query) {
        if(query.ContainsKey("idseccion") && query.ContainsKey("seccion")) {
            IdSeccion = (int)query["idseccion"];
            Seccion = (string)query["seccion"];
        }
        if (IdSeccion > 0) {
            GetPreguntasBySeccion();
            CargarFotosGuardadasPorSeccion();
        }
    }

    [RelayCommand]
    public async Task Guardar() {
        if(!await ValidaPreguntas()) {
            await Toast.Make("Responda todos los puntos porfavor.", ToastDuration.Short).Show();
            return;
        }
        IsLoading = true;

        try {
             _supervisionService.GuardarRespuestasPorSeccion(Preguntas.ToList());
            _supervisionService.MarcarSeccionTerminada(IdSeccion);
            await Task.Delay(1500);
            await Shell.Current.GoToAsync("..");
            // opcional: marcar sección como terminada
        } catch(Exception ex) {
            // manejar error
        } finally {
            IsLoading = false;
        }
    }

    public async Task<bool> ValidaPreguntas() {
        try {
            foreach (var pre in Preguntas) {
                if( pre.Estado == 0) {
                    return false;
                }
            }
            return true;
        }
        catch(Exception ex) {
            await Toast.Make(ex.Message, ToastDuration.Long).Show();
            return false;
            
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
                        var foto = new SupervisionMantenimientoFotosSeccionModel {
                            IdSeccion = IdSeccion,
                            FotoPath = localFilePath
                            
                        };
                        _supervisionService.GuardarFotoSeccion(foto);
                        CargarFotosGuardadasPorSeccion();
                    }
                }
            }
        } catch(Exception ex) {
            Console.WriteLine("Error:" + ex.Message);
        }
    }
    public void CargarFotosGuardadasPorSeccion() {
        //PhotoList.Clear(); 
        PhotoList = new ObservableCollection<string>(
            _supervisionService.ObtenerFotosPorSeccion(IdSeccion).Select(x => x.FotoPath));
    }

    [RelayCommand]
    public void RemovePhoto(string filePath) {
        var foto = new SupervisionMantenimientoFotosSeccionModel {
            IdSeccion = IdSeccion,
            FotoPath = filePath
        };
        _supervisionService.EliminarFotoSeccion(foto);
        CargarFotosGuardadasPorSeccion();

    }
}