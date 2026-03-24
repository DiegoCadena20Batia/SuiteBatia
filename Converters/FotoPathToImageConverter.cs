using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Converters
{
    public class FotoPathToImageConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var fotoPath = value as string;

            if(string.IsNullOrWhiteSpace(fotoPath)) {
                return "extintor_default.png";
            }

            return fotoPath;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

}
