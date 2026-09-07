using BatiaSuite.Data;
using BatiaSuite.Interfaz.Repositories;
using BatiaSuite.Models.EntidadesLocal.Supervisiones;
using BatiaSuite.Models.SupervisionMantenimiento.Operarios;
using BatiaSuite.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BatiaSuite.Repositories {
    internal class PlantillasRepository : IPlantillasRepository {

        private readonly HttpHelper _httpHelper;
        private readonly LocalDbContext _localDbContext;
        private readonly string _baseUrlApi = Constants.API_BASE_URL;

        public PlantillasRepository(HttpHelper httpHelper, LocalDbContext localDbContext) {
            _httpHelper = httpHelper;
            _localDbContext = localDbContext;
        }
        public async Task<List<SeccionModel>> ObtenerPlantillaAsync(int idRol) {
            string claveRegistro = nameof(SeccionesSupervisionLocal);

            // RAMA ONLINE: Consultar API y actualizar Caché Local
            if(InternetUtil.IsConnectedInternet()) {
                string url = $"{_baseUrlApi}SupervisionMantenimeintoChecklist?id_rol={idRol}";

                // 1. Obtenemos el JSON en formato de objetos
                var plantillaApi = await _httpHelper.GetAsync<List<SeccionModel>>(url);

                if(plantillaApi != null && plantillaApi.Any()) {
                    // 2. Guardar/Actualizar la caché local para cuando se pierda la conexión
                    try {
                        string rawJson = JsonSerializer.Serialize(plantillaApi);

                        var cacheLocal = new SeccionesSupervisionLocal();
                        cacheLocal.CargarDatosCache(rawJson);

                    
                        var existentes = await _localDbContext.ObtenerListaLocalAsync<SeccionesSupervisionLocal>(x => x.Clave == claveRegistro);
                        var existente = existentes?.FirstOrDefault();

                        if(existente != null) {
                            await _localDbContext.BorrarLocalAsync(existente); 
                        }

                      
                        await _localDbContext.GuardarLocalAsync(cacheLocal); 
                    } catch(Exception ex) {
                        //await Shell.Current.DisplayAlert("Error", $"Error actualizando caché de secciones: {ex.Message}", "OK");
                        System.Diagnostics.Debug.WriteLine($"Error actualizando caché de secciones: {ex.Message}");
                    }

                    return plantillaApi;
                }
            }

            try {
                var registrosLocales = await _localDbContext.ObtenerListaLocalAsync<SeccionesSupervisionLocal>(x => x.Clave == claveRegistro);
                var registroCache = registrosLocales?.FirstOrDefault();

                if(registroCache != null && !string.IsNullOrWhiteSpace(registroCache.JsonData)) {
                    var plantillaLocal = JsonSerializer.Deserialize<List<SeccionModel>>(registroCache.JsonData, new JsonSerializerOptions {
                        PropertyNameCaseInsensitive = true
                    });

                    return plantillaLocal ?? new List<SeccionModel>();
                }
            } catch(Exception ex) {
                await Shell.Current.DisplayAlert("Error", $"Error al deserializar secciones locales, intente de nuevo", "OK");
                System.Diagnostics.Debug.WriteLine($"Error al deserializar secciones locales: {ex.Message}");
            }

            return new List<SeccionModel>();
        }
    }
}
