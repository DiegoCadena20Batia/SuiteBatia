using BatiaSuite.Models.Supervision;
using CommunityToolkit.Maui.Core;
using Newtonsoft.Json;
using System.Text;

namespace BatiaSuite.Utils;

public class HttpHelper {

    readonly HttpClient _httpClient;

    public HttpHelper() {
        _httpClient = new HttpClient {
            BaseAddress = new Uri(Constants.API_BASE_URL),
            Timeout = TimeSpan.FromMinutes(2) // ✅ Configurar Timeout aquí una sola vez
        };

        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    public async Task<Response> GetAsync<Response>(string url, CancellationToken cancellationToken = default) {
        Response response = default;

        try {
            HttpResponseMessage result = await _httpClient.GetAsync(url, cancellationToken);

            if(result.IsSuccessStatusCode) {
                response = JsonConvert.DeserializeObject<Response>(result.Content.ReadAsStringAsync().Result);
            }
        } catch(Exception ex) {
            Console.WriteLine("Error GET: " + ex.Message.ToString());
            return response;
        }

        return response;
    }

    public async Task<Response> PostBodyAsync<Request, Response>(string uri, Request objet) {
        Response response = default;

        try {
            string json = JsonConvert.SerializeObject(objet);
            StringContent stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage result = await _httpClient.PostAsync(uri, stringContent);

            if(result.IsSuccessStatusCode) {
                response = JsonConvert.DeserializeObject<Response>(result.Content.ReadAsStringAsync().Result);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error POST: " + ex.Message.ToString());
            return response;
        }

        return response;
    }

    public async Task<Response> PostMultipartAsync<Response>(string url, MultipartFormDataContent content, bool isSupervisionModule = false) {
        Response response = default;

        try {
            if(isSupervisionModule) {
                _httpClient.DefaultRequestHeaders.Add("folder", "supervision");
            }
            HttpResponseMessage result = await _httpClient.PostAsync(url, content);

            if(result.IsSuccessStatusCode) {
                response = JsonConvert.DeserializeObject<Response>(result.Content.ReadAsStringAsync().Result);
            }

            if(isSupervisionModule) {
                _httpClient.DefaultRequestHeaders.Remove("folder");
            }
        } catch(Exception ex) {
            return response;
        }

        return response;
    }

    public async Task<List<ArchivoModel>> PostMultipartAsyncNew(string url, MultipartFormDataContent content, bool isSupervisionModule = false, CancellationToken cancellationToken = default) {
        try {
            // Agregar header de supervisión si es necesario
            if(isSupervisionModule) {
                _httpClient.DefaultRequestHeaders.Add("folder", "supervision");
            }

            // Construir URI completa
            var fullUri = new Uri(_httpClient.BaseAddress, url);

            // Enviar la petición con el token de cancelación
            var response = await _httpClient.PostAsync(
                fullUri,
                content,
                cancellationToken);

            // Limpiar header si se agregó
            if(isSupervisionModule) {
                _httpClient.DefaultRequestHeaders.Remove("folder");
            }

            // Procesar respuesta
            if(response.IsSuccessStatusCode) {
                var responseString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<ArchivoModel>>(responseString);
            } else {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"HTTP Error: {response.StatusCode} - {errorContent}");
            }
        } catch(OperationCanceledException) {
            // Limpiar headers antes de relanzar la excepción
            if(isSupervisionModule && _httpClient.DefaultRequestHeaders.Contains("folder")) {
                _httpClient.DefaultRequestHeaders.Remove("folder");
            }
            throw new HttpRequestException("La operación fue cancelada");
        } catch(Exception) {
            // Limpiar headers en caso de otros errores
            if(isSupervisionModule && _httpClient.DefaultRequestHeaders.Contains("folder")) {
                _httpClient.DefaultRequestHeaders.Remove("folder");
            }
            throw;
        }
    }

    public async Task<bool> PostMultipartAsync(string url, MultipartFormDataContent content) {
        var response = await _httpClient.PostAsync(url, content);
        return response.IsSuccessStatusCode;
    }
}
