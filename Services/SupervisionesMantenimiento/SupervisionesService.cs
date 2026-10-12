using BatiaSuite.Data;
using BatiaSuite.Models.EntidadesLocal.Supervisiones;
using BatiaSuite.Models.SupervisionMantenimiento;
using BatiaSuite.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BatiaSuite.Services.SupervisionesMantenimiento {
    public class SupervisionesService {
        private readonly HttpHelper _httpHelper;
        private readonly LocalDbContext _localDbContext;
        private const string ApiBaseUrl = Constants.API_BASE_URL; 

        public SupervisionesService(HttpHelper httpHelper, LocalDbContext localDbContext) {
            _httpHelper = httpHelper;
            _localDbContext = localDbContext;
        }

        /// <summary>
        /// Procesa el guardado de la supervisión. Si hay internet intenta enviarla a la API,
        /// si falla o no hay conexión, la guarda en SQLite de forma local.
        /// </summary>
        public async Task<(bool FueEnviadoOnline, string Mensaje)> ProcesarGuardadoSupervisionAsync(
     SupervisionPayloadDto payload,
     Dictionary<string, string> mapaFotos,
     List<string> rutasFotosLocales) {
            // 1. Diagnóstico explícito de red antes de intentar
            bool hayInternet = Utils.InternetUtil.IsConnectedInternet();

            if(hayInternet) {
                try {
                    var response = await GuardarCompletaApiAsync(payload);

                    // CASO A: El servidor devolvió NULL (Respondió 400 Bad Request, 500 Error, etc.)
                    if(response == null) {
                        return (false, "El servidor rechazó o no pudo procesar la solicitud. Revisa la información e intenta de nuevo.");
                    }

                    // CASO B: El servidor respondió pero indicó fallo en la validación de negocio
                    if(!response.Success) {
                        return (false, !string.IsNullOrWhiteSpace(response.Mensaje)
                            ? response.Mensaje
                            : "La API no pudo registrar la supervisión por una validación de datos.");
                    }

                    // CASO C: Guardado exitoso en el backend central
                    if(response.Id_Supervisionm > 0) {
                        if(rutasFotosLocales != null && rutasFotosLocales.Any()) {
                            await SubirFotografiasApiAsync(rutasFotosLocales, response.Id_Supervisionm, mapaFotos);
                        }

                        return (true, !string.IsNullOrWhiteSpace(response.Mensaje)
                            ? response.Mensaje
                            : "Supervisión guardada correctamente.");
                    }

                    return (false, "No se recibió un identificador válido desde el servidor.");
                } catch(Exception ex) {
                    // Solo si ocurrió una interrupción de red/transporte inesperada a mitad de camino
                    System.Diagnostics.Debug.WriteLine($"[SupervisionesService] Excepción de transporte HTTP: {ex.Message}. Procediendo a respaldo local...");
                }
            }

            // 2. CONTINGENCIA OFFLINE: Se ejecuta si no hay red o si falló el transporte HTTP
            try {
                var registroPendiente = new SupervisionPendienteLocal {
                    IdOrdenSupervisionM = payload.IdOrdenSupervisionM,
                    PayloadJson = JsonSerializer.Serialize(payload),
                    MapaFotosJson = JsonSerializer.Serialize(mapaFotos),
                    FechaCreacion = DateTime.Now,
                    Status = "Pendiente"
                };

                await _localDbContext.GuardarLocalAsync(registroPendiente);

                return (false, "Sin conexión a Internet. La supervisión se guardó localmente en el dispositivo y se enviará automáticamente al reconectarse.");
            } catch(Exception ex) {
                return (false, $"Error crítico al intentar guardar en la base de datos local SQLite: {ex.Message}");
            }
        }
        #region Métodos de Red (API)

        /// <summary>
        /// Envía la estructura JSON a la API.
        /// </summary>
        public async Task<SupervisionResponseDto?> GuardarCompletaApiAsync(SupervisionPayloadDto payload) {
            string urlGuardar = $"{ApiBaseUrl}SupervisionMantenimiento/GuardarCompleta";

            return await _httpHelper.PostBodyAsync<SupervisionPayloadDto, SupervisionResponseDto>(
                urlGuardar,
                payload
            );
        }

        /// <summary>
        /// Sube el arreglo de imágenes vía Multipart Content utilizando el diccionario de mapeo.
        /// </summary>
        public async Task<bool> SubirFotografiasApiAsync(List<string> rutasLocales, int idSupervisionM, Dictionary<string, string> mapaNombres) {
            if(rutasLocales == null || !rutasLocales.Any())
                return true;

            try {
                using var content = new MultipartFormDataContent();

                // 1. Agregar el ID generado para la subcarpeta
                content.Add(new StringContent(idSupervisionM.ToString()), "idSupervisionM");

                // 2. Adjuntar los bytes de cada archivo de foto
                foreach(var ruta in rutasLocales) {
                    if(File.Exists(ruta)) {
                        var fileBytes = await File.ReadAllBytesAsync(ruta);
                        var byteContent = new ByteArrayContent(fileBytes);
                        byteContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

                        string nombreArchivoServidor = mapaNombres.TryGetValue(ruta, out var nombre)
                            ? nombre
                            : Path.GetFileName(ruta);

                        content.Add(byteContent, "files", nombreArchivoServidor);
                    }
                }

                string url = $"{ApiBaseUrl}SupervisionMantenimiento/subir-fotos";

                var respuesta = await _httpHelper.PostMultipartAsync<List<string>>(
                    url,
                    content,
                    isSupervisionModule: true
                );

                return respuesta != null;
            } catch(Exception ex) {
                System.Diagnostics.Debug.WriteLine($"[SupervisionesService] Error al subir fotografías: {ex.Message}");
                return false;
            }
        }

        #endregion
    }
}
