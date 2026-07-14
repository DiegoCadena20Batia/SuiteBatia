using BatiaSuite.Models.NotificacionesSupervisores;
using BatiaSuite.Utils;
using BatiaSuite.Utils.NotificacionesSupervisor;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Plugin.LocalNotification;
using System.Collections.ObjectModel;

namespace BatiaSuite.ViewModel.NotificacionesSupervisores {
    public partial class CentroNotificacionesSupervisorViewModel : ObservableObject {
        private readonly HttpHelper _httpHelper;

        [ObservableProperty]
        private ObservableCollection<EmpleadoFaltaModel> _empleadosQueFaltaron;

        [ObservableProperty]
        private bool _isBusy;

        public CentroNotificacionesSupervisorViewModel() {
            _httpHelper = new HttpHelper();
            EmpleadosQueFaltaron = new ObservableCollection<EmpleadoFaltaModel>();

            Task.Run(async () => await CargarFaltasAsync());
        }

        [RelayCommand]
        private async Task CargarFaltasAsync() {
            if(IsBusy) return;

            try {
                IsBusy = true;

                int idSupervisor = UserSession.IdPersonal;

                string url = $"{Constants.API_BASE_URL}FaltasEmpleados?idsupervisor={idSupervisor}";

                var resultado = await _httpHelper.GetAsync<FaltasResponseModel>(url);

                MainThread.BeginInvokeOnMainThread(() => {
                    EmpleadosQueFaltaron.Clear();
                    if(resultado?.Empleados != null) {
                        foreach(var emp in resultado.Empleados) {
                            EmpleadosQueFaltaron.Add(emp);
                        }
                    }

                    // 1. Actualizamos la campanita visual
                    WeakReferenceMessenger.Default.Send(new NotificationCountMessage(EmpleadosQueFaltaron.Count));

                    
                });
            } catch(Exception ex) {
                Console.WriteLine($"Error al cargar notificaciones en ViewModel: {ex.Message}");
            } finally {
                IsBusy = false;
            }
        }

        /* 🌟 Método corregido con la sintaxis exacta de tu librería por si lo usas en el futuro
        private async Task LanzarNotificacionLocalAsync(int totalFaltas) {
            string mensaje = totalFaltas == 1
                ? "Se ha registrado 1 falta de personal el día de hoy."
                : $"Se han registrado {totalFaltas} faltas de personal el día de hoy.";

            var request = new NotificationRequest {
                NotificationId = 2002,
                Title = "⚠️ Reporte de Ausentismo",
                Description = mensaje,
                BadgeNumber = totalFaltas,
                Schedule = { NotifyTime = DateTime.Now },
                Android = new Plugin.LocalNotification.AndroidOption.AndroidOptions {
                    Priority = Plugin.LocalNotification.AndroidOption.AndroidPriority.High,
                    ChannelId = "faltas_channel_id",
                    Icon = new Plugin.LocalNotification.AndroidOption.AndroidIcon { Name = "onfo" },
                    Color = new Plugin.LocalNotification.AndroidOption.AndroidColor("#007AFF")
                },
                iOS = new Plugin.LocalNotification.iOSOption.iOSOptions {
                    PlayForegroundSound = true
                }
            };

            await LocalNotificationCenter.Current.Show(request);
        }*/
    }
}