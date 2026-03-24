using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Converters
{
    public class BoolToTextColorConverter : IValueConverter {
        public Color TrueColor { get; set; } = Color.FromArgb("#0F172A"); // Negro azulado
        public Color FalseColor { get; set; } = Color.FromArgb("#475569"); // Gris

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return (bool)value ? TrueColor : FalseColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
