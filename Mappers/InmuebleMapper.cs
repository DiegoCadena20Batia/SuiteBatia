using BatiaSuite.Models;
using BatiaSuite.Models.EntidadesLocal.Supervisiones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Mappers {
    public static class InmuebleMapper {
        public static InmuebleModel ToModel(this InmueblesLocal local) {
            if(local == null) return null!;
            return new InmuebleModel {
                id_inmueble = local.id_inmueble,
                nombre = local.nombre,
                latitud = local.latitud,
                longitud = local.longitud,
                id_cliente = local.id_cliente
            };
        }
        public static List<InmuebleModel> ToModelList(this IEnumerable<InmueblesLocal> entidades) {
            return entidades?.Select(x => x.ToModel()).ToList() ?? new List<InmuebleModel>();
        }
    }
}
