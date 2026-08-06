using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Models.SupervisionMantenimiento.Operarios {
    public class RespuestasDTO {
        public int IdSeccion { get; set; }
        public int IdPregunta { get; set; }
        public int Estado { get; set; } // "Bueno", "Malo", "N/A"
        public int DispositivosPorNivel { get; set; } = 0;
        public string Comentarios { get; set; } = string.Empty;
    }
}
