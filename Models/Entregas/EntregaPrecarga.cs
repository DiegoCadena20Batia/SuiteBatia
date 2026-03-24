using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BatiaSuite.Models;

namespace BatiaSuite.Models.Entregas
{
    public class EntregaPrecarga
    {
        public List<ClienteEntregaPrecarga>? Clientes { get; set; }
        public List<InmuebleEntregaPrecarga>? Inmuebles { get; set; }
        public List<ListadoEntregaPrecarga>? Listados { get; set; }
        public List<ListadoMaterialEntregaPrecarga>? ListadosDetalle { get; set; }
    }

    public class ClienteEntregaPrecarga {
        public int IdCliente { get; set; }
        public string Nombre { get; set; }
    }
    public class InmuebleEntregaPrecarga {
        public int IdCliente { get; set; }
        public int IdInmueble { get; set; }
        public string Nombre { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
    }
    public class ListadoEntregaPrecarga {
        public int IdInmueble { get; set; }
        public int IdListado { get; set; }
        public string tipo { get; set; }
        public string estatus { get; set; }
        public DateTime falta { get; set; }
    }

    public class ListadoMaterialEntregaPrecarga {
        public int IdListado { get; set; }
        public string clave { get; set; }
        public string producto { get; set; }
        public int cantidad { get; set; }
        public int entregado { get; set; }

        public int EntregadoEntry {
            get { return cantidad; }
            set { entregado = value; }
        }

        public string unidad { get; set; }

        public void ModificarEntregado(int valor) {
            entregado = valor;
        }

    }
}
