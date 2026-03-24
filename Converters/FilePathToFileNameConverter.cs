using System.Globalization;

namespace BatiaSuite.Converters;

public class FilePathToFileNameConverter : IValueConverter {
    object? IValueConverter.Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        string filePath = (string)value;

        if(string.IsNullOrWhiteSpace(filePath)) {
            return null;
        }

        string[] tokens = filePath.Split('/');

        return tokens[tokens.Length - 1];
    }

    object? IValueConverter.ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}