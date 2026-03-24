using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Models
{
    public class ListadoMaterialesModel
    {
        public string clave { get; set; }
        public string descripcion { get; set; }
        public int cantidad { get; set; }
        public int entregado { get; set; }

        public int EntregadoEntry
        {
            get { return cantidad; }
            set { entregado = value; }
        }

        public string unidad { get; set; }

        public void ModificarEntregado(int valor)
        {
            entregado = valor; 
        }
    }
}
