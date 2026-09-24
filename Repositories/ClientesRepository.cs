using BatiaSuite.Data;
using BatiaSuite.Interfaz.Repositories;
using BatiaSuite.Mappers;
using BatiaSuite.Models;
using BatiaSuite.Models.EntidadesLocal.Supervisiones;
using BatiaSuite.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Repositories {
    public class ClientesRepository : IClientesRepository {
        private readonly HttpHelper _httpHelper;
        private readonly LocalDbContext _dbContext;
        private readonly string _baseUrl = Constants.API_BASE_URL;

        public ClientesRepository(HttpHelper httpHelper, LocalDbContext dbContext) {
            _httpHelper = httpHelper;
            _dbContext = dbContext;
        }


        public async Task<List<ClientsModel>> ObtenerClientesAsync() {
            if(InternetUtil.IsConnectedInternet()) {
                string url = $"{_baseUrl}Cliente/ClientesMatenimiento";
                var resultado = await _httpHelper.GetAsync<List<ClientsModel>>(url);
                if(resultado==null) return new List<ClientsModel>();

                return resultado;
            }

            var entidadesLocales=await _dbContext.ObtenerListaLocalAsync<ClientesLocal>(x=>true);

            if(entidadesLocales==null || !entidadesLocales.Any()) 
                return new List<ClientsModel>();

            return entidadesLocales.ToModelList();
        }
    }
}
