using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Converters {
    public class IntToBoolConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            int val = (int)value;
            string target = parameter as string;

            return (val == 1 && target == "S") ||
                   (val == 0 && target == "N");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            bool isChecked = (bool)value;
            string target = parameter as string;

            if(!isChecked)
                return null;

            return target == "S" ? 1 : 0;
        }
    }

}

