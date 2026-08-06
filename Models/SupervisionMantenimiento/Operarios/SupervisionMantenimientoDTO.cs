using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Models.SupervisionMantenimiento.Operarios {
    public class SupervisionMantenimientoDTO {
        public int IdSupervision { get; set; }
        public int IdOrden { get; set; }
        public int IdPersonal { get; set; }
        public DateTime Fechainicio { get; set; }
        public DateTime Fechafin { get; set; }
        public int IdCliente { get; set; }
        public int IdInmueble { get; set; }
        public string Observaciones { get; set; } = string.Empty;
        public string Latitud { get; set; } = string.Empty;
        public string Longitud { get; set; } = string.Empty;

        // Listas compuestas
        public List<RespuestasDTO> Preguntas { get; set; } = new();
        public List<FotosSeccionDTO> FotosSeccion { get; set; } = new();
        public List<FirmasSeccionDTO> FirmasBytes { get; set; } = new();

        
    }
}
