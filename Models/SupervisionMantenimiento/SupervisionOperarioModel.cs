using BatiaSuite.Models.SupervisionMantenimiento.Operarios;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Models.SupervisionMantenimiento {

    public class SupervisionPayloadDto {
        public int IdOrdenSupervisionM { get; set; }
        public int IdPersonal { get; set; }
        public DateTime FechaIni { get; set; }
        public DateTime FechaFin { get; set; }
        public int IdCliente { get; set; }
        public int IdInmueble { get; set; }
        public string Latitud { get; set; } = string.Empty;
        public string Longitud { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;
        public int IdRol { get; set; }
        public int IdTipoServicio { get; set; }
        public string ResumenSupervision { get; set; }

        public List<InstanciaDto> Instancias { get; set; } = new();
        public List<FirmaDto> Firmas { get; set; } = new();
    }

    public class InstanciaDto {
        public int IdSeccion { get; set; }
        public string AreaPisoUbicacion { get; set; } = string.Empty;
        public int NumeroIteracion { get; set; }
        public List<RespuestaDto> Respuestas { get; set; } = new();
        public List<string> Fotos { get; set; } = new();
    }

    public class RespuestaDto {
        public int IdPregunta { get; set; }
        public int Estado { get; set; }
        public int DispNivel { get; set; }
        public string Comentarios { get; set; } = string.Empty;
    }

    public class FirmaDto {
        public int IdFirma { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Firmas { get; set; } = string.Empty;
    }
    public class SupervisionResponseDto {
        public bool Success { get; set; }
        public int Id_Supervisionm { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
    }
}