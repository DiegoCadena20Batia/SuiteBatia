using BatiaSuite.ViewModel.CheckListSupervisionesAldoConti;
using BatiaSuite.ViewModel.DiarioGerenteAldoConti;

namespace BatiaSuite.Views.DiarioGerenteAldoConti;

public partial class DiarioGerentePage : ContentPage
{
    public DiarioGerenteViewModel ViewModel { get; set; }
    public DiarioGerentePage()
	{
		InitializeComponent();

        ViewModel = new DiarioGerenteViewModel();
        BindingContext = ViewModel;

        DiarioGerenteViewModel.OnClearGerenteRequested += LimpiarLienzoGerente;
        DiarioGerenteViewModel.OnClearSupervisorRequested += LimpiarLienzoSupervisor;
    }

    protected override void OnAppearing() {
        base.OnAppearing();
        if(ViewModel != null) {
            ViewModel.AntesDeEnviarChecklist = InterceptaryCapturarFirmasAsync;
        }
    }


    // Evento del botón Limpiar del Supervisor
    private void OnBtnLimpiarSupervisorClicked(object sender, EventArgs e) {
        LimpiarLienzoSupervisor();
    }

    // Evento del botón Limpiar del Gerente
    private void OnBtnLimpiarGerenteClicked(object sender, EventArgs e) {
        LimpiarLienzoGerente();
    }

    private void LimpiarLienzoSupervisor() {
        MainThread.BeginInvokeOnMainThread(() => {
            PadSupervisor.Lines.Clear();
        });
    }

    private void LimpiarLienzoGerente() {
        MainThread.BeginInvokeOnMainThread(() => {
            PadGerente.Lines.Clear();
        });
    }

    private async Task<bool> InterceptaryCapturarFirmasAsync() {
        if(!PadSupervisor.Lines.Any()) {
            await DisplayAlert("Firma Requerida", "Por favor, ingresa la firma del Aparadorista.", "OK");
            return false;
        }

        if(!PadGerente.Lines.Any()) {
            await DisplayAlert("Firma Requerida", "Por favor, ingresa la firma del Gerente de la Tienda.", "OK");
            return false;
        }

        try {
            var streamSupervisor = await PadSupervisor.GetImageStream(300, 150);
            var streamGerente = await PadGerente.GetImageStream(300, 150);

            using(var ms1 = new MemoryStream()) {
                await streamSupervisor.CopyToAsync(ms1);
                ViewModel.FirmaSupervisorBytes = ms1.ToArray();
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
        DiarioGerenteViewModel.OnClearSupervisorRequested -= LimpiarLienzoSupervisor;
        DiarioGerenteViewModel.OnClearGerenteRequested -= LimpiarLienzoGerente;

        if(ViewModel != null) {
            ViewModel.AntesDeEnviarChecklist = null;
        }
    }

    private void PadSupervisor_DrawingLineCompleted(object sender, CommunityToolkit.Maui.Core.DrawingLineCompletedEventArgs e) {
        // Evitamos que el componente limpie el lienzo añadiendo el trazo actual de forma permanente
        if(e.LastDrawingLine != null && !PadSupervisor.Lines.Contains(e.LastDrawingLine)) {
            PadSupervisor.Lines.Add(e.LastDrawingLine);
        }
    }
}