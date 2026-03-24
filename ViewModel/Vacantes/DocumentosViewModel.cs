using BatiaSuite.Models.Supervision;
using BatiaSuite.Models.Vacantes;
using BatiaSuite.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BatiaSuite.ViewModel.Vacantes;

public partial class DocumentosViewModel : ViewModelBase, IQueryAttributable {

    [ObservableProperty]
    DocumentoCandidatoModel _archivoDocumentos;

    [ObservableProperty]
    bool _isLoading;

    [ObservableProperty]
    string _loadingText;

    [ObservableProperty]
    bool _isBusy;

    [ObservableProperty]
    VacanteModel _vacante;

    [ObservableProperty]
    string _selectedFile;
    public DocumentosViewModel() {
        InitValues();
    }

    [RelayCommand]
    async Task SelectDocuments(DocumentoCandidatoModel documento) {
        try {
            FileResult fileResult = await FilePicker.Default.PickAsync(Constants.GetPickOptions(false, true));
            if(fileResult is not null) {

                string localFilePath = Path.Combine(FileSystem.CacheDirectory, fileResult.FileName);

                using(Stream stream = await fileResult.OpenReadAsync()) {
                    using FileStream localFileStream = File.OpenWrite(localFilePath);
                    await stream.CopyToAsync(localFileStream);
                }

                byte[] fileBytesArray = File.ReadAllBytes(localFilePath);
                string[] tokens = fileResult.FileName.Split('.');
                string extension = tokens[tokens.Length - 1];

                documento.PhotoPath = localFilePath;
                documento.Tamano = fileBytesArray.Length;
                documento.NombreRegistro = $"F_{documento.Nombre}.{extension}";

                SelectedFile = string.IsNullOrWhiteSpace(fileResult.FileName) ? documento.NombreRegistro : fileResult.FileName;
            }
        } catch(Exception) { }
    }

    [RelayCommand]
    void DeleteDocumento(DocumentoCandidatoModel documento) {
        documento.PhotoPath = string.Empty;
        documento.Tamano = 0;
        documento.NombreRegistro = string.Empty;
        SelectedFile = string.Empty;
    }

    [RelayCommand]
    async Task Continuar() {

        if(string.IsNullOrWhiteSpace(SelectedFile)) {
            await App.Current.MainPage.DisplayAlert("", "Agregue archivo con los documentos", Constants.ACEPTAR);
            return;
        }

        IsLoading = true;
        LoadingText = "Enviando datos ...";

        Vacante.ArchivosApp = new List<DocumentoCandidatoModel>();

        if(!string.IsNullOrWhiteSpace(ArchivoDocumentos.PhotoPath)) {
            ArchivoDocumentos.Nombre = ArchivoDocumentos.NombreRegistro;
            Vacante.ArchivosApp.Add(ArchivoDocumentos);
        }

        Vacante.CantHijos = Vacante.CantHijos is null ? 0 : Vacante.CantHijos;
        Vacante.FormComunDesc = Vacante.FormComunDesc is null ? "" : Vacante.FormComunDesc;

        int idEmpleado = await _httpHelper.PostBodyAsync<VacanteModel, int>(Constants.VAC_POST_VACANTE_DATA, Vacante);

        if(idEmpleado == 0) {
            await App.Current.MainPage.DisplayAlert("", Constants.ERROR_API, Constants.ACEPTAR);
            IsLoading = false;
            LoadingText = string.Empty;
            return;
        }

        await UploadPhotosAsync(idEmpleado);

        await Shell.Current.GoToAsync("//MyMenu");
        await App.Current.MainPage.DisplayAlert("", Constants.VACANTE_ENVIADA, Constants.ACEPTAR);
        IsLoading = false;
    }

    async void InitValues() {
        IsLoading = true;
        await LoadDocumentosList();
        IsLoading = false;
    }

    async Task<bool> UploadPhotosAsync(int idEmpleado) {
        using(MultipartFormDataContent multipartContent = new MultipartFormDataContent()) {

            multipartContent.Headers.ContentType.MediaType = "multipart/form-data";
            multipartContent.Headers.Add("folder", $"RH/Candidatos/{idEmpleado}");

            if(!string.IsNullOrWhiteSpace(ArchivoDocumentos.PhotoPath)) {
                byte[] fileBytesArray = File.ReadAllBytes(ArchivoDocumentos.PhotoPath);
                Stream stream = new MemoryStream(fileBytesArray);
                multipartContent.Add(new StreamContent(stream), "files", ArchivoDocumentos.NombreRegistro);
            }

            List<ArchivoModel> result = await _httpHelper.PostMultipartAsync<List<ArchivoModel>>(Constants.SUP_POST_FOTOS, multipartContent, false);
            return result is not null;
        }
    }

    async Task LoadDocumentosList() {
        List<DocumentoCandidatoModel> documentos = await _httpHelper.GetAsync<List<DocumentoCandidatoModel>>(Constants.VAC_GET_CANDIDATO_DOCUMENTOS);
        ArchivoDocumentos = new DocumentoCandidatoModel();
        foreach(DocumentoCandidatoModel documento in documentos) {
            if(documento.IdDocumento == 8) {
                ArchivoDocumentos = documento;
            }
        }
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query) {
        if(query.ContainsKey(Constants.VACANTE_DATA_KEY)) {
            Vacante = (VacanteModel)query[Constants.VACANTE_DATA_KEY];
            query.Remove(Constants.VACANTE_DATA_KEY);
        };
    }
}
