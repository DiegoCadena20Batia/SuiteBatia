using BatiaSuite.Models;
using BatiaSuite.Models.SupervisionMantenimiento.Operarios;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;

namespace BatiaSuite.ViewModel.SupervisionMantenimiento.Operarios {
    [QueryProperty(nameof(Seccion), "SeccionSeleccionada")]
    [QueryProperty(nameof(IdOrden), "IdOrden")]
    public partial class PreguntasSeccionViewModel : ObservableObject {

        [ObservableProperty]
        private SeccionModel _seccion;

        [ObservableProperty]
        private int _idOrden;

        [ObservableProperty]
        private ObservableCollection<PhotosModel> _photoPaths = new();

        private const int MaxPhotos = 5;

        private int CountPhoto = 0;

        // Guardar o confirmar respuestas de esta sección
        [RelayCommand]
        private async Task GuardarSeccionAsync() {
            if(Seccion == null) return;

            // Validamos si hay preguntas sin responder
            int sinResponder = Seccion.Preguntas.Count(p => !p.EstaRespondida);

            if(sinResponder > 0) {
                bool continuar = await Shell.Current.DisplayAlert(
                    "Preguntas pendientes",
                    $"Tienes {sinResponder} pregunta(s) sin responder en esta sección. ¿Deseas regresar de todos modos?",
                    "Sí, regresar",
                    "Cancelar");

                if(!continuar) return;
            }

            // Regresamos a la pantalla de secciones
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private void MarcarBueno(PreguntaModel pregunta) {
            if(pregunta == null) return;
            pregunta.Respuesta = "Bueno";
            Seccion?.NotificarCambioProgreso();
        }

        [RelayCommand]
        private void MarcarMalo(PreguntaModel pregunta) {
            if(pregunta == null) return;
            pregunta.Respuesta = "Malo";
            Seccion?.NotificarCambioProgreso();
        }

        [RelayCommand]
        private void MarcarNA(PreguntaModel pregunta) {
            if(pregunta == null) return;
            pregunta.Respuesta = "N/A";
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

       

        [RelayCommand]
        private void DeletePhoto(PhotosModel elemento) {
            PhotoPaths.Remove(elemento);
            CountPhoto--;
        }
    }
}