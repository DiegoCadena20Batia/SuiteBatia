using BatiaSuite.Models.Supervision;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Converters {
    public class TurnoConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if(value == null)
                return "Sin turno";

            // Si es string, intenta convertirlo a int
            int idTurno;
            if(value is int turnoInt) {
                idTurno = turnoInt;
            } else if(value is string turnoStr && int.TryParse(turnoStr, out int parsedTurno)) {
                idTurno = parsedTurno;
            } else {
                return "Turno inválido";
            }

            return idTurno switch {
                1 => "Matutino",
                2 => "Vespertino",
                3 => "Nocturno",
                4 => "Mixto",
                5 => "1/2 tiempo",
                6 => "1/4 tiempo",
                7 => "",
                8 => "Turno y medio",
                9 => "24x24",
                10 => "Completo",
                _ => $"Turno {idTurno}" // Si hay otros valores numéricos
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            // Opcional: Para convertir de texto a número si es necesario
            if(value is string turnoDescripcion) {
                return turnoDescripcion switch {
                    "Matutino" => 1,
                    "Vespertino" => 2,
                    "Nocturno" => 3,
                    "Mixto" => 4,
                    "Especial" => 5,
                    "Fin de semana" => 6,
                    "Sin asignar" => 0,
                    _ => 0
                };
            }
            return 0;
        }
    }
}

