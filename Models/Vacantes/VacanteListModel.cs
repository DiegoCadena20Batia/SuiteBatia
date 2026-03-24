namespace BatiaSuite.Models.Vacantes;
public class VacanteListModel {
    public int IdVacante { get; set; }
    public string Cliente { get; set; }
    public string Inmueble { get; set; }
    public string Puesto { get; set; }
    public string Turno { get; set; }
    public int Jornal { get; set; }
    public int Sueldo { get; set; }
    public DateTime Falta { get; set; }
}
