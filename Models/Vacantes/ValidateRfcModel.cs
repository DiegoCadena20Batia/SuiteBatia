namespace BatiaSuite.Models.Vacantes;

public class ValidateRfcModel {
    public string Nombre { get; set; }
    public string PrimerApellido { get; set; }
    public string SegundoApellido { get; set; }
    public string Fecha { get; set; }
}

public class ValidateCurpModel {
    public string Nombre { get; set; }
    public string PrimerApellido { get; set; }
    public string SegundoApellido { get; set; }
    public string Fecha { get; set; }
    public string Genero { get; set; }
    public string Entidad { get; set; }
}

public class ValidateCurpRfcNss {
    public int StatusCode { get; set; }
    public string Message { get; set; }
}
