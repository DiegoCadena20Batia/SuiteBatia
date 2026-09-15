using BatiaSuite.Interfaz;
using BatiaSuite.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Models.EntidadesLocal.Supervisiones {
    [ModulosRequeridos(20)]
    public class InmueblesLocal : InmuebleModel, IDescargable {
        public string ObtenerUrlDescarga(string baseUrl, int Parametro) {
            return $"{baseUrl}Inmueble/todos";
        }
    }
}
