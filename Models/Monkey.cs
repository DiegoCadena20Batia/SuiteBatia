

using Newtonsoft.Json;

namespace BatiaSuite.Models;

public class Monkey {
    public string Name { get; set; }
    public string Location { get; set; }
    public string Details { get; set; }
    public string ImageUrl { get; set; }
    public bool IsFavorite { get; set; }
    public int IdModulo { get; set; }
    public OrdenesSupervisionTotal OrdenesTotales { get; set; } = new OrdenesSupervisionTotal();
}

public class OrdenesSupervisionTotal {
    [JsonProperty("supTotales")]
    public double SupTotales { get; set; }
    [JsonProperty("supAlta")]
    public double SupNoRealizadas { get; set; }
    [JsonIgnore]
    public double SupRealizadas { get => SupTotales - SupNoRealizadas; }
    [JsonIgnore]
    public double Height { get => 105d; }
    [JsonIgnore]
    public double Width { get; set; } = 0;
    [JsonIgnore]
    public double PorcentajeSupRealizadas { get => SupTotales == 0 ? 0 : (SupRealizadas / SupTotales) * 100d; }
    [JsonIgnore]
    public double PorcentajeSupNoRealizadas { get => SupTotales == 0 ? 0 : (SupNoRealizadas / SupTotales) * 100d; }
    [JsonIgnore]
    public double HeightSupRealizadas { get => (PorcentajeSupRealizadas * Height) / 100d; }
    [JsonIgnore]
    public double HeightSupNoRealizadas { get => (PorcentajeSupNoRealizadas * Height) / 100d; }
}