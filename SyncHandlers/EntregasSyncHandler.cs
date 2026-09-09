using BatiaSuite.Data;
using BatiaSuite.Interfaz;
using BatiaSuite.Models.EntidadesLocal.RutasEntregas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.SyncHandlers {
    public class EntregasSyncHandler : ISyncHandler {
        private readonly SyncService _syncService;

        public string NombreEntidad => "Entrega";

        public EntregasSyncHandler(SyncService syncService) {
            _syncService = syncService;
        }

        public async Task<int> SincronizarPendientesAsync() {
            return await _syncService.ProcesarPendientesAsync<RutaInmueblePendiente>();
        }
    }
}
