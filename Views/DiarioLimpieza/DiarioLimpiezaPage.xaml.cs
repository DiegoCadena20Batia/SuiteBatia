using BatiaSuite.ViewModel.ChecklistMonitoreo;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace BatiaSuite.Views.DiarioLimpieza;

public partial class DiarioLimpiezaPage : ContentPage {
    public DiarioLimpiezaViewModel ViewModel { get; set; }

    public DiarioLimpiezaPage() {
        InitializeComponent();

        ViewModel = new DiarioLimpiezaViewModel();
        BindingContext = ViewModel;
    }

    protected override void OnAppearing() {
        base.OnAppearing();

        if(ViewModel != null) {
            ViewModel.AntesDeEnviarChecklist = InterceptaryCapturarFirmasAsync;
        }

        MonitoreoViewModel.OnClearGerenteRequested += LimpiarLienzoGerente;
    }

    protected override void OnDisappearing() {
        base.OnDisappearing();

        MonitoreoViewModel.OnClearGerenteRequested -= LimpiarLienzoGerente;

        if(ViewModel != null) {
            ViewModel.AntesDeEnviarChecklist = null;
        }
    }

    private void OnBtnLimpiarGerenteClicked(object sender, EventArgs e) {
        LimpiarLienzoGerente();
    }

    private void LimpiarLienzoGerente() {
        MainThread.BeginInvokeOnMainThread(() => {
            PadGerente.Lines.Clear();
        });
    }

    private async Task<bool> InterceptaryCapturarFirmasAsync() {
        if(!PadGerente.Lines.Any()) {
            await DisplayAlert("Firma Requerida", "Por favor, ingresa la firma del Gerente de la Tienda.", "OK");
            return false;
        }

        try {
            var streamGerente = await PadGerente.GetImageStream(300, 150);

            using(var ms1 = new MemoryStream()) {
                await streamGerente.CopyToAsync(ms1);
                ViewModel.FirmaGerenteBytes = ms1.ToArray();
            }

            return true;
        } catch(Exception ex) {
            await DisplayAlert("Error", $"No se pudieron procesar las firmas: {ex.Message}", "OK");
            return false;
        }
    }

    private void PadGerente_DrawingLineCompleted(object sender, CommunityToolkit.Maui.Core.DrawingLineCompletedEventArgs e) {
        if(e.LastDrawingLine != null && !PadGerente.Lines.Contains(e.LastDrawingLine)) {
            PadGerente.Lines.Add(e.LastDrawingLine);
        }
    }
}