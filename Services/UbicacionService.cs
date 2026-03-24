using BatiaSuite.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Services {
    public class UbicacionService {
        private Timer _timer;

        public void IniciarReporteUbicacion() {
            _timer = new Timer(async (e) =>
            {
                await ReportarUbicacion();
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(15));
        }

        private async Task ReportarUbicacion() {
            try {
                var location = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium));
                if(location != null) {
                    Console.WriteLine($"Ubicación actual: {location.Latitude}, {location.Longitude}");
                    // Aquí llamas a tu API o guardas localmente
                    await EnviarUbicacionAlServidor(location.Latitude, location.Longitude);
                    
                }
            } catch(Exception ex) {
                Console.WriteLine($"Error al obtener ubicación: {ex.Message}");
            }
        }
        public class LocationReport {
            public int IdPersonal { get; set; }
            public int IdInmueble { get; set; }
            public double Latitud { get; set; }
            public double Longitud { get; set; }
        }
        private async Task EnviarUbicacionAlServidor(double lat, double lng) {
            // Enviar reporte de ubicacion al backend
            string url = "https://www.singa.com.mx:8086/api/SeguimientoRuta";
            var data = new LocationReport {
                IdPersonal = UserSession.IdPersonal,
                IdInmueble = 1620,
                Latitud = lat,
                Longitud = lng
            };
            var json = JsonConvert.SerializeObject(data);

            using var http = new HttpClient();
            await http.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
        }

        public void DetenerReporte() {
            _timer?.Dispose();
        }
    }
}
