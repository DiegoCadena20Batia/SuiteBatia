using BatiaSuite.Interfaz;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace BatiaSuite.Models.EntidadesLocal.RutasEntregas {

    [Table("RutasInmueblesPendientes")]
    public class RutaInmueblePendiente : ISincronizable {

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string JsonData { get; set; } = string.Empty;

        public DateTime FechaCaptura { get; set; } = DateTime.Now;

        /// <summary>
        /// Define el endpoint específico de del API donde se procesará la entrega.
        /// </summary>
        public string ObtenerUrlApi(string baseUrl) {
            // Ajusta este endpoint según el controlador real de tu API backend
            return $"{baseUrl}RutasOperador/ActualizarEntrega";
        }

        /// <summary>
        /// Reconstruye el payload mapeado en un diccionario listo para enviarse por PostAsJsonAsync.
        /// </summary>
        public Task<Dictionary<string, object>?> PrepararPayloadAsync() {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var payload = JsonSerializer.Deserialize<Dictionary<string, object>>(JsonData, options);

            return Task.FromResult(payload);
        }

        /// <summary>
        /// Al no almacenar imágenes temporales en el disco duro, este método se queda vacío pero cumple con el contrato.
        /// </summary>
        public Task LimpiarArchivosLocalesAsync() {
            return Task.CompletedTask;
        }
    }
}