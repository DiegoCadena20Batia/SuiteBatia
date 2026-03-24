using BatiaSuite.Utils;
using Newtonsoft.Json;
using Shiny.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Services {
    public class LocationDelegate : IGpsDelegate {
        private DateTime _ultimoEnvio = DateTime.MinValue;


        public async Task OnReading(GpsReading reading) {
            var ahora = DateTime.UtcNow;

            // Verificar si han pasado 15 minutos desde el último envío
            if((ahora - _ultimoEnvio).TotalMinutes < 15)
                return;

            // Obtener los datos de ubicación
            var position = reading.Position;
            var timestamp = reading.Timestamp.ToLocalTime();

            // Crear objeto con los datos del conductor
            var locationData = new {
                Latitude = position.Latitude,
                Longitude = position.Longitude,
                Timestamp = timestamp,
                Accuracy = reading.PositionAccuracy,
                Speed = reading.Speed
            };

            // Enviar al servidor
            await EnviarUbicacionAlServidor(position.Latitude, position.Longitude);
            _ultimoEnvio = ahora;
        }
    

        public async Task EnviarUbicacionAlServidor(double lat, double lng) {
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
    }
}

