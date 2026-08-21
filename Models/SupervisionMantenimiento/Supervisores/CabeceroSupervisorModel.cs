using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Models.SupervisionMantenimiento.Supervisores
{
    public class CabeceroSupervisorModel
    {
        public int IdTipoServicio { get; set; }
        public int IdCliente { get; set; }
        public int IdInmueble { get; set; }
        public string Area { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public DateTime FechaAlta { get; set; }
        public string Observaciones { get; set; }
        public string ResumenSupervision { get; set; }
        public string NombreSupervisor { get; set; }
      
    }
}
