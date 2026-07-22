using CommunityToolkit.Mvvm.Messaging;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace BatiaSuite.Utils.NotificacionesSupervisor {
    public class SignalRService {
        private readonly string _hubUrl = "https://www.singa.com.mx:8086/notificacionesHub";
        private readonly HubConnection _hubConnection;

        public SignalRService(string idSupervisor) {
            // 1. Configurar la URL con el ID del supervisor como parámetro
            string urlConParametro = $"{_hubUrl}?supervisorId={idSupervisor}";

            // 2. Construir la conexión (sin iniciarla)
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(urlConParametro, options => {
                    options.Transports = HttpTransportType.LongPolling;

                    options.HttpMessageHandlerFactory = (handler) => {
                        if(handler is HttpClientHandler clientHandler) {
                            // Bypass de verificación SSL para desarrollo local/pruebas
                            clientHandler.ServerCertificateCustomValidationCallback =
                                (message, cert, chain, errors) => true;
                        }
                        return handler;
                    };
                })
                .WithAutomaticReconnect()
                .Build();

            // 3. REGISTRAR LOS LISTENERS (Indispensable hacerlo ANTES de llamar a StartAsync)
            _hubConnection.On<int>("RecibirAlertaFaltas", async (totalFaltas) => {
                try {
                    Debug.WriteLine($"[SignalR_Debug] ¡Llegó el evento! Conteo recibido: {totalFaltas}");

                    // Le avisamos a la app para que pinte la campanita en la interfaz
                    WeakReferenceMessenger.Default.Send(new NotificationCountMessage(totalFaltas));

                    // Leemos cuántas vio la última vez el supervisor
                    int ultimoConteoLeido = Microsoft.Maui.Storage.Preferences.Get("UltimoConteoLeido", 0);

                    if(totalFaltas > ultimoConteoLeido) {
                        Debug.WriteLine($"[RACE_CHECK] {DateTime.Now:HH:mm:ss.fff} - SignalR ENCIENDE badge con {totalFaltas} y lanza notificación nativa.");

                        await LanzarNotificacionNativaAsync(totalFaltas);
                    } else {
                        Debug.WriteLine("[SignalR_Debug] Alerta omitida para evitar spam. El usuario ya vio estas notificaciones.");
                    }
                } catch(Exception ex) {
                    Debug.WriteLine($"[SignalR_Error] Error al procesar conteo en el cliente: {ex.Message}");
                }
            });

            Debug.WriteLine("[SignalR_Debug] Servicio inicializado y escuchas registradas.");
        }

        public async Task ConectarAsync() {
            try {
                if(_hubConnection.State == HubConnectionState.Disconnected) {
                    Debug.WriteLine("[SignalR_Debug] Intentando conectar al servidor de producción...");
                    await _hubConnection.StartAsync();
                    Debug.WriteLine($"[SignalR_Debug] ¡CONECTADO EXITOSAMENTE! Estado: {_hubConnection.State}");
                } else {
                    Debug.WriteLine($"[SignalR_Debug] No se inició conexión porque el estado actual es: {_hubConnection.State}");
                }
            } catch(Exception ex) {
                Debug.WriteLine($"[SignalR_Error] Error crítico al conectar al Hub: {ex.Message}");
                if(ex.InnerException != null) {
                    Debug.WriteLine($"[SignalR_Error] Detalle interno: {ex.InnerException.Message}");
                }
            }
        }

        private async Task LanzarNotificacionNativaAsync(int totalFaltas) {
            try {
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
                Debug.WriteLine("[SignalR_Debug] Notificación nativa local enviada con éxito al sistema operativo.");
            } catch(Exception ex) {
                Debug.WriteLine($"[SignalR_Error] Error al lanzar notificación nativa local: {ex.Message}");
            }
        }

        public async Task MarcarComoLeidasAsync(string idSupervisor) {
            try {
                if(_hubConnection is { State: HubConnectionState.Connected }) {
                    await _hubConnection.InvokeAsync("MarcarNotificacionesComoLeidas", idSupervisor);
                    Debug.WriteLine($"[SignalR_Debug] Solicitado reinicio de conteo para el supervisor: {idSupervisor}");
                } else {
                    Debug.WriteLine("[SignalR_Debug] No se puede marcar como leídas, SignalR no está conectado.");
                }
            } catch(Exception ex) {
                Debug.WriteLine($"[SignalR_Error] Error al invocar MarcarNotificacionesComoLeidas: {ex.Message}");
            }
        }
    }
}