using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Models {

    public class ListCorrecM {
        public int IdCorrectivoLocal { get; set; }
        public int idClaveCM { get; set; }
        public int idInmueble { get; set; }
        public int idCliente { get; set; }
        public string tipo { get; set; }

        public string cliente { get; set; }
        public string inmueble { get; set; }
        public string estatus { get; set; }
        public string fregistro { get; set; }
        public string desTrabajos { get; set; }
        public DateTime SyncDate { get; internal set; }
    }
}