using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BatiaSuite.Models.CheckListAparadores {
    public class CheckListPreguntasModel {
        public int IdSeccion { get; set; }
        public int IdPregunta { get; set; }
        [JsonIgnore]
        public string Pregunta { get; set; }
        public int Valor1 { get; set; }
        public int Valor2 { get; set; }
        [JsonIgnore]
        public bool Valor3 { get; set; }
        public string Comentarios { get; set; }
    }
}
