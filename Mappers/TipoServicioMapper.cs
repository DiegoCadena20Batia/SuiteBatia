using BatiaSuite.Models;
using BatiaSuite.Models.EntidadesLocal.Supervisiones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Mappers {

    public static class TipoServicioMapper {

        public static TipoServicioModel ToModel(this TipoServicioLocal local) {
            if(local == null) return null!;

            return new TipoServicioModel {
                IdTipoServicio = local.IdTipoServicio,
                Descripcion = local.Descripcion
            };
        }

        public static List<TipoServicioModel> ToModelList(this IEnumerable<TipoServicioLocal> entidades) {
            return entidades?.Select(x => x.ToModel()).ToList() ?? new List<TipoServicioModel>();
        }
    }
}