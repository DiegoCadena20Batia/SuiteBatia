using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Models.CheckListSupervisionesAldoConti {
    using BatiaSuite.Selectors;
    using CommunityToolkit.Mvvm.ComponentModel;
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
        public class PreguntaTemplate : ObservableObject {
            public ObservableCollection<FilaVentaModel> FilasTablaVentas { get; set; } = new ObservableCollection<FilaVentaModel>();

            public int Id { get; set; }
            public int SeccionId { get; set; }
            public string TextoPregunta { get; set; } = string.Empty;
            public int TipoDatoId { get; set; }
            public bool EsRequerido { get; set; }
            public int Orden { get; set; }

            public List<string> OpcionesDisponibles { get; set; } = new List<string>();

            private string _valorSeleccionado = string.Empty;
            public string ValorSeleccionado {
                get => _valorSeleccionado;
                set => SetProperty(ref _valorSeleccionado, value);
            }

            private string? _valorRespondido;
            public string? ValorRespondido {
                get => _valorRespondido;
                set {
                    if(SetProperty(ref _valorRespondido, value)) {
                        OnPropertyChanged(nameof(RespuestaBool));
                    }
                }
            }

            private string? _observaciones;
            public string? Observaciones {
                get => _observaciones;
                set => SetProperty(ref _observaciones, value);
            }

            public bool RespuestaBool {
                get => _valorRespondido == "1";
                set {
                    string nuevoValor = value ? "1" : "0";
                    if(_valorRespondido != nuevoValor) {
                        _valorRespondido = nuevoValor;
                        OnPropertyChanged(nameof(RespuestaBool));
                        OnPropertyChanged(nameof(ValorRespondido));
                    }
                }
            }
        }
    }
}
