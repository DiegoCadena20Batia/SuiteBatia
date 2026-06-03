using BatiaSuite.ViewModel.ChecklistLimpieza;

namespace BatiaSuite.Views.ChecklistLimpieza;

public partial class LimpiezaPage : ContentPage {
    public LimpiezaViewModel ViewModel { get; set; }

    public LimpiezaPage() {
        InitializeComponent();

        ViewModel = new LimpiezaViewModel();
        BindingContext = ViewModel;

        // Suscripción a eventos estáticos de limpieza
        LimpiezaViewModel.OnClearLimpiezaRequested += LimpiarLienzoLimpieza;
        LimpiezaViewModel.OnClearGerenteRequested += LimpiarLienzoGerente;
    }

    protected override void OnAppearing() {
        base.OnAppearing();
        if(ViewModel != null) {
            ViewModel.AntesDeEnviarChecklist = InterceptaryCapturarFirmasAsync;
        }
    }

    private void OnBtnLimpiarLimpiezaClicked(object sender, EventArgs e) {
        LimpiarLienzoLimpieza();
    }

    private void OnBtnLimpiarGerenteClicked(object sender, EventArgs e) {
        LimpiarLienzoGerente();
    }

    private void LimpiarLienzoLimpieza() {
        MainThread.BeginInvokeOnMainThread(() => {
            PadLimpieza.Lines.Clear();
        });
    }

    private void LimpiarLienzoGerente() {
        MainThread.BeginInvokeOnMainThread(() => {
            PadGerente.Lines.Clear();
        });
    }

    private async Task<bool> InterceptaryCapturarFirmasAsync() {
        if(!PadLimpieza.Lines.Any()) {
            await DisplayAlert("Firma Requerida", "Por favor, ingresa la firma del Auditor de Limpieza.", "OK");
            return false;
        }

        if(!PadGerente.Lines.Any()) {
            await DisplayAlert("Firma Requerida", "Por favor, ingresa la firma del Gerente de la Tienda.", "OK");
            return false;
        }

        try {
            var streamLimpieza = await PadLimpieza.GetImageStream(300, 150);
            var streamGerente = await PadGerente.GetImageStream(300, 150);

            using(var ms1 = new MemoryStream()) {
                await streamLimpieza.CopyToAsync(ms1);
                ViewModel.FirmaEmpleadoBytes = ms1.ToArray();
            }

            using(var ms2 = new MemoryStream()) {
                await streamGerente.CopyToAsync(ms2);
                ViewModel.FirmaGerenteBytes = ms2.ToArray();
            }

            return true;
        } catch(Exception ex) {
            await DisplayAlert("Error", $"No se pudieron procesar las firmas: {ex.Message}", "OK");
            return false;
        }
    }

    protected override void OnDisappearing() {
        base.OnDisappearing();
        // Desuscripción limpia para evitar fugas de memoria o llamadas cruzadas
        LimpiezaViewModel.OnClearLimpiezaRequested -= LimpiarLienzoLimpieza;
        LimpiezaViewModel.OnClearGerenteRequested -= LimpiarLienzoGerente;

        if(ViewModel != null) {
            ViewModel.AntesDeEnviarChecklist = null;
        }
    }

    private void PadLimpieza_DrawingLineCompleted(object sender, CommunityToolkit.Maui.Core.DrawingLineCompletedEventArgs e) {
        if(e.LastDrawingLine != null && !PadLimpieza.Lines.Contains(e.LastDrawingLine)) {
            PadLimpieza.Lines.Add(e.LastDrawingLine);
        }
    }

    private void PadGerente_DrawingLineCompleted(object sender, CommunityToolkit.Maui.Core.DrawingLineCompletedEventArgs e) {
        if(e.LastDrawingLine != null && !PadGerente.Lines.Contains(e.LastDrawingLine)) {
            PadGerente.Lines.Add(e.LastDrawingLine);
        }
    }
}