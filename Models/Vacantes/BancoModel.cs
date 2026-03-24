using Newtonsoft.Json;

namespace BatiaSuite.Models.Vacantes;

public class BancoModel {

    [JsonProperty("id_banco")]
    public int IdBanco { get; set; }

    public string Descripcion { get; set; }
}