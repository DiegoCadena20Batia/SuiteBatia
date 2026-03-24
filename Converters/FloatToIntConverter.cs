using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Converters {
    public class FloatToIntConverter : JsonConverter {
        public override bool CanConvert(Type objectType) {
            return objectType == typeof(int) || objectType == typeof(int?);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            if(reader.TokenType == JsonToken.Float) {
                double d = (double)reader.Value;
                return (int)Math.Round(d); // redondea 4.0 -> 4
            } else if(reader.TokenType == JsonToken.Integer) {
                return Convert.ToInt32(reader.Value);
            } else if(reader.TokenType == JsonToken.Null && objectType == typeof(int?)) {
                return null;
            }

            throw new JsonReaderException($"Valor inesperado {reader.Value} para {objectType}");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            writer.WriteValue(value);
        }
    }

}
