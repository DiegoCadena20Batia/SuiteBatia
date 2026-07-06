using BatiaSuite.Interfaz;
using BatiaSuite.Models.Entregas;
using BatiaSuite.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace BatiaSuite.Models.EntidadesLocal.RutasEntregas {
    public class RutaInmueblePendiente : ISincronizable {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int Id { get; set; }
        public string JsonData { get; set; }
        public DateTime FechaCaptura { get; set; }

        private PayloadContingencia _payloadCache;
        private PayloadContingencia GetPayload() {
            if(_payloadCache == null && !string.IsNullOrEmpty(JsonData)) {
                _payloadCache = JsonConvert.DeserializeObject<PayloadContingencia>(JsonData);
            }
            return _payloadCache;
        }

        public string ObtenerUrlApi(string baseUrl) {
            return $"{baseUrl}EntregaAppN";
        }

        /// <summary>
        /// Satisface tu interfaz retornando el Dictionary con la estructura que el API espera recibir en el body
        /// </summary>
        public async Task<Dictionary<string, object>?> PrepararPayloadAsync() {
            var payload = GetPayload();
            if(payload == null) return null;

            try {
                // 1. Procesar archivos físicos en disco
                var listaArchivos = new List<string>();
                if(payload.RutasFotosLocales != null) listaArchivos.AddRange(payload.RutasFotosLocales);
                if(!string.IsNullOrEmpty(payload.RutaFirmaLocal)) listaArchivos.Add(payload.RutaFirmaLocal);

                // Subida de archivos binarios en contingencia
                if(listaArchivos.Count > 0 && (payload.RutaFirmaLocal.Contains("/") || payload.RutaFirmaLocal.Contains("\\"))) {
                    using var client = new HttpClient();
                    using var formData = new MultipartFormDataContent();

                    foreach(var file in listaArchivos) {
                        if(File.Exists(file)) {
                            byte[] fileBytes = await File.ReadAllBytesAsync(file);
                            bool isSignature = Path.GetFileName(file).StartsWith("Firma", StringComparison.OrdinalIgnoreCase);

                            byte[] resizedImage = await ImageResizerHelper.ResizeImage(fileBytes, 480, 640, !isSignature);
                            var byteArrayContent = new ByteArrayContent(resizedImage);
                            formData.Add(byteArrayContent, "files", Path.GetFileName(file));
                        }
                    }

                    var response = await client.PostAsync(Constants.API_BASE_URL + $"FilesEntregaApp/CargaMul?folio={payload.IdListado}", formData);

                    if(!response.IsSuccessStatusCode) {
                        return null; // Si fallan los archivos, frena el ciclo para reintentar después
                    }
                }

                // 2. Construir el diccionario plano requerido por ISincronizable
                var diccionarioPayload = new Dictionary<string, object>
                {
                    { "Usuario", payload.Usuario },
                    { "NombreRecibe", payload.NombreRecibe },
                    { "ComentarioMateriales", payload.ComentarioMateriales },
                    { "Bidones", payload.Bidones },
                    { "IdListado", payload.IdListado },
                    { "Materiales", payload.Materiales },
                    { "Fentrega", payload.Fentrega }
                };

                return diccionarioPayload;
            } catch(Exception) {
                return null;
            }
        }

        public async Task LimpiarArchivosLocalesAsync() {
            var payload = GetPayload();
            if(payload == null) return;

            try {
                if(payload.RutasFotosLocales != null) {
                    foreach(var foto in payload.RutasFotosLocales) {
                        if(File.Exists(foto)) File.Delete(foto);
                    }
                }

                if(!string.IsNullOrEmpty(payload.RutaFirmaLocal) && File.Exists(payload.RutaFirmaLocal)) {
                    File.Delete(payload.RutaFirmaLocal);
                }
            } catch(Exception ex) {
                System.Diagnostics.Debug.WriteLine($"[Sync_Disk] Error limpiando temporales: {ex.Message}");
            }
        }

        private class PayloadContingencia {
            public int Usuario { get; set; }
            public string NombreRecibe { get; set; }
            public string ComentarioMateriales { get; set; }
            public int Bidones { get; set; }
            public int IdListado { get; set; }
            public RegisterMaterialsModel.Materiale[] Materiales { get; set; }
            public DateTime Fentrega { get; set; }
            public List<string> RutasFotosLocales { get; set; }
            public string RutaFirmaLocal { get; set; }
        }
    }
}