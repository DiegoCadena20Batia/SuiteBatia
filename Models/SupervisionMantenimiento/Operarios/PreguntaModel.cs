using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Models.SupervisionMantenimiento.Operarios
{
    public partial class PreguntaModel : ObservableObject {
        public int IdPregunta { get; set; }
        public string Pregunta { get; set; } = string.Empty;

        [ObservableProperty]
        private string _respuesta = string.Empty;

        [ObservableProperty]
        private string _observaciones = string.Empty;

        // Se notifica automáticamente cuando cambia 'Respuesta'
        partial void OnRespuestaChanged(string value) {
            OnPropertyChanged(nameof(EstaRespondida));
            OnPropertyChanged(nameof(EsBueno));
            OnPropertyChanged(nameof(EsMalo));
            OnPropertyChanged(nameof(EsNA));
        }

        public bool EstaRespondida => !string.IsNullOrEmpty(Respuesta);

        // Banderas para activar los estilos de los botones
        public bool EsBueno => Respuesta == "Bueno";
        public bool EsMalo => Respuesta == "Malo";
        public bool EsNA => Respuesta == "N/A";
    }
}
