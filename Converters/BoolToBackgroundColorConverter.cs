using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Converters
{
    public class BoolToBackgroundColorConverter : IValueConverter {
        public Color TrueColor { get; set; } = Color.FromArgb("#E0F2FE"); // Azul claro
        public Color FalseColor { get; set; } = Colors.White;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return (bool)value ? TrueColor : FalseColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
