using CommunityToolkit.Mvvm.Messaging;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Utils.NotificacionesSupervisor {

    public class SignalRService {
        private readonly string _hubUrl = "https://www.singa.com.mx:8086/notificacionesHub";
        private HubConnection _hubConnection;

        public SignalRService(string idSupervisor) // 🌟 Asegúrate de que reciba un string
        {
            // Mandamos el ID al hub de producción
            string urlConParametro = $"{_hubUrl}?supervisorId={idSupervisor}";

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(urlConParametro, options => {
                    // Mantenemos LongPolling si WebSockets te dio problemas antes, o déjalo por defecto
                    options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling;

                    options.HttpMessageHandlerFactory = (handler) => {
                        if(handler is HttpClientHandler clientHandler) {
                            clientHandler.ServerCertificateCustomValidationCallback =
                                (message, cert, chain, errors) => true;
                        }
                        return handler;
                    };
                })
                .WithAutomaticReconnect()
                .Build();


            _hubConnection.On<object>("RecibirAlertaFaltas", async (totalFaltasRaw) => {
                try {
                    Console.WriteLine($"[SignalR_Debug] ¡Llegó el evento! Contenido crudo: {totalFaltasRaw}");

                    int totalFaltas = 0;

                    if(totalFaltasRaw is System.Text.Json.JsonElement jsonElement) {
                        totalFaltas = jsonElement.GetInt32();
                    } else {
                        totalFaltas = Convert.ToInt32(totalFaltasRaw);
                    }

                    System.Diagnostics.Debug.WriteLine($"[SignalR_Debug] Conteo decodificado con éxito: {totalFaltas}");

                    WeakReferenceMessenger.Default.Send(new NotificationCountMessage(totalFaltas));


             
                    int ultimoConteoLeido = Microsoft.Maui.Storage.Preferences.Get("UltimoConteoLeido", 0);

                   
                    if(totalFaltas > 0 && totalFaltas > ultimoConteoLeido) {
                        System.Diagnostics.Debug.WriteLine("[SignalR_Debug] Hay alertas nuevas que el usuario no ha visto. Ejecutando LanzarNotificacionNativaAsync...");
                        await LanzarNotificacionNativaAsync(totalFaltas);
                    } else {
                        System.Diagnostics.Debug.WriteLine("[SignalR_Debug] Alerta omitida para evitar spam. El usuario ya vio estas notificaciones.");
                    }

                } catch(Exception ex) {
                    System.Diagnostics.Debug.WriteLine($"[SignalR_Error] Error al procesar conteo: {ex.Message}");
                }
            });
        }

        public async Task ConectarAsync() {
            try {
                if(_hubConnection.State == HubConnectionState.Disconnected) {
                    System.Diagnostics.Debug.WriteLine("[SignalR_Debug] Intentando conectar al servidor de producción...");
                    await _hubConnection.StartAsync();
                    System.Diagnostics.Debug.WriteLine($"[SignalR_Debug] ¡CONECTADO EXITOSAMENTE! Estado: {_hubConnection.State}");
                }
            } catch(Exception ex) {
                System.Diagnostics.Debug.WriteLine($"[SignalR_Error] Error crítico al conectar al Hub: {ex.Message}");
                if(ex.InnerException != null) {
                    System.Diagnostics.Debug.WriteLine($"[SignalR_Error] Detalle interno: {ex.InnerException.Message}");
                }
            }
        }

        private async Task LanzarNotificacionNativaAsync(int totalFaltas) {
            string mensaje = totalFaltas == 1
                ? "Se ha registrado 1 empleado con tres faltas consecutivas."
                : $"Se han registrado {totalFaltas} empleados con tres faltas consecutivas.";

            var request = new NotificationRequest {
                NotificationId = 2002,
                Title = "Notificación de Faltas",
                Description = mensaje,
                BadgeNumber = totalFaltas,
                Schedule = { NotifyTime = DateTime.Now },
            };

            await LocalNotificationCenter.Current.Show(request);
        }
    }
}