using BatiaSuite.Models.SupervisionMantenimiento;
using BatiaSuite.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;

namespace BatiaSuite.ViewModel.SupervisionMantenimiento;

public partial class SupervisionMantenimientoPreguntasViewModel : ViewModelBase {
    private readonly SupervisionMantenimientoService _supervisionService;
    private SupervisionMantenimientoSeccionesModel _currentSection;

    [ObservableProperty]
    private string _tituloSeccion;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _textLoading = "Cargando...";

    public ObservableCollection<SupervisionMantenimientoPreguntasModel> Preguntas { get; } = new();

    public SupervisionMantenimientoSeccionesModel CurrentSection {
        get => _currentSection;
        set {
            SetProperty(ref _currentSection, value);
            if(value != null) {
                TituloSeccion = value.Seccion;
            }
        }
    }

    public string ProgressText =>
        $"Sección {_supervisionService.GetCurrentSectionIndex()} de {_supervisionService.GetTotalSections()}";

    public bool CanGoBack => _supervisionService.HasPreviousSection();
    public bool CanGoForward => _supervisionService.HasNextSection();

    // Comandos para los botones de opción
    public ICommand SelectAplicaCommand { get; }
    public ICommand SelectNoAplicaCommand { get; }
    public ICommand SelectNACommand { get; }

    // Comandos de navegación
    public ICommand ContinuarCommand { get; }
    public ICommand AnteriorCommand { get; }

    public SupervisionMantenimientoPreguntasViewModel(SupervisionMantenimientoService supervisionService) {
        _supervisionService = supervisionService;

        // Inicializar comandos para las opciones
        SelectAplicaCommand = new Command<SupervisionMantenimientoPreguntasModel>(
            (pregunta) => SelectOption(pregunta, 1));

        SelectNoAplicaCommand = new Command<SupervisionMantenimientoPreguntasModel>(
            (pregunta) => SelectOption(pregunta, 2));

        SelectNACommand = new Command<SupervisionMantenimientoPreguntasModel>(
            (pregunta) => SelectOption(pregunta, 3));

        // Comando para continuar (floating button)
        ContinuarCommand = new AsyncRelayCommand(Continuar);
        AnteriorCommand = new AsyncRelayCommand(Anterior);

        //InitializeAsync();
    }

    private void SelectOption(SupervisionMantenimientoPreguntasModel pregunta, int valor) {
        Console.WriteLine($"=== SelectOption ===");
        Console.WriteLine($"Pregunta ID: {pregunta.IdPregunta}");
        Console.WriteLine($"Valor seleccionado: {valor}");
        Console.WriteLine($"Estado anterior: {pregunta.Estado}");

        var index = Preguntas.IndexOf(pregunta);
        if(index >= 0) {
            // Método que SI funciona para forzar DataTriggers
            // 1. Crear una COPIA completa con el nuevo estado
            var nuevaPregunta = new SupervisionMantenimientoPreguntasModel {
                IdSeccion = pregunta.IdSeccion,
                IdPregunta = pregunta.IdPregunta,
                Pregunta = pregunta.Pregunta,
                Estado = valor,  // NUEVO valor
                Comentarios = pregunta.Comentarios
            };

            // 2. Reemplazar en la colección usando Remove/Insert
            Preguntas.RemoveAt(index);
            Preguntas.Insert(index, nuevaPregunta);

            Console.WriteLine($"UI actualizada - Nuevo objeto insertado en índice {index}");
            Console.WriteLine($"Nuevo estado verificado: {Preguntas[index].Estado}");
        } else {
            Console.WriteLine($"ERROR: Pregunta no encontrada");
        }

        Console.WriteLine($"=== SelectOption completado ===");
    }

    public void LoadCurrentSection() {
        CurrentSection = _supervisionService.GetCurrentSection();

        if(CurrentSection != null) {
            var questions = _supervisionService.GetPreguntasBySeccion(CurrentSection.IdSeccion);
            Preguntas.Clear();

            foreach(var question in questions) {
                Preguntas.Add(question);
            }

            OnPropertyChanged(nameof(ProgressText));
            OnPropertyChanged(nameof(CanGoBack));
            OnPropertyChanged(nameof(CanGoForward));
        }
    }

    private async Task Continuar() {
        if(!ValidateCurrentSection()) {
            await Shell.Current.DisplayAlert("Validación",
                "Por favor responda todas las preguntas antes de continuar", "OK");
            return;
        }

        // Guardar respuestas de esta sección
        SaveCurrentSection();

        // Ir a la siguiente sección o finalizar
        var nextSection = _supervisionService.GetNextSection();
        if(nextSection != null) {
            LoadCurrentSection();
            // Hacer scroll al inicio
            await ScrollToTop();
        } else {
            // Terminó todas las secciones
            await FinalizarSupervision();
        }
    }

    private void SaveCurrentSection() {
        // Guardar todas las respuestas de esta sección en el servicio
        foreach(var pregunta in Preguntas) {
            var preguntas = _supervisionService.GetPreguntasBySeccion(CurrentSection.IdSeccion);
            var preguntaExistente = preguntas.FirstOrDefault(p => p.IdPregunta == pregunta.IdPregunta);

            if(preguntaExistente != null) {
                preguntaExistente.Estado = pregunta.Estado;
                preguntaExistente.Comentarios = pregunta.Comentarios;
            }
        }
    }

    private bool ValidateCurrentSection() {
        // Verificar que todas las preguntas tengan respuesta
        foreach(var pregunta in Preguntas) {
            if(pregunta.Estado == 0) // 0 = Sin responder
            {
                return false;
            }

            // Validación adicional: Si es "NO APLICA", sugerir comentarios
            if(pregunta.Estado == 2 && string.IsNullOrWhiteSpace(pregunta.Comentarios)) {
                // Podría ser una advertencia en lugar de un bloqueo
                // return false; // Descomentar para hacer obligatorio
            }
        }
        return true;
    }

    private async Task ScrollToTop() {
        // Método para hacer scroll al inicio (se implementa en el code-behind)
        // Este método será llamado desde la página
    }

    private async Task FinalizarSupervision() {
        // Aquí puedes guardar todo el modelo y navegar a la pantalla de firma
        // await _supervisionService.GuardarSupervision();
        await Shell.Current.GoToAsync("//supervisionMantenimiento/firma");
    }

    // Método para cargar desde la página anterior
    public async Task InitializeAsync(int idCliente, int idInmueble) {
        IsLoading = true;

        try {
            //Preguntas = _supervisionService.GetPreguntasBySeccion(1);
            // Cargar secciones dinámicas para este cliente
            _supervisionService.ObtenerSecciones();

            // Iniciar supervisión
            _supervisionService.InicioSupervision(idCliente, idInmueble);

            // Cargar primera sección
            LoadCurrentSection();
        } catch(Exception ex) {
            await Shell.Current.DisplayAlert("Error",
                $"Error al cargar las preguntas: {ex.Message}", "OK");
        } finally {
            IsLoading = false;
        }
    }

    // Método para ir a la sección anterior (si necesitas botón en la UI)
   // [RelayCommand]
    public async Task Anterior() {
        var previousSection = _supervisionService.GetPreviousSection();
        if(previousSection != null) {
            LoadCurrentSection();
            await ScrollToTop();
        }
    }
}

// Converter para mostrar/ocultar comentarios
public class ShowCommentsConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if(value is int valor) {
            // Mostrar comentarios solo cuando es "NO APLICA" (2) o "N/A" (3)
            return valor == 2 || valor == 3;
        }
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

// Converter para el color de fondo de los botones
public class OptionBackgroundColorConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if(value is int valorActual && parameter is string paramStr) {
            int valorEsperado = int.Parse(paramStr);

            // Si es la opción seleccionada, devolver color de selección
            if(valorActual == valorEsperado) {
                return valorEsperado switch {
                    1 => Color.FromArgb("#4CAF50"), // Verde para APLICA
                    2 => Color.FromArgb("#F44336"), // Rojo para NO APLICA
                    3 => Color.FromArgb("#FF9800"), // Naranja para N/A
                    _ => Colors.Transparent
                };
            }

            // Si no está seleccionado, transparente
            return Colors.Transparent;
        }
        return Colors.Transparent;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

// Converter para el color del texto de los botones
public class OptionTextColorConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if(value is int valorActual && parameter is string paramStr) {
            int valorEsperado = int.Parse(paramStr);

            // Si es la opción seleccionada, texto blanco
            if(valorActual == valorEsperado) {
                return Colors.White;
            }

            // Si no está seleccionado, color según la opción
            return valorEsperado switch {
                1 => Color.FromArgb("#4CAF50"), // Verde
                2 => Color.FromArgb("#F44336"), // Rojo
                3 => Color.FromArgb("#FF9800"), // Naranja
                _ => Colors.Black
            };
        }
        return Colors.Black;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}