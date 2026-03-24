using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Models
{
    public class RegisterMaterialsModel
    {
        public int Usuario { get; set; }
        public string NombreRecibe { get; set; }
        public string ComentarioMateriales { get; set; }
        public int Bidones { get; set; }
        public int IdListado { get; set; }
        public Materiale[] Materiales { get; set; }
        public DateTime Fentrega { get; set; }

        public class Materiale
        {
            public int Entregado { get; set; }
            public int Cantidad { get; set; }
            public string Clave { get; set; }
        }
    }
}
