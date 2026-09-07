using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Interfaz {
    public interface ICacheable : IDescargable {
        /// <summary>
        /// Asigna los datos crudos y la clave a la propia instancia antes de guardarla.
        /// </summary>
        void CargarDatosCache(string rawJson);
    }
}
