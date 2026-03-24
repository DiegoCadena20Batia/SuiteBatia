using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Models.IncidenciasBiometa {
    public class IncidenciaBiometa {
        public string Id { get; set; }
        public int IdEmpleado { get; set; }
        public string Empleado { get; set; }
        public string Cliente { get; set; }
        public string Inmueble { get; set; }
        public string Movimiento { get; set; }
        public string CodigoDoblete { get; set; }
        public int IdTurno { get; set; }
        public DateTime FechaFormat { get; set; }
    }
}
