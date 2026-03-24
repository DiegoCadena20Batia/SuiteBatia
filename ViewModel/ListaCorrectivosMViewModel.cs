using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BatiaSuite.Models;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows.Input;
using BatiaSuite.Views;
using System.Globalization;






namespace BatiaSuite.ViewModel
{
    public class ListaCorrectivosMViewModel : BaseViewModel, IQueryAttributable
    {

        int idclave;

        string tipo;

        string fecha;

        string detalle;

        //private ObservableCollection<ListCorrecM> listApps;

        //public ObservableCollection<ListCorrecM> ListApps
        //{
        //    get { return listApps; }
        //    set { listApps = value; OnPropertyChanged(); }
        //}
        private string _tipos;
        public string Tipos
        {
            get { return _tipos; }
            set { _tipos = value; OnPropertyChanged(); }
        }

        private string _detalles;

        private string _fechas;
        public string Fechas
        {
            get { return _fechas; }
            set { _fechas = value; OnPropertyChanged(); }
        }

        //public string Cliente
        //{
        //    get { return _cliente; }
        //    set { _cliente = value; OnPropertyChanged(); }
        //}
        //private string _cliente;

        //public string Inmueble
        //{
        //    get { return _inmueble; }
        //    set { _inmueble = value; OnPropertyChanged(); }
        //}
        //private string _inmueble;

        public string Detalles
        {
            get { return _detalles; }
            set { _detalles = value; OnPropertyChanged(); }
        }

        private ObservableCollection<PhotosModel> _photoPaths = new ObservableCollection<PhotosModel>();
        public ObservableCollection<PhotosModel> photoPaths
        {
            get { return _photoPaths; }
            set
            {
                _photoPaths = value;
                OnPropertyChanged();
            }
        }

        private bool _isSignature;
        public bool IsSignature
        {
            get { return _isSignature; }
            set { _isSignature = value; OnPropertyChanged(); }
        }

        private string _pathPhotoLocal;
        public string PathPhotoLocal
        {
            get { return _pathPhotoLocal; }
            set { _pathPhotoLocal = value; OnPropertyChanged(); }
        }

        public BackButtonBehavior BackButtonBehavior { get; set; }
        public ICommand NextPageCommand { get; set; }
        public ICommand CommandListadoSelec { get; set; }
        public ICommand DeletePhotoCommand { get; }
        public ICommand PhotoCommand { get; set; }

        public IMediaPicker mediaPicker;

        public ListaCorrectivosMViewModel()
        {
            PhotoCommand = new Command(async () => await Photo());
            DeletePhotoCommand = new Command<PhotosModel>(DeletePhoto);
            IsSignature = true;
            NextPageCommand = new Command(async () => await ( Nextpage()));
        }

        void DeletePhoto(PhotosModel photo) {
            try {
                photoPaths.Remove(photo);
            } catch(Exception ex) {
                var msj = ex.Message;
            }
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {

            idclave = (int)query["idClave"];
            tipo = (string)query["Tipo"];
            Tipos = tipo;
            fecha = (string)query["Fecha"];
            Fechas = fecha;
            //_fecha = DateTime.ParseExact(Fechas, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            detalle = (string)query["Detalles"];
            Detalles = detalle;
        }

        //private async Task Photo()
        //{
        //    try
        //    {
        //        if (CountPhoto < 5)
        //        {
        //            CountPhoto++;
        //            if (this.mediaPicker.IsCaptureSupported)
        //            {
        //                FileResult photo = await MediaPicker.CapturePhotoAsync();
        //                if (photo != null)
        //                {
        //                    string LocalFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
        //                    using (Stream source = await photo.OpenReadAsync())
        //                    {
        //                        using FileStream localFile = File.OpenWrite(LocalFilePath);
        //                        await source.CopyToAsync(localFile);

        //                    }
        //                    PhotosModel photosModel = new PhotosModel();
        //                    photosModel.UrlPhoto = LocalFilePath;
        //                    _photoPaths.Add(photosModel);
        //                    //photoPaths.Add(photosModel);
        //                    PathPhotoLocal = LocalFilePath;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            await DisplayAlert("Mensaje", "Se a alcanzado el número máximo de fotos permitidas", "Cerrar");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await DisplayAlert("Error", ex.Message, "Cerrar");
        //    }
        //}
        private async Task Photo()
        {
            try
            {
                if (_photoPaths.Count < 5)
                {
                    if (this.mediaPicker.IsCaptureSupported)
                    {
                        // Present the user with an option to capture a new photo or select from the gallery
                     
                        string action = await Application.Current.MainPage.DisplayActionSheet("Selecciona una opción", "Cancelar", null, "Tomar foto", "Seleccionar de la galería");

                        if (action == "Tomar foto")
                        {
                            // Capture a new photo
                            FileResult photo = await MediaPicker.Default.CapturePhotoAsync();
                            if (photo != null)
                            {
                                // Save the file into local storage
                                string localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                                using (Stream sourceStream = await photo.OpenReadAsync())
                                {
                                    using FileStream localFileStream = File.OpenWrite(localFilePath);
                                    await sourceStream.CopyToAsync(localFileStream);
                                }

                                PhotosModel photosModel = new PhotosModel();
                                photosModel.UrlPhoto = localFilePath;
                                _photoPaths.Add(photosModel);
                                PathPhotoLocal = localFilePath;
                            }
                        }
                        else if (action == "Seleccionar de la galería")
                        {
                            // Choose an existing photo from the gallery
                            FileResult selectedPhoto = await MediaPicker.Default.PickPhotoAsync();
                            if (selectedPhoto != null)
                            {
                                // Save the selected file into local storage
                                string localFilePath = Path.Combine(FileSystem.CacheDirectory, selectedPhoto.FileName);

                                using (Stream sourceStream = await selectedPhoto.OpenReadAsync())
                                {
                                    using FileStream localFileStream = File.OpenWrite(localFilePath);
                                    await sourceStream.CopyToAsync(localFileStream);
                                }

                                PhotosModel photosModel = new PhotosModel();
                                photosModel.UrlPhoto = localFilePath;
                                _photoPaths.Add(photosModel);
                                PathPhotoLocal = localFilePath;
                            }
                        }
                    }
                }
                else
                {
                    await DisplayAlert("Mensaje", "Se ha alcanzado el número máximo de fotos permitidas", "Cerrar");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Cerrar");
            }
        }


        public async Task Nextpage()
        {
            IsBusy= true;  
            Dictionary<string, object> datos = new Dictionary<string, object>
            {
                { "idClave", idclave },
                { "Fotos", _photoPaths }
            };
            //await Shell.Current.GoToAsync("//MyRegistrosCorrctivosM", true, datos);
            var route = $"{nameof(RegistrosCorrctivosM)}";
            await Shell.Current.GoToAsync(route, true, datos);
            IsBusy = false;
        }
    }
}

