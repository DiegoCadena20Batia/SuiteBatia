using BatiaSuite.Utils.NotificacionesSupervisor;
using BatiaSuite.ViewModel.NotificacionesSupervisores;
using CommunityToolkit.Mvvm.Messaging;

namespace BatiaSuite.Views.NotificacionesSupervisores;

public partial class CentroNotificacionesSupervisor : ContentPage {
    private readonly CentroNotificacionesSupervisorViewModel _viewModel;

    public CentroNotificacionesSupervisor(CentroNotificacionesSupervisorViewModel viewModel) {
        InitializeComponent();

        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing() {
        base.OnAppearing();

        try {
            string idSupervisor = BatiaSuite.Utils.UserSession.IdPersonal.ToString();
            System.Diagnostics.Debug.WriteLine($"[Debug_Leido] ID del supervisor obtenido: {idSupervisor}");

            // Intentamos obtener el servicio de SignalR registrado
            var signalRService = App.Current.Handler?.MauiContext?.Services.GetService<SignalRService>();

            if(signalRService == null) {
                System.Diagnostics.Debug.WriteLine("[Debug_Leido] ¡ALERTA! El servicio SignalRService obtenido es NULL.");

                // Alternativa: Si guardas tu instancia de SignalRService de forma estática en App.xaml.cs, búscala ahí.
                // Ejemplo: signalRService = App.MiServicioSignalR;
                return;
            }

            System.Diagnostics.Debug.WriteLine("[Debug_Leido] Servicio SignalRService localizado con éxito. Enviando petición de lectura...");

            // Ejecutamos la petición de lectura
            await signalRService.MarcarComoLeidasAsync(idSupervisor);

            // Limpiamos la campana localmente
            WeakReferenceMessenger.Default.Send(new NotificationCountMessage(0));
        } catch(Exception ex) {
            System.Diagnostics.Debug.WriteLine($"[Debug_Leido] Excepción atrapada en OnAppearing: {ex.Message}");
        }
    }
}