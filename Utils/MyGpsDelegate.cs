using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shiny;
using Shiny.Locations;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BatiaSuite.Data;
using BatiaSuite.Models.Entregas;


#if ANDROID
using Android.App;
#endif
namespace BatiaSuite.Utils;

public partial class MyGpsDelegate : GpsDelegate {
    readonly ILogger logger;
    private Location _ultimaUbicacionEnviada;
    private DateTime _ultimoEnvio = DateTime.MinValue;

    public MyGpsDelegate(ILogger<MyGpsDelegate> logger) : base(logger) {
        this.logger = logger;

        //this.MinimumDistance = Distance.FromMeters(50);
        this.MinimumTime = TimeSpan.FromMilliseconds(10000);
    }


    protected override Task OnGpsReading(GpsReading reading) {
        _ = Task.Run(() => ValidarYEnviarUbicacion(reading.Position.Latitude, reading.Position.Longitude));
        return Task.CompletedTask;
    }

    private async Task ValidarYEnviarUbicacion(double lat, double lng) {
        var nuevaUbicacion = new Location(lat, lng);
        var ahora = DateTime.UtcNow;

        // Validar si hay una ubicación previa
        if(_ultimaUbicacionEnviada != null) {
            var distancia = Location.CalculateDistance(_ultimaUbicacionEnviada, nuevaUbicacion, DistanceUnits.Kilometers);
            var segundos = (ahora - _ultimoEnvio).TotalSeconds;

            // Si no se cumplen las condiciones, no se envía
            if(distancia < 5) {
                Console.WriteLine($"Ubicación omitida (Distancia: {distancia} km, Tiempo: {segundos} s)");
                return;
            }
        }

        // Actualizar última ubicación y tiempo
        _ultimaUbicacionEnviada = nuevaUbicacion;
        _ultimoEnvio = ahora;

        await EnviarUbicacionAlServidor(lat, lng);
    }

    private async Task EnviarUbicacionAlServidor(double lat, double lng) {
        try {
            string url = Constants.API_BASE_URL + "SeguimientoRuta";
            var data = new {
                IdPersonal = UserSession.IdPersonal,
                IdInmueble = UserSession.IdInmuebleTracking,
                Latitud = lat,
                Longitud = lng,
                IdListado = 0,
                IdTipo = 2
            };

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            using var httpClient = new HttpClient();

            var response = await httpClient.PostAsync(url, content);
            if(response.IsSuccessStatusCode) {
                Console.WriteLine($"Ubicación enviada: {lat}, {lng}");
                return;
            } else {
                Console.WriteLine($"Error al enviar ubicación: {response.StatusCode}");
                await InsertarUbicacion(lat, lng);
                return;
            }
        } catch(Exception ex) when(ex is HttpRequestException || ex is TaskCanceledException) {
            Console.WriteLine($"Error al enviar ubicación local: {ex.Message}");
            await InsertarUbicacion(lat, lng);
            return;
        }
    }

    private async Task<bool> InsertarUbicacion(double lat, double lng) {
        try {
            var entrega = new EntregaReporteUbicacionLocal {
                IdPersonal = UserSession.IdPersonal,
                IdInmueble = UserSession.IdInmuebleTracking,
                Latitud = lat.ToString(),
                Longitud = lng.ToString(),
                IdListado = 0,
                IdTipo = 2,
                Fecha = DateTime.Now
            };
            var _dbContext = new DbContext();
            await _dbContext.InsertarUbicacionesEntrega(entrega);
            return true;
        }
        catch(Exception ex) {
            Console.WriteLine($"Error al insertar ubicación local: {ex.Message}");
            return false;
        }
    }
}