using Mopups.Pages;

namespace BatiaSuite.Views.SolicitudCotizacion {
    public partial class AgregarProductoPopup : PopupPage {
        public AgregarProductoPopup() {
            InitializeComponent();
        }

        // Opcional: Para manejar cuando se cierra el popup
        protected override void OnDisappearing() {
            base.OnDisappearing();
            // Limpieza si es necesaria
        }
    }
}