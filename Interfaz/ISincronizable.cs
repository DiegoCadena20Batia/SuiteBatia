using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Interfaz {
    public interface ISincronizable {
        string ObtenerUrlApi(string baseUrl);
        Task<Dictionary<string, object>?> PrepararPayloadAsync();
        Task LimpiarArchivosLocalesAsync();
    }
}
