using BatiaSuite.ViewModel.ChecklistMonitoreo;

namespace BatiaSuite.Views.ChecklistMonitoreo;

public partial class MonitoreoPage : ContentPage
{
    public MonitoreoViewModel ViewModel { get; set; }

    public MonitoreoPage() {
        InitializeComponent();

        ViewModel = new MonitoreoViewModel();
        BindingContext = ViewModel;

        // Suscripción a eventos estáticos de monitoreo
        MonitoreoViewModel.OnClearMonitoreoRequeste += LimpiarLienzoMonitoreo;
        MonitoreoViewModel.OnClearGerenteRequested += LimpiarLienzoGerente;
    }

    protected override void OnAppearing() {
        base.OnAppearing();
        if(ViewModel != null) {
            ViewModel.AntesDeEnviarChecklist = InterceptaryCapturarFirmasAsync;
        }
    }

    private void OnBtnLimpiarMonitoreoClicked(object sender, EventArgs e) {
        LimpiarLienzoMonitoreo();
    }

    private void OnBtnLimpiarGerenteClicked(object sender, EventArgs e) {
        LimpiarLienzoGerente();
    }

    private void LimpiarLienzoMonitoreo() {
        MainThread.BeginInvokeOnMainThread(() => {
            PadMonitoreo.Lines.Clear();
        });
    }

    private void LimpiarLienzoGerente() {
        MainThread.BeginInvokeOnMainThread(() => {
            PadGerente.Lines.Clear();
        });
    }

    private async Task<bool> InterceptaryCapturarFirmasAsync() {
        if(!PadMonitoreo.Lines.Any()) {
            await DisplayAlert("Firma Requerida", "Por favor, ingresa la firma del Auditor de Monitoreo.", "OK");
            return false;
        }

        if(!PadGerente.Lines.Any()) {
            await DisplayAlert("Firma Requerida", "Por favor, ingresa la firma del Gerente de la Tienda.", "OK");
            return false;
        }

        try {
            var streamLimpieza = await PadMonitoreo.GetImageStream(300, 150);
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
        MonitoreoViewModel.OnClearMonitoreoRequeste -= LimpiarLienzoMonitoreo;
        MonitoreoViewModel.OnClearGerenteRequested -= LimpiarLienzoGerente;

        if(ViewModel != null) {
            ViewModel.AntesDeEnviarChecklist = null;
        }
    }

    private void PadMonitoreo_DrawingLineCompleted(object sender, CommunityToolkit.Maui.Core.DrawingLineCompletedEventArgs e) {
        if(e.LastDrawingLine != null && !PadMonitoreo.Lines.Contains(e.LastDrawingLine)) {
            PadMonitoreo.Lines.Add(e.LastDrawingLine);
        }
    }

    private void PadGerente_DrawingLineCompleted(object sender, CommunityToolkit.Maui.Core.DrawingLineCompletedEventArgs e) {
        if(e.LastDrawingLine != null && !PadGerente.Lines.Contains(e.LastDrawingLine)) {
            PadGerente.Lines.Add(e.LastDrawingLine);
        }
    }
}