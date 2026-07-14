using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Models.NotificacionesSupervisores {
    public class FaltasResponseModel {
        public string IdPersonal { get; set; }
        public string Supervisor { get; set; }
        public List<EmpleadoFaltaModel> Empleados { get; set; } = new();
    }

    public class EmpleadoFaltaModel {
        public int IdEmpleado { get; set; }
        public string Nombre { get; set; }
    }
}
