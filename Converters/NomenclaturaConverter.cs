using System.Globalization;

namespace BatiaSuite.Converters;

public class NomenclaturaConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if(value == null)
            return string.Empty;

        string movimiento = value.ToString();

        return movimiento switch {
            "A" => "Asistencia",
            "A2" => "Salida a comer",
            "A3" => "Entrada de comer",
            "A4" => "Fin de labores",
            "N" => "Descanso",
            "D" => "Doblete",
            "D4" => "Salida doblete",
            "F" => "Falta",
            "FJ" => "Falta justificada",
            "IEG" => "Incapacidad por enfermedad general",
            "IRT" => "Incapacidad por riesgo de trabajo",
            "V" => "Vacaciones",
            "R" => "Retardo",
            "L" => "Vacante",
            _ => movimiento
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        // Opcional: Para convertir de vuelta si es necesario (TwoWay binding)
        throw new NotImplementedException();
    }
}