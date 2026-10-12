using BatiaSuite.Models.SupervisionMantenimiento.Supervisores;
using System;
using System.Collections.Generic;
using System.Text;

namespace BatiaSuite.Interfaz.Repositories {
    public interface IPlantillaResumenRepository {
        Task<List<PreguntaResumenModel>> ObtenerPlantillaResumenAsync();
    }
}
