//using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Models.Supervision {
    public class SupervisionLocal {
        //[PrimaryKey, AutoIncrement]
        public int IdLocal { get; set; }
        public int IdStatusLocal {  get; set; }
        public int IdOrden { get; set; }
        public int Usuario { get; set; }
        public DateTime Fechaini { get; set; }
        public DateTime Fechafin { get; set; }
        public int Id_Cliente { get; set; }
        public int Id_Inmueble { get; set; }

        public string Cliente { get; set; }
        public string Inmueble { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }

        public string NombreOperador { get; set; }

        public bool _clienteentrevista { get; set; }
        public string Clientenombre { get; set; }
        public string Clientecomentario { get; set; }
        public int Evalua { get; set; }
        public int Trabrealizados { get; set; }
        public int Tratopersonal { get; set; }
        public bool Uniformcompleto { get; set; }
        public bool Suprecorrido { get; set; }
        public bool Areaoportunidad { get; set; }
        public bool Plancorrectivo { get; set; }
        public int Calificasup { get; set; }
        //public int Ejecutivocgo { get; set; }
        public bool Reporteasiscgo { get; set; }
        public bool Matetiquetados { get; set; }
        public bool Matrequerimientos { get; set; }

        public int IdSupervisionGeneradaSinga {  get; set; }
    }
}
