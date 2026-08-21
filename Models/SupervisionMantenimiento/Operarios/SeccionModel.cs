using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BatiaSuite.Models.SupervisionMantenimiento.Operarios {

    public partial class SeccionModel : ObservableObject {
        public int IdSeccion { get; set; }
        public string Seccion { get; set; } = string.Empty;

        [JsonIgnore]
        public ObservableCollection<PreguntaModel> Preguntas { get; set; } = new();

        //public List<FotoModel> Fotos { get; set; } = new();

        public ObservableCollection<IteracionModel> Iteraciones { get; set; } = new();

        //public bool EstaCompletada => Preguntas.Any() && Preguntas.All(p => p.EstaRespondida);
        [JsonIgnore]
        public bool EstaCompletada { get; set; } = false;

        [JsonIgnore]
        public int PreguntasRespondidas => Preguntas.Count(p => p.EstaRespondida);

        [JsonIgnore]
        public int TotalPreguntas => Preguntas.Count;
    }
}