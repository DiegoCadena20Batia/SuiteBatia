using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Models
{
    public class LogueoModel
    {
        public int idPersonal { get; set; }
        public string per_Nombre { get; set; }
        public int idCliente { get; set; }
        public int cliente {  get; set; }
        public int idEmpleado { get; set; }
        public int idProveedor { get; set; }
        public int idPuesto { get; set; }
        public int idRol { get; set; }
        public List<int>? Modulos { get; set; }
    }
}
