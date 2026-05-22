using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace BatiaSuite.Models.Supervision;

public class Inmueble {

    [JsonProperty("id_inmueble")]
    [JsonPropertyName("id_inmueble")]
    public int IdInmueble { get; set; }

    public string Nombre { get; set; }

    public int Tipo { get; set; }
}