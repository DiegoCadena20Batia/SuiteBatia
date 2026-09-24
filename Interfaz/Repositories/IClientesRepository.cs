using BatiaSuite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Interfaz.Repositories {
    public interface IClientesRepository {
        Task<List<ClientsModel>> ObtenerClientesAsync();
    }
}
