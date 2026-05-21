using BatiaSuite.Models.CheckListSupervisionesAldoConti;
using BatiaSuite.Models.CheckListSupervisionesAldoConti.singamobiletest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls; // Asegura la referencia limpia de MAUI

namespace BatiaSuite.Selectors {
    public class PreguntaTemplateSelector : DataTemplateSelector {
        // Estas propiedades se asignarán desde el XAML principal
        public DataTemplate BooleanoTemplate { get; set; } = null!;
        public DataTemplate EnteroTemplate { get; set; } = null!;
        public DataTemplate DecimalTemplate { get; set; } = null!;
        public DataTemplate TextoTemplate { get; set; } = null!;
        public DataTemplate FechaTemplate { get; set; } = null!;

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container) {
            if(item is not PreguntaTemplate pregunta)
                return TextoTemplate; // Por defecto

            // Ahora evaluamos por el ID entero (TipoDatoId) que mapeamos desde SQL Server
            return pregunta.TipoDatoId switch {
                1 => BooleanoTemplate, // Booleano_SiNo
                2 => EnteroTemplate,   // Entero
                3 => DecimalTemplate,  // Decimal
                4 => TextoTemplate,    // Texto
                5 => FechaTemplate,    // Fecha
                _ => TextoTemplate     // Por defecto si viene cualquier otro valor
            };
        }
    }
}