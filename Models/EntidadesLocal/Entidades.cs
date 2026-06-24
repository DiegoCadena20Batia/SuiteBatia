using BatiaSuite.Interfaz;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Models.EntidadesLocal {
    public class CatalogoCacheEntity : IDescargable {
        [PrimaryKey]
        public string Clave { get; set; } = string.Empty; 
        public string JsonData { get; set; } = string.Empty;
        public DateTime UltimaSincronizacion { get; set; }

        [Ignore] 
        public string ClaveCatalogo => "DiarioGerente";

        public string ObtenerUrlDescarga(string baseUrl, int clienteId) {
            // Esta entidad sabe que va a buscar la estructura del formulario
            return $"{baseUrl}estructura/DiarioGerente";
        }
    }

 
    public class InmuebleEntity : IDescargable {
        [PrimaryKey]
        public int IdInmueble { get; set; }
        public int IdCliente { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int IdEstado { get; set; }

        [Ignore] 
        public string ClaveCatalogo => string.Empty;
        public string ObtenerUrlDescarga(string baseUrl, int clienteId) {
            int idEstadoDefault = 0;
            return $"{baseUrl}Sucursales?idcliente={clienteId}&idestado={idEstadoDefault}";
        }
    }
}
