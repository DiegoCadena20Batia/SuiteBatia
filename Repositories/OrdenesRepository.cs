using BatiaSuite.Data;
using BatiaSuite.Interfaz.Repositories;
using BatiaSuite.Models.EntidadesLocal.Supervisiones;
using BatiaSuite.Models.OrdenesTrabajo;
using BatiaSuite.Utils;
using BatiaSuite.Mappers;
namespace BatiaSuite.Repositories {
    public class OrdenesRepository : IOrdenesRepository {
        private readonly HttpHelper _httpHelper;
        private readonly LocalDbContext _dbContext;
        private readonly string _baseUrl = Constants.API_BASE_URL;

        public OrdenesRepository(HttpHelper httpHelper, LocalDbContext dbContext) {
            _httpHelper = httpHelper;
            _dbContext = dbContext;
        }


        public async Task<List<OrdenTrabajoModel>> ObtenerOrdenesAsync(int idTecnico, int mes, int anio) {
            if(InternetUtil.IsConnectedInternet()) {
                string url = $"{_baseUrl}SupervisionMantenimientoProgramada/SupervisionesProgramadas?idTecnico={idTecnico}";
                var resultado = await _httpHelper.GetAsync<List<OrdenTrabajoModel>>(url);

                if(resultado == null) return new List<OrdenTrabajoModel>();

                // Filtrar por fecha
                return resultado.Where(x => {
                    return DateTime.TryParse(x.falta, out DateTime fecha)
                           && fecha.Month == mes
                           && fecha.Year == anio;
                }).ToList();
            }

            var entidadesLocales = await _dbContext.ObtenerListaLocalAsync<OrdenTrabajoLocal>(x => true);

            if(entidadesLocales == null || !entidadesLocales.Any())
                return new List<OrdenTrabajoModel>();

            // 1. Filtrar los registros locales por fecha
            var filtradosLocales = entidadesLocales.Where(x => {
                return DateTime.TryParse(x.falta, out DateTime fecha)
                       && fecha.Month == mes
                       && fecha.Year == anio;
            });

            // 2. Mapear de OrdenTrabajoLocal -> OrdenTrabajoModel usando el Mapper
            return filtradosLocales.ToModelList();
        }
    }
}
