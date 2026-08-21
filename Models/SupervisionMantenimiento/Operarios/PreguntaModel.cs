using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Models.SupervisionMantenimiento.Operarios {
    public partial class PreguntaModel : ObservableObject {
        public int IdPregunta { get; set; }
        public string Pregunta { get; set; } = string.Empty;

        [ObservableProperty]
        private int _respuesta = -1; // -1: Sin responder, 0: NA, 1: Malo, 2: Bueno

        [ObservableProperty]
        private string _observaciones = string.Empty;

        public bool EstaRespondida => Respuesta >= 0;

        [RelayCommand]
        private void SeleccionarRespuesta(string opcion) {
            if(int.TryParse(opcion, out int valor)) {
                Respuesta = valor;
            }
        }
    }
}
