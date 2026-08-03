using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BatiaSuite.Models.SupervisionMantenimiento.Operarios
{
    public partial class SeccionModel : ObservableObject {
        public int IdSeccion { get; set; }
        public string Seccion { get; set; } = string.Empty;
        public List<PreguntaModel> Preguntas { get; set; } = new();

        // Total de preguntas en la sección
        public int TotalPreguntas => Preguntas?.Count ?? 0;

        // Preguntas respondidas en tiempo real
        public int PreguntasRespondidas => Preguntas?.Count(p => p.EstaRespondida) ?? 0;

        // Porcentaje de avance de 0.0 a 1.0 para la ProgressBar
        public double ProgresoPorcentaje => TotalPreguntas > 0
            ? (double)PreguntasRespondidas / TotalPreguntas
            : 0.0;

        // Método para notificar a la vista que el progreso cambió
        public void NotificarCambioProgreso() {
            OnPropertyChanged(nameof(PreguntasRespondidas));
            OnPropertyChanged(nameof(ProgresoPorcentaje));
        }

        public string CategoriaSuperior => IdSeccion switch {
            >= 1 and <= 7 => "Estructura y Acabados",
            8 or 9 or 10 or 11 or 20 or 21 => "Sistema Eléctrico",
            >= 12 and <= 16 => "HVAC y Sanitarios",
            17 or 18 or 19 or 22 => "Seguridad y Servicios",
            _ => "Otros"
        };

        public string Icono => IdSeccion switch {
            1 => "🏢",
            2 => "🎨",
            3 => "📐",
            4 => "🏗️",
            8 or 9 => "⚡",
            11 => "💡",
            12 or 13 or 14 => "🚰",
            15 or 16 => "❄️",
            17 => "🧯",
            22 => "🛗",
            _ => "📋"
        };
    }
}
