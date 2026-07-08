using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Utils {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ModulosRequeridosAttribute : Attribute {
        public int[] Modulos { get; }

        // El modificador 'params' permite pasar uno o varios IDs de módulos
        public ModulosRequeridosAttribute(params int[] modulos) {
            Modulos = modulos;
        }
    }
}
