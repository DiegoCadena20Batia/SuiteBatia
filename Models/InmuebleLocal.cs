using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Models
{
    public class InmuebleLocal
    {
        public int IdInmueble { get; set; }
        public string Nombre { get; set; }
        public int Tipo { get; set; }
        [Column("id_cliente")]
        public int IdCliente { get; set; }
        public int IdEstado { get; set; }
        public bool AreaBanco { get; set; }
    }
}
