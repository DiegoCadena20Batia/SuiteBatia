using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BatiaSuite.Models.SupervisionMantenimiento.Operarios {
    public partial class PisoModel : ObservableObject {
        public Guid Id { get; set; } = Guid.NewGuid();

        [ObservableProperty]
        [property: JsonPropertyName("nombre")]
        private string _nombre = string.Empty;

        public ObservableCollection<SeccionModel> Secciones { get; set; } = new();

        [JsonIgnore]
        public bool EstaCompletado;
        //public bool EstaCompletado => Secciones.Any() && Secciones.All(s => s.EstaCompletada);
        [JsonIgnore]
        public int SeccionesCompletadas => Secciones.Count(s => s.EstaCompletada);
        [JsonIgnore]
        public int TotalSecciones => Secciones.Count;

        /// <summary>
        /// Notifica a la interfaz que SeccionesCompletadas cambió
        /// </summary>
        public void NotificarCambios() {
            OnPropertyChanged(nameof(SeccionesCompletadas));
        }
    }
}
