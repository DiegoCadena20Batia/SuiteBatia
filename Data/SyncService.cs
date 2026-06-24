using BatiaSuite.Interfaz;
using BatiaSuite.Models.EntidadesLocal;
using BatiaSuite.Models.Supervision;
using BatiaSuite.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BatiaSuite.Data {

    public class SyncService {
        private readonly HttpClient _httpClient;
        private readonly LocalDbContext _dbContext;
        private readonly string _baseApiUrl = $"{Constants.API_BASE_URL}";

        public SyncService() {
            _httpClient = new HttpClient();
            _dbContext = new LocalDbContext();
        }

        public async Task<bool> SincronizarDatosInicialesAsync<T>(int clienteId) where T : class, IDescargable, new() {
            if(!InternetUtil.IsConnectedInternet()) return false;

            try {
                var instanciador = new T();
                string url = instanciador.ObtenerUrlDescarga(_baseApiUrl, clienteId);

                var response = await _httpClient.GetAsync(url);
                if(!response.IsSuccessStatusCode) return false;

                string rawJson = await response.Content.ReadAsStringAsync();

               
                if(typeof(T) == typeof(CatalogoCacheEntity)) {
                    var cache = new CatalogoCacheEntity {
                        Clave = instanciador.ClaveCatalogo,
                        JsonData = rawJson,
                        UltimaSincronizacion = DateTime.Now
                    };

                    await _dbContext.GuardarLocalAsync(cache as T);
                }   

                else {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                    var listaEntidades = JsonSerializer.Deserialize<List<T>>(rawJson, options);

                    if(listaEntidades != null && listaEntidades.Count > 0) {
                      
                        foreach(var entidad in listaEntidades) {
                            if(entidad is InmuebleEntity inmueble) {
                                inmueble.IdCliente = clienteId;
                                inmueble.IdEstado = 0; 
                            }

                            await _dbContext.GuardarLocalAsync<T>(entidad);
                        }
                    }
                }

                return true;
            } catch(Exception ex) {
                Console.WriteLine($"Error crítico en sincronización genérica de {typeof(T).Name}: {ex.Message}");
                return false;
            }
        }

        public async Task ProcesarPendientesAsync<T>() where T : class, ISincronizable, new() {
            if(!InternetUtil.IsConnectedInternet()) return;

            var localDb = new LocalDbContext();

            List<T> pendientes = await localDb.ObtenerTodosLocalAsync<T>();

            if(pendientes == null || pendientes.Count == 0) return;

            foreach(var pendiente in pendientes) {
                try {
                    var payloadFinal = await pendiente.PrepararPayloadAsync();
                    if(payloadFinal == null) continue;

                    string url = pendiente.ObtenerUrlApi(_baseApiUrl);

                    var response = await _httpClient.PostAsJsonAsync(url, payloadFinal);

                    if(response.IsSuccessStatusCode) {
                        await localDb.BorrarLocalAsync<T>(pendiente);

                        await pendiente.LimpiarArchivosLocalesAsync();

                        System.Diagnostics.Debug.WriteLine($"[Sync] Registro genérico sincronizado y eliminado.");
                    }
                } catch(Exception ex) {
                    System.Diagnostics.Debug.WriteLine($"[Sync] Error al sincronizar registro pendiente: {ex.Message}");
                }
            }
        }

        public async Task SincronizarTodoElEcosistemaAsync(int clienteId) {
            if(!InternetUtil.IsConnectedInternet()) return;

            var tiposDescargables = Assembly.GetExecutingAssembly()
                                            .GetTypes()
                                            .Where(t => typeof(IDescargable).IsAssignableFrom(t)
                                                        && !t.IsInterface
                                                        && !t.IsAbstract);

            MethodInfo? metodoBase = typeof(SyncService)
                .GetMethod(nameof(SincronizarDatosInicialesAsync), new[] { typeof(int) });

            if(metodoBase == null) return;

            foreach(var tipo in tiposDescargables) {
                try {
                    System.Diagnostics.Debug.WriteLine($"[Sync] Detectada entidad automática: {tipo.Name}");

                    MethodInfo metodoGenericoCerrado = metodoBase.MakeGenericMethod(tipo);

                    var tarea = (Task<bool>?)metodoGenericoCerrado.Invoke(this, new object[] { clienteId });

                    if(tarea != null) {
                        bool resultado = await tarea;
                        System.Diagnostics.Debug.WriteLine($"[Sync] Resultado para {tipo.Name}: {(resultado ? "Éxito" : "Fallo")}");
                    }
                } catch(Exception ex) {
                    System.Diagnostics.Debug.WriteLine($"[Sync] Error crítico al sincronizar dinámicamente {tipo.Name}: {ex.InnerException?.Message ?? ex.Message}");
                }
            }
        }
    }
}