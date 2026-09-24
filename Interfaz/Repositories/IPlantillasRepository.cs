using BatiaSuite.Models.SupervisionMantenimiento.Operarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Interfaz.Repositories {
    public interface IPlantillasRepository {
        Task<List<SeccionModel>> ObtenerPlantillaAsync(int idRol);
    }
}
