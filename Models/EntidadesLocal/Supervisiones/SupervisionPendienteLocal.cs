using BatiaSuite.Interfaz;
using BatiaSuite.Models.SupervisionMantenimiento;
using BatiaSuite.Utils;
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
            try {
                if(!string.IsNullOrWhiteSpace(MapaFotosJson)) {
                    var mapaFotos = JsonSerializer.Deserialize<Dictionary<string, string>>(MapaFotosJson);
                    if(mapaFotos != null) {
                        // Las Keys de mapaFotos contienen las rutas locales del archivo
                        foreach(var rutaLocal in mapaFotos.Keys) {
                            if(File.Exists(rutaLocal)) {
                                File.Delete(rutaLocal);
                                System.Diagnostics.Debug.WriteLine($"[Limpieza] Archivo borrado: {rutaLocal}");
                            }
                        }
                    }
                }
            } catch(Exception ex) {
                System.Diagnostics.Debug.WriteLine($"[Limpieza_Error] No se pudieron borrar archivos temporales: {ex.Message}");
            }

            return Task.CompletedTask;
        }

        public string ObtenerUrlApi(string baseUrl) {
            return $"{baseUrl}SupervisionMantenimiento/GuardarCompleta";
        }

        public async Task<Dictionary<string, object>?> PrepararPayloadAsync() {
            if(string.IsNullOrWhiteSpace(PayloadJson))
                return null;

            try {
                // 1. Deserializar los datos de SQLite
                var payloadDto = JsonSerializer.Deserialize<SupervisionPayloadDto>(PayloadJson);
                var mapaFotos = !string.IsNullOrWhiteSpace(MapaFotosJson)
                    ? JsonSerializer.Deserialize<Dictionary<string, string>>(MapaFotosJson)
                    : new Dictionary<string, string>();

                if(payloadDto == null) return null;

                // 2. Extraer rutas locales físicas buscando por las Keys del mapa o dentro de las instancias
                var rutasFotosLocales = new List<string>();

                if(mapaFotos.Any()) {
                    rutasFotosLocales.AddRange(mapaFotos.Keys);
                } else if(payloadDto.Instancias != null) {
                    foreach(var instancia in payloadDto.Instancias) {
                        if(instancia.Fotos != null) {
                            rutasFotosLocales.AddRange(instancia.Fotos);
                        }
                    }
                }

                // 3. Retornar los componentes empaquetados para el Handler
                var resultado = new Dictionary<string, object>
                {
            { "Payload", payloadDto },
            { "MapaFotos", mapaFotos },
            { "RutasFotosLocales", rutasFotosLocales }
        };

                return await Task.FromResult(resultado);
            } catch(Exception ex) {
                System.Diagnostics.Debug.WriteLine($"[PrepararPayloadAsync_Exception] Error al deserializar datos: {ex.Message}");
                return null;
            }
        }
    }
}