using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Models
{

    public class RegistrosCorrectivosMModel
    {
        public int IdClaveCM { get; set; }
        public int TrabajosGeneral { get; set; }
        public int TecnicosUniforme { get; set; }
        public int TratoTecnicos { get; set; }
        public int TrabajosOrden { get; set; }
        public int MaterialesAdecuados { get; set; }
        //public string FirmaCliente { get; set; }
        public string Encuestado { get; set; }
    }
}
