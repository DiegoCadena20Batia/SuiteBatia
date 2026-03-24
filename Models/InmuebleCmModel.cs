using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Models
{
    public class InmuebleCmModel
    {

        public class Rootobject
        {
            public InmuebleCorrec[] Clientes { get; set; }
        }

        public class InmuebleCorrec
        {
            public int id_inmueble { get; set; }
            public string nombre { get; set; }
            public string latitud { get; set; }
            public string longitud { get; set; }
        }

    }
}

