using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Models.SupervisionMantenimiento.Operarios {
    public class CierreSupervisionResult {
        public string Observaciones { get; set; } = string.Empty;
        public string NombreFirmante { get; set; } = string.Empty;
        public byte[] FirmaBytes { get; set; } = Array.Empty<byte>();
    }
}
