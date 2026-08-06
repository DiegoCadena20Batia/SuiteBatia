using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Models.SupervisionMantenimiento.Operarios
{
    public class FotosSeccionDTO {
        public int IdSupervision { get; set; }
        public int IdSeccion { get; set; }
        public byte[] FotoBytes { get; set; }
        
    }
    public class FirmasSeccionDTO {
        public int IdSupervision { get; set; }
       
        public byte[] FirmaBytes { get; set; }
    }

    public class FotoSeccionEstado {
        public int IdSupervision { get; set; }
        public int IdSeccion { get; set; }
        public string LocalPath { get; set; }
        public bool Subida { get; set; } = false;
    }
}
