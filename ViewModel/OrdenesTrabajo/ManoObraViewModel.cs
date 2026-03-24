using BatiaSuite.Models.OrdenesTrabajo;
using BatiaSuite.Utils;
using BatiaSuite.Views.OrdenesTrabajo;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace BatiaSuite.ViewModel.OrdenesTrabajo;

public partial class ManoObraViewModel : ViewModelBase, IQueryAttributable {

    OrdenTrabajoEjecutadaModel _ordenTrabajo;

    [ObservableProperty]
    ObservableCollection<PersonalOrdenTrabajoRequest> _selectedPersonalList;

    [ObservableProperty]
    string _descripcion;
    [ObservableProperty]
    string _cliente;
    [ObservableProperty]
    string _inmueble;
    [ObservableProperty]
    string _falta;
    [ObservableProperty]
    string _tipo;
    [ObservableProperty]
    string _descripcionTrabajos;

    [ObservableProperty]
    string _title;

    [ObservableProperty]
    bool _isLoading;

    [ObservableProperty]
    string _textLoading;

    public ManoObraViewModel() {
        SelectedPersonalList = new ObservableCollection<PersonalOrdenTrabajoRequest>();
    }

    [RelayCommand]
    async Task AgregarPersonal() {
        PersonalOrdenTrabajoResponse personal = await PopupUtil.GetPersonalAsync();

        if(string.IsNullOrWhiteSpace(personal.nombre)) {
            return;
        }

        if(SelectedPersonalList.Any(p => p.IdEmpleado == personal.idEmpleado)) {
            await App.Current.MainPage.DisplayAlert("", Constants.PERSONAL_AGREGADO, Constants.ACEPTAR);
            return;
        }

        string? horasString = await App.Current.MainPage.DisplayPromptAsync(personal.nombre, Constants.INGRESE_HORAS_TRABAJADAS,
            Constants.ACEPTAR, Constants.CANCELAR, keyboard: Keyboard.Numeric);

        if(string.IsNullOrWhiteSpace(horasString)) {
            return;
        }

        float.TryParse(horasString, out float horasFloat);

        SelectedPersonalList.Add(new PersonalOrdenTrabajoRequest {
            IdEmpleado = personal.idEmpleado,
            Nombre = personal.nombre,
            Costo = personal.sueldo,
            Horas = horasFloat,
            Usuario = UserSession.IdEmpleado
        });
    }

    [RelayCommand]
    void EliminarEmpleadoSeleccionado(PersonalOrdenTrabajoRequest tecnico) =>
        SelectedPersonalList.Remove(tecnico);

    [RelayCommand]
    async Task Continuar() {
        if(string.IsNullOrWhiteSpace(Descripcion)) {
            await Toast.Make(Constants.INGRESE_TRABAJOS_REALIZADOS, ToastDuration.Short).Show();
            return;
        }

        if(SelectedPersonalList is null || SelectedPersonalList.Count == 0) {
            await Toast.Make(Constants.SELECCIONE_PERSONAL, ToastDuration.Short).Show();
            return;
        }
        IsLoading = true;
        TextLoading = "Cargando...";
        _ordenTrabajo.Personal = SelectedPersonalList;

        _ordenTrabajo.Trabajo.Trabejecutados = Descripcion;

        Dictionary<string, object> datos = new Dictionary<string, object>{
            { Constants.ORDEN_TRABAJO_KEY, _ordenTrabajo }
        };

        await Shell.Current.GoToAsync($"{nameof(MaterialesUtilizados)}", true, datos);
        IsLoading = false;
        TextLoading = "";
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query) {
        try {
            _ordenTrabajo = (OrdenTrabajoEjecutadaModel)query[Constants.ORDEN_TRABAJO_KEY];

            Title = $"Orden de trabajo : {_ordenTrabajo.Trabajo?.IdOrden}";
            Cliente = $"{_ordenTrabajo.Trabajo?.Cliente}";
            Inmueble = $"{_ordenTrabajo.Trabajo?.Inmueble}";
            Falta = $"{_ordenTrabajo.Trabajo?.Falta}";
            Tipo = $"{_ordenTrabajo.Trabajo?.Tipo}";
            DescripcionTrabajos = $"{_ordenTrabajo.Trabajo?.Descripcion}";
        } catch(Exception) { }
    }
}