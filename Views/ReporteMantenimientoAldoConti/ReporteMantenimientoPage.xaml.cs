using BatiaSuite.ViewModel.DiarioGerenteAldoConti;
using BatiaSuite.ViewModel.ReporteMantenimientoAldoConti;

namespace BatiaSuite.Views.ReporteMantenimientoAldoConti;

public partial class ReporteMantenimientoPage : ContentPage {
    public ReporteMantenimientoViewModel ViewModel { get; set; }
    public ReporteMantenimientoPage() {
        InitializeComponent();

        ViewModel = new ReporteMantenimientoViewModel();
        BindingContext = ViewModel;

        ReporteMantenimientoViewModel.OnClearTiendaRequested += LimpiarLienzoTienda;
        ReporteMantenimientoViewModel.OnClearResponsableRequested += LimpiarLienzoResponsable;
    }

    protected override void OnAppearing() {
        base.OnAppearing();
        if(ViewModel != null) {
            ViewModel.AntesDeEnviarChecklist = InterceptaryCapturarFirmasAsync;
        }
    }


    // Evento del botón Limpiar del Responsable
    private void OnBtnLimpiarResponsableClicked(object sender, EventArgs e) {
        LimpiarLienzoResponsable();
    }

    // Evento del botón Limpiar del Tienda
    private void OnBtnLimpiarTiendaClicked(object sender, EventArgs e) {
        LimpiarLienzoTienda();
    }

    private void LimpiarLienzoResponsable() {
        MainThread.BeginInvokeOnMainThread(() => {
            PadResponsable.Lines.Clear();
        });
    }

    private void LimpiarLienzoTienda() {
        MainThread.BeginInvokeOnMainThread(() => {
            PadTienda.Lines.Clear();
        });
    }

    private async Task<bool> InterceptaryCapturarFirmasAsync() {
        if(!PadResponsable.Lines.Any()) {
            await DisplayAlert("Firma Requerida", "Por favor, ingresa la firma del Aparadorista.", "OK");
            return false;
        }

        if(!PadResponsable.Lines.Any()) {
            await DisplayAlert("Firma Requerida", "Por favor, ingresa la firma del Gerente de la Tienda.", "OK");
            return false;
        }

        try {
            var streamResponsable = await PadResponsable.GetImageStream(300, 150);
            var streamTienda = await PadTienda.GetImageStream(300, 150);

            using(var ms1 = new MemoryStream()) {
                await streamResponsable.CopyToAsync(ms1);
                ViewModel.FirmaResponsableBytes = ms1.ToArray();
            }

            using(var ms2 = new MemoryStream()) {
                await streamTienda.CopyToAsync(ms2);
                ViewModel.FirmaTiendaBytes = ms2.ToArray();
            }

            return true;
        } catch(Exception ex) {
            await DisplayAlert("Error", $"No se pudieron procesar las firmas: {ex.Message}", "OK");
            return false;
        }
    }

    protected override void OnDisappearing() {
        base.OnDisappearing();
        ReporteMantenimientoViewModel.OnClearResponsableRequested -= LimpiarLienzoResponsable;
        ReporteMantenimientoViewModel.OnClearTiendaRequested -= LimpiarLienzoTienda;

        if(ViewModel != null) {
            ViewModel.AntesDeEnviarChecklist = null;
        }
    }

    private void PadResponsable_DrawingLineCompleted(object sender, CommunityToolkit.Maui.Core.DrawingLineCompletedEventArgs e) {
        // Evitamos que el componente limpie el lienzo añadiendo el trazo actual de forma permanente
        if(e.LastDrawingLine != null && !PadResponsable.Lines.Contains(e.LastDrawingLine)) {
            PadResponsable.Lines.Add(e.LastDrawingLine);
        }
    }
}