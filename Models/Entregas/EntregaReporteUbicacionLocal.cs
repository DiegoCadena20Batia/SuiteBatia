using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Models.Entregas
{
    public class EntregaReporteUbicacionLocal
    {
        public int IdLocal { get; set; }
        public int IdPersonal { get; set; }
        public int IdInmueble { get; set; }
        public string? Latitud { get; set; }
        public string? Longitud { get; set; }
        public int IdListado { get; set; }
        public int IdTipo { get; set; }
        public DateTime Fecha { get; set; }
    }
}
