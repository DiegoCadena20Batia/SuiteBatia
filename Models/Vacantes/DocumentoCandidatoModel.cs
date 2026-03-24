using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json.Serialization;

namespace BatiaSuite.Models.Vacantes;

public partial class DocumentoCandidatoModel : ObservableObject {
    public int IdDocumento { get; set; }
    public string Nombre { get; set; }
    public int Tamano { get; set; }
    public byte[] Image { get; set; }

    [JsonIgnore]
    [ObservableProperty]
    string _photoPath;

    [JsonIgnore]
    [ObservableProperty]
    string _nombreRegistro;
}
