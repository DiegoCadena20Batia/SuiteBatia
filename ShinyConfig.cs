using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BatiaSuite.Utils;
using Shiny;
namespace BatiaSuite {
    // ShinyConfig.cs
    public static class ShinyConfig {
        public static void AddShinyServices(this IServiceCollection services) {
            //services.AddGps<MyGpsDelegate>();
            // Puedes agregar más servicios Shiny aquí
        }
    }
}
