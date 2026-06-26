using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Interfaz {
    public interface IDescargable {
        string ClaveCatalogo => this.GetType().Name;
        string ObtenerUrlDescarga(string baseUrl, int Parametro);
    }
}
