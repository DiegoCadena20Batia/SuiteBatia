using BatiaSuite.Interfaz;
using BatiaSuite.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Models.EntidadesLocal.RutasEntregas {

    public class RutasInmuebles : IDescargable {
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
        public string Mes { get; set; }
        public string Anio { get; set; }

        public string ClaveCatalogo => string.Empty;

        public string ObtenerUrlDescarga(string baseUrl, int ParametroId) {
            if(UserSession.IdPersonal != 0) {
                ParametroId = UserSession.IdPersonal;
                return $"{baseUrl}RutasOperador?idoperador={ParametroId}&mes={DateTime.Now.Month}&anio={DateTime.Now.Year}";
            } else {
                return "";
            }
        }
    }
}