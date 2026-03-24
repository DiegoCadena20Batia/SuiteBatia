using System.Globalization;

namespace BatiaSuite.Converters;

public class DateToBoolConverter : IValueConverter {
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        string date = ((DateTime)value).ToShortDateString();

        if(date.Equals("1/1/1900")) {
            return false;
        }

        return true;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}