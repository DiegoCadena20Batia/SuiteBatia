//using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Models.Supervision {
    public class ArchivoLocal {
        //[PrimaryKey, AutoIncrement]
        public int IdConsec { get; set; }
        public int IdLocal { get; set; }
        public int IdStatusLocal { get; set; }
        public string Nombre { get; set; }
        public string Path { get; set; }
        public int Seccion { get; set; }
        public int Tamano { get; set; }
    }
}
