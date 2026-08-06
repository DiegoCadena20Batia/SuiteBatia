using BatiaSuite.Models.SupervisionMantenimiento.Operarios;
using BatiaSuite.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Services.SupervisionesMantenimiento {
    public class SupervisionMantenimientoStateService {
        public Dictionary<int, List<FotoSeccionEstado>> FotosPorSeccion { get; set; } = new();
       
        public int IdSupervisionActual { get; set; }

        public DateTime FechaInicio { get; set; } = DateTime.Now;
        public List<FirmasSeccionDTO> Firmas { get; set; } = new();

        public void GuardarFotosSeccion(int idSeccion, List<FotoSeccionEstado> fotos) {
            FotosPorSeccion[idSeccion] = fotos;
        }

        public List<FotoSeccionEstado> ObtenerTodasLasFotos() {
            return FotosPorSeccion.Values
                .SelectMany(f => f)
                .Select(f => new FotoSeccionEstado {
                    IdSeccion = f.IdSeccion,
                    IdSupervision = f.IdSupervision,    
                   Subida=f.Subida
                })
                .ToList();
        }

        public void Limpiar() {
            FotosPorSeccion.Clear();
            Firmas.Clear();
            FechaInicio = DateTime.Now;
            IdSupervisionActual = 0;
        }
    }
}
