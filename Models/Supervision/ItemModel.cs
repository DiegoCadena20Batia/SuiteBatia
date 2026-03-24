using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Models.Supervision {
    public class ItemModel {
        public int Posicion { get; set; } // Posición del ítem en la lista
        public string Descripcion { get; set; } // Texto del CheckBox
        public bool Valor { get; set; } // Estado del CheckBox
    }
}
