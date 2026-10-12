using BatiaSuite.Data;
using BatiaSuite.Interfaz.Repositories;
using BatiaSuite.Models.EntidadesLocal.Supervisiones;
using BatiaSuite.Models.SupervisionMantenimiento.Operarios;
using BatiaSuite.Models.SupervisionMantenimiento.Supervisores;
using BatiaSuite.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BatiaSuite.Repositories {
    public class PlantillaResumenRepository : IPlantillaResumenRepository {

        private readonly HttpHelper _httpHelper;
        private readonly LocalDbContext _localDbContext;
        private readonly string _baseUrlApi = Constants.API_BASE_URL;

        public PlantillaResumenRepository(HttpHelper httpHelper, LocalDbContext localDbContext) {
            _httpHelper = httpHelper;
            _localDbContext = localDbContext;
        }
        public async Task<List<PreguntaResumenModel>> ObtenerPlantillaResumenAsync() {
            string claveRegistro = nameof(PreguntaResumenLocal);

            // RAMA ONLINE: Consultar API y actualizar Caché Local
            if(InternetUtil.IsConnectedInternet()) {
                string url = $"{_baseUrlApi}SupervisionMantenimeintoChecklist/ResumenSupervision";

                // 1. Obtenemos el JSON en formato de objetos
                var plantillaApi = await _httpHelper.GetAsync<List<PreguntaResumenModel>>(url);

                if(plantillaApi != null && plantillaApi.Any()) {
                    // 2. Guardar/Actualizar la caché local para cuando se pierda la conexión
                    try {
                        string rawJson = System.Text.Json.JsonSerializer.Serialize(plantillaApi);

                        var cacheLocal = new PreguntaResumenLocal();
                        cacheLocal.CargarDatosCache(rawJson);

                        var existentes = await _localDbContext.ObtenerListaLocalAsync<PreguntaResumenLocal>(x => x.Clave == claveRegistro);
                        var existente = existentes?.FirstOrDefault();

                        if(existente != null) {
                            await _localDbContext.BorrarLocalAsync(existente);
                        }

                        await _localDbContext.GuardarLocalAsync(cacheLocal);
                    } catch(Exception ex) {
                        //await Shell.Current.DisplayAlert("Error", $"Error actualizando caché de secciones: {ex.Message}", "OK");
                        System.Diagnostics.Debug.WriteLine($"Error actualizando caché de preguntas de resumen: {ex.Message}");
                    }

                    return plantillaApi;
                }
            }

            try {
                var registrosLocales = await _localDbContext.ObtenerListaLocalAsync<PreguntaResumenLocal>(x => x.Clave == claveRegistro);
                var registroCache = registrosLocales?.FirstOrDefault();

                if(registroCache != null && !string.IsNullOrWhiteSpace(registroCache.JsonData)) {
                    //var plantillaLocal = JsonSerializer.Deserialize<List<SeccionModel>>(registroCache.JsonData, new JsonSerializerOptions {
                    //    PropertyNameCaseInsensitive = true
                    //});
                    var plantillaLocal = JsonConvert.DeserializeObject<List<PreguntaResumenModel>>(registroCache.JsonData);

                    return plantillaLocal ?? new List<PreguntaResumenModel>();
                }
            } catch(Exception ex) {
                await Shell.Current.DisplayAlert("Error", $"Error al deserializar preguntas de resumen locales, intente de nuevo", "OK");
                System.Diagnostics.Debug.WriteLine($"Error al deserializar preguntas de resumen locales: {ex.Message}");
            }

            return new List<PreguntaResumenModel>();
        }
    }
}
