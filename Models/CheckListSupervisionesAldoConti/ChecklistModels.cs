using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BatiaSuite.Models.CheckListSupervisionesAldoConti {
    using System.Collections.ObjectModel;

    namespace singamobiletest.Models {
        public class SeccionTemplate {
            public int Id { get; set; } 
            public string Nombre { get; set; } = string.Empty;
            public int Orden { get; set; }
            public bool Activa { get; set; }

            public ObservableCollection<PreguntaTemplate> Preguntas { get; set; } = new();
        }
    }

    

namespace singamobiletest.Models {
        public class PreguntaTemplate : INotifyPropertyChanged {
            public int Id { get; set; }        
            public int SeccionId { get; set; } 
            public string TextoPregunta { get; set; } = string.Empty;
            public int TipoDatoId { get; set; }
            public bool EsRequerido { get; set; }
            public int Orden { get; set; }

            private string? _valorRespondido;
            public string? ValorRespondido {
                get => _valorRespondido;
                set { _valorRespondido = value; OnPropertyChanged(); }
            }

            private string? _observaciones;
            public string? Observaciones {
                get => _observaciones;
                set { _observaciones = value; OnPropertyChanged(); }
            }

            private bool _respuestaBool;
            public bool RespuestaBool {
                get => _valorRespondido == "1";
                set {
                    _valorRespondido = value ? "1" : "0";
                    OnPropertyChanged();
                }
            }

            public event PropertyChangedEventHandler? PropertyChanged;
            protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
