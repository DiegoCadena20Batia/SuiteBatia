using BatiaSuite.Models.EntidadesLocal.Supervisiones;
using BatiaSuite.Models.OrdenesTrabajo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Mappers {
    public static class OrdenTrabajoMapper {
        public static OrdenTrabajoModel ToModel(this OrdenTrabajoLocal local) {
            if(local == null) return null!;

            return new OrdenTrabajoModel {
                idOrden = local.idOrden,
                idCliente = local.idCliente,
                idInmueble = local.idInmueble,
                latitud = local.latitud,
                longitud = local.longitud,
                sucursal = local.sucursal,
                cliente = local.cliente,
                falta = local.falta,
                status = local.status,
                tipomanto = local.tipomanto,
                tipoOrden = local.tipoOrden,
                descripcion = local.descripcion,
                SyncDate = local.SyncDate
            };
        }

        public static List<OrdenTrabajoModel> ToModelList(this IEnumerable<OrdenTrabajoLocal> entidades) {
            return entidades?.Select(x => x.ToModel()).ToList() ?? new List<OrdenTrabajoModel>();
        }
    }
}
