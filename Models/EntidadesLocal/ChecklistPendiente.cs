using BatiaSuite.Interfaz;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BatiaSuite.Models.EntidadesLocal {

    [Table("ChecklistsPendientes")]
    public class ChecklistPendiente : ISincronizable {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string TipoChecklist { get; set; } = string.Empty;

        public string JsonData { get; set; } = string.Empty;

        public string? RutaFirmaSupervisor { get; set; }
        public string? RutaFirmaGerente { get; set; }

        public DateTime FechaCaptura { get; set; }
        // 1. Define a dónde se envía este checklist específico
        public string ObtenerUrlApi(string baseUrl) {
            return $"{baseUrl}ChecklistAC/diariogerente";
        }

        // 2. Reconstruye su propio payload inyectando las firmas si existen
        public async Task<Dictionary<string, object>?> PrepararPayloadAsync() {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var payload = JsonSerializer.Deserialize<Dictionary<string, object>>(JsonData, options);

            if(payload == null) return null;

            if(!string.IsNullOrEmpty(RutaFirmaSupervisor) && File.Exists(RutaFirmaSupervisor)) {
                byte[] bytes = await File.ReadAllBytesAsync(RutaFirmaSupervisor);
                payload["FirmaSupervisor"] = Convert.ToBase64String(bytes);
            }

            if(!string.IsNullOrEmpty(RutaFirmaGerente) && File.Exists(RutaFirmaGerente)) {
                byte[] bytes = await File.ReadAllBytesAsync(RutaFirmaGerente);
                payload["FirmaGerente"] = Convert.ToBase64String(bytes);
            }

            return payload;
        }

        // 3. Sabe cómo limpiar sus propios archivos del teléfono
        public Task LimpiarArchivosLocalesAsync() {
            if(!string.IsNullOrEmpty(RutaFirmaSupervisor) && File.Exists(RutaFirmaSupervisor))
                File.Delete(RutaFirmaSupervisor);

            if(!string.IsNullOrEmpty(RutaFirmaGerente) && File.Exists(RutaFirmaGerente))
                File.Delete(RutaFirmaGerente);

            return Task.CompletedTask;
        }
    }
}