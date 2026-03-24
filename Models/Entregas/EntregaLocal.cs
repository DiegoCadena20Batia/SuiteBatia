using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Models.Entregas {
    public class EntregaLocal {
        public int IdLocal { get; set; }
        public int Usuario { get; set; }
        public string NombreRecibe { get; set; }
        public string ComentarioMateriales { get; set; }
        public int Bidones { get; set; }
        public int IdListado { get; set; }
        public DateTime Fentrega { get; set; }
    }

    public class EntregaMaterialLocal {
        public int IdEntregaLocal { get; set; }
        public int Entregado { get; set; }
        public int Cantidad { get; set; }
        public string Clave { get; set; }
    }

    public class FotoEntregaLocal {
        public int IdEntregaLocal { get; set; }
        public string Path { get; set; }
    }

    public class EntregaLocalModel {
        public EntregaLocal Header { get; set; }
        public List<EntregaMaterialLocal> Materiales { get; set; }
        public List<FotoEntregaLocal> Archivos { get; set; }
    }

}
