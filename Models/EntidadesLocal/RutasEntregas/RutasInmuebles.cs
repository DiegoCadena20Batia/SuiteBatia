using BatiaSuite.Interfaz;
using BatiaSuite.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Models.EntidadesLocal.RutasEntregas {
    [ModulosRequeridos(1,7)]
    public class RutasInmuebles: IDescargable {

        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int? IdLocal { get; set; } 

        [SQLite.Ignore]
        public bool IsCompleted { get; set; } = false;

        public int IdRuta { get; set; }
        public string Ruta { get; set; }
        public string Nomenclatura { get; set; }
        public int IdOperador { get; set; }
        public int IdInmueble { get; set; }
        public string Inmueble { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public int Consecutivo { get; set; }
        public int IdListado { get; set; }
        public string Tipo { get; set; }
        public string Estatusl { get; set; }
        public DateTime Falta { get; set; }
        public string Clave { get; set; }
        public string Producto { get; set; }
        public int Cantidad { get; set; }
        public int Entregado { get; set; }
        public string Unidad { get; set; }
        public int Mes { get; set; }
        public int Anio { get; set; }

        public string ObtenerUrlDescarga(string baseUrl, int Parametro) {
            return "";
        }
    }
}