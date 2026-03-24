using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Utils {
    public static class PolylineDecoder {
        public static List<Location> Decode(string encodedPoints) {
            if(string.IsNullOrEmpty(encodedPoints))
                return new List<Location>();

            var poly = new List<Location>();
            int index = 0;
            int len = encodedPoints.Length;
            int lat = 0, lng = 0;

            while(index < len) {
                int b, shift = 0, result = 0;
                do {
                    b = encodedPoints[index++] - 63;
                    result |= (b & 0x1f) << shift;
                    shift += 5;
                } while(b >= 0x20);
                int dlat = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
                lat += dlat;

                shift = 0;
                result = 0;
                do {
                    b = encodedPoints[index++] - 63;
                    result |= (b & 0x1f) << shift;
                    shift += 5;
                } while(b >= 0x20);
                int dlng = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
                lng += dlng;

                var p = new Location((double)lat / 1E5, (double)lng / 1E5);
                poly.Add(p);
            }

            return poly;
        }
    }
}
