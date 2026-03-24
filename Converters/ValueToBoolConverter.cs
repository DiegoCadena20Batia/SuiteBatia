using System.Globalization;

namespace BatiaSuite.Converters;

public class ValueToBoolConverter : IValueConverter {
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        float? valor = value is null ? null : (float)value;
        if(valor is not null && valor == -0.5f) {
            return true;
        }

        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}