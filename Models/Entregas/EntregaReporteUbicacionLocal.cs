using BatiaSuite.Interfaz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Models.Entregas
{
    public class EntregaReporteUbicacionLocal : ISincronizable {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int IdLocal { get; set; }
        public int IdPersonal { get; set; }
        public int IdInmueble { get; set; }
        public string? Latitud { get; set; }
        public string? Longitud { get; set; }
        public int IdListado { get; set; }
        public int IdTipo { get; set; }
        public DateTime Fecha { get; set; }

        // --- IMPLEMENTACIÓN DE LA INTERFAZ ISINCRONIZABLE ---

        public string ObtenerUrlApi(string baseUrl) {
            // Apunta al endpoint de seguimiento que usas en el ViewModel
            return $"{baseUrl}SeguimientoRuta";
        }

        public async Task<Dictionary<string, object>?> PrepararPayloadAsync() {
            // Parseo seguro de coordenadas para entregar el objeto exacto que espera tu API
            double.TryParse(Latitud, out double lat);
            double.TryParse(Longitud, out double lon);

            var diccionarioPayload = new Dictionary<string, object>
            {
                { "IdPersonal", IdPersonal },
                { "IdInmueble", IdInmueble },
                { "Latitud", lat },
                { "Longitud", lon },
                { "IdListado", IdListado },
                { "IdTipo", IdTipo }
            };

            return await Task.FromResult(diccionarioPayload);
        }

        public async Task LimpiarArchivosLocalesAsync() {
            // Este modelo no genera archivos binarios físicos (como fotos o firmas) en el disco,
            // por lo que simplemente completamos la tarea sin hacer nada.
            await Task.CompletedTask;
        }
    }
}