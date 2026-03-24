using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Models.SupervisionMantenimiento {
    public class SupervisionMantenimientoPrecargaModel {
        public List<SupervisionMantenimientoPrecargaClientes>? Clientes { get; set; }
        public List<SupervisionMantenimientoPrecargaInmuebles>? Inmuebles { get; set; }
        public List<SupervisionMantenimientoPrecargaClienteSeccion>? ClienteSeccion { get; set; }
        public List<SupervisionMantenimientoPrecargaSecciones>? Seccion { get; set; }
        public List<SupervisionMantenimientoPrecargaSeccionPregunta>? SeccionPregunta { get; set; }
    }

    public class SupervisionMantenimientoPrecargaClientes {
        public int IdCliente { get; set; }
        public int Cliente { get; set; }
    }

    public class SupervisionMantenimientoPrecargaInmuebles {
        public int IdCliente { get; set; }
        public int IdInmueble { get; set; }
        public int Inmueble { get; set; }
    }
    
    public class SupervisionMantenimientoPrecargaClienteSeccion {
        public int IdCliente { get; set; }
        public int IdSeccion { get; set; }
    }

    public class SupervisionMantenimientoPrecargaSecciones {
        public int IdSeccion { get; set; }
        public string? Seccion { get; set; }
    }

    public class SupervisionMantenimientoPrecargaSeccionPregunta {
        public int IdSeccion { get; set; }
        public int IdPregunta { get; set; }
        public string? Pregunta { get; set; }
    }
}
