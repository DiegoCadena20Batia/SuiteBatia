using BatiaSuite.Interfaz;
using BatiaSuite.Models.SupervisionMantenimiento.Operarios;
using BatiaSuite.Utils;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Models.EntidadesLocal.Supervisiones {
    [ModulosRequeridos(19, 20)]
    public class SeccionesSupervisionLocal : ICacheable {
        [PrimaryKey]
        public string Clave { get; set; } = string.Empty;
        public string JsonData { get; set; } = string.Empty;
        public DateTime UltimaSincronizacion { get; set; }

        public string ClaveCatalogo => "SeccionesSupervision";

        public string ObtenerUrlDescarga(string baseUrl, int parametro) {
            return $"{baseUrl}SupervisionMantenimeintoChecklist?id_rol={UserSession.IdRol}";
        }

        public void CargarDatosCache(string rawJson) {
            Clave = nameof(SeccionesSupervisionLocal); 
            JsonData = rawJson;
            UltimaSincronizacion = DateTime.Now;
        }
    }
}