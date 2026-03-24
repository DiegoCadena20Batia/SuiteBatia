//using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Models.Supervision {
    public class EvaluacionOperadorLocal {
        /// <summary>
        /// [PrimaryKey, AutoIncrement]
        /// </summary>
        public int IdConsec { get; set; }
        public int IdLocal { get; set; }
        public int IdStatusLocal { get; set; }

    }
}
