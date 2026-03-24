using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Converters
{
    public class BoolToBorderColorConverter : IValueConverter {
        public Color TrueColor { get; set; } = Color.FromArgb("#0EA5E9"); // Azul
        public Color FalseColor { get; set; } = Color.FromArgb("#E2E8F0"); // Gris

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return (bool)value ? TrueColor : FalseColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
