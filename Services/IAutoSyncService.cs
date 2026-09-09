using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Services {
    public interface IAutoSyncService {
        /// <summary>
        /// Ejecuta dinámicamente la sincronización de todos los módulos que tengan pendientes.
        /// </summary>
        Task SincronizarTodoAsync();
    }
}
