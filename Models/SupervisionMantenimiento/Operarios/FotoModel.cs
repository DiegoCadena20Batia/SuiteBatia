using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Models.SupervisionMantenimiento.Operarios {
    public partial class FotoModel : ObservableObject {
        public Guid Id { get; set; } = Guid.NewGuid();

        [ObservableProperty]
        private string _localPath = string.Empty;

        [ObservableProperty]
        private bool _subida;

      
    }
}
