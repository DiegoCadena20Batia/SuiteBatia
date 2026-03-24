using BatiaSuite.Models.Supervision;

namespace BatiaSuite.Models.Sanitizacion;

public class SanitizacionModel {
    public int IdSanitizacion { get; set; }
    public int IdCliente { get; set; }
    public int IdInmueble { get; set; }
    public int IdUsuario { get; set; }
    public string Area { get; set; }
    public int Procedimiento { get; set; }
    public string Recibe { get; set; } 
    public DateTime Fregistro { get; set; } 
    public string Latitud { get; set; } = string.Empty; 
    public string Longitud { get; set; } = string.Empty; 
    public List<ArchivoModel> Imagenes { get; set; } 
}