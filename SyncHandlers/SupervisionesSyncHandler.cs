using BatiaSuite.Data;
using BatiaSuite.Interfaz;
using BatiaSuite.Models.EntidadesLocal.Supervisiones;
using BatiaSuite.Models.SupervisionMantenimiento;
using BatiaSuite.Services.SupervisionesMantenimiento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.SyncHandlers {
    public class SupervisionesSyncHandler : ISyncHandler {
        private readonly SyncService _syncService;
        private readonly SupervisionesService _supervisionesService;
        private readonly LocalDbContext _localDbContext;

        public string NombreEntidad => "Supervisión";

        public SupervisionesSyncHandler(SyncService syncService, SupervisionesService supervisionesService, LocalDbContext localDbContext) {
            _syncService = syncService;
            _supervisionesService = supervisionesService;
            _localDbContext = localDbContext;
        }

        public async Task<int> SincronizarPendientesAsync() {
            var pendientes = await _localDbContext.ObtenerTodosLocalAsync<SupervisionPendienteLocal>();
            int procesadosExitosamente = 0;

            foreach(var pendiente in pendientes) {
                try {
                    // 1. Extraer payload y metadatos de fotos
                    var datosPayload = await pendiente.PrepararPayloadAsync();
                    if(datosPayload == null) continue;

                    var payload = datosPayload["Payload"] as SupervisionPayloadDto;
                    var mapaFotos = datosPayload["MapaFotos"] as Dictionary<string, string> ?? new();
                    var rutasFotosLocales = datosPayload["RutasFotosLocales"] as List<string> ?? new();

                    if(payload == null) continue;

                    // 2. Paso A: Guardar cabecera / datos maestros en la API
                    var response = await _supervisionesService.GuardarCompletaApiAsync(payload);

                    if(response != null && response.Success && response.Id_Supervisionm > 0) {
                        // 3. Paso B: Subir fotografías asociadas al Id_Supervisionm generado
                        bool fotosSubidasConExito = true;
                        if(rutasFotosLocales.Any()) {
                            fotosSubidasConExito = await _supervisionesService.SubirFotografiasApiAsync(
                                rutasFotosLocales,
                                response.Id_Supervisionm,
                                mapaFotos
                            );
                        }

                        // 4. Si todo el proceso fue correcto, eliminar de la BD local (SQLite)
                        if(fotosSubidasConExito) {
                            await _localDbContext.BorrarLocalAsync(pendiente);
                            procesadosExitosamente++;
                        }
                    }
                } catch(Exception ex) {
                    System.Diagnostics.Debug.WriteLine($"[SupervisionesSyncHandler_Error] Falló la sincronización del registro {pendiente.Id}: {ex.Message}");
                }
            }

            return procesadosExitosamente;
        }
    }
}
