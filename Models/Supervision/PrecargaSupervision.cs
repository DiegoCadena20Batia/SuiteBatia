using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using SQLite;

namespace BatiaSuite.Models.Supervision
{
    public class PrecargaSupervision
    {
        //[PrimaryKey, AutoIncrement]
        public int IdCarga { get; set; }
        public DateTime FechaCarga { get; set; }
    }
}
