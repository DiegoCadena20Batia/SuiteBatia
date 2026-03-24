using System.Text.Json.Serialization;

namespace BatiaSuite.Models.Supervision;

public class ArchivoModel {

    [JsonIgnore]
    public string Path { get; set; }
    private string _nombre;
    public string Nombre
    {
        get
        {
            if (!string.IsNullOrEmpty(_nombre))
                return _nombre;
            if(!string.IsNullOrEmpty(Path))
            {
                string[] tokens = Path.Split('/');
                return tokens[tokens.Length - 1];
            }
            return string.Empty;
        }
        set
        {
            _nombre = value;
        }
    }

    public int Tamano { get; set; }
    public int Seccion { get; set; }
}