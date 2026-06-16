using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Models.DiarioLimpieza {
    public class ChecklistFormularioDto {
        public int SucursalId { get; set; }
        public string GerenteNombre { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; } = DateTime.Today;
        public string ColaboradoresRol { get; set; } = "ASIGNADOS";

        public string? FirmaGerente { get; set; }

        public List<string> FotosBase64 { get; set; } = new List<string>();

        public List<DiarioLimpiezaItem> Directivas { get; set; } = new List<DiarioLimpiezaItem>();
    }
}
