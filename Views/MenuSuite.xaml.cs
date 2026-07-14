using BatiaSuite.Utils;
using Plugin.LocalNotification;

namespace BatiaSuite.Views;

public partial class MenuSuite : ContentPage {
    public MenuSuite() {
        InitializeComponent();
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e) {
        Frame selectedFrame = (Frame)sender;

        selectedFrame.BackgroundColor = Color.FromArgb("#FFC8C8C8");
        await Task.Delay(100);
        selectedFrame.BackgroundColor = Color.FromArgb("#FFFFFF");
    }

    protected override void OnAppearing() {
        base.OnAppearing();

        bool esSupervisor = UserSession.IdPuesto == 118;
        if(esSupervisor) {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try {
                    bool habilitadas = await LocalNotificationCenter.Current.AreNotificationsEnabled();
                    if(!habilitadas) {
                        // Al estar la pantalla 100% activa, el diálogo nativo se mostrará sin problemas
                        await LocalNotificationCenter.Current.RequestNotificationPermission();
                    }
                } catch(Exception ex) {
                    System.Diagnostics.Debug.WriteLine($"Error al solicitar permisos: {ex.Message}");
                }
            });
        }
    }
}