using BatiaSuite.ViewModel.CheckListSupervisionesAldoConti;
using CommunityToolkit.Maui.Views;

namespace BatiaSuite.Views.CheckListAparadoristasAldoConti;

public partial class AparadoristasPage : ContentPage {
    public AparadoristasViewModel ViewModel { get; set; }

    public AparadoristasPage() {
        InitializeComponent();

        ViewModel = new AparadoristasViewModel();
        BindingContext = ViewModel;

        AparadoristasViewModel.OnClearAparadoristaRequested += LimpiarLienzoAparadorista;
        AparadoristasViewModel.OnClearGerenteRequested += LimpiarLienzoGerente;
    }

    protected override void OnAppearing() {
        base.OnAppearing();
        if(ViewModel != null) {
            ViewModel.AntesDeEnviarChecklist = InterceptaryCapturarFirmasAsync;
        }
    }

    // Evento del botón Limpiar del Aparadorista
    private void OnBtnLimpiarAparadoristaClicked(object sender, EventArgs e) {
        LimpiarLienzoAparadorista();
    }

    // Evento del botón Limpiar del Gerente
    private void OnBtnLimpiarGerenteClicked(object sender, EventArgs e) {
        LimpiarLienzoGerente();
    }

    private void LimpiarLienzoAparadorista() {
        MainThread.BeginInvokeOnMainThread(() => {
            PadAparadorista.Lines.Clear();
        });
    }

    private void LimpiarLienzoGerente() {
        MainThread.BeginInvokeOnMainThread(() => {
            PadGerente.Lines.Clear();
        });
    }

    private async Task<bool> InterceptaryCapturarFirmasAsync() {
        if(!PadAparadorista.Lines.Any()) {
            await DisplayAlert("Firma Requerida", "Por favor, ingresa la firma del Aparadorista.", "OK");
            return false;
        }

        if(!PadGerente.Lines.Any()) {
            await DisplayAlert("Firma Requerida", "Por favor, ingresa la firma del Gerente de la Tienda.", "OK");
            return false;
        }

        try {
            var streamAparadorista = await PadAparadorista.GetImageStream(300, 150);
            var streamGerente = await PadGerente.GetImageStream(300, 150);

            using(var ms1 = new MemoryStream()) {
                await streamAparadorista.CopyToAsync(ms1);
                ViewModel.FirmaAparadoristaBytes = ms1.ToArray();
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
        AparadoristasViewModel.OnClearAparadoristaRequested -= LimpiarLienzoAparadorista;
        AparadoristasViewModel.OnClearGerenteRequested -= LimpiarLienzoGerente;

        if(ViewModel != null) {
            ViewModel.AntesDeEnviarChecklist = null;
        }
    }

    private void PadAparadorista_DrawingLineCompleted(object sender, CommunityToolkit.Maui.Core.DrawingLineCompletedEventArgs e) {
        // Evitamos que el componente limpie el lienzo añadiendo el trazo actual de forma permanente
        if(e.LastDrawingLine != null && !PadAparadorista.Lines.Contains(e.LastDrawingLine)) {
            PadAparadorista.Lines.Add(e.LastDrawingLine);
        }
    }
}