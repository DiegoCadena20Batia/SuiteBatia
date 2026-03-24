using BatiaSuite.Models.Supervision;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Converters {
    public class OrdenFechaConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if(value is int idOrden) {
                // Si el IdOrden es 0, devuelve "No Programada"
                if(idOrden == 0) {
                    return "No programada";
                }

                // Si el IdOrden es mayor que 0, devuelve el mismo IdOrden
                return $"Orden: {idOrden}";
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}

