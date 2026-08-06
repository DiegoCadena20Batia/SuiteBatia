using BatiaSuite.Models;
using BatiaSuite.Models.OrdenesTrabajo;
using BatiaSuite.Models.SupervisionMantenimiento.Operarios;
using BatiaSuite.Services;
using BatiaSuite.Services.SupervisionesMantenimiento;
using BatiaSuite.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BatiaSuite.ViewModel.SupervisionMantenimiento.Operarios {

    [QueryProperty(nameof(Seccion), "SeccionSeleccionada")]
    [QueryProperty(nameof(Orden), "Orden")]
    public partial class PreguntasSeccionViewModel : ObservableObject {
        private readonly HttpHelper _httpHelper;

        private readonly SupervisionMantenimientoStateService _supervisionService;

        [ObservableProperty]
        private SeccionModel _seccion;

        [ObservableProperty]
        private OrdenTrabajoModel _orden;

        [ObservableProperty]
        private int _idSupervision;

        [ObservableProperty]
        private bool _isSubiendoFotos;

        [ObservableProperty]
        private ObservableCollection<PhotosModel> _photoPaths = new();

        private const int MaxPhotos = 5;

        private int CountPhoto = 0;

        private string baseUrlApi = Constants.API_BASE_URL;

        public PreguntasSeccionViewModel(HttpHelper httpHelper, SupervisionMantenimientoStateService supervisionMantenimientoService) {
            _httpHelper = httpHelper;
            _supervisionService = supervisionMantenimientoService;
        }

         partial void OnSeccionChanged(SeccionModel value) {
            if(value == null) return;

            // Limpiamos la colección para la nueva sección cargada
            PhotoPaths.Clear();

            // Verificamos si ya existen fotos guardadas previamente para esta sección en el servicio
            if(_supervisionService.FotosPorSeccion.TryGetValue(value.IdSeccion, out var fotosGuardadas)) {
                foreach(var foto in fotosGuardadas) {
                    // Reconstruimos la lista bindable para la interfaz gráfica
                    PhotoPaths.Add(new PhotosModel {
                        UrlPhoto = foto.LocalPath
                    });
                }
            }
        }

        // Guardar o confirmar respuestas de esta sección
        [RelayCommand]
        private async Task GuardarSeccionAsync() {
            if(Seccion == null) return;

            int sinResponder = Seccion.Preguntas.Count(p => !p.EstaRespondida);
            if(sinResponder > 0) {
                bool continuar = await Shell.Current.DisplayAlert(
                    "Preguntas pendientes",
                    $"Tienes {sinResponder} pregunta(s) sin responder en esta sección. ¿Deseas regresar de todos modos?",
                    "Sí, regresar",
                    "Cancelar");
                if(!continuar) return;
            }

            try {
                IsSubiendoFotos = true;

                if(_supervisionService.IdSupervisionActual <= 0) {
                    int idNuevo = await CrearCabeceraSiNoExisteAsync();
                    if(idNuevo <= 0) {
                        await Shell.Current.DisplayAlert("Error", "No se pudo iniciar la supervisión.", "OK");
                        return;
                    }
                    _supervisionService.IdSupervisionActual = idNuevo;
                }

                // Refs existentes de esta sección (con su estado de "Subida")
                _supervisionService.FotosPorSeccion.TryGetValue(Seccion.IdSeccion, out var refsExistentes);
                refsExistentes ??= new List<FotoSeccionEstado>();

                // Rutas válidas actualmente en pantalla
                var rutasActuales = PhotoPaths
                    .Where(f => !string.IsNullOrEmpty(f.UrlPhoto) && File.Exists(f.UrlPhoto))
                    .Select(f => f.UrlPhoto)
                    .ToList();

                // Solo las que NO estaban ya registradas como subidas
                var rutasNuevas = rutasActuales
                    .Where(r => !refsExistentes.Any(e => e.LocalPath == r && e.Subida))
                    .ToList();

                if(rutasNuevas.Any()) {
                    bool ok = await SubirFotosDeSeccionAsync(_supervisionService.IdSupervisionActual, Seccion.IdSeccion, rutasNuevas);
                    if(!ok) {
                        await Shell.Current.DisplayAlert("Aviso", "No se pudieron subir las fotos nuevas de esta sección. Se reintentará más adelante.", "OK");
                    }

                    // Actualiza/agrega refs SOLO de las nuevas, marcadas según resultado
                    foreach(var ruta in rutasNuevas) {
                        var existente = refsExistentes.FirstOrDefault(e => e.LocalPath == ruta);
                        if(existente != null) {
                            existente.Subida = ok;
                        } else {
                            refsExistentes.Add(new FotoSeccionEstado { IdSeccion = Seccion.IdSeccion, LocalPath = ruta, Subida = ok });
                        }
                    }
                }

                _supervisionService.FotosPorSeccion[Seccion.IdSeccion] = refsExistentes;

                await Shell.Current.GoToAsync("..");
            } finally {
                IsSubiendoFotos = false;
            }
        }

        private async Task<int> CrearCabeceraSiNoExisteAsync() {
            var payloadCabecera = new SupervisionMantenimientoDTO {
                IdPersonal = UserSession.IdPersonal,
                IdOrden = Orden?.idOrden ?? 0,
                IdCliente = Orden?.idCliente ?? 0,
                IdInmueble = Orden?.idInmueble ?? 0,
                Fechainicio = _supervisionService.FechaInicio,
                Fechafin = DateTime.Now, // se sobreescribe al final, en el cierre real
                Observaciones = "",
                Latitud = Orden.latitud,
                Longitud = Orden.longitud
            };

            string url = $"{baseUrlApi}SupervisionMantenimiento/cabecera";
            return await _httpHelper.PostBodyAsync<SupervisionMantenimientoDTO, int>(url, payloadCabecera);
        }

        private async Task<bool> SubirFotosDeSeccionAsync(int idSupervision, int idSeccion, List<string> rutas) {
            try {
                using var content = new MultipartFormDataContent();
                content.Add(new StringContent(idSeccion.ToString()), "idSeccion");

                // Streams abiertos directo desde disco — nunca se cargan todos en memoria a la vez
                var streams = new List<FileStream>();
                try {
                    foreach(var ruta in rutas) {
                        var stream = File.OpenRead(ruta);
                        streams.Add(stream);
                        var fileContent = new StreamContent(stream);
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                        content.Add(fileContent, "fotos", Path.GetFileName(ruta));
                    }

                    string url = $"{baseUrlApi}SupervisionMantenimiento/{idSupervision}/fotos";
                    return await _httpHelper.PostMultipartAsync(url, content);
                } finally {
                    foreach(var s in streams) s.Dispose();
                }
            } catch {
                return false;
            }
        }

        [RelayCommand]
        private void MarcarBueno(PreguntaModel pregunta) {
            if(pregunta == null) return;
            pregunta.Respuesta = 2;
            Seccion?.NotificarCambioProgreso();
        }

        [RelayCommand]
        private void MarcarMalo(PreguntaModel pregunta) {
            if(pregunta == null) return;
            pregunta.Respuesta = 1;
            Seccion?.NotificarCambioProgreso();
        }

        [RelayCommand]
        private void MarcarNA(PreguntaModel pregunta) {
            if(pregunta == null) return;
            pregunta.Respuesta = 0;
            Seccion?.NotificarCambioProgreso();
        }

        [RelayCommand]
        private async Task PhotoAsync() {
            try {
                if(PhotoPaths.Count >= MaxPhotos) {
                    await Shell.Current.DisplayAlert("Mensaje", $"Se ha alcanzado el número máximo de fotos permitidas ({MaxPhotos})", "Cerrar");
                    return;
                }

                FileResult photo = await MediaPicker.Default.CapturePhotoAsync();
                if(photo == null) return;

                string localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                using(Stream source = await photo.OpenReadAsync())
                using(FileStream localFile = File.Create(localFilePath)) {
                    await source.CopyToAsync(localFile);
                }

                PhotoPaths.Add(new PhotosModel {
                    UrlPhoto = localFilePath
                });
            } catch(Exception ex) {
                await Shell.Current.DisplayAlert("Error", ex.Message, "Cerrar");
            }
        }

        private async Task<List<FotoSeccionEstado>> ObtenerFotosSeccionAsync(int idSeccion) {
            var listaFotos = new List<FotoSeccionEstado>();

            foreach(var foto in PhotoPaths) {
                if(!string.IsNullOrEmpty(foto.UrlPhoto) && File.Exists(foto.UrlPhoto)) {
                    // Leemos la imagen almacenada en CacheDirectory como arreglo de bytes
                    byte[] bytes = await File.ReadAllBytesAsync(foto.UrlPhoto);

                    listaFotos.Add(new FotoSeccionEstado {
                        IdSeccion = idSeccion,

                        LocalPath = foto.UrlPhoto
                    });
                }
            }

            return listaFotos;
        }

        [RelayCommand]
        private void DeletePhoto(PhotosModel elemento) {
            PhotoPaths.Remove(elemento);
            CountPhoto--;
        }
    }
}