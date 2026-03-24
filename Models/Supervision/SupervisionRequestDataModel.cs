using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BatiaSuite.Models.Supervision;

public class SupervisionRequestDataModel : INotifyPropertyChanged {

    public event PropertyChangedEventHandler PropertyChanged;   

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public int IdOrden { get; set; }
    public int Usuario { get; set; }
    public DateTime Fechaini { get; set; }
    public DateTime Fechafin { get; set; }
    public int Id_Cliente { get; set; }
    public int Id_Inmueble { get; set; }
    [JsonIgnore]
    public bool AreaBanco { get; set; }

    [JsonIgnore]
    public string Cliente { get; set; }
    [JsonIgnore]
    public string Inmueble { get; set; }
    [JsonIgnore]
    public TipoSucursal TipoSucursal { get; set; }
    [JsonIgnore]
    public int Anio { get; set; }
    [JsonIgnore]
    public int Mes { get; set; }


    public string Latitud { get; set; }
    public string Longitud { get; set; }



    [JsonIgnore]
    public List<SupervisionPregunta> PreguntasSeccion1 { get; set; }
    [JsonIgnore]
    public List<SupervisionPregunta> PreguntasSeccion2 { get; set; }
    [JsonIgnore]
    public List<SupervisionPregunta> PreguntasSeccion3 { get; set; }
    [JsonIgnore]
    public List<SupervisionPregunta> PreguntasSeccion4 { get; set; }
    public List<SupervisionPregunta> Preguntas { get; set; }



    [JsonProperty("Evaluacion")]
    public List<Evaluacion> PreguntasEvaluacion { get; set; }



    [JsonProperty("EvaluacionOperador")]
    public List<ChecklistPregunta> ChecklistPreguntas { get; set; }
    public string NombreOperador { get; set; }
    [JsonIgnore]
    public string PathFirmaOperador { get; set; }



    [JsonIgnore]
    public List<ArchivoModel> FotosPantalla1 { get; set; }
    [JsonIgnore]
    public List<ArchivoModel> FotosPantalla2 { get; set; }
    [JsonIgnore]
    public List<ArchivoModel> FotosPantalla3 { get; set; }
    [JsonIgnore]
    public List<ArchivoModel> FotosPantalla4 { get; set; }
    [JsonIgnore]
    public List<ArchivoModel> FotosPantalla5 { get; set; }
    public List<ArchivoModel> FotosPantalla6 { get; set; }



    [JsonIgnore]
    public string PathVideo { get; set; }
    public List<ListadoMaterial> ListadoMateriales { get; set; }



    public List<ArchivoModel> Archivos { get; set; }




    bool _clienteentrevista;
    public bool Clienteentrevista {
        get => _clienteentrevista;
        set {
            if(_clienteentrevista == value) {
                return;
            }
            _clienteentrevista = value;

            OnPropertyChanged();

            if(!_clienteentrevista) {
                Clientenombre = null;
                Clientecomentario = null;
                Evalua = 1;
                Evalua = 0;
                Trabrealizados = 1;
                Trabrealizados = 0;
                Tratopersonal = 1;
                Tratopersonal = 0;
                Uniformcompleto = false;
                Suprecorrido = false;
                Areaoportunidad = false;
                Plancorrectivo = false;
                Calificasup = 1;
                Calificasup = 0;
                //Ejecutivocgo = 1;
                //Ejecutivocgo = 0;
                Reporteasiscgo = false;
                Matetiquetados = false;
                Matrequerimientos = false;
            }
        }
    }
    public string Clientenombre { get; set; }
    public string Clientecomentario { get; set; }
    public int Evalua { get; set; }
    public int Trabrealizados { get; set; }
    public int Tratopersonal { get; set; }
    public bool Uniformcompleto { get; set; }
    public bool Suprecorrido { get; set; }
    public bool Areaoportunidad { get; set; }
    public bool Plancorrectivo { get; set; }
    public int Calificasup { get; set; }
    //public int Ejecutivocgo { get; set; }
    public bool Reporteasiscgo { get; set; }
    public bool Matetiquetados { get; set; }
    public bool Matrequerimientos { get; set; }   
}