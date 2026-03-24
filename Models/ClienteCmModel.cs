using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Models
{
    public class ClienteCmModel
    {

        public class Rootobject
        {
            public ClienteCorrec[] Cliente { get; set; }
        }

        public class ClienteCorrec
        {
            public int idCliente { get; set; }
            public string nombre { get; set; }
        }

    }
}
