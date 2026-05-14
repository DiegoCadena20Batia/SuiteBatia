
namespace BatiaSuite.Models.OrdenesTrabajo;

public class OrdenTrabajoModel {
    public int idOrden { get; set; }
    public int idCliente { get; set; }
    public string sucursal { get; set; }
    public string cliente { get; set; }
    public string falta { get; set; }
    public string status { get; set; }
    public string tipomanto { get; set; }
    public string tipoOrden { get; set; }
    public string descripcion { get; set; }
    public DateTime SyncDate { get; internal set; }
}