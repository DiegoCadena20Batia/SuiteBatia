using BatiaSuite.Models.SupervisionMantenimiento;
using BatiaSuite.Models.SupervisionMantenimiento.Operarios;
using BatiaSuite.Services.SupervisionesMantenimiento;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Graphics.Platform;
using System.Collections.ObjectModel;

namespace BatiaSuite.ViewModel.SupervisionMantenimiento.Operarios {

    public partial class PreguntasSeccionViewModel : ObservableObject {
        private SupervisionStateService _stateService;
        private const int MAX_FOTOS_POR_SECCION = 5;

        #region Propiedades de Navegación

        [ObservableProperty]
        private ObservableCollection<FotoModel> _photoPaths = new();

        [ObservableProperty]
        private SeccionModel _seccion;
        [ObservableProperty]
        private IteracionModel _iteracion;
        [ObservableProperty]
        private PisoModel _piso;
        [ObservableProperty]
        private int _countPhoto;

        [ObservableProperty]
        private bool _isBusy;
        [ObservableProperty]
        private bool _esBueno;
        [ObservableProperty]
        private bool _esMalo;
        [ObservableProperty]
        private bool _esNA;

        #endregion Propiedades de Navegación

        public PreguntasSeccionViewModel(SupervisionStateService stateService) {
            _stateService = stateService;

            Seccion = _stateService.SeccionActual;
            Iteracion = _stateService.IteracionActual;
            Piso = _stateService.PisoActual;

            CargarDatosSeccion();

        }

        private void CargarDatosSeccion() {
            IsBusy = true;

            try {
                PhotoPaths.Clear();
                if(Iteracion.Fotos != null && Iteracion.Fotos.Any()) {
                    foreach(var foto in Iteracion.Fotos) {
                        PhotoPaths.Add(foto);
                    }
                }

               
                if(Seccion.Preguntas == null) {
                    Seccion.Preguntas = new ObservableCollection<PreguntaModel>();
                }
            } catch(Exception ex) {
                System.Diagnostics.Debug.WriteLine($"Error al cargar preguntas de la sección: {ex.Message}");
            } finally {
                IsBusy = false;
            }
        }

        #region Asignacion de Respuestas
        [RelayCommand]
        private void MarcarBueno(PreguntaModel pregunta) {
            pregunta?.SeleccionarRespuestaCommand.Execute("2");
        }
        [RelayCommand]
        private void MarcarMalo(PreguntaModel pregunta) {
            pregunta?.SeleccionarRespuestaCommand.Execute("1");
        }
        [RelayCommand]
        private void MarcarNA(PreguntaModel pregunta) {
            pregunta?.SeleccionarRespuestaCommand.Execute("0");
        }

        #endregion

        #region Comandos de Gestión de Fotos (Máximo 5 por Sección)

        [RelayCommand]
        private async Task Photo() {
            try {
                if(CountPhoto < 5) {
                    FileResult photo = await MediaPicker.CapturePhotoAsync();
                    if(photo != null) {
                        string localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                        // Usa File.Create en lugar de File.OpenWrite para liberar y cerrar el Stream correctamente
                        using(Stream source = await photo.OpenReadAsync())
                        using(FileStream localFile = File.Create(localFilePath)) {
                            await source.CopyToAsync(localFile);
                        }

                        // Agrega a la PROPIEDAD PÚBLICA (PhotoPaths), no al campo privado (_photoPaths)
                        PhotoPaths.Add(new FotoModel {
                            LocalPath = localFilePath
                        });

                        CountPhoto++;
                    }
                } else {
                    await Shell.Current.DisplayAlert("Mensaje", "Se ha alcanzado el número máximo de fotos permitidas", "Cerrar");
                }
            } catch(Exception ex) {
                await Shell.Current.DisplayAlert("Error", ex.Message, "Cerrar");
            }
        }

        //private async Task PhotoAsync() {
        //    if(PhotoPaths.Count >= MAX_FOTOS_POR_SECCION) {
        //        await Shell.Current.DisplayAlert("Límite Alcanzado", $"Solo se permiten un máximo de {MAX_FOTOS_POR_SECCION} fotografías de evidencia por sección.", "Aceptar");
        //        return;
        //    }

        //    try {
        //        if(!MediaPicker.Default.IsCaptureSupported) {
        //            await Shell.Current.DisplayAlert("Cámara no disponible", "Este dispositivo no permite capturar fotografías.", "Aceptar");
        //            return;
        //        }

        //        FileResult? photo = await MediaPicker.Default.CapturePhotoAsync(new MediaPickerOptions {
        //            Title = $"Evidencia_{_stateService.SeccionActual.Seccion}_{PhotoPaths.Count + 1}"
        //        });

        //        if(photo != null) {
        //            string extension = Path.GetExtension(photo.FileName);
        //            if(string.IsNullOrWhiteSpace(extension)) extension = ".jpg";
        //            string fileName = $"{Guid.NewGuid()}{extension}";
        //            string localFilePath = Path.Combine(FileSystem.CacheDirectory, fileName);

        //            using(Stream sourceStream = await photo.OpenReadAsync())
        //            using(Microsoft.Maui.Graphics.IImage originalImage = PlatformImage.FromStream(sourceStream))
        //            using(Microsoft.Maui.Graphics.IImage resizedImage = originalImage.Downsize(1600, disposeOriginal: false))
        //            using(Stream resizedStream = resizedImage.AsStream(ImageFormat.Jpeg))
        //            using(FileStream localFileStream = File.Create(localFilePath)) {
        //                await resizedStream.CopyToAsync(localFileStream);
        //            }
        //            System.Diagnostics.Debug.WriteLine($"Path: {localFilePath}");
        //            System.Diagnostics.Debug.WriteLine($"Existe: {File.Exists(localFilePath)}, Tamaño: {new FileInfo(localFilePath).Length} bytes");
        //            MainThread.BeginInvokeOnMainThread(() =>
        //            {
        //                PhotoPaths.Add(new FotoModel { LocalPath = localFilePath });
        //            });
        //        }
        //    } catch(PermissionException) {
        //        await Shell.Current.DisplayAlert("Permisos denegados", "Se requieren permisos de cámara para continuar.", "Aceptar");
        //    } catch(Exception ex) {
        //        System.Diagnostics.Debug.WriteLine($"Error al tomar foto: {ex.Message}");
        //    }
        //}

        [RelayCommand]
        private void DeletePhoto(FotoModel foto) {
            if(foto == null) return;

            PhotoPaths.Remove(foto);
            CountPhoto--;
        }

        #endregion Comandos de Gestión de Fotos (Máximo 5 por Sección)

        #region Guardar y Finalizar

        [RelayCommand]
        private async Task GuardarSeccionAsync() {
            int preguntasSinResponder = _stateService.IteracionActual.Preguntas?.Count(p => !p.EstaRespondida) ?? 0;

            if(preguntasSinResponder > 0) {
                bool salir = await Shell.Current.DisplayAlert(
                    "Preguntas Pendientes",
                    $"Faltan {preguntasSinResponder} preguntas por responder en esta sección. ¿Deseas salir de todos modos?",
                    "Sí, Salir",
                    "Permanecer");

                if(!salir) return;
            }

            // Asignar la lista final de fotos de la iteración al modelo de la iteración
            _stateService.IteracionActual.Fotos = PhotoPaths.ToList();


            await Shell.Current.GoToAsync("..");
        }

        #endregion Guardar y Finalizar
    }
}