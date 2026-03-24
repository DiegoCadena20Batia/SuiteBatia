using BatiaSuite.Models.SupervisionMantenimiento;
using BatiaSuite.Services;
using BatiaSuite.Utils;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;

namespace BatiaSuite.ViewModel.SupervisionMantenimiento;

public partial class SupervisionMantenimientoExtintoresObjectViewModel : ViewModelBase, IQueryAttributable {
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
    public int _extintoresGuardados;


    [ObservableProperty]
    public string _seccion;
    [ObservableProperty]
    public string _comentarios;
    [ObservableProperty]
    public string _fotoPath;

    [ObservableProperty]
    public bool _esEdita;

    [ObservableProperty]
    public int _editaConsec;

    [ObservableProperty]
    public bool _isVisible;

    [ObservableProperty]
    public string _agregaroEditarExtintor;

    public event Action? ScrollPreguntasToTop;



    public ObservableCollection<SupervisionMantenimientoExtintoresPreguntasModel> PreguntasList { get; set; } = new();
    public ObservableCollection<SupervisionMantenimientoExtintoresObjectModel> ExtintoresList { get; set; } = new();

    public ICommand SelectAplicaCommand { get; }
    public ICommand SelectNoAplicaCommand { get; }
    public ICommand SelectNACommand { get; }

    [ObservableProperty]
    ObservableCollection<string> _photoList;

    int _indice, _filesMaxNum = 1;
    public SupervisionMantenimientoExtintoresObjectViewModel(SupervisionMantenimientoService supervisionService) {
        _supervisionService = supervisionService;
        PreguntasList = new ObservableCollection<SupervisionMantenimientoExtintoresPreguntasModel>();

        SelectAplicaCommand = new Command<SupervisionMantenimientoExtintoresPreguntasModel>(
            (pregunta) => SelectOption(pregunta, 1));

        SelectNoAplicaCommand = new Command<SupervisionMantenimientoExtintoresPreguntasModel>(
            (pregunta) => SelectOption(pregunta, 2));

        SelectNACommand = new Command<SupervisionMantenimientoExtintoresPreguntasModel>(
            (pregunta) => SelectOption(pregunta, 3));
        //ContarExtintoresGuardados();
        PhotoList = new ObservableCollection<string>();
        ObtenerExtintoresGuardados();
        _isVisible = false;
    }

    private void SelectOption(SupervisionMantenimientoExtintoresPreguntasModel pregunta, int valor) {

        pregunta.Valor = valor;
    }

    public bool GetPreguntasBySeccion() {
        try {
            var preguntas = _supervisionService.GetExtintoresPreguntas();
            foreach (var cl in preguntas) {
                cl.Valor = 0;
                cl.Comentarios = "";
            }
            PreguntasList = new ObservableCollection<SupervisionMantenimientoExtintoresPreguntasModel>(preguntas);
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
        if(IdSeccion > 0) {
            GetPreguntasBySeccion();
        }
    }

    [RelayCommand]
    public async Task GuardarySalir() {
        if(!await ValidaPreguntas()) {
            await Toast.Make("Responda todos los puntos porfavor.", ToastDuration.Short).Show();
            return;
        }
        IsLoading = true;

        try {
            _supervisionService.MarcarSeccionTerminada(IdSeccion);
            var preguntas = _supervisionService.GetExtintoresPreguntas();
            var respuestas = PreguntasList.ToList();
            PreguntasList.Clear();
            PreguntasList = new ObservableCollection<SupervisionMantenimientoExtintoresPreguntasModel>(preguntas);
            _supervisionService.GuardarRespuestaExtintor(respuestas, Comentarios, FotoPath);
            await Shell.Current.GoToAsync("..");
            await Toast.Make("Registro Guardado", ToastDuration.Short).Show();
            await Task.Delay(1500);
        } catch(Exception ex) {
        } finally {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task GuardaryRegistrarNuevo() {
        if(!EsEdita) {
            IsLoading = true;
            if(!await ValidaPreguntas()) {
                await Toast.Make("Responda todos los puntos porfavor.", ToastDuration.Short).Show();
                IsLoading = false;
                return;
            }
            IsLoading = true;

            try {
                _supervisionService.MarcarSeccionTerminada(IdSeccion);
                var preguntas = _supervisionService.GetExtintoresPreguntas();
                var respuestas = PreguntasList.ToList();
                PreguntasList.Clear();
                PreguntasList = new ObservableCollection<SupervisionMantenimientoExtintoresPreguntasModel>(preguntas);
                _supervisionService.GuardarRespuestaExtintor(respuestas, Comentarios, FotoPath);
                await Task.Delay(1500);
                PhotoList = new ObservableCollection<string>();
                ExtintoresGuardados = _supervisionService.ContarExtintoresGuardados();
                await Toast.Make("Registro Guardado", ToastDuration.Short).Show();

            } catch(Exception ex) {
                await Toast.Make(ex.Message, ToastDuration.Long).Show();
            } finally {
                IsLoading = false;
            }
        } else {

            IsLoading = true;
            try {
                var respuestas = PreguntasList.ToList();
                PreguntasList.Clear();
                //GENERAR MODELO PARA GUARDAR EL EXTINTOR

                var list = new List<SupervisionMantenimientoExtintoresObject>();

                foreach (var pre in respuestas) {
                    var prenew = new SupervisionMantenimientoExtintoresObject {
                        IdPregunta = pre.IdPregunta,
                        Pregunta = pre.Pregunta,
                        Estado = pre.Valor,
                        Comentarios = pre.Comentarios
                    };
                    list.Add(prenew);
                }
                var ext = new SupervisionMantenimientoExtintoresObjectModel {
                    IdConsec = EditaConsec,
                    ComentarioGeneral = Comentarios,
                    FotoPath = FotoPath,
                    Respuestas = list
                };
                var preguntas = _supervisionService.ActualizarExtintor(ext);
                PreguntasList = new ObservableCollection<SupervisionMantenimientoExtintoresPreguntasModel>();
                //_supervisionService.ActualizarRespuestaExtintor(respuestas, Comentarios, FotoPath);
                await Task.Delay(1500);
                PhotoList = new ObservableCollection<string>();
                ExtintoresGuardados = _supervisionService.ContarExtintoresGuardados();
                await Toast.Make("Registro Actualizado", ToastDuration.Short).Show();
                EsEdita = false;
            } catch(Exception ex) {
                await Toast.Make(ex.Message, ToastDuration.Long).Show();
            } finally {
                IsLoading = false;
            }
        }
        ObtenerExtintoresGuardados();
        GetPreguntasBySeccion();
        IsVisible = false;
        EsEdita = false;

    }

    public async Task<bool> ValidaPreguntas() {
        try {
            foreach(var pre in PreguntasList) {
                if(pre.Valor == 0) {
                    return false;
                }
            }
            return true;
        } catch(Exception ex) {
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
                        PhotoList.Add(localFilePath);
                        FotoPath = localFilePath;
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
        PhotoList = new ObservableCollection<string>();
        FotoPath = "";
        //var foto = new SupervisionMantenimientoFotosSeccionModel {
        //    IdSeccion = IdSeccion,
        //    FotoPath = filePath
        //};
        //_supervisionService.EliminarFotoSeccion(foto);
        //CargarFotosGuardadasPorSeccion();

    }

    public bool ContarExtintoresGuardados() {
        try {
            ExtintoresGuardados = _supervisionService.ContarExtintoresGuardados();
            return true;
        } catch(Exception ex) {
            Console.WriteLine("Error:" + ex.Message);
            return false;
        }
    }

    public void ObtenerExtintoresGuardados() {
        try {
            IsLoading = true;
            var extintores = _supervisionService.ObtenerExtintoresGuardados();
            ExtintoresList = new ObservableCollection<SupervisionMantenimientoExtintoresObjectModel>(extintores);
            IsLoading = false;
        }
        catch (Exception ex) {
            IsLoading = false;
            Console.WriteLine(ex.Message);
        }
       
    }
    [RelayCommand]
    public void VerExtintor(SupervisionMantenimientoExtintoresObjectModel extintor) {
        try {
            if (extintor.IsSelected == true) {
                extintor.IsSelected = false;
                IsVisible = false;
                return;
            }
            AgregaroEditarExtintor = "Editar extintor " + extintor.IdConsec;
            foreach (var reset in ExtintoresList) {
                reset.IsSelected = false;
            }
            IsLoading = true;
            extintor.IsSelected = true;

            var ext = _supervisionService.GetExtintorById(extintor.IdConsec);

            var list = new List<SupervisionMantenimientoExtintoresPreguntasModel>();

            foreach(var pre in ext.Respuestas) {
                var prenew = new SupervisionMantenimientoExtintoresPreguntasModel {
                    IdPregunta = pre.IdPregunta,
                    Pregunta = pre.Pregunta,
                    Valor = pre.Estado,
                    Comentarios = pre.Comentarios
                };
                EditaConsec = ext.IdConsec;
                list.Add(prenew);
                    }

            PhotoList = new ObservableCollection<string>();

            if(!string.IsNullOrWhiteSpace(ext.FotoPath)) {
                FotoPath = ext.FotoPath;
                PhotoList.Add(ext.FotoPath);
            }
                
            
            PreguntasList = new ObservableCollection<SupervisionMantenimientoExtintoresPreguntasModel>(list);
            IsVisible = true;
            EsEdita = true;
            IsLoading = false;
            ScrollPreguntasToTop?.Invoke();
        }
        catch (Exception ex) {
            IsLoading = false;
            Console.WriteLine(ex.Message);
        }
    }

    [RelayCommand]
    public void CleanForAddNew() {
        ScrollPreguntasToTop?.Invoke();
        AgregaroEditarExtintor = "Agregar extintor";
        foreach(var reset in ExtintoresList) {
            reset.IsSelected = false;
        }
        IsLoading = true;
        PreguntasList.Clear();
        GetPreguntasBySeccion();
        Comentarios = "";
        FotoPath = "";
        PhotoList = new ObservableCollection<string>();
        IsVisible = true;
        EsEdita = false;
        IsLoading = false;
    }

}