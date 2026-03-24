using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BatiaSuite.Models.SupervisionMantenimiento {
    public class SupervisionMantenimientoModel {
        public int IdCliente { get; set; }
        public int IdInmueble { get; set; }
        public int IdPersonal { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string? Observaciones { get; set; }
        public string? Latitud { get; set; }
        public string? Longitud { get; set; }
        [JsonIgnore]
        public List<SupervisionMantenimientoSeccionesModel>? Secciones { get; set; }
        public List<SupervisionMantenimientoPreguntasModel>? Preguntas { get; set; }
        public List<SupervisionMantenimientoFotosSeccionModel>? FotosSeccion { get; set; }
        public List<FirmaSupervisionMantenimientoModel>? FirmasBytes { get; set; }
        [JsonIgnore]
        public List<SupervisionMantenimientoHidrantesPreguntasModel>? HidrantesyAspersoresPreguntas { get; set; }
        [JsonIgnore]
        public List<SupervisionMantenimientoExtintoresPreguntasModel>? ExtintoresPreguntas { get; set; }
        public List<SupervisionMantenimientoHidrantesObjectModel>? HidrantesyAspersoresObjects { get; set; }
        public List<SupervisionMantenimientoExtintoresObjectModel>? ExtintoresObjects { get; set; }
    }

    public class SupervisionMantenimientoFotosSeccionModel {
        public int IdSeccion { get; set; }
        [JsonIgnore]
        public string? FotoPath { get; set; }
        public byte[]? FotoBytes { get; set; }
    }

    public class SupervisionMantenimientoSeccionesModel {
        public int IdSeccion { get; set; }
        public string? Seccion { get; set; }
        public bool Terminada { get; set; }
        public bool EsSeccionDeObjetos { get; set; }
    }   
    public partial class SupervisionMantenimientoPreguntasModel : ObservableObject {
        public int IdSeccion { get; set; }
        public int IdPregunta { get; set; }
        [JsonIgnore]
        public string? Pregunta { get; set; }
        [ObservableProperty]
        public int estado;
        public int DispositivosPorNivel { get; set; }
        public string? Comentarios { get; set; }
    }
    public class FirmaSupervisionMantenimientoModel {
        public int IdFirma { get; set; }
        public string? Nombre { get; set; }
        public byte[]? FirmaBytes { get; set; }
    }

    public partial class SupervisionMantenimientoHidrantesPreguntasModel : ObservableObject {
        public int IdPregunta { get; set; }
        public string? Pregunta { get; set; }
        [ObservableProperty]
        public int valor;
        [ObservableProperty]
        public string? comentarios;
    }
    public partial class SupervisionMantenimientoExtintoresPreguntasModel : ObservableObject {
        public int IdPregunta { get; set; }
        public string? Pregunta { get; set; }
        [ObservableProperty]
        public int valor;
        [ObservableProperty]
        public string? comentarios;
    }

    public partial class SupervisionMantenimientoHidrantesObjectModel : ObservableObject {
        [JsonIgnore]
        [ObservableProperty]
        public bool isSelected;
        public int IdConsec { get; set; }
        public string? ComentarioGeneral { get; set; }
        public List<SupervisionMantenimientoHidrantesObject>? Respuestas { get; set; }
        [JsonIgnore]
        public string? FotoPath { get; set; }
        public byte[]? FotoBytes { get; set; }
    }

    public partial class SupervisionMantenimientoExtintoresObjectModel : ObservableObject {
        [JsonIgnore]
        [ObservableProperty]
        public bool isSelected;
        public int IdConsec { get; set; }
        public string? ComentarioGeneral { get; set; }
        public List<SupervisionMantenimientoExtintoresObject>? Respuestas { get; set; }
        [JsonIgnore]
        public string? FotoPath { get; set; }
        public byte[]? FotoBytes { get; set; }
    }

    public class SupervisionMantenimientoHidrantesObject {
        public int IdPregunta { get; set; }
        [JsonIgnore]
        public string? Pregunta { get; set; }
        public int Estado { get; set; }
        public string? Comentarios { get; set; }

    }
    public class SupervisionMantenimientoExtintoresObject {
        public int IdPregunta { get; set; }
        [JsonIgnore]
        public string? Pregunta { get; set; }
        public int Estado { get; set; }
        public string? Comentarios { get; set; }
    }
}
