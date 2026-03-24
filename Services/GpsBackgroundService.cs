using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Shiny.Locations;
using System.Diagnostics;

public class GpsBackgroundService : IGpsDelegate {
    public async Task OnReading(Shiny.Locations.GpsReading reading) {
        var lat = reading.Position.Latitude;
        var lon = reading.Position.Longitude;
        var fecha = DateTime.UtcNow;

        Console.WriteLine($"[GPS] Coordenadas recibidas: {lat}, {lon} - {fecha}");

        // Aquí puedes guardar en tu base de datos o enviar al servidor
        // await _dbContext.InsertUbicacionAsync(new Ubicacion { ... });
    }

    public void OnError(Exception ex) {
        Console.WriteLine($"[GPS ERROR] {ex.Message}");
    }
}