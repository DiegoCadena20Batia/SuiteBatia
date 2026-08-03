using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Converters
{
    public class NullOrEmptyToDefaultConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            string alternativeText = parameter?.ToString() ?? "Texto alternativo";

            // Validamos si es null, string vacío ("") o espacios en blanco ("   ")
            if(value is string str) {
                if(string.IsNullOrWhiteSpace(str)) {
                    return alternativeText;
                }
                return str;
            }

            // Si por alguna razón llega null desde el binding
            if(value == null) {
                return alternativeText;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
