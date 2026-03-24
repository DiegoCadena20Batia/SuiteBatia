using System.Globalization;

namespace BatiaSuite.Converters;

public class DoubleToBoolConverter : IValueConverter {
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        double porcentaje = (double)value;
        if(porcentaje < 13) {
            return false;
        }
        return true;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}
