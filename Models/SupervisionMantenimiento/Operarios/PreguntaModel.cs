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
        private int? _respuesta;

        [ObservableProperty]
        private string _observaciones = string.Empty;

        partial void OnRespuestaChanged(int? value) {
            OnPropertyChanged(nameof(EstaRespondida));
            OnPropertyChanged(nameof(EsBueno));
            OnPropertyChanged(nameof(EsMalo));
            OnPropertyChanged(nameof(EsNA));
        }

        public bool EstaRespondida => Respuesta.HasValue;

        public bool EsMalo => Respuesta == 1;
        public bool EsBueno => Respuesta == 2;
        public bool EsNA => Respuesta == 0;
    }
}
