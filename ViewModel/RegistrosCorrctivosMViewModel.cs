using BatiaSuite.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BatiaSuite.Models.RegistrosCorrectivosMModel;
using BatiaSuite.Views;
using System.ComponentModel;
using System.Globalization;
using BatiaSuite.Models.OrdenesTrabajo;
using BatiaSuite.Utils;
using BatiaSuite.Models.Encuestas;


namespace BatiaSuite.ViewModel
{
    public class RegistrosCorrctivosMViewModel : BaseViewModel, IQueryAttributable
    {
        #region Orden de trabajo
        bool _esOrdenTrabajo;
        OrdenTrabajoEjecutadaModel _ordenTrabajo;
        #endregion

        public int idClave { get; set; }

        List<string> responses = new List<string>();

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

        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; OnPropertyChanged(); }
        }

        public int _selectionRadio;
        public int SelectionRadio
        {
            get { return _selectionRadio; }
            set { _selectionRadio = value; OnPropertyChanged(); }
        }

        public int _selectionRadio1;
        public int SelectionRadio1
        {
            get { return _selectionRadio1; }
            set { _selectionRadio1 = value; OnPropertyChanged(); }
        }

        public int _selectionRadio2;
        public int SelectionRadio2
        {
            get { return _selectionRadio2; }
            set { _selectionRadio2 = value; OnPropertyChanged(); }
        }

        public int _selectionRadio3;
        public int SelectionRadio3
        {
            get { return _selectionRadio3; }
            set { _selectionRadio3 = value; OnPropertyChanged(); }
        }

        public int _selectionRadio4;
        public int SelectionRadio4
        {
            get { return _selectionRadio4; }
            set { _selectionRadio4 = value; OnPropertyChanged(); }
        }

        private bool _isVisible = false;

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    OnPropertyChanged();

                }
            }
        }


        private string _nombreRecibe = "";

        public string NombreRecibe
        {
            get { return _nombreRecibe; }
            set { _nombreRecibe = value; OnPropertyChanged(); }
        }

        private string _pathPhotoLocal;

        public string PathPhotoLocal
        {
            get { return _pathPhotoLocal; }
            set { _pathPhotoLocal = value; OnPropertyChanged(); }
        }

        private string  _firma = null;
        public string Firma
        {
            get { return _firma; }
            set { _firma = value; OnPropertyChanged(); }
        }

        private string _pathFirmaLocal;

        public string PathFirmaLocal
        {
            get { return _pathFirmaLocal; }
            set { _pathFirmaLocal = value; }
        }

        private bool _isSignature;

        public bool IsSignature
        {
            get { return _isSignature; }
            set { _isSignature = value; OnPropertyChanged(); }
        }

        public void IsEncuesta()
        {
            if (IsVisible)
            {
                IsVisible = false;

            }
            else if (!IsVisible)
            {
                IsVisible = true;

            }
        }

        ObservableCollection<PhotosModel> fotos;
        public ICommand RegisterCommand { get; set; }
        public ICommand ShowPasswordCommand { get; set; }
      
        public RegistrosCorrctivosMViewModel()
        {

            ShowPasswordCommand = new Command(() => IsEncuesta());
          
            RegisterCommand = new Command(async () => await RegisterCorrectivo());

            IsEnabled = true;   
            IsSignature = true;

        }

        private async Task RegisterCorrectivo()
        {
            #region Orden de Trabajo
            if(_esOrdenTrabajo) {
                await EnviarOrdenTrabajoEjecutada();
                return;
            }
            #endregion

            IsEnabled = false;
            IsBusy = true;
            await UploadPhotosAsync();

            RegistrosCorrectivosMModel registroscorrectivosM = new RegistrosCorrectivosMModel
            {
                 IdClaveCM = idClave,
                 TrabajosGeneral = _selectionRadio,
                 TecnicosUniforme = _selectionRadio1,
                 TratoTecnicos = _selectionRadio2,
                 TrabajosOrden = _selectionRadio3,
                 MaterialesAdecuados  = _selectionRadio4,
                 Encuestado = _nombreRecibe,
            };
            var httpClient = new HttpClient();
            var json = JsonConvert.SerializeObject(registroscorrectivosM);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            //var response = await httpClient.PostAsync("https://www.singa.com.mx:5500/api/CorrectivosMReporte", content);
            var response = await httpClient.PostAsync(Constants.API_BASE_URL +"CorrectivosMReporte", content);
            IsBusy = false;
            IsEnabled = true;
            if (response.StatusCode == HttpStatusCode.OK)
            {
               
                await DisplayAlert("Mensaje", "Evidencias Enviadas", "Ok");
              
                await Shell.Current.GoToAsync("//MyMenu");
            }
            else
            {
                //await DisplayAlert("Mensaje", "Evidencias Enviadas", "Ok");

                //await Shell.Current.GoToAsync("//MyMenu");
                await DisplayAlert("Error", "Ocurrió un error al registrar la información", "Cerrar");
            }
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if(query.ContainsKey(Constants.ORDEN_TRABAJO_KEY)) {
                _ordenTrabajo = (OrdenTrabajoEjecutadaModel)query[Constants.ORDEN_TRABAJO_KEY];
                _esOrdenTrabajo = true;
                return;
            }

            // Declarar variables locales para cada atributo del query.
            idClave = (int)query["idClave"];
             //cliente = (string)query["Cliente"];
             //inmueble = (string)query["Inmueble"];
             //_fecha = (DateTime)query["Fecha"];
             //detalle = (string)query["Detalles"];
             fotos = (ObservableCollection<PhotosModel>)query["Fotos"];           
        }

        //private string SearchWord(List<string> firma)
        //{
        //    /*string Firma = null; */// Inicializamos la variable Firma como null
        //    foreach (string palabra in firma)
        //    {
        //        if (palabra.ToLower().Contains("signature")) // Verificamos si la palabra contiene "signature" (ignorando las mayúsculas/minúsculas)
        //        {
        //            Firma = palabra; // Asignamos la palabra que contiene "signature" a la variable Firma
        //            break; // Salimos del bucle una vez encontrada la palabra
        //        }
        //    }
        //    return Firma; // Devolvemos la palabra encontrada o null si no se encontró
        //}
        //public string FiltrarPorPalabra(List<string> res, string signature = "signature")
        //{
        //    // Filtrar las subcadenas que contienen la palabra de filtro
        //    var resultado = res.Where(item => !item.Contains(signature)).ToList();
        //    Resultado = string.Join(",", resultado);
        //    return Resultado;
        //    ////var resultado = res.Where(item => !item.Contains(signature)).ToArray();
        //    ////return resultado;
        //}

        //public List<string> FiltrarPorPalabra(List<string> res, string signature = "signature")
        //{
        //    // Filtrar las subcadenas que contienen la palabra de filtro
        //    var resultado = res.Where(item => !item.Contains(signature)).ToList();
        //    Resultado = string.Join(",", resultado);
        //    return Resultado;
        //    ////var resultado = res.Where(item => !item.Contains(signature)).ToArray();
        //    ////return resultado;
        //}

        //public void Main(string[] args)
        //{


        //    UrlWithSignature = responses.FirstOrDefault(url => url.Contains("signature"));

        //    if (UrlWithSignature != null)
        //    {
        //        //Console.WriteLine("URL con la palabra 'signature': " + UrlWithSignature);
        //    }
        //    else
        //    {
        //        //Console.WriteLine("No se encontró ninguna URL con la palabra 'signature'.");
        //    }
        //}


        //private string FilterWords(List<string> firma)
        //{
        //    // Inicializamos una variable para almacenar las palabras filtradas
        //    StringBuilder palabrasFiltradas = new StringBuilder();

        //    foreach (string palabra in firma)
        //    {
        //        // Verificamos si la palabra contiene "asignature" (ignorando las mayúsculas/minúsculas)
        //        if (!palabra.ToLower().Contains("asignature"))
        //        {
        //            // Agregamos la palabra a la variable palabrasFiltradas
        //            palabrasFiltradas.Append(palabra + " ");
        //        }
        //    }

        //    // Convertimos la variable StringBuilder a una cadena de texto
        //    //string resultado = palabrasFiltradas.ToString().Trim();
        //    resultado = palabrasFiltradas.ToString().Trim();

        //    return resultado;
        //}

        public async Task<List<string>> UploadPhotosAsync(bool enviarFirma = true, int idOrden = 0)
        {
            // Lista para almacenar las respuestas de la API
            // Llenar la lista FotosSend con las fotos y la imagen de la firma
            List<string> FotosSend = new List<string>();
            foreach (var item in fotos)
            {
                PhotosModel foto = new PhotosModel();
                foto.UrlPhoto = item.UrlPhoto.ToString();
                FotosSend.Add(foto.UrlPhoto);
            }
            if (!string.IsNullOrEmpty(PathFirmaLocal) && enviarFirma)
            {
                FotosSend.Add(PathFirmaLocal);
            }
            using (var client = new HttpClient())
            {
                foreach (var photoUrl in FotosSend)
                {
                    using (var content = new MultipartFormDataContent())
                    {
                        byte[] fileBytes = File.ReadAllBytes(photoUrl);
                        var fileContent = new ByteArrayContent(fileBytes);
                        content.Add(fileContent, "files", Path.GetFileName(photoUrl));
                        // Realizar la solicitud HTTP POST al API
                        //string url = _esOrdenTrabajo
                        //    ? $"https://www.singa.com.mx:5500/api/FilesOrdenesTrabajo/CargaMul?folio={idOrden}"
                        //    : $"https://www.singa.com.mx:5500/api/FilesImagenesCM/CargaMul?folio={idClave}";


                        string url = _esOrdenTrabajo
                            ? Constants.API_BASE_URL + $"FilesOrdenesTrabajo/CargaMul?folio={idOrden}"
                            : Constants.API_BASE_URL + $"FilesImagenesCM/CargaMul?folio={idClave}";
                        var response = await client.PostAsync(url, content);
                        // Manejar la respuesta del servidor
                        if (response.IsSuccessStatusCode)
                        {
                            //await DisplayAlert("Guardado", "Archivos guardados correctamente", "Ok");
                        }
                        else
                        {
                            await DisplayAlert("Error", "Ocurrió un error al guardar", "Ok");
                        }
                        // Leer el contenido de la respuesta y agregarlo a la lista de respuestas
                        string responseContent = await response.Content.ReadAsStringAsync();
                        responses.Add(responseContent);
                    }
                }
            }
            return responses; // Devolver la lista de respuestas
        }

        #region Order Trabajo
        async Task EnviarOrdenTrabajoEjecutada() {
            if(_ordenTrabajo is null) {
                return;
            }

            IsEnabled = false;
            IsBusy = true;

            if(_ordenTrabajo.FotosList is not null && _ordenTrabajo.FotosList.Count() > 0) {
                fotos = EnumerablesConverter(_ordenTrabajo.FotosList);
                await UploadPhotosAsync(idOrden:_ordenTrabajo.Trabajo.IdOrden);
            }

            if(_ordenTrabajo.FilesList is not null && _ordenTrabajo.FilesList.Count() > 0) {
                fotos = EnumerablesConverter(_ordenTrabajo.FilesList);
                await UploadPhotosAsync(false, _ordenTrabajo.Trabajo.IdOrden);
            }

            _ordenTrabajo.Reporte = new Reporte() {
                IdOrden = _ordenTrabajo.Trabajo.IdOrden,
                TrabajosGeneral = _selectionRadio,
                TecnicosUniforme = _selectionRadio1,
                TratoTecnicos = _selectionRadio2,
                TrabajosOrden = _selectionRadio3,
                MaterialesAdecuados = _selectionRadio4,
                Encuestado = _nombreRecibe,
            };

            _ordenTrabajo.Trabajo.Fejecucion = (DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss");
            HttpHelper _httpHelper = new HttpHelper();

            int result = await _httpHelper.PostBodyAsync<OrdenTrabajoEjecutadaModel, int>(Constants.OT_ENVIAR_ORDEN_EJECUTADA_API, _ordenTrabajo);

            if(result > 0) {
                await DisplayAlert("", "Orden de trabajo enviada correctamente", Constants.ACEPTAR);
                await Shell.Current.GoToAsync("//MyMenu");
            } else {
                await App.Current.MainPage.DisplayAlert("", "Ocurrió un error al enviar los datos", Constants.ACEPTAR);
            }            

            IsEnabled = true;
            IsBusy = false;
        }

        ObservableCollection<PhotosModel> EnumerablesConverter(IEnumerable<string> items) {
            ObservableCollection<PhotosModel> fotos =new ObservableCollection<PhotosModel>();
            
            foreach(string item in items) {
                fotos.Add(new PhotosModel { UrlPhoto = item});
            }

            return fotos;
        }
        #endregion
    }
}
