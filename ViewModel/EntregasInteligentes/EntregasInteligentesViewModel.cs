using BatiaSuite.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
namespace BatiaSuite.ViewModel.EntregasInteligentes;

using Microsoft.Maui.Controls.Handlers;
using Microsoft.Maui.Maps; // asegúrate de tener esto
using Newtonsoft.Json.Linq;
using BatiaSuite.Utils;
using BatiaSuite.Services;

public partial class EntregasInteligentesViewModel : ViewModelBase
{
    [ObservableProperty]
    ObservableCollection<Location> rutaCoordenadas = new();

    [ObservableProperty]
    bool _isLoading;

    [ObservableProperty]
    string _textLoading = "";
   
    [ObservableProperty]
    bool _isRefreshing;
    DbContext _dbContext;
    private readonly LocationTrackerService _tracker;

    [ObservableProperty]
    private bool _isTracking;

    public EntregasInteligentesViewModel(LocationTrackerService tracker)
    {
        _tracker = tracker;
        _dbContext = new DbContext();
        
    }
    private UbicacionService _ubicacionService = new();

    [RelayCommand]
    public async Task IniciarRuta() {
        // 1. Llamará al LocationTrackerService
        await _tracker.StartTrackingAsync(UserSession.IdPersonal, 1620);
    }

    [RelayCommand]
    public async Task FinalizarRuta() {
        await _tracker.StopTrackingAsync();
    }
    [RelayCommand]
    public async Task AbrirGoogleMaps() {
        string originLoc = "19.36478522971544, -99.17254690402716";
        string destinationLoc = "19.617355752814138, -99.28939833885762";

        string url = $"https://www.google.com/maps/dir/?api=1&origin={originLoc}&destination={destinationLoc}&travelmode=driving";

    try {
        await Launcher.Default.OpenAsync(new Uri(url));
    } catch(Exception ex) {
        // Manejar errores (ej: sin conexión, sin navegador, etc.)
        Console.WriteLine($"Error al abrir Google Maps: {ex.Message}");
    }
}
    [RelayCommand]
    public async Task AbrirWaze() {
        string originLoc = "19.36478522971544, -99.17254690402716";
        string destinationLoc = "19.617355752814138, -99.28939833885762";

        string destinationLat = "19.617355752814138";
        string destinationLng = "-99.28939833885762";

        string wazeUrl = $"https://waze.com/ul?ll={destinationLat},{destinationLng}&navigate=yes";
        try {
            await Launcher.Default.OpenAsync(new Uri(wazeUrl));
        } catch(Exception ex) {
            // Manejar errores (ej: sin conexión, sin navegador, etc.)
            Console.WriteLine($"Error al abrir Waze: {ex.Message}");
        }
    }




    [RelayCommand]
    public async Task ObtenerRuta() {
        var location = new Location(19.36478522971544, -99.17254690402716);

        string originLoc = "19.36478522971544, -99.17254690402716";
        string destinationLoc = "19.617355752814138, -99.28939833885762";
        string puntoMed = "19.496827918060387, -99.18182852337158";
        var url = "https://maps.googleapis.com/maps/api/directions/json?"
          + "origin= " + originLoc
          + "&destination=" + destinationLoc
          + "&waypoints=optimize:true|" + puntoMed
          + "&mode=driving"
          + "&departure_time=now"
          + "&traffic_model=best_guess"
          + "&key=AIzaSyBpoRJAkG1NFtHFuE3uTRbVnRcTG7ndz18";
        using var http = new HttpClient();
        var response = await http.GetStringAsync(url);

        var json = JObject.Parse(response);
        var points = json["routes"]?[0]?["overview_polyline"]?["points"]?.ToString();

        if(!string.IsNullOrEmpty(points)) {
            var decoded = PolylineDecoder.Decode(points);
            RutaCoordenadas.Clear();
            foreach(var coord in decoded) {
                RutaCoordenadas.Add(new Location(coord.Latitude, coord.Longitude));
            }
        }

    }


    [RelayCommand]
    async Task RefreshPage()
    {
        IniciarCarga("Cargando...");
        //await LoadListOffline();
        //await LoadList();
        DetenerCarga();
        IsRefreshing = false;
    }


    public void IniciarCarga(string mensaje)
    {
        IsLoading = true;
        TextLoading = mensaje;
    }

    public void DetenerCarga()
    {
        IsLoading = false;
        TextLoading = "";
    }
}