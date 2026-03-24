using BatiaSuite.Models.CheckListAparadores;
using BatiaSuite.Models.Supervision;
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
using System.Text.Json.Serialization;
using System.Windows.Input;
namespace BatiaSuite.ViewModel.CheckListAparadores;

public partial class CheckListAparadoresPreguntasResumenViewModel : ViewModelBase {
    [ObservableProperty]
    bool _isLoading;

    [ObservableProperty]
    string _textLoading;

    private readonly CheckListService _checkListService;
    public ObservableCollection<SeccionGrupo> ListaResumen { get; set; }
    public ObservableCollection<CheckListPreguntasModel> Preguntas { get; set; }
        = new ObservableCollection<CheckListPreguntasModel>();

    // Propiedades para almacenar las firmas como byte[]
    [ObservableProperty]
    private byte[] _firmaGerenteBytes;

    [ObservableProperty]
    private byte[] _firmaAparadoristaBytes;

    [ObservableProperty]
    private byte[] _firmaEncargadoBytes;

    [ObservableProperty]
    private byte[] _firmaAuditorBytes;

    [ObservableProperty]
    string _nombreAparadorista;

    [ObservableProperty]
    string _nombreEncargado;

    [ObservableProperty]
    string _comentariosFinales;

    private DrawingView _gerenteDrawingView;
    private DrawingView _aparadoristaDrawingView;
    private DrawingView _encargadoDrawingView;
    private DrawingView _auditorDrawingView;
    [ObservableProperty]
    ObservableCollection<string> _photoList;

    List<CheckListFoto> listaFotosByte = new List<CheckListFoto>();

    public CheckListAparadoresPreguntasResumenViewModel(CheckListService checkListService) {
        _checkListService = checkListService;
        ObservableCollection<string> _photoList;

        CargarResumen();
    }

    public ICommand GoBackCommand => new Command(async () => {
        await Shell.Current.GoToAsync("..");
    });

    private void CargarResumen() {
        var total = _checkListService.GetTotalCheckList();

        ListaResumen = new ObservableCollection<SeccionGrupo>
        {
            new SeccionGrupo("Vestir", total.Where(x => x.IdSeccion == 1)),
            new SeccionGrupo("Casual", total.Where(x => x.IdSeccion == 2)),
            new SeccionGrupo("Accesorios", total.Where(x => x.IdSeccion == 3)),
            new SeccionGrupo("Estado de la tienda (inicio)", total.Where(x => x.IdSeccion == 4)),
            new SeccionGrupo("Estado al finalizar", total.Where(x => x.IdSeccion == 5)),
        };
        PhotoList = new ObservableCollection<string>(
           _checkListService.GetAllFotos().Select(x => x.Path));

    }

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
    private async Task SaveGerenteSignature() {
        if(_gerenteDrawingView != null && TieneContenido(_gerenteDrawingView)) {
            try {
                var stream = await _gerenteDrawingView.GetImageStream(300, 300);
                if(stream != null) {
                    FirmaGerenteBytes = await StreamToBytes(stream);
                    await Toast.Make("Firma de Gerente guardada", ToastDuration.Short).Show();
                }
            } catch(Exception ex) {
                await App.Current.MainPage.DisplayAlert("Error", "No se pudo guardar la firma del Gerente", "OK");
            }
        } else {
            FirmaGerenteBytes = null;
            Console.WriteLine("ℹ️ No hay firma de Gerente para guardar");
        }
    }

    [RelayCommand]
    private async Task SaveAparadoristaSignature() {
        if(_aparadoristaDrawingView != null && TieneContenido(_aparadoristaDrawingView)) {
            try {
                var stream = await _aparadoristaDrawingView.GetImageStream(300, 300);
                if(stream != null) {
                    FirmaAparadoristaBytes = await StreamToBytes(stream);
                    await Toast.Make("Firma de Aparadorista guardada", ToastDuration.Short).Show();
                }
            } catch(Exception ex) {
                await App.Current.MainPage.DisplayAlert("Error", "No se pudo guardar la firma del Aparadorista", "OK");
            }
        } else {
            FirmaAparadoristaBytes = null;
            Console.WriteLine("ℹ️ No hay firma de Aparadorista para guardar");
        }
    }

    [RelayCommand]
    private async Task SaveEncargadoSignature() {
        if(_encargadoDrawingView != null && TieneContenido(_encargadoDrawingView)) {
            try {
                var stream = await _encargadoDrawingView.GetImageStream(300, 300);
                if(stream != null) {
                    FirmaEncargadoBytes = await StreamToBytes(stream);
                    await Toast.Make("Firma de Encargado guardada", ToastDuration.Short).Show();
                }
            } catch(Exception ex) {
                await App.Current.MainPage.DisplayAlert("Error", "No se pudo guardar la firma del Encargado", "OK");
            }
        } else {
            FirmaEncargadoBytes = null;
            Console.WriteLine("ℹ️ No hay firma de Encargado para guardar");
        }
    }

    [RelayCommand]
    private async Task SaveAuditorSignature() {
        if(_auditorDrawingView != null && TieneContenido(_auditorDrawingView)) {
            try {
                var stream = await _auditorDrawingView.GetImageStream(300, 300);
                if(stream != null) {
                    FirmaAuditorBytes = await StreamToBytes(stream);
                    await Toast.Make("Firma de Auditor guardada", ToastDuration.Short).Show();
                }
            } catch(Exception ex) {
                await App.Current.MainPage.DisplayAlert("Error", "No se pudo guardar la firma del Auditor", "OK");
            }
        } else {
            FirmaAuditorBytes = null;
            Console.WriteLine("ℹ️ No hay firma de Auditor para guardar");
        }
    }

    // 🔥 MÉTODO MEJORADO PARA GUARDAR TODAS LAS FIRMAS
    public async Task GuardarFirmas() {
        try {
            Console.WriteLine("💾 Guardando firmas automáticamente...");

            await SaveGerenteSignature();
            await SaveAparadoristaSignature();
            await SaveEncargadoSignature();
            await SaveAuditorSignature();

            Console.WriteLine("✅ Proceso de guardado de firmas completado");
        } catch(Exception ex) {
            Console.WriteLine($"❌ Error en GuardarFirmas: {ex.Message}");
        }
    }

    // COMANDOS PARA LIMPIAR FIRMAS
    [RelayCommand]
    private void ClearGerenteSignature() {
        _gerenteDrawingView?.Clear();
        FirmaGerenteBytes = null;
    }

    [RelayCommand]
    private void ClearAparadoristaSignature() {
        _aparadoristaDrawingView?.Clear();
        FirmaAparadoristaBytes = null;
    }

    [RelayCommand]
    private void ClearEncargadoSignature() {
        _encargadoDrawingView?.Clear();
        FirmaEncargadoBytes = null;
    }

    [RelayCommand]
    private void ClearAuditorSignature() {
        _auditorDrawingView?.Clear();
        FirmaAuditorBytes = null;
    }

    // 🔥 VALIDAR SI HAY AL MENOS UNA FIRMA (EN LUGAR DE TODAS)
    private bool ValidarFirmasMinimas() {
        var firmasConContenido = new List<byte[]> {
        FirmaAparadoristaBytes,
        FirmaGerenteBytes,
        FirmaEncargadoBytes,
        FirmaAuditorBytes
    }.Where(firma => firma != null && firma.Length > 0).ToList();

        Console.WriteLine($"📊 Firmas con contenido: {firmasConContenido.Count}");
        return firmasConContenido.Count > 0;
    }

    [RelayCommand]
    async Task EnviarRegistroCompleto() {
        try {
            // 🔥 PRIMERO: Convertir los valores booleanos a enteros
            await ConvertirValoresRespuesta();

            IsLoading = true;
            TextLoading = "Guardando firmas...";

            // 🔥 GUARDAR FIRMAS AUTOMÁTICAMENTE
            await GuardarFirmas();

            // 🔥 VALIDAR QUE HAYA AL MENOS UNA FIRMA
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

            //  Armar el modelo a enviar al backend
            var modelo = new CheckListAparadoresCompletoModel {
                IdCliente = UserSession.IdClienteCheckList,
                IdInmueble = UserSession.IdInmuebleCheckList,
                IdTecnico = UserSession.IdPersonal,
                Finicio = DateTime.Now,
                Ffin = DateTime.Now,
                NAparadorista = NombreAparadorista,
                NEncargadoT = NombreEncargado,
                Comentarios = ComentariosFinales,
                Latitud = latitud,
                Longitud = longitud,
                Preguntas = _checkListService.GetTotalCheckList(),

                Firmas = new Firmas {
                    FirmaAparadorista = FirmaAparadoristaBytes,
                    FirmaGerente = FirmaGerenteBytes,
                    FirmaAuditor = FirmaAuditorBytes,
                    FirmaEncargado = FirmaEncargadoBytes
                },
                Fotos = listaFotosByte
            };

            // 🔥 Enviar al API
            bool envioExitoso = await EnviarCheckListAPI(modelo);

            if(envioExitoso) {
                await App.Current.MainPage.DisplayAlert("Éxito", "Checklist enviado correctamente", "Aceptar");

                // Limpiar después del envío exitoso
                LimpiarFirmas();
                UserSession.IdClienteCheckList = 0;
                UserSession.IdInmuebleCheckList = 0;

                // Navegar al menú principal
                await Shell.Current.GoToAsync("//MyMenu");
            } else {
                await App.Current.MainPage.DisplayAlert("Error", "No se pudo enviar el checklist", "OK");
            }

        } catch(Exception ex) {
            await App.Current.MainPage.DisplayAlert("Error", $"Error al enviar: {ex.Message}", "OK");
        } finally {
            IsLoading = false;
        }
    }

    // 🔥 Método para enviar a la API
    private async Task<bool> EnviarCheckListAPI(CheckListAparadoresCompletoModel modelo) {
        try {
            Uri requestUri = new Uri(Constants.API_BASE_URL + "CheklistEnvio");

            // Serializar el modelo a JSON
            var json = System.Text.Json.JsonSerializer.Serialize(modelo);
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

    public async Task ConvertirValoresRespuesta() {
        try {
            var preguntasAct = _checkListService.GetTotalCheckList();

            foreach(var pre in preguntasAct) {
                if(pre.IdSeccion == 4 || pre.IdSeccion == 5) {
                    // Convertir bool a int: true → 1, false → 0
                    pre.Valor1 = pre.Valor3 ? 1 : 0;

                    // Actualizar en el servicio
                    _checkListService.UpdatePregunta(pre);

                    Console.WriteLine($"✅ Convertido - Sección: {pre.IdSeccion}, Pregunta: {pre.IdPregunta}, Valor3: {pre.Valor3} → Valor1: {pre.Valor1}");
                }
            }

            // Verificar la conversión
            //var preguntasConvertidas = _checkListService.GetTotalCheckList()
            //    .Where(x => x.IdSeccion == 4 || x.IdSeccion == 5)
            //    .ToList();

            //Console.WriteLine($"🔄 Se convirtieron {preguntasConvertidas.Count} preguntas de las secciones 4 y 5");
        } catch(Exception ex) {
            Console.WriteLine($"❌ Error en ConvertirValoresRespuesta: {ex.Message}");
            throw;
        }
    }

    private bool ValidarFirmasGuardadas() {
        return FirmaGerenteBytes != null &&
               FirmaAparadoristaBytes != null &&
               FirmaEncargadoBytes != null &&
               FirmaAuditorBytes != null;
    }

    private void LimpiarFirmas() {
        // Limpiar los bytes
        FirmaGerenteBytes = null;
        FirmaAparadoristaBytes = null;
        FirmaEncargadoBytes = null;
        FirmaAuditorBytes = null;

        // Limpiar los DrawingViews
        _gerenteDrawingView?.Clear();
        _aparadoristaDrawingView?.Clear();
        _encargadoDrawingView?.Clear();
        _auditorDrawingView?.Clear();
    }

    private async Task<byte[]> StreamToBytes(Stream stream) {
        if(stream == null)
            return null;

        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        return ms.ToArray();
    }

    public void SetDrawingViews(
        DrawingView gerenteDrawingView,
        DrawingView aparadoristaDrawingView,
        DrawingView encargadoDrawingView,
        DrawingView auditorDrawingView) {
        _gerenteDrawingView = gerenteDrawingView;
        _aparadoristaDrawingView = aparadoristaDrawingView;
        _encargadoDrawingView = encargadoDrawingView;
        _auditorDrawingView = auditorDrawingView;
    }

    public async Task ConvertirFotosaBytes() {
        //var listaFotosByte = new List<CheckListFoto>();
        var listaFotosService = _checkListService.GetAllFotos();
        if(listaFotosService != null && listaFotosService.Count > 0) {
            foreach(var photo in listaFotosService) {
                byte[] fileBytesArray = File.ReadAllBytes(photo.Path);
                byte[] resizedImage = await ImageResizerHelperSuiteBatia.ResizeImage(fileBytesArray, 640, 480);
#if ANDROID
                resizedImage = ImageResizerHelperSuiteBatia.ForceVertical(resizedImage);

#endif
                listaFotosByte.Add(new CheckListFoto {
                    IdSeccion = photo.IdSeccion,
                    Path = photo.Path,
                    FotoBytes = resizedImage
                });
            }
        }


    }
}

public class SeccionGrupo : List<CheckListPreguntasModel> {
    public string Titulo { get; set; }

    public SeccionGrupo(string titulo, IEnumerable<CheckListPreguntasModel> preguntas)
        : base(preguntas) {
        Titulo = titulo;
    }
}

public class CheckListAparadoresCompletoModel {
    public int IdCliente { get; set; }
    public int IdInmueble { get; set; }
    public int IdTecnico { get; set; }
    public DateTime Finicio { get; set; }
    public DateTime Ffin { get; set; }
    public string NAparadorista { get; set; }
    public string NEncargadoT { get; set; }
    public string Latitud { get; set; }
    public string Longitud { get; set; }
    public string Comentarios { get; set; }
    public List<CheckListPreguntasModel> Preguntas { get; set; }
    public Firmas Firmas { get; set; }
    public List<CheckListFoto> Fotos { get; set; }
}

public class Firmas {
    public byte[] FirmaAparadorista { get; set; }
    public byte[] FirmaGerente { get; set; }
    public byte[] FirmaAuditor { get; set; }
    public byte[] FirmaEncargado { get; set; }
}
public class CheckListFoto {
    public int IdSeccion { get; set; }
    [JsonIgnore]
    public string Path { get; set; }
    public byte[] FotoBytes { get; set; }
}