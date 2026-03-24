using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Models.SolicitudCotizacion {
    public class SolicitudCotizacion {
        public int IdSolicitudCotizacion { get; set; }
        public int IdCliente { get; set; }
        public int IdInmueble { get; set; }
        public List<SolicitudCotizacionProductos> Productos { get; set; }
    }

    public class SolicitudCotizacionProductos {
        public string Clave { get; set; }
        public string Nombre { get; set; }
        public string Marca { get; set; }
        public int Unidad { get; set; }
        public int Cantidad { get; set; }
        public string FotoPath { get; set; }
        public bool MaterialFueraDeInventario { get; set; }

    }
}
