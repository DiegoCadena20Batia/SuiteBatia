using BatiaSuite.Data;
using BatiaSuite.Interfaz.Repositories;
using BatiaSuite.Mappers;
using BatiaSuite.Models;
using BatiaSuite.Models.EntidadesLocal.Supervisiones;
using BatiaSuite.Utils;

namespace BatiaSuite.Repositories {
    public class TiposServicioRepository : ITiposServicioRepository {

        private readonly HttpHelper _httpHelper;
        private readonly LocalDbContext _dbContext;
        private readonly string _baseUrl = Constants.API_BASE_URL;

        public TiposServicioRepository(HttpHelper httpHelper, LocalDbContext dbContext) {
            _httpHelper = httpHelper;
            _dbContext = dbContext;
        }
        public async Task<List<TipoServicioModel>> ObtenerTiposServicioAsync() {
            if(Utils.InternetUtil.IsConnectedInternet()) {
                string url = $"{_baseUrl}TipoServicio";
                var resultado = await _httpHelper.GetAsync<List<TipoServicioModel>>(url);

                if(resultado == null) return new List<TipoServicioModel>();

                return resultado.ToList();
            }

            var entidadesLocales = await _dbContext.ObtenerListaLocalAsync<TipoServicioLocal>(x=>true);

            if(entidadesLocales==null || !entidadesLocales.Any()) 
                return new List<TipoServicioModel>();

            return entidadesLocales.ToModelList();
        }
    }
}
