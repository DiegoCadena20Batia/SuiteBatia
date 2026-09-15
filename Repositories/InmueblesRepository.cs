using BatiaSuite.Data;
using BatiaSuite.Interfaz.Repositories;
using BatiaSuite.Mappers;
using BatiaSuite.Models;
using BatiaSuite.Models.EntidadesLocal.Supervisiones;
using BatiaSuite.Utils;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Repositories {

    public class InmueblesRepository : IInmueblesRepository {
        private readonly HttpHelper _httpHelper;
        private readonly LocalDbContext _localDbContext;
        private readonly string _baseUrl = Constants.API_BASE_URL;

        public InmueblesRepository(HttpHelper httpHelper, LocalDbContext localDbContext) {
            _httpHelper = httpHelper;
            _localDbContext = localDbContext;
        }

        public async Task<List<InmuebleModel>> ObtenerInmueblesAsync(int idCliente) {
            if(InternetUtil.IsConnectedInternet()) {
                string url = $"{_baseUrl}Inmueble?idcliente={idCliente}";
                var resultado = await _httpHelper.GetAsync<List<InmuebleModel>>(url);

                if(resultado==null) return new List<InmuebleModel>();

                return resultado;
            }

            var entidadesLocales=await _localDbContext.ObtenerListaLocalAsync<InmueblesLocal>(x=>true);
            if(entidadesLocales==null || !entidadesLocales.Any())
                return new List<InmuebleModel>();
            var filtradosLocales=entidadesLocales.Where(x=>x.id_cliente == idCliente).ToList();
            return filtradosLocales.ToModelList();
        }
    }
}