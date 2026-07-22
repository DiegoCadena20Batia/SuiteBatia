using BatiaSuite.Interfaz;
using BatiaSuite.Models.EntidadesLocal;
using BatiaSuite.Models.EntidadesLocal.RutasEntregas;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Data {
    public class LocalDbContext {
        private SQLiteAsyncConnection? _database;
        private const string DatabaseFilename = "BatiaSuiteLocal.db3";

        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        private static string DatabasePath =>
            Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);

        private async Task InitAsync() {
            if(_database != null) return;

            await _semaphore.WaitAsync();

            try {
                if(_database == null) {
                    System.Diagnostics.Debug.WriteLine("[SQLite_Debug] Iniciando creación del archivo físico...");

                    string path = DatabasePath;
                    System.Diagnostics.Debug.WriteLine($"[SQLite_Debug] Ruta destino: {path}");

                    var connection = new SQLiteAsyncConnection(
                        path,
                        SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache
                    );

                    System.Diagnostics.Debug.WriteLine("[SQLite_Debug] Conexión creada. Creando tablas automáticamente...");

                    var tiposEntidades = Assembly.GetExecutingAssembly()
                                                    .GetTypes()
                                                    .Where(t => (typeof(IDescargable).IsAssignableFrom(t) || typeof(ISincronizable).IsAssignableFrom(t))
                                                             && !t.IsInterface
                                                             && !t.IsAbstract);

                    foreach(var tipo in tiposEntidades) {
                        await connection.CreateTableAsync(tipo);
                        System.Diagnostics.Debug.WriteLine($"[SQLite_Debug] Tabla creada o verificada: {tipo.Name}");
                    }

                    System.Diagnostics.Debug.WriteLine("[SQLite_Debug] Todas las tablas inicializadas con éxito.");

                    _database = connection;
                }
            } catch(DllNotFoundException dllEx) {
                System.Diagnostics.Debug.WriteLine($"[SQLite_CRÍTICO] Falta inicializar SQLitePCL en esta plataforma: {dllEx.Message}");
            } catch(Exception ex) {
                System.Diagnostics.Debug.WriteLine($"[SQLite_CRÍTICO] Error físico al inicializar la base de datos: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[SQLite_CRÍTICO] StackTrace: {ex.StackTrace}");
            } finally {
                _semaphore.Release();
            }
        }

        // --- MÉTODOS GENÉRICOS UNIVERSALES (Sirven para cualquier tabla) ---

        /// <summary>
        /// Inserta o reemplaza cualquier objeto en su respectiva tabla.
        /// </summary>
        public async Task GuardarLocalAsync<T>(T entity) where T : class, new() {
            await InitAsync();
            if(_database == null) throw new InvalidOperationException("Base de datos no inicializada.");

            // SQLite detecta automáticamente la tabla según el tipo <T>
            await _database.InsertOrReplaceAsync(entity);
        }

        /// <summary>
        /// Trae todos los registros de cualquier tabla.
        /// </summary>
        public async Task<List<T>> ObtenerTodosLocalAsync<T>() where T : class, new() {
            await InitAsync();
            if(_database == null) throw new InvalidOperationException("Base de datos no inicializada.");

            return await _database.Table<T>().ToListAsync();
        }

        /// <summary>
        /// Borra un registro de cualquier tabla pasando el objeto o su ID.
        /// </summary>
        public async Task BorrarLocalAsync<T>(T entity) where T : class, new() {
            await InitAsync();
            if(_database == null) throw new InvalidOperationException("Base de datos no inicializada.");

            await _database.DeleteAsync(entity);
        }

        /// <summary>
        /// Trae los registros de cualquier tabla que cumplan con el predicado especificado.
        /// </summary>

        public async Task<T?> BuscarLocalAsync<T>(Expression<Func<T, bool>> predicado) where T : class, new() {
            await InitAsync();
            if(_database == null) throw new InvalidOperationException("Base de datos no inicializada.");

            return await _database.Table<T>().FirstOrDefaultAsync(predicado);
        }

        /// <summary>
        /// Trae una lista filtrada de cualquier tabla usando una expresión lambda.
        /// </summary>
        public async Task<List<T>> ObtenerListaLocalAsync<T>(Expression<Func<T, bool>> predicado) where T : class, new() {
            await InitAsync();
            if(_database == null) throw new InvalidOperationException("Base de datos no inicializada.");

            // Filtra directamente en la base de datos de SQLite antes de hacer el ToListAsync
            return await _database.Table<T>().Where(predicado).ToListAsync();
        }

        /// <summary>
        /// Elimina absolutamente todos los registros de la tabla especificada por el tipo <T>.
        /// Retorna la cantidad de filas que fueron eliminadas.
        /// </summary>
        public async Task<int> BorrarTablaCompletaAsync<T>() where T : class, new() {
            await InitAsync();
            if(_database == null) throw new InvalidOperationException("Base de datos no inicializada.");

            // DeleteAllAsync borra todo el contenido de la tabla mapeada al tipo T de forma masiva
            int filasEliminadas = await _database.DeleteAllAsync<T>();

            System.Diagnostics.Debug.WriteLine($"[SQLite_Debug] Se vació la tabla {typeof(T).Name}. Filas eliminadas: {filasEliminadas}");

            return filasEliminadas;
        }

        public async Task VerificarRegistros() {
            var todasLasRutasGuardadas = await ObtenerListaLocalAsync<RutasInmuebles>(x => true);
            try {
                System.Diagnostics.Debug.WriteLine($"--- INICIO SELECT * FROM RutasCompletas ({todasLasRutasGuardadas.Count} registros) ---");

                foreach(var r in todasLasRutasGuardadas) {
                    System.Diagnostics.Debug.WriteLine($"IdRuta: {r.IdRuta} | ClaveProducto: {r.Clave} | Nombre: {r.Inmueble} | IdLocal: {r.IdLocal}");
                }

                System.Diagnostics.Debug.WriteLine("--- FIN SELECT * FROM RutasInmuebles ---");
            } catch(Exception ex) {
                System.Diagnostics.Debug.WriteLine($"Error al hacer SELECT en RutasInmuebles: {ex.Message}");
            }
        }

        /// <summary>
        /// Elimina directamente de SQLite los registros que cumplan con la condición especificada.
        /// </summary>
        public async Task<int> BorrarPorPredicadoAsync<T>(Expression<Func<T, bool>> predicado) where T : class, new() {
            await InitAsync();
            if(_database == null) throw new InvalidOperationException("Base de datos no inicializada.");

            // Obtenemos los elementos que cumplen la condición
            var elementosABorrar = await _database.Table<T>().Where(predicado).ToListAsync();
            int totalBorrados = 0;

            // Los eliminamos de SQLite
            foreach(var item in elementosABorrar) {
                totalBorrados += await _database.DeleteAsync(item);
            }

            System.Diagnostics.Debug.WriteLine($"[SQLite_Debug] Se eliminaron {totalBorrados} registros de {typeof(T).Name}.");
            return totalBorrados;
        }
    }
}
