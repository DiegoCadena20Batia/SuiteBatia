using BatiaSuite.Models.SupervisionMantenimiento.Operarios;
using CommunityToolkit.Maui.Views;

namespace BatiaSuite.Popups.SupervisionMantenimiento;

    public partial class CierreSupervisionPopup : Popup {
        public CierreSupervisionPopup() {
            InitializeComponent();
        }

        private void OnLimpiarFirmaClicked(object sender, EventArgs e) {
            drawingViewFirma.Lines.Clear();
        }

        private void OnCancelarClicked(object sender, EventArgs e) {
            Close(null); // Cierra el Popup regresando null
        }

        private async void OnConfirmarClicked(object sender, EventArgs e) {
            // Validaciones básicas
            if(string.IsNullOrWhiteSpace(txtNombreFirmante.Text)) {
                await Shell.Current.DisplayAlert("Atención", "Por favor ingrese el nombre del firmante.", "OK");
                return;
            }

            if(drawingViewFirma.Lines.Count == 0) {
                await Shell.Current.DisplayAlert("Atención", "Por favor capture la firma antes de continuar.", "OK");
                return;
            }

            try {
                // Convertir el trazado de la firma a un Stream PNG y luego a byte[]
                using var stream = await drawingViewFirma.GetImageStream(200, 100);
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                byte[] firmaBytes = memoryStream.ToArray();

                //var resultado = new CierreSupervisionResult {
                //    Observaciones = txtObservaciones.Text ?? string.Empty,
                //    NombreFirmante = txtNombreFirmante.Text.Trim(),
                //    FirmaBytes = firmaBytes
                //};

                //Close(resultado); // Cierra regresando los datos de la firma y observaciones
            } catch(Exception ex) {
                await Shell.Current.DisplayAlert("Error", $"No se pudo procesar la firma: {ex.Message}", "OK");
            }
        }
    }
