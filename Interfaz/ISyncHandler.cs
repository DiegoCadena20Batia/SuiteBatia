using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Interfaz {
    public interface ISyncHandler {
        string NombreEntidad { get; }

        Task<int> SincronizarPendientesAsync();
    }
}
