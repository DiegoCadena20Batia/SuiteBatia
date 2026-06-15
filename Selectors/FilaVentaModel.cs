using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BatiaSuite.Selectors {
    public class FilaVentaModel : ObservableObject {
        private int _numeroFila;
        private string _nombreVendedor = string.Empty;
        private string _cuantoLleva = string.Empty;
        private string _cuantoDeberia = string.Empty;
        private string _diferencia = string.Empty;

        public int NumeroFila {
            get => _numeroFila;
            set => SetProperty(ref _numeroFila, value);
        }

        public string NombreVendedor {
            get => _nombreVendedor;
            set => SetProperty(ref _nombreVendedor, value);
        }

        public string CuantoLleva {
            get => _cuantoLleva;
            set {
                if(SetProperty(ref _cuantoLleva, value)) {
                    RecalcularDiferencia();
                }
            }
        }

        public string CuantoDeberia {
            get => _cuantoDeberia;
            set {
                if(SetProperty(ref _cuantoDeberia, value)) {
                    RecalcularDiferencia();
                }
            }
        }

        public string Diferencia {
            get => _diferencia;
            set => SetProperty(ref _diferencia, value);
        }

        private void RecalcularDiferencia() {
            // Limpiamos espacios por seguridad
            string llevaStr = (_cuantoLleva ?? "").Trim();
            string deberiaStr = (_cuantoDeberia ?? "").Trim();

            if(decimal.TryParse(llevaStr, out decimal lleva) &&
                decimal.TryParse(deberiaStr, out decimal deberia)) {
                decimal resultado = deberia-lleva;
                // Asignamos directamente a la propiedad pública para lanzar el evento de PropertyChanged
                Diferencia = resultado.ToString("F2");
            } else {
                Diferencia = string.Empty;
            }
        }
    }
}
