using BatiaSuite.Models.SupervisionMantenimiento.Operarios;
using CommunityToolkit.Mvvm.ComponentModel;
using Prism.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BatiaSuite.Models.SupervisionMantenimiento {

    public partial class IteracionModel : ObservableObject {
        public Guid Id { get; set; } = Guid.NewGuid();

        [ObservableProperty]
        
        private string _nombre = string.Empty; // Ej. "Baño Hombres", "Equipo de Aire 1"

        public List<PreguntaModel> Preguntas { get; set; } = new();

        public List<FotoModel> Fotos { get; set; } = new();

        //public string PreguntasRespondidas => $"{Preguntas.Count(p => p.EstaRespondida)} de {Preguntas.Count}";

        public bool EstaCompletada => Preguntas.Any() && Preguntas.All(p => p.EstaRespondida);
    }
}