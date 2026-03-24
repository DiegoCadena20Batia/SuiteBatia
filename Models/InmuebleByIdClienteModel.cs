using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Models 
    {
    public class InmuebleByIdClienteModel
        {
        public class Rootobject
            {
            public InmuebleModel[] Inmueble { get; set; }
        }

        public class InmuebleModel 
            {
            public int idInmueble { get; set; }
            public string nombre { get; set; }
            public string latitud { get; set; }
            public string longitud { get; set; }
        }
    }
}
