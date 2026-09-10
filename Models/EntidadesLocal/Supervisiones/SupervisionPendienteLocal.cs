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

        public async Task<Dictionary<string, object>?> PrepararPayloadAsync() {
            if(string.IsNullOrWhiteSpace(PayloadJson))
                return null;

            try {
                // 1. Deserializar los JSONs almacenados localmente en SQLite
                var payloadDto = JsonSerializer.Deserialize<SupervisionPayloadDto>(PayloadJson);
                var mapaFotos = !string.IsNullOrWhiteSpace(MapaFotosJson)
                    ? JsonSerializer.Deserialize<Dictionary<string, string>>(MapaFotosJson)
                    : new Dictionary<string, string>();

                if(payloadDto == null) return null;

                // 2. Extraer las rutas de fotos locales desde todas las Instancias
                var listaArchivosLocales = new List<string>();

                if(payloadDto.Instancias != null) {
                    foreach(var instancia in payloadDto.Instancias) {
                        if(instancia.Fotos != null && instancia.Fotos.Any()) {
                            listaArchivosLocales.AddRange(instancia.Fotos);
                        }
                    }
                }

                // Extraer también las rutas de firmas locales si existen en la lista de Firmas
                if(payloadDto.Firmas != null) {
                    foreach(var firma in payloadDto.Firmas) {
                        if(!string.IsNullOrWhiteSpace(firma.Firmas) && File.Exists(firma.Firmas)) {
                            listaArchivosLocales.Add(firma.Firmas);
                        }
                    }
                }

                // 3. Carga Multipart de fotografías a la API de Supervisiones
                if(listaArchivosLocales.Count > 0) {
                    using var formData = new MultipartFormDataContent();

                    // ID de la orden requerido por IIS para crear la carpeta de almacenamiento
                    formData.Add(new StringContent(payloadDto.IdOrdenSupervisionM.ToString()), "idSupervisionM");

                    foreach(var rutaLocal in listaArchivosLocales.Distinct()) {
                        if(File.Exists(rutaLocal)) {
                            byte[] fileBytes = await File.ReadAllBytesAsync(rutaLocal);

                            // Validar si es firma para preservar transparencias/proporciones
                            bool isSignature = Path.GetFileName(rutaLocal).StartsWith("Firma", StringComparison.OrdinalIgnoreCase);

                            // Redimensionar/Comprimir la imagen previa al envío
                            byte[] resizedImage = await ImageResizerHelper.ResizeImage(fileBytes, 480, 640, !isSignature);

                            var byteContent = new ByteArrayContent(resizedImage);
                            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

                            // Obtener nombre formateado guardado en MapaFotosJson (o usar el nombre original del archivo)
                            string nombreArchivoServidor = (mapaFotos != null && mapaFotos.TryGetValue(rutaLocal, out var nombre))
                                ? nombre
                                : Path.GetFileName(rutaLocal);

                            formData.Add(byteContent, "files", nombreArchivoServidor);
                        }
                    }

                    string url = $"{Constants.API_BASE_URL}SupervisionMantenimiento/subir-fotos";

                    using var client = new HttpClient();
                    var response = await client.PostAsync(url, formData);

                    // Si falla la carga binaria, retornamos null para abortar el ciclo y reintentar al recuperar señal
                    if(!response.IsSuccessStatusCode) {
                        System.Diagnostics.Debug.WriteLine($"[PrepararPayloadAsync_Error] Falló la subida de fotos para la orden: {payloadDto.IdOrdenSupervisionM}");
                        return null;
                    }
                }

                // 4. Armar la estructura lista que consumirá la API para registrar el JSON de la supervisión
                var resultado = new Dictionary<string, object>
                {
            { "Payload", payloadDto },
            { "MapaFotos", mapaFotos ?? new Dictionary<string, string>() }
        };

                return resultado;
            } catch(Exception ex) {
                System.Diagnostics.Debug.WriteLine($"[PrepararPayloadAsync_Exception] Error al procesar payload de supervisión: {ex.Message}");
                return null;
            }
        }
    }
}
