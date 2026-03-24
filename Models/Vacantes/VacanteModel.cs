using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace BatiaSuite.Models.Vacantes;

public partial class VacanteModel : ObservableObject {
    public int IdVacante { get; set; }
    public int IdUsuario { get; set; }


    public string ApellidoPaterno { get; set; }
    public string ApellidoMaterno { get; set; }
    public string Nombre { get; set; }
    public string FechaNacimiento { get; set; }
    public string LugarNacimiento { get; set; }
    public string Nacionalidad { get; set; }
    public int Genero { get; set; }
    public int EstadoCivil { get; set; }
    public string Curp { get; set; }
    public string Rfc { get; set; }
    public bool Pensionado { get; set; }
    [ObservableProperty]
    string _seguroSocial;
    public string TallaUniforme { get; set; }
    public string TallaCalzado { get; set; }
    public string FuenteReclutamiento { get; set; }


    [JsonProperty("Sueldo")]
    public int SalarioMensual { get; set; }
    public string FechaIngreso { get; set; }
    public int Banco { get; set; }
    public string Clabe { get; set; }
    public string Cuenta { get; set; }
    public string Tarjeta { get; set; }


    public string Calle { get; set; }
    public string NumeroExterior { get; set; }
    public string NumeroInterior { get; set; }
    public string Colonia { get; set; }
    public string CodigoPostal { get; set; }
    public string Municipio { get; set; }
    public int IdEstado { get; set; }
    public string Telefono { get; set; }
    [JsonIgnore]
    public string CorreoPersonal { get; set; }
    [JsonProperty("Contacto")]
    public string ContactoEmergencia { get; set; }
    [JsonProperty("TelefonoAd")]
    public string TelefonoEmergencia { get; set; }



    public string Callef { get; set; }
    public string Coloniaf { get; set; }
    public int? Cpf { get; set; }
    public string Municipiof { get; set; }
    public int IdEstadof { get; set; }



    public int GradoEstudio { get; set; }
    public bool TieneHijos { get; set; }
    [ObservableProperty]
    int? _cantHijos;
    [ObservableProperty]
    bool _dependeEconomico;
    public bool SabLeer { get; set; }
    public bool SabEscribir { get; set; }
    public bool TelIntel { get; set; }
    public int FormComunicacion { get; set; }
    [ObservableProperty]
    string _formComunDesc = "";
    public int Transporte { get; set; }
    [ObservableProperty]
    string _transporteDesc;
    public int TransporteUnidad { get; set; }
    public bool TransporteBono { get; set; }
    public string TransporteGasto { get; set; }
    public bool BonoPuntual { get; set; }



    public bool Excepcion { get; set; }
    [JsonIgnore]
    public int SueldoVacante { get; set; }

    public List<DocumentoCandidatoModel> ArchivosApp { get; set; }
}
