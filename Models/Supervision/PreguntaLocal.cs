//using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Models.Supervision {
    public  class PreguntaLocal {
        //[PrimaryKey, AutoIncrement]
        public int IdConsec { get; set; }
        public int IdLocal { get; set; }
        public int IdStatusLocal { get; set; }
        public int IdPregunta { get; set; }
        public int IdSeccion { get; set; }
        public int IdSupervision { get; set; }
        public string Descripcion { get; set; }
        public string Observaciones { get; set; }
        public float? Valor { get; set; }
    }
}
