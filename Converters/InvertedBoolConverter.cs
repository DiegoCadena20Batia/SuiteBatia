using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Converters
{
    public class InvertedBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => !(value is bool boolValue) || !boolValue;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => !(value is bool boolValue) || !boolValue;
    }

}
