using System.ComponentModel;

namespace BatiaSuite.Models.Supervision;

public class SupervisionModel : INotifyPropertyChanged {
    public int Orden { get; set; }
    public int IdCliente { get; set; }
    public string Cliente { get; set; }
    public int IdInmueble { get; set; }
    public string Inmueble { get; set; }
    public DateTime Fecha { get; set; }
    public int Tipo { get; set; }

    private bool _isEnabled = true;
    public bool IsEnabled {
        get => _isEnabled;
        set {
            if(_isEnabled != value) {
                _isEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}