using BatiaSuite.Models.OrdenesTrabajo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Interfaz.Repositories {
    public interface IOrdenesRepository {
        Task<List<OrdenTrabajoModel>> ObtenerOrdenesAsync(int idTecnico, int mes, int anio);
    }
}
