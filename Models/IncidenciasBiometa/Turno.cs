using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Models.IncidenciasBiometa
{
    public class Turno {

        [JsonProperty("id_inmueble")]
        public int IdTurno { get; set; }

        public string Nombre { get; set; }

    }
}
