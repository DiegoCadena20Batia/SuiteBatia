using BatiaSuite.Interfaz;
using CommunityToolkit.Mvvm.Messaging;
using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Services {
    public class AutoSyncService : IAutoSyncService {
        private readonly IEnumerable<ISyncHandler> _handlers;

        // Inyecta automáticamente la colección de todos los ISyncHandler registrados en DI
        public AutoSyncService(IEnumerable<ISyncHandler> handlers) {
            _handlers = handlers;
        }

        public async Task SincronizarTodoAsync() {
            var resultados = new Dictionary<string, int>();

            // 1. Recorrer y ejecutar cada sincronizador disponible
            foreach(var handler in _handlers) {
                try {
                    int cantidad = await handler.SincronizarPendientesAsync();
                    if(cantidad > 0) {
                        resultados[handler.NombreEntidad] = cantidad;
                    }
                } catch(Exception ex) {
                    System.Diagnostics.Debug.WriteLine($"[AutoSyncService_Error] Error al sincronizar {handler.NombreEntidad}: {ex.Message}");
                }
            }

            int totalGeneral = resultados.Values.Sum();

            // 2. Si hubo registros subidos, notificar al usuario y actualizar la UI
            if(totalGeneral > 0) {
                string descripcion = ConstruirMensajeNotificacion(resultados);
                await NotificarUsuarioAsync(descripcion);

                MainThread.BeginInvokeOnMainThread(() => {
                    WeakReferenceMessenger.Default.Send("SyncCompletado");
                });
            } else {
                System.Diagnostics.Debug.WriteLine("[AutoSyncService] No se encontraron registros pendientes en ningún módulo.");
            }
        }

        private string ConstruirMensajeNotificacion(Dictionary<string, int> resultados) {
            var lineas = resultados.Select(r =>
                $"{r.Value} {r.Key}{(r.Value > 1 ? "es" : "")}");

            return $"Se enviaron correctamente: {string.Join(", ", lineas)}. 👍";
        }

        private async Task NotificarUsuarioAsync(string descripcion) {
            var notificacion = new NotificationRequest {
                NotificationId = 1001,
                Title = "BatiaSuite - Sincronización Exitosa",
                Description = descripcion,
                BadgeNumber = 0,
                Schedule = new NotificationRequestSchedule {
                    NotifyTime = DateTime.Now
                },
                Android = new Plugin.LocalNotification.AndroidOption.AndroidOptions { }
            };

            await LocalNotificationCenter.Current.Show(notificacion);
        }
    }
}
