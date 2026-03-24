using BatiaSuite.Models.CheckListAparadores;
using BatiaSuite.Models.Supervision;
using BatiaSuite.Models.SupervisionMantenimiento;
using BatiaSuite.Services;
using BatiaSuite.Utils;
using BatiaSuite.Views.Supervision;
using BatiaSuite.Views.SupervisionMantenimiento;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Input;
namespace BatiaSuite.ViewModel.SupervisionMantenimiento;

public partial class SupervisionMantenimientoFirmasViewModel : ViewModelBase {
    [ObservableProperty]
    bool _isLoading;

    [ObservableProperty]
    string _textLoading;

    //private readonly CheckListService _checkListService;
    //public ObservableCollection<SeccionGrupo> ListaResumen { get; set; }
    //public ObservableCollection<CheckListPreguntasModel> Preguntas { get; set; }
    //    = new ObservableCollection<CheckListPreguntasModel>();

    private readonly SupervisionMantenimientoService _supervisionService;

    // Propiedades para almacenar las firmas como byte[]
    [ObservableProperty]
    private byte[] _firmaAdministracionEntranteBytes;
    [ObservableProperty]
    private byte[] _firmaAdministracionSalienteBytes;
    [ObservableProperty]
    private byte[] _firmaTestigoUnoBytes;
    [ObservableProperty]
    private byte[] _firmaTestigoDosBytes;
    [ObservableProperty]
    private byte[] _firmaTestigoTresBytes;
    [ObservableProperty]
    private byte[] _firmaTestigoCuatroBytes;


    [ObservableProperty]
    string _nombreAdministracionEntrante;
    [ObservableProperty]
    string _nombreAdministracionSaliente;
    [ObservableProperty]
    string _nombreTestigoUno;
    [ObservableProperty]
    string _nombreTestigoDos;
    [ObservableProperty]
    string _nombreTestigoTres;
    [ObservableProperty]
    string _nombreTestigoCuatro;

    [ObservableProperty]
    string _comentariosFinales;

    private DrawingView _administracionEntranteDrawingView;
    private DrawingView _administracionSalienteDrawingView;
    private DrawingView _testigoUnoDrawingView;
    private DrawingView _testigoDosDrawingView;
    private DrawingView _testigoTresDrawingView;
    private DrawingView _testigoCuatroDrawingView;
    //[ObservableProperty]
    //ObservableCollection<string> _photoList;

    //List<CheckListFoto> listaFotosByte = new List<CheckListFoto>();

    public SupervisionMantenimientoFirmasViewModel(SupervisionMantenimientoService supervisionService) {
        _supervisionService = supervisionService;
        //ObservableCollection<string> _photoList;

        //CargarResumen();
    }

    public ICommand GoBackCommand => new Command(async () => {
        await Shell.Current.GoToAsync("..");
    });

    //private void CargarResumen() {
    //    var total = _checkListService.GetTotalCheckList();

    //    ListaResumen = new ObservableCollection<SeccionGrupo>
    //    {
    //        new SeccionGrupo("Vestir", total.Where(x => x.IdSeccion == 1)),
    //        new SeccionGrupo("Casual", total.Where(x => x.IdSeccion == 2)),
    //        new SeccionGrupo("Accesorios", total.Where(x => x.IdSeccion == 3)),
    //        new SeccionGrupo("Estado de la tienda (inicio)", total.Where(x => x.IdSeccion == 4)),
    //        new SeccionGrupo("Estado al finalizar", total.Where(x => x.IdSeccion == 5)),
    //    };
    //    PhotoList = new ObservableCollection<string>(
    //       _checkListService.GetAllFotos().Select(x => x.Path));

    //}

    // 🔥 MÉTODO PARA VERIFICAR SI HAY CONTENIDO
    private bool TieneContenido(DrawingView drawingView) {
        try {
            return drawingView?.Lines != null && drawingView.Lines.Count > 0;
        } catch {
            return false;
        }
    }

    // COMANDOS PARA GUARDAR FIRMAS CON VALIDACIÓN
    [RelayCommand]
    private async Task SaveAdministracionEntranteSignature() {
        if(_administracionEntranteDrawingView != null && TieneContenido(_administracionEntranteDrawingView)) {
            try {
                var stream = await _administracionEntranteDrawingView.GetImageStream(300, 300);
                if(stream != null) {
                    FirmaAdministracionEntranteBytes = await StreamToBytes(stream);
                }
            } catch(Exception ex) {
                await App.Current.MainPage.DisplayAlert("Error", "No se pudo guardar la firma de Administracion Entrante", "OK");
            }
        } else {
            FirmaAdministracionEntranteBytes = null;
            Console.WriteLine("No hay firma de Administracion Entrante para guardar");
        }
    }

    [RelayCommand]
    private async Task SaveAdministracionSalienteSignature() {
        if(_administracionSalienteDrawingView != null && TieneContenido(_administracionSalienteDrawingView)) {
            try {
                var stream = await _administracionSalienteDrawingView.GetImageStream(300, 300);
                if(stream != null) {
                    FirmaAdministracionSalienteBytes = await StreamToBytes(stream);
                }
            } catch(Exception ex) {
                await App.Current.MainPage.DisplayAlert("Error", "No se pudo guardar la firma de Administracion Saliente", "OK");
            }
        } else {
            FirmaAdministracionSalienteBytes = null;
            Console.WriteLine("No hay firma de Administracion Saliente para guardar");
        }
    }

    [RelayCommand]
    private async Task SaveTestigoUnoSignature() {
        if(_testigoUnoDrawingView != null && TieneContenido(_testigoUnoDrawingView)) {
            try {
                var stream = await _testigoUnoDrawingView.GetImageStream(300, 300);
                if(stream != null) {
                    FirmaTestigoUnoBytes = await StreamToBytes(stream);
                }
            } catch(Exception ex) {
                await App.Current.MainPage.DisplayAlert("Error", "No se pudo guardar la firma de Testigo Uno", "OK");
            }
        } else {
            FirmaTestigoUnoBytes = null;
            Console.WriteLine("No hay firma de testigo uno para guardar");
        }
    }

    [RelayCommand]
    private async Task SaveTestigoDosSignature() {
        if(_testigoDosDrawingView != null && TieneContenido(_testigoDosDrawingView)) {
            try {
                var stream = await _testigoDosDrawingView.GetImageStream(300, 300);
                if(stream != null) {
                    FirmaTestigoDosBytes = await StreamToBytes(stream);
                }
            } catch(Exception ex) {
                await App.Current.MainPage.DisplayAlert("Error", "No se pudo guardar la firma de Testigo Dos", "OK");
            }
        } else {
            FirmaTestigoDosBytes = null;
            Console.WriteLine("No hay firma de testigo dos para guardar");
        }
    }

    [RelayCommand]
    private async Task SaveTestigoTresSignature() {
        if(_testigoTresDrawingView != null && TieneContenido(_testigoTresDrawingView)) {
            try {
                var stream = await _testigoTresDrawingView.GetImageStream(300, 300);
                if(stream != null) {
                    FirmaTestigoTresBytes = await StreamToBytes(stream);
                }
            } catch(Exception ex) {
                await App.Current.MainPage.DisplayAlert("Error", "No se pudo guardar la firma de Testigo Tres", "OK");
            }
        } else {
            FirmaTestigoTresBytes = null;
            Console.WriteLine("No hay firma de testigo tres para guardar");
        }
    }

    [RelayCommand]
    private async Task SaveTestigoCuatroSignature() {
        if(_testigoCuatroDrawingView != null && TieneContenido(_testigoCuatroDrawingView)) {
            try {
                var stream = await _testigoCuatroDrawingView.GetImageStream(300, 300);
                if(stream != null) {
                    FirmaTestigoCuatroBytes = await StreamToBytes(stream);
                }
            } catch(Exception ex) {
                await App.Current.MainPage.DisplayAlert("Error", "No se pudo guardar la firma de Testigo Cuatro", "OK");
            }
        } else {
            FirmaTestigoCuatroBytes = null;
            Console.WriteLine("No hay firma de testigo cuatro para guardar");
        }
    }

    public async Task GuardarFirmas() {
        try {
            await SaveAdministracionEntranteSignature();
            await SaveAdministracionSalienteSignature();
            await SaveTestigoUnoSignature();
            await SaveTestigoDosSignature();
            await SaveTestigoTresSignature();
            await SaveTestigoCuatroSignature();
            await Toast.Make("Firmas guardadas correctamente", ToastDuration.Short).Show();
        } catch(Exception ex) {
            await Toast.Make(ex.Message, ToastDuration.Short).Show();

        }
    }

    // COMANDOS PARA LIMPIAR FIRMAS
    [RelayCommand]
    private void ClearAdministracionEntranteSignature() {
        _administracionEntranteDrawingView?.Clear();
        FirmaAdministracionEntranteBytes = null;
    }

    [RelayCommand]
    private void ClearAdministracionSalienteSignature() {
        _administracionSalienteDrawingView?.Clear();
        FirmaAdministracionSalienteBytes = null;
    }

    [RelayCommand]
    private void ClearTestigoUnoSignature() {
        _testigoUnoDrawingView?.Clear();
        FirmaTestigoUnoBytes = null;
    }

    [RelayCommand]
    private void ClearTestigoDosSignature() {
        _testigoDosDrawingView?.Clear();
        FirmaTestigoDosBytes = null;
    }
    [RelayCommand]
    private void ClearTestigoTresSignature() {
        _testigoTresDrawingView?.Clear();
        FirmaTestigoTresBytes = null;
    }
    [RelayCommand]
    private void ClearTestigoCuatroSignature() {
        _testigoCuatroDrawingView?.Clear();
        FirmaTestigoCuatroBytes = null;
    }

    private bool ValidarFirmasMinimas() {
        var firmasConContenido = new List<byte[]> {
        FirmaAdministracionEntranteBytes,
        FirmaAdministracionSalienteBytes,
        FirmaTestigoUnoBytes,
        FirmaTestigoDosBytes,
        FirmaTestigoTresBytes,
        FirmaTestigoCuatroBytes
    }.Where(firma => firma != null && firma.Length > 0).ToList();
        return firmasConContenido.Count > 0;
    }

    

    private bool ValidarFirmasGuardadas() {
        return FirmaAdministracionEntranteBytes != null &&
               FirmaAdministracionSalienteBytes != null &&
               FirmaTestigoUnoBytes != null &&
               FirmaTestigoDosBytes != null &&
               FirmaTestigoTresBytes != null &&
               FirmaTestigoCuatroBytes != null;
    }

    private void LimpiarFirmas() {
        // Limpiar los bytes
        FirmaAdministracionEntranteBytes = null;
        FirmaAdministracionSalienteBytes = null;
        FirmaTestigoUnoBytes = null;
        FirmaTestigoDosBytes = null;
        FirmaTestigoTresBytes = null;
        FirmaTestigoCuatroBytes = null;

        // Limpiar los DrawingViews
        _administracionEntranteDrawingView?.Clear();
        _administracionSalienteDrawingView?.Clear();
        _testigoUnoDrawingView?.Clear();
        _testigoDosDrawingView?.Clear();
        _testigoTresDrawingView?.Clear();
        _testigoCuatroDrawingView?.Clear();
    }

    private async Task<byte[]> StreamToBytes(Stream stream) {
        if(stream == null)
            return null;

        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        return ms.ToArray();
    }

    public void SetDrawingViews(
        DrawingView administracionEntranteDrawingView,
        DrawingView administracionSalienteDrawingView,
        DrawingView testigoUnoDrawingView,
        DrawingView testigoDosDrawingView,
        DrawingView testigoTresDrawingView,
        DrawingView testigoCuatroDrawingView) {
        _administracionEntranteDrawingView = administracionEntranteDrawingView;
        _administracionSalienteDrawingView = administracionSalienteDrawingView;
        _testigoUnoDrawingView = testigoUnoDrawingView;
        _testigoDosDrawingView = testigoDosDrawingView;
        _testigoTresDrawingView = testigoTresDrawingView;
        _testigoCuatroDrawingView = testigoCuatroDrawingView;
    }

    public async Task ConvertirFotosaBytes() {
        //var listaFotosByte = new List<CheckListFoto>();
        //var listaFotosService = _checkListService.GetAllFotos();
        //var listaFotosService = 
        


    }

    [RelayCommand]
    async Task EnviarRegistroCompleto() {
        try {
           // await ConvertirValoresRespuesta();

            IsLoading = true;
            TextLoading = "Guardando firmas...";

            await GuardarFirmas();

            if(!ValidarFirmasMinimas()) {
                await App.Current.MainPage.DisplayAlert("Advertencia",
                    "Es necesario contar con al menos una firma para enviar el registro", "OK");
                return;
            }

            TextLoading = "Enviando checklist...";

            var location = await Utils.LocationUtil.GetCurrentLocationAsync();
            var latitud = "";
            var longitud = "";
            if(location != null) {
                latitud = location.Latitude.ToString();
                longitud = location.Longitude.ToString();
            }
            await ConvertirFotosaBytes();

            var modelSupervision = _supervisionService.GetSupervisionModel();
            if (modelSupervision != null) {
                modelSupervision.Latitud = latitud;
                modelSupervision.Longitud = longitud;
                modelSupervision.Observaciones = ComentariosFinales;
                modelSupervision.IdPersonal = UserSession.IdPersonal;
                modelSupervision.FechaFin = DateTime.Now;
            }

            //ENVIAR SOLO PREGUNTAS DE SECCIONES TERMINADAS
            if(modelSupervision != null && modelSupervision.Preguntas != null && modelSupervision.Preguntas.Count > 0 && modelSupervision.Secciones != null) {
                var seccionesTerminadasIds = modelSupervision.Secciones
                    .Where(s => s.Terminada)
                    .Select(s => s.IdSeccion)
                    .ToHashSet();

                modelSupervision.Preguntas = modelSupervision.Preguntas
                    .Where(p => seccionesTerminadasIds.Contains(p.IdSeccion))
                    .ToList();
            }

            //VALIDAR Y AGREGAR FIRMAS AL MODELO

            if(FirmaAdministracionEntranteBytes?.Length > 0) {
                var firmaEntrante = new FirmaSupervisionMantenimientoModel {
                    IdFirma = 1,
                    FirmaBytes = FirmaAdministracionEntranteBytes,
                    Nombre = NombreAdministracionEntrante
                };

                modelSupervision?.FirmasBytes?.Add(firmaEntrante);
            }
            if(FirmaAdministracionSalienteBytes?.Length > 0) {
                var firmaSaliente = new FirmaSupervisionMantenimientoModel {
                    IdFirma = 2,
                    FirmaBytes = FirmaAdministracionSalienteBytes,
                    Nombre = NombreAdministracionSaliente
                };

                modelSupervision?.FirmasBytes?.Add(firmaSaliente);
            }
            if(FirmaTestigoUnoBytes?.Length > 0) {
                var firmaTestigoUno = new FirmaSupervisionMantenimientoModel {
                    IdFirma = 3,
                    FirmaBytes = FirmaTestigoUnoBytes,
                    Nombre = NombreTestigoUno
                };

                modelSupervision?.FirmasBytes?.Add(firmaTestigoUno);
            }
            if(FirmaTestigoDosBytes?.Length > 0) {
                var firmaTestigoDos = new FirmaSupervisionMantenimientoModel {
                    IdFirma = 4,
                    FirmaBytes = FirmaTestigoDosBytes,
                    Nombre = NombreTestigoDos
                };

                modelSupervision?.FirmasBytes?.Add(firmaTestigoDos);
            }
            if(FirmaTestigoTresBytes?.Length > 0) {
                var firmaTestigoTres = new FirmaSupervisionMantenimientoModel {
                    IdFirma = 5,
                    FirmaBytes = FirmaTestigoTresBytes,
                    Nombre = NombreTestigoTres
                };

                modelSupervision?.FirmasBytes?.Add(firmaTestigoTres);
            }
            if(FirmaTestigoCuatroBytes?.Length > 0) {
                var firmaTestigoCuatro = new FirmaSupervisionMantenimientoModel {
                    IdFirma = 6,
                    FirmaBytes = FirmaTestigoCuatroBytes,
                    Nombre = NombreTestigoCuatro
                };

                modelSupervision?.FirmasBytes?.Add(firmaTestigoCuatro);
            }

            //CONVERTIR FOTOS DE SECCIONES A BYTES Y AGREGAR AL MODELO
            if(modelSupervision != null && modelSupervision.FotosSeccion != null && modelSupervision.FotosSeccion.Count > 0) {
                foreach(var foto in modelSupervision.FotosSeccion) {
                    if(foto != null && foto.FotoPath != null && foto.FotoPath != "") {
                        byte[] fileBytesArray = File.ReadAllBytes(foto.FotoPath);
                        byte[] resizedImage = await ImageResizerHelperSuiteBatia.ResizeImage(fileBytesArray, 640,360);
#if ANDROID
                        resizedImage = ImageResizerHelperSuiteBatia.ForceVertical(resizedImage);
#endif
                        foto.FotoBytes = resizedImage;
                    }
                }
            }
            //CONVERTIR FOTOS DE HIDRANTES Y ASPERSORES A BYTES Y AGREGAR AL MODELO
            if(modelSupervision != null && modelSupervision.HidrantesyAspersoresObjects != null && modelSupervision.HidrantesyAspersoresObjects.Count > 0) {
                foreach(var obj in modelSupervision.HidrantesyAspersoresObjects) {
                    if(obj != null && obj.FotoPath != null && obj.FotoPath != "") {
                        byte[] fileBytesArray = File.ReadAllBytes(obj.FotoPath);
                        byte[] resizedImage = await ImageResizerHelperSuiteBatia.ResizeImage(fileBytesArray, 640,360);
#if ANDROID
                        resizedImage = ImageResizerHelperSuiteBatia.ForceVertical(resizedImage);
#endif
                        obj.FotoBytes = resizedImage;
                    }
                }
            }
            //CONVERTIR FOTOS DE EXTINTORES A BYTES Y AGREGAR AL MODELO
            if(modelSupervision != null && modelSupervision.ExtintoresObjects != null && modelSupervision.ExtintoresObjects.Count > 0) {
                foreach(var obj in modelSupervision.ExtintoresObjects) {
                    if(obj != null && obj.FotoPath != null && obj.FotoPath != "") {
                        byte[] fileBytesArray = File.ReadAllBytes(obj.FotoPath);
                        byte[] resizedImage = await ImageResizerHelperSuiteBatia.ResizeImage(fileBytesArray, 640,360);
#if ANDROID
                        resizedImage = ImageResizerHelperSuiteBatia.ForceVertical(resizedImage);
#endif
                        obj.FotoBytes = resizedImage;
                    }
                }
            }

            bool envioExitoso = await EnviarCheckListAPI(modelSupervision);

            if(envioExitoso) {
                await App.Current.MainPage.DisplayAlert("Éxito", "Supervisión enviada correctamente", "Aceptar");

                LimpiarFirmas();
                //UserSession.IdClienteCheckList = 0;
                //UserSession.IdInmuebleCheckList = 0;

                await Shell.Current.GoToAsync("//MyMenu");
            } else {
                await App.Current.MainPage.DisplayAlert("Error", "No se pudo enviar la Supervisión", "OK");
            }

        } catch(Exception ex) {
            await App.Current.MainPage.DisplayAlert("Error", $"Error al enviar: {ex.Message}", "OK");
        } finally {
            IsLoading = false;
        }
    }

     //Enviar a la API
    private async Task<bool> EnviarCheckListAPI(SupervisionMantenimientoModel modelo) {
        try {
            Uri requestUri = new Uri(Constants.API_BASE_URL + "SupervisionMantenimiento");

            // Serializar el modelo a JSON
            var options = new JsonSerializerOptions {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            var json = System.Text.Json.JsonSerializer.Serialize(modelo,options);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            using var client = new HttpClient();

            // Configurar timeout (opcional)
            client.Timeout = TimeSpan.FromSeconds(30);

            var response = await client.PostAsync(requestUri, content);

            if(response.IsSuccessStatusCode) {
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"✅ Respuesta del servidor: {responseContent}");
                return true;
            } else {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ Error del servidor: {response.StatusCode} - {errorContent}");
                return false;
            }
        } catch(HttpRequestException httpEx) {
            Console.WriteLine($"❌ Error de conexión: {httpEx.Message}");
            return false;
        } catch(TaskCanceledException) {
            Console.WriteLine("❌ Timeout de la petición");
            return false;
        } catch(Exception ex) {
            Console.WriteLine($"❌ Error inesperado: {ex.Message}");
            return false;
        }
    }

    //public async Task ConvertirValoresRespuesta() {
    //    try {
    //        var preguntasAct = _checkListService.GetTotalCheckList();

    //        foreach(var pre in preguntasAct) {
    //            if(pre.IdSeccion == 4 || pre.IdSeccion == 5) {
    //                // Convertir bool a int: true → 1, false → 0
    //                pre.Valor1 = pre.Valor3 ? 1 : 0;

    //                // Actualizar en el servicio
    //                _checkListService.UpdatePregunta(pre);

    //                Console.WriteLine($"✅ Convertido - Sección: {pre.IdSeccion}, Pregunta: {pre.IdPregunta}, Valor3: {pre.Valor3} → Valor1: {pre.Valor1}");
    //            }
    //        }

    //        // Verificar la conversión
    //        //var preguntasConvertidas = _checkListService.GetTotalCheckList()
    //        //    .Where(x => x.IdSeccion == 4 || x.IdSeccion == 5)
    //        //    .ToList();

    //        //Console.WriteLine($"🔄 Se convirtieron {preguntasConvertidas.Count} preguntas de las secciones 4 y 5");
    //    } catch(Exception ex) {
    //        Console.WriteLine($"❌ Error en ConvertirValoresRespuesta: {ex.Message}");
    //        throw;
    //    }
    //}
}