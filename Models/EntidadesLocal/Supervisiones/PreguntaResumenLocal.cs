using BatiaSuite.Interfaz;
using BatiaSuite.Utils;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace BatiaSuite.Models.EntidadesLocal.Supervisiones {
    [ModulosRequeridos(20)]
    public class PreguntaResumenLocal : ICacheable {
        [PrimaryKey]
        public string Clave { get; set; } = string.Empty;
        public string JsonData { get; set; } = string.Empty;
        public DateTime UltimaSincronizacion { get; set; }

        public string ClaveCatalogo => "ResumenSupervision";

        public string ObtenerUrlDescarga(string baseUrl, int parametro) {
            return $"{baseUrl}SupervisionMantenimeintoChecklist/ResumenSupervision";
        }

        public void CargarDatosCache(string rawJson) {
            Clave = nameof(PreguntaResumenLocal);
            JsonData = rawJson;
            UltimaSincronizacion = DateTime.Now;
        }
    }
}
