using BatiaSuite.Interfaz;
using BatiaSuite.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;

namespace BatiaSuite.Models.SupervisionMantenimiento.Supervisores {
    [ModulosRequeridos(20)]
    public partial class PreguntaResumenModel : ObservableObject {
        public int id_pregunta { get; set; }
        public string descripcion { get; set; } = string.Empty;
        public int id_status { get; set; }
        public DateTime falta { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(EstaRespondida))]
        private int _respuesta = -1;

        public bool EstaRespondida => Respuesta >= 0;

        [RelayCommand]
        private void SeleccionarRespuesta(string opcion) {
            if(int.TryParse(opcion, out int valor)) {
                if(Respuesta == valor) {
                    Respuesta = -1;
                } else {
                    Respuesta = valor;
                }
            }
        }
    }
}
