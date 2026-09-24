using BatiaSuite.Models;
using BatiaSuite.Models.EntidadesLocal.Supervisiones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Mappers {

    public static class ClienteMapper {

        public static ClientsModel ToModel(this ClientesLocal local) {
            if(local == null) return null!;
            return new ClientsModel {
                idCliente = local.idCliente,
                nombre = local.nombre,
                latitud = local.latitud,
                longitud = local.longitud
            };
        }

        public static List<ClientsModel> ToModelList(this IEnumerable<ClientesLocal> entidades) {
            return entidades?.Select(x => x.ToModel()).ToList() ?? new List<ClientsModel>();
        }
    }
}