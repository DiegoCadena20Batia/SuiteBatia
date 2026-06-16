using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Models.DiarioLimpieza {
    public class DiarioLimpiezaItem {
        public int TareaId { get; set; }
        public int Consecutivo { get; set; }
        public string Area { get; set; } = string.Empty;
        public string ActividadEspecifica { get; set; } = string.Empty;
        public string Frecuencia { get; set; } = string.Empty;
        public string ResponsableRol { get; set; } = string.Empty;

        public string NombrePersonalAsignado { get; set; } = string.Empty;
        public bool Cumple { get; set; } = false;
    }
}
