using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Models.Supervision {
    public class ListadoMaterialLocal {
        public int IdConsec { get; set; }
        public int IdLocal { get; set; }
        public int IdStatusLocal { get; set; }
        public int IdListado { get; set; }
        public string Clave { get; set; }
        public string Descripcion { get; set; }
        public string Cantidad { get; set; }
        public int Entregado { get; set; }
        public int Sugerido { get; set; }
    }
}
