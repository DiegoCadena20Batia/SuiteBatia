using BatiaSuite.Models;
using BatiaSuite.Models.EntidadesLocal.Supervisiones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Interfaz.Repositories {
    public interface ITiposServicioRepository {
        Task<List<TipoServicioModel>> ObtenerTiposServicioAsync();
    }
}
