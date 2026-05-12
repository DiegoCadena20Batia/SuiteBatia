using BatiaSuite.Models.Encuestas;
using Newtonsoft.Json;

namespace BatiaSuite.Models.OrdenesTrabajo;

public class OrdenTrabajoEjecutadaModel {
    public Trabajo Trabajo { get; set; }
    public IEnumerable<PersonalOrdenTrabajoRequest> Personal { get; set; }
    public IEnumerable<MaterialRequest> Material { get; set; }
    public Reporte Reporte { get; set; }

    [JsonIgnore]
    public IEnumerable<string> FotosList { get; set; }

    [JsonIgnore]
    public IEnumerable<string> FilesList { get; set; }
}

public class Trabajo {

    [JsonProperty("Id")]
    public int IdOrden { get; set; }

    public int IdCliente { get; set; }
    public string Fejecucion { get; set; }
    public string Trabejecutados { get; set; }

    [JsonIgnore]
    public string Cliente { get; set; }

    [JsonIgnore]
    public string Inmueble { get; set; }

    [JsonIgnore]
    public string TipoMantenimiento { get; set; }

    [JsonIgnore]
    public string Descripcion { get; set; }

    [JsonIgnore]
    public string Falta { get; set; }

    [JsonIgnore]
    public string Tipo { get; set; }
}

public class PersonalOrdenTrabajoRequest {
    public int IdEmpleado { get; set; }

    [JsonIgnore]
    public string Nombre { get; set; }

    public float Costo { get; set; }
    public float Horas { get; set; }
    public float Total { get => Costo * Horas; }
    public int Usuario { get; set; }
    public int IdUnidad { get; set; }
}

public class MaterialRequest {
    public int BtAlmacen { get; set; }
    public string Clave { get; set; }
    public string Descripcion { get; set; }
    public int Cantidad { get; set; }
    public int Cantcob { get; set; }
    public float Preciocob { get; set; }
    public float Total { get => IsCompra ? Cantidad * Preciocob : Cantcob * Preciocob; }
    public int IdAlmacen { get; set; }
    public int IdUnidad { get; set; }
    public int CantUtilizada { get; set; }

    [JsonIgnore]
    public UnidadMedidaModel Unidad { get; set; }

    [JsonIgnore]
    public bool IsCompra { get; set; }
}

public class UnidadMedidaModel {
    public int IdUnidad { get; set; }
    public string Descripcion { get; set; }
}

public class PersonalOrdenTrabajoResponse {
    public int idEmpleado { get; set; }
    public string nombre { get; set; }
    public float sueldo { get; set; }
}