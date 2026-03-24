using BatiaSuite.Utils;
using System.Text.Json.Serialization;

namespace BatiaSuite.Models.Encuestas;

public class Reporte {
    public int IdOrden { get; set; }

    int _trabajosGeneral;
    public int TrabajosGeneral {
        get {
            if(_trabajosGeneral == 0) {
                ErrorMessage = Constants.RESPONDA_TODAS_PREGUNTAS;
            }
            return _trabajosGeneral;
        }
        set {
            if(_trabajosGeneral == value) {
                return;
            }
            _trabajosGeneral = value;
        }
    }

    int _tecnicosUniforme;
    public int TecnicosUniforme {
        get {
            if(_tecnicosUniforme == 0) {
                ErrorMessage = Constants.RESPONDA_TODAS_PREGUNTAS;
            }
            return _tecnicosUniforme;
        }
        set {
            if(_tecnicosUniforme == value) {
                return;
            }
            _tecnicosUniforme = value;
        }
    }

    int _tratoTecnicos;
    public int TratoTecnicos {
        get {
            if(_tratoTecnicos == 0) {
                ErrorMessage = Constants.RESPONDA_TODAS_PREGUNTAS;
            }
            return _tratoTecnicos;
        }
        set {
            if(_tratoTecnicos == value) {
                return;
            }
            _tratoTecnicos = value;
        }
    }

    int _trabajosOrden;
    public int TrabajosOrden {
        get {
            if(_trabajosOrden == 0) {
                ErrorMessage = Constants.RESPONDA_TODAS_PREGUNTAS;
            }
            return _trabajosOrden;
        }
        set {
            if(_trabajosOrden == value) {
                return;
            }
            _trabajosOrden = value;
        }
    }

    int _materialesAdecuados;
    public int MaterialesAdecuados {
        get {
            if(_materialesAdecuados == 0) {
                ErrorMessage = Constants.RESPONDA_TODAS_PREGUNTAS;
            }
            return _materialesAdecuados;
        }
        set {
            if(_materialesAdecuados == value) {
                return;
            }
            _materialesAdecuados = value;
        }
    }

    string _encuestado;
    public string Encuestado {
        get {
            if(string.IsNullOrWhiteSpace(_encuestado)) {
                ErrorMessage = Constants.INGRESE_ENCUESTADO;
            }
            return _encuestado;
        }
        set {
            if(_encuestado == value) {
                return;
            }
            _encuestado = value;
        }
    }

    [JsonIgnore]
    public string ErrorMessage { get; set; } = Constants.RESPONDA_TODAS_PREGUNTAS;

    [JsonIgnore]
    public bool IsValid {
        get => !(TrabajosGeneral == 0 || TecnicosUniforme == 0 || TratoTecnicos == 0 
            || TrabajosOrden == 0 || MaterialesAdecuados == 0 || string.IsNullOrWhiteSpace(Encuestado));
    }
}
