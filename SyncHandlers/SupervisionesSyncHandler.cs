using BatiaSuite.Data;
using BatiaSuite.Interfaz;
using BatiaSuite.Models.EntidadesLocal.Supervisiones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.SyncHandlers {
    public class SupervisionesSyncHandler : ISyncHandler {
        private readonly SyncService _syncService;

        public string NombreEntidad => "Supervisión";

        public SupervisionesSyncHandler(SyncService syncService) {
            _syncService = syncService;
        }

        public async Task<int> SincronizarPendientesAsync() {
            return await _syncService.ProcesarPendientesAsync<SupervisionPendienteLocal>();
        }
    }
}
