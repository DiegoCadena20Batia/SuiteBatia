using BatiaSuite.Models.CheckListSupervisionesAldoConti;
using BatiaSuite.Models.CheckListSupervisionesAldoConti.singamobiletest.Models;
using System;
using Microsoft.Maui.Controls;

namespace BatiaSuite.Selectors {

    public class PreguntaTemplateSelector : DataTemplateSelector {
        public DataTemplate BooleanoTemplate { get; set; } = null!;
        public DataTemplate EnteroTemplate { get; set; } = null!;
        public DataTemplate DecimalTemplate { get; set; } = null!;
        public DataTemplate TextoTemplate { get; set; } = null!;
        public DataTemplate FechaTemplate { get; set; } = null!;
        public DataTemplate TablaVentasTemplate { get; set; } = null!;

        public DataTemplate SeleccionSiNoTemplate { get; set; } = null!;
        public DataTemplate SeleccionMultipleTemplate { get; set; } = null!;

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container) {
            if(item is not PreguntaTemplate pregunta)
                return TextoTemplate;

            if(!string.IsNullOrEmpty(pregunta.TextoPregunta) &&
                pregunta.TextoPregunta.StartsWith("TABLA:", StringComparison.OrdinalIgnoreCase)) {
                return TablaVentasTemplate;
            }

            return pregunta.TipoDatoId switch {
                1 => BooleanoTemplate,
                2 => EnteroTemplate,
                3 => DecimalTemplate,
                4 => TextoTemplate,
                5 => FechaTemplate,
                6 => SeleccionMultipleTemplate,
                7=>SeleccionSiNoTemplate,
                _ => TextoTemplate
            };
        }
    }
}