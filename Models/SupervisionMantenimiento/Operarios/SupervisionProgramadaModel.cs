using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Models.SipervisionesMantenimientoProgramadas
{
    class SupervisionProgramadaModel
    {
        public int IdOrden { get; set; }
        public int IdCliente { get; set; }
        public string Cliente { get; set; }
        public string Sucursal { get; set; }
        public string Falta { get; set; }
        public string Estatus { get; set; }
        public string TipoManto { get; set; }
        public string TipoOrden { get; set; }
        public string Descripcion { get; set; }
    }
}
