using BatiaSuite.Interfaz;
using BatiaSuite.Models.SupervisionMantenimiento;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BatiaSuite.Models.EntidadesLocal.Supervisiones {
    [Table("SupervisionesPendientes")]
    public class SupervisionPendienteLocal : ISincronizable {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int IdOrdenSupervisionM { get; set; }

        public string PayloadJson { get; set; } = string.Empty;

        public string MapaFotosJson { get; set; } = string.Empty;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public int Intentos { get; set; } = 0;

        public string Status { get; set; } = "Pendiente"; // "Pendiente", "Error", "Procesando"

        public string UltimoError { get; set; } = string.Empty;

        public Task LimpiarArchivosLocalesAsync() {
          try
            {
                if (!string.IsNullOrWhiteSpace(MapaFotosJson))
                {
                    var mapaFotos = JsonSerializer.Deserialize<Dictionary<string, string>>(MapaFotosJson);
                    if (mapaFotos != null)
                    {
                        // Las Keys de mapaFotos contienen las rutas locales del archivo
                        foreach (var rutaLocal in mapaFotos.Keys)
                        {
                            if (File.Exists(rutaLocal))
                            {
                                File.Delete(rutaLocal);
                                System.Diagnostics.Debug.WriteLine($"[Limpieza] Archivo borrado: {rutaLocal}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Limpieza_Error] No se pudieron borrar archivos temporales: {ex.Message}");
            }

            return Task.CompletedTask;
        }

        public string ObtenerUrlApi(string baseUrl) {
            return $"{baseUrl}SupervisionMantenimiento/GuardarCompleta";
        }

        public Task<Dictionary<string, object>?> PrepararPayloadAsync() {
            if(string.IsNullOrWhiteSpace(PayloadJson))
                return Task.FromResult<Dictionary<string, object>?>(null);

            // Deserializamos los JSONs guardados localmente
            var payloadDto = JsonSerializer.Deserialize<SupervisionPayloadDto>(PayloadJson);
            var mapaFotos = !string.IsNullOrWhiteSpace(MapaFotosJson)
                ? JsonSerializer.Deserialize<Dictionary<string, string>>(MapaFotosJson)
                : new Dictionary<string, string>();

            // Empaquetamos todo en un diccionario
            var resultado = new Dictionary<string, object>
            {
                { "Payload", payloadDto! },
                { "MapaFotos", mapaFotos! }
            };

            return Task.FromResult<Dictionary<string, object>?>(resultado);
        }
    }
}
