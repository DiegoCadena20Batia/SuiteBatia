using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Shiny;
using Shiny.Locations;

public class LocationTrackerService {
    private readonly IGpsManager _gpsManager;
    public bool IsTracking { get; private set; }
    public LocationTrackerService(IGpsManager gpsManager) {
        _gpsManager = gpsManager;
    }

    public async Task StartTrackingAsync(int idPersonal, int idInmueble) {
        var request = new GpsRequest {
            Accuracy = GpsAccuracy.Normal,  // Precisión balanceada
            DistanceFilterMeters = 50,      // Actualizar cada ~50 metros
            BackgroundMode = GpsBackgroundMode.Realtime
        };

        var access = await _gpsManager.RequestAccess(request);
        if(access != AccessState.Available)
            throw new Exception("Permisos de ubicación no concedidos");

        await _gpsManager.StartListener(request);
        IsTracking = true;
    }

    public async Task StopTrackingAsync() {
        await _gpsManager.StopListener();
        IsTracking = false;
    }
}