using BatiaSuite.ViewModel.ChecklistMantenimiento;

namespace BatiaSuite.Views.ChecklistMantenimiento;

public partial class MantenimientoPage : ContentPage
{
    public MantenimientoViewModel ViewModel { get; set; }

    public MantenimientoPage() {
        InitializeComponent();

        ViewModel = new MantenimientoViewModel();
        BindingContext = ViewModel;

        // Suscripción a eventos estáticos de mantenimiento
        MantenimientoViewModel.OnClearMantenimientoRequested += LimpiarLienzoMantenimiento;
        MantenimientoViewModel.OnClearGerenteRequested += LimpiarLienzoGerente;
    }

    protected override void OnAppearing() {
        base.OnAppearing();
        if(ViewModel != null) {
            ViewModel.AntesDeEnviarChecklist = InterceptaryCapturarFirmasAsync;
        }
    }

    private void OnBtnLimpiarMantenimientoClicked(object sender, EventArgs e) {
        LimpiarLienzoMantenimiento();
    }

    private void OnBtnLimpiarGerenteClicked(object sender, EventArgs e) {
        LimpiarLienzoGerente();
    }

    private void LimpiarLienzoMantenimiento() {
        MainThread.BeginInvokeOnMainThread(() => {
            PadMantenimiento.Lines.Clear();
        });
    }

    private void LimpiarLienzoGerente() {
        MainThread.BeginInvokeOnMainThread(() => {
            PadGerente.Lines.Clear();
        });
    }

    private async Task<bool> InterceptaryCapturarFirmasAsync() {
        if(!PadMantenimiento.Lines.Any()) {
            await DisplayAlert("Firma Requerida", "Por favor, ingresa la firma del Auditor de Mantenimiento.", "OK");
            return false;
        }

        if(!PadGerente.Lines.Any()) {
            await DisplayAlert("Firma Requerida", "Por favor, ingresa la firma del Gerente de la Tienda.", "OK");
            return false;
        }

        try {
            var streamMantenimiento = await PadMantenimiento.GetImageStream(300, 150);
            var streamGerente = await PadGerente.GetImageStream(300, 150);

            using(var ms1 = new MemoryStream()) {
                await streamMantenimiento.CopyToAsync(ms1);
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
        MantenimientoViewModel.OnClearMantenimientoRequested -= LimpiarLienzoMantenimiento;
        MantenimientoViewModel.OnClearGerenteRequested -= LimpiarLienzoGerente;

        if(ViewModel != null) {
            ViewModel.AntesDeEnviarChecklist = null;
        }
    }

    private void PadMantenimiento_DrawingLineCompleted(object sender, CommunityToolkit.Maui.Core.DrawingLineCompletedEventArgs e) {
        if(e.LastDrawingLine != null && !PadMantenimiento.Lines.Contains(e.LastDrawingLine)) {
            PadMantenimiento.Lines.Add(e.LastDrawingLine);
        }
    }

    private void PadGerente_DrawingLineCompleted(object sender, CommunityToolkit.Maui.Core.DrawingLineCompletedEventArgs e) {
        if(e.LastDrawingLine != null && !PadGerente.Lines.Contains(e.LastDrawingLine)) {
            PadGerente.Lines.Add(e.LastDrawingLine);
        }
    }
}