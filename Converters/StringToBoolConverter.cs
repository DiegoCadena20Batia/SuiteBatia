using BatiaSuite.Utils;
using System.Globalization;

namespace BatiaSuite.Converters;

public class StringToBoolConverter : IValueConverter {
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        if(value is null) {
            return false;
        }

        string texto = (string)value;

        if(texto.Equals(Constants.OTRO)) {
            return true;
        }

        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}