using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BatiaSuite.Models.OrdenesTrabajo
{
    public class GenerarOrdenTrabajoModel {
        public int Id { get; set; }
        public int IdTipo { get; set; }
        public int IdCliente { get; set; }
        public int IdInmueble { get; set; }
        public int IdTecnico { get; set; }
        
        public string? IdReporte { get; set; }
        public string? Trabajos { get; set; }
        public int IdUsuario { get; set; }
        public int IdStatus { get; set; }
        public string? Edificio { get; set; }
        public string? Piso { get; set; }
        public string? Area { get; set; }
        public string? Subarea { get; set; }
        
        public byte[]  Imagen { get; set; }
    }
}
