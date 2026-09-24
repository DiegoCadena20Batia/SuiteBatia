using BatiaSuite.Interfaz;
using BatiaSuite.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Models.EntidadesLocal.Supervisiones {
    [ModulosRequeridos(20)]
    public class TipoServicioLocal : TipoServicioModel,IDescargable {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int? IdLocal { get; set; }

        public string ObtenerUrlDescarga(string baseUrl, int Parametro) {
            return $"{baseUrl}TipoServicio";
        }
    }
}
