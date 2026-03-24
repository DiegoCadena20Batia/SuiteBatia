using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CommunityToolkit.Maui.Core.Platform;
using Microsoft.Maui.Devices.Sensors;

namespace BatiaSuite.Utils {
    public class BackgroundLocationService {
        ////private IGeolocationListener listener;
        //private bool _isRunning;

        //public async Task StartTrackingAsync() {
        //    if(_isRunning) return;

        //    // 1. Pedir permisos
        //    var status = await Permissions.RequestAsync<Permissions.LocationAlways>();
        //    if(status != PermissionStatus.Granted) return;

        //    // 2. Configurar solicitud de ubicación
        //    var request = new GeolocationListeningRequest(
        //        GeolocationAccuracy.Best,
        //        TimeSpan.FromMinutes(15));

        //    // 3. Iniciar listener (corrección importante aquí)
        //    listener = await Geolocation.Default.StartListeningForegroundAsync(request);

        //    listener.LocationChanged += (sender, e) =>
        //    {
        //        var location = e.Location;
        //        // Procesar la nueva ubicación
        //        Console.WriteLine($"Nueva ubicación: {location.Latitude}, {location.Longitude}");
        //        _ = SendLocationToServer(location);
        //    };

        //    listener.Error += (sender, e) =>
        //    {
        //        Console.WriteLine($"Error en GPS: {e.Exception.Message}");
        //    };

        //    _isRunning = true;
        //}

        //public async Task StopTrackingAsync() {
        //    if(!_isRunning) return;

        //    await Geolocation.Default.StopListeningForegroundAsync(listener);
        //    _isRunning = false;
        //}

        //private async Task SendLocationToServer(Location location) {
        //    string url = "https://www.singa.com.mx:8086/api/SeguimientoRuta";
        //    var data = new LocationReport {
        //        IdPersonal = UserSession.IdPersonal,
        //        IdInmueble = 1620,
        //        Latitud = location.Latitude,
        //        Longitud = location.Longitude
        //    };
        //    var json = JsonConvert.SerializeObject(data);

        //    using var http = new HttpClient();
        //    await http.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
        //}
    }
}

public class LocationReport {
    public int IdPersonal { get; set; }
    public int IdInmueble { get; set; }
    public double Latitud { get; set; }
    public double Longitud { get; set; }
}