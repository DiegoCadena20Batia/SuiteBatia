using BatiaSuite.Models;
using BatiaSuite.Models.Entregas;
using BatiaSuite.Models.OrdenesTrabajo;
using BatiaSuite.Models.Supervision;
using BatiaSuite.Utils;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace BatiaSuite.Data;

public static class SqliteConnectionExtensions {

    public static TableQuery<T> Table<T>(this SqliteConnection connection) where T : class, new() {
        return new TableQuery<T>(connection);
    }

    // Método ExecuteAsync para queries que no retornan datos (INSERT, UPDATE, DELETE)
    public static async Task<int> ExecuteAsync(this SqliteConnection connection, string sql, params object[] parameters) {
        using var command = connection.CreateCommand();
        command.CommandText = sql;

        // Agregar parámetros
        for(int i = 0; i < parameters.Length; i++) {
            var paramName = $"$p{i}";
            command.Parameters.AddWithValue(paramName, parameters[i] ?? DBNull.Value);
        }

        return await command.ExecuteNonQueryAsync();
    }

    // Versión con parámetros nombrados
    public static async Task<int> ExecuteAsync(this SqliteConnection connection, string sql, Dictionary<string, object> parameters) {
        using var command = connection.CreateCommand();
        command.CommandText = sql;

        foreach(var param in parameters) {
            command.Parameters.AddWithValue($"${param.Key}", param.Value ?? DBNull.Value);
        }

        return await command.ExecuteNonQueryAsync();
    }

    // Método para queries que retornan un solo valor
    public static async Task<T> ExecuteScalarAsync<T>(this SqliteConnection connection, string sql, params object[] parameters) {
        using var command = connection.CreateCommand();
        command.CommandText = sql;

        for(int i = 0; i < parameters.Length; i++) {
            var paramName = $"$p{i}";
            command.Parameters.AddWithValue(paramName, parameters[i] ?? DBNull.Value);
        }

        var result = await command.ExecuteScalarAsync();
        return result == DBNull.Value ? default(T) : (T)Convert.ChangeType(result, typeof(T));
    }
}

public class DbContext {
    public SqliteConnection _dbConn;
    HttpHelper _httpHelper;

    //private SQLiteAsyncConnection _dbConn;
    private bool _tablesCreated = false;

    private SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public DbContext() {
        _httpHelper= new HttpHelper();  
    }

    private static readonly Type[] _tableTypes = new[]
    {
    // SUPERVISION
    typeof(SupervisionLocal),
    typeof(ArchivoLocal),
    typeof(PreguntaLocal),
    typeof(EvaluacionLocal),
    typeof(ListadoMaterialLocal),
    typeof(CheckListLocal),
    typeof(SupervisionModel),
    typeof(EstadoModel),
    typeof(InmuebleLocal),
    typeof(ClientsModel),
    typeof(PrecargaSupervision),

    // ENTREGAS
    typeof(PrecargaEntrega),
    typeof(ClienteEntregaPrecarga),
    typeof(InmuebleEntregaPrecarga),
    typeof(ListadoEntregaPrecarga),
    typeof(ListadoMaterialEntregaPrecarga),
    typeof(EntregaLocal),
    typeof(EntregaMaterialLocal),
    typeof(FotoEntregaLocal),
    typeof(EntregaReporteUbicacionLocal),

    // CORRECTIVOS
    typeof(ListCorrecM),
    typeof(ClienteCmModel.ClienteCorrec),
    typeof(InmuebleCmModel.InmuebleCorrec),
    typeof(CorrectivoMPendienteLocal),
    typeof(FotoCorrectivoPendienteLocal),

    // ORDENES
    typeof(OrdenTrabajoModel),
    typeof(PersonalOrdenTrabajoResponse),
    typeof(UnidadMedidaModel),
    typeof(AlmacenModel),
    typeof(MaterialResponse)
};

    public async Task EnsureInitialized() {
        if(_dbConn != null && _tablesCreated) return;

        await _semaphore.WaitAsync();
        try {
            if(_dbConn != null && _tablesCreated) return;

            _dbConn = new SqliteConnection($"Data Source={Constants.DATABASE_PATH}");
            await _dbConn.OpenAsync();
            foreach(var type in _tableTypes) {
                await CreateTableAsync(type);
            }

            _tablesCreated = true;
        } catch(Exception ex) {
            Debug.WriteLine($"Error inicializando base de datos: {ex.Message}");
            throw;
        } finally {
            _semaphore.Release();
        }
    }

    // Método auxiliar para crear tablas
    private async Task CreateTableAsync(Type tableType) {
        // Necesitas mapear tus clases a comandos CREATE TABLE
        var createTableSql = GenerateCreateTableSql(tableType);

        using var command = _dbConn.CreateCommand();
        command.CommandText = createTableSql;
        await command.ExecuteNonQueryAsync();
    }

    // Método para generar SQL CREATE TABLE basado en el tipo de forma dinámica
    private string GenerateCreateTableSql(Type tableType) {
        var tableName = tableType.Name;
        var properties = tableType.GetProperties();

        var columns = new List<string>();
        var primaryKeyAssigned = false;
        foreach(var prop in properties) {
            var columnDefinition = GetColumnDefinition(prop, ref primaryKeyAssigned);
            columns.Add(columnDefinition);
        }

        return $"CREATE TABLE IF NOT EXISTS {tableName} (\n    {string.Join(",\n    ", columns)}\n)";
    }

    private string GetColumnDefinition(PropertyInfo prop, ref bool primaryKeyAssigned) {
        var columnName = prop.Name;
        var sqlType = GetSqliteType(prop.PropertyType);
        var constraints = new List<string>();

        // ORDEN DE PRIORIDAD - solo asignar PK si no se ha asignado ya
        if(!primaryKeyAssigned) {
            // PKs válidas
            if(prop.Name.Equals("IdConsec", StringComparison.OrdinalIgnoreCase) ||
               prop.Name.Equals("IdLocal", StringComparison.OrdinalIgnoreCase) ||
               prop.Name.Equals("IdCorrectivoLocal", StringComparison.OrdinalIgnoreCase) ||
               prop.Name.Equals("IdCarga", StringComparison.OrdinalIgnoreCase) ||

               // PKs reales de catálogos
               prop.Name.Equals("id_inmueble", StringComparison.OrdinalIgnoreCase) ||
               prop.Name.Equals("idCliente", StringComparison.OrdinalIgnoreCase)) {
                constraints.Add("PRIMARY KEY");

                // AUTOINCREMENT solo para IDs locales internos
                if(prop.Name.Equals("IdConsec", StringComparison.OrdinalIgnoreCase) ||
                   prop.Name.Equals("IdLocal", StringComparison.OrdinalIgnoreCase) ||
                   prop.Name.Equals("IdCorrectivoLocal", StringComparison.OrdinalIgnoreCase) ||
                   prop.Name.Equals("IdCarga", StringComparison.OrdinalIgnoreCase)) {
                    constraints.Add("AUTOINCREMENT");
                }

                primaryKeyAssigned = true;
            }
        }

        // Lógica para NOT NULL
        var isNullable = IsPropertyNullable(prop);

        if(!isNullable && !constraints.Contains("PRIMARY KEY")) {
            constraints.Add("NOT NULL");
        }

        return $"{columnName} {sqlType} {string.Join(" ", constraints)}".Trim();
    }

    private bool IsPropertyNullable(PropertyInfo prop) {
        // Strings son siempre nullable
        if(prop.PropertyType == typeof(string))
            return true;

        // Tipos nullable (int?, DateTime?, bool?, etc.)
        if(Nullable.GetUnderlyingType(prop.PropertyType) != null)
            return true;

        // Tipos clase (excepto string que ya manejamos)
        if(prop.PropertyType.IsClass)
            return true;

        // Tipos valor (int, DateTime, bool, etc.) - NO son nullable
        if(prop.PropertyType.IsValueType)
            return false;

        // Por defecto, considerar como nullable
        return true;
    }

    private string GetSqliteType(Type propertyType) {
        // Manejar tipos nullable
        var underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

        return underlyingType switch {
            Type t when t == typeof(int) || t == typeof(long) || t == typeof(short) || t == typeof(bool) => "INTEGER",
            Type t when t == typeof(double) || t == typeof(float) || t == typeof(decimal) => "REAL",
            Type t when t == typeof(DateTime) => "TEXT",
            Type t when t == typeof(byte[]) => "BLOB",
            Type t when t.IsEnum => "INTEGER",
            _ => "TEXT"
        };
    }

    private string GetColumnConstraints(PropertyInfo prop) {
        var constraints = new List<string>();

        if(prop.Name.Equals("IdConsec", StringComparison.OrdinalIgnoreCase)) {
            constraints.Add("PRIMARY KEY");
            constraints.Add("AUTOINCREMENT");
        } else if(prop.Name.Equals("IdLocal", StringComparison.OrdinalIgnoreCase)) {
            constraints.Add("PRIMARY KEY");
            constraints.Add("AUTOINCREMENT");
        } else if(prop.Name.Equals("IdCarga", StringComparison.OrdinalIgnoreCase)) {
            constraints.Add("PRIMARY KEY");
            constraints.Add("AUTOINCREMENT");
        }

        //// Si la propiedad se llama "Id" asumimos que es la clave primaria
        //if(prop.Name.Equals("Id", StringComparison.OrdinalIgnoreCase) ||
        //prop.Name.Equals("IdLocal", StringComparison.OrdinalIgnoreCase) ||
        ////prop.Name.Equals("IdConsec", StringComparison.OrdinalIgnoreCase) ||
        //prop.Name.Equals("IdCarga", StringComparison.OrdinalIgnoreCase)) {
        //    constraints.Add("PRIMARY KEY");
        //    constraints.Add("AUTOINCREMENT");
        //}

        // Si no es nullable y no es la clave primaria, agregar NOT NULL
        var underlyingType = Nullable.GetUnderlyingType(prop.PropertyType);
        var isNullable = underlyingType != null ||
                        (prop.PropertyType.IsClass && prop.PropertyType != typeof(string)) ||
                        prop.PropertyType == typeof(string);

        if(!isNullable && !prop.Name.Equals("Id", StringComparison.OrdinalIgnoreCase)) {
            constraints.Add("NOT NULL");
        }

        return string.Join(" ", constraints);
    }

    #region Supervision

    public async Task<bool> InsertSupervisionTotal(SupervisionRequestDataModel supervision) {
        try {
            if(supervision != null) {
                await EnsureInitialized();
                SupervisionLocal supervisionLocal = new SupervisionLocal {
                    IdStatusLocal = 1,
                    IdOrden = supervision.IdOrden,
                    Usuario = supervision.Usuario,
                    Fechaini = supervision.Fechaini,
                    Fechafin = supervision.Fechafin,
                    Id_Cliente = supervision.Id_Cliente,
                    Id_Inmueble = supervision.Id_Inmueble,
                    Cliente = supervision.Cliente,
                    Inmueble = supervision.Inmueble,
                    Latitud = supervision.Latitud,
                    Longitud = supervision.Longitud,
                    NombreOperador = supervision.NombreOperador,
                    _clienteentrevista = supervision.Clienteentrevista,
                    Clientenombre = supervision.Clientenombre,
                    Clientecomentario = supervision.Clientecomentario,
                    Evalua = supervision.Evalua,
                    Trabrealizados = supervision.Trabrealizados,
                    Tratopersonal = supervision.Tratopersonal,
                    Uniformcompleto = supervision.Uniformcompleto,
                    Suprecorrido = supervision.Suprecorrido,
                    Areaoportunidad = supervision.Areaoportunidad,
                    Plancorrectivo = supervision.Plancorrectivo,
                    Calificasup = supervision.Calificasup,
                    Reporteasiscgo = supervision.Reporteasiscgo,
                    Matetiquetados = supervision.Matetiquetados,
                    Matrequerimientos = supervision.Matrequerimientos,
                };
                int idSupervisionLocal = await InsertSupervisionLocal(supervisionLocal);

                if(supervision.Archivos != null) {
                    if(supervision.Archivos.Count > 0) {
                        List<ArchivoLocal> archivoLocal = new List<ArchivoLocal>();
                        foreach(var archivo in supervision.Archivos) {
                            ArchivoLocal al = new ArchivoLocal {
                                IdLocal = idSupervisionLocal,
                                Nombre = archivo.Nombre,
                                Path = archivo.Path,
                                Seccion = archivo.Seccion,
                                Tamano = archivo.Tamano,
                                IdStatusLocal = 1
                            };
                            archivoLocal.Add(al);
                        }
                        await InsertArchivoLocal(archivoLocal);
                    }
                }

                if(supervision.Preguntas != null) {
                    if(supervision.Preguntas.Count > 0) {
                        List<PreguntaLocal> preguntaLocal = new List<PreguntaLocal>();
                        foreach(var pregunta in supervision.Preguntas) {
                            PreguntaLocal pre = new PreguntaLocal {
                                IdLocal = idSupervisionLocal,
                                IdPregunta = pregunta.IdPregunta,
                                IdSeccion = pregunta.IdSeccion,
                                IdSupervision = pregunta.IdSupervision,
                                Descripcion = pregunta.Descripcion,
                                Observaciones = pregunta.Observaciones,
                                Valor = (int)pregunta.Valor,
                                IdStatusLocal = 1
                            };
                            preguntaLocal.Add(pre);
                        }
                        await InsertPreguntaLocal(preguntaLocal);
                    }
                }

                if(supervision.ChecklistPreguntas != null) {
                    if(supervision.ChecklistPreguntas.Count > 0) {
                        List<CheckListLocal> checkListLocal = new List<CheckListLocal>();
                        foreach(var cl in supervision.ChecklistPreguntas) {
                            CheckListLocal checklist = new CheckListLocal {
                                IdLocal = idSupervisionLocal,
                                IdSupervision = cl.IdSupervision,
                                IdPregunta = cl.IdPregunta,
                                Descripcion = cl.Descripcion,
                                Observaciones = cl.Observaciones,
                                Valor = cl.Valor,
                                IdStatusLocal = 1
                            };
                            checkListLocal.Add(checklist);
                        }
                        await InsertCheckListLocal(checkListLocal);
                    }
                }

                if(supervision.ListadoMateriales != null) {
                    if(supervision.ListadoMateriales.Count > 0) {
                        List<ListadoMaterialLocal> listadoLocal = new List<ListadoMaterialLocal>();
                        foreach(var list in supervision.ListadoMateriales) {
                            ListadoMaterialLocal listado = new ListadoMaterialLocal {
                                IdLocal = idSupervisionLocal,
                                IdListado = list.IdListado,
                                Clave = list.Clave,
                                Descripcion = list.Descripcion,
                                Cantidad = list.Cantidad,
                                Entregado = list.Entregado,
                                Sugerido = list.Sugerido,
                                IdStatusLocal = 1
                            };
                            listadoLocal.Add(listado);
                        }
                        await InsertListadoMaterialLocal(listadoLocal);
                    }
                }

                if(supervision.PreguntasEvaluacion != null) {
                    if(supervision.PreguntasEvaluacion.Count > 0) {
                        List<EvaluacionLocal> evaluacionLocal = new List<EvaluacionLocal>();
                        foreach(var item in supervision.PreguntasEvaluacion) {
                            EvaluacionLocal evaluacion = new EvaluacionLocal {
                                IdLocal = idSupervisionLocal,
                                IdSupervision = item.IdSupervision,
                                IdPregunta = item.IdPregunta,
                                Descripcion = item.Descripcion,
                                _valor = item.Valor,
                                Observaciones = item.Observaciones,
                                IdStatusLocal = 1
                            };
                            evaluacionLocal.Add(evaluacion);
                        }
                        await InsertEvaluacionLocal(evaluacionLocal);
                    }
                }

                return true;
            }
            return true;
        } catch(Exception ex) {
            Debug.WriteLine($"Error insertando supervisión: {ex.Message}");
            return false;
        }
    }

    public async Task<int> InsertSupervisionLocal(SupervisionLocal supervision) {
        await InsertAsync(supervision);
        return supervision.IdLocal;
    }

    public async Task<bool> InsertPreguntaLocal(List<PreguntaLocal> preguntas) {
        try {
            await InsertAllAsync(preguntas);
            return true;
        } catch(Exception ex) {
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    public async Task<bool> InsertArchivoLocal(List<ArchivoLocal> archivos) {
        try {
            await InsertAllAsync(archivos);
            return true;
        } catch(Exception ex) {
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    public async Task<bool> InsertCheckListLocal(List<CheckListLocal> supervision) {
        try {
            await InsertAllAsync(supervision);
            return true;
        } catch(Exception ex) {
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    public async Task<bool> InsertListadoMaterialLocal(List<ListadoMaterialLocal> listado) {
        try {
            await InsertAllAsync(listado);
            return true;
        } catch(Exception ex) {
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    public async Task<bool> InsertEvaluacionLocal(List<EvaluacionLocal> evaluacion) {
        try {
            await InsertAllAsync(evaluacion);
            return true;
        } catch(Exception ex) {
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    public async Task<SupervisionRequestDataModel> GetSupervisionLocal(int IdLocal) {
        try {
            SupervisionRequestDataModel supervision = new SupervisionRequestDataModel();
            await EnsureInitialized();
            //Obtener Header
            var supervisionLocal = await _dbConn.Table<SupervisionLocal>().Where(e => e.IdLocal == IdLocal).FirstOrDefaultAsync();

            //Obtener Archivos
            var archivos = await _dbConn.Table<ArchivoLocal>().Where(a => a.IdLocal == IdLocal).ToListAsync();

            //Obtener Preguntas
            var preguntas = await _dbConn.Table<PreguntaLocal>().Where(a => a.IdLocal == IdLocal).ToListAsync();

            //Obtener Checklist
            var checklist = await _dbConn.Table<CheckListLocal>().Where(a => a.IdLocal == IdLocal).ToListAsync();

            //Obtener Evaluaciones
            var evaluacion = await _dbConn.Table<EvaluacionLocal>().Where(a => a.IdLocal == IdLocal).ToListAsync();

            //Obtener Listados
            var listados = await _dbConn.Table<ListadoMaterialLocal>().Where(a => a.IdLocal == IdLocal).ToListAsync();

            if(supervisionLocal != null) {
                //Asignar el header
                supervision.IdOrden = supervisionLocal.IdOrden;
                supervision.Usuario = supervisionLocal.Usuario;
                supervision.Fechaini = supervisionLocal.Fechaini;
                supervision.Fechafin = supervisionLocal.Fechafin;
                supervision.Id_Cliente = supervisionLocal.Id_Cliente;
                supervision.Id_Inmueble = supervisionLocal.Id_Inmueble;
                supervision.Latitud = supervisionLocal.Latitud;
                supervision.Longitud = supervisionLocal.Longitud;
                supervision.NombreOperador = supervisionLocal.NombreOperador;
                supervision.Clienteentrevista = supervisionLocal._clienteentrevista;
                supervision.Clientenombre = supervisionLocal.Clientenombre;
                supervision.Clientecomentario = supervisionLocal.Clientecomentario;
                supervision.Evalua = supervisionLocal.Evalua;
                supervision.Trabrealizados = supervisionLocal.Trabrealizados;
                supervision.Tratopersonal = supervisionLocal.Tratopersonal;
                supervision.Uniformcompleto = supervisionLocal.Uniformcompleto;
                supervision.Suprecorrido = supervisionLocal.Suprecorrido;
                supervision.Areaoportunidad = supervisionLocal.Areaoportunidad;
                supervision.Plancorrectivo = supervisionLocal.Plancorrectivo;
                supervision.Calificasup = supervisionLocal.Calificasup;
                supervision.Reporteasiscgo = supervisionLocal.Reporteasiscgo;
                supervision.Matetiquetados = supervisionLocal.Matetiquetados;
                supervision.Matrequerimientos = supervisionLocal.Matrequerimientos;
            }

            //Asingar archivos
            if(archivos != null) {
                if(archivos.Count > 0) {
                    supervision.Archivos = new List<ArchivoModel>();
                    foreach(var item in archivos) {
                        ArchivoModel a = new ArchivoModel {
                            Path = item.Path,
                            Nombre = item.Nombre,
                            Tamano = item.Tamano,
                            Seccion = item.Seccion
                        };
                        supervision.Archivos.Add(a);
                    }
                }
            }

            //Asignar preguntas
            if(preguntas != null) {
                if(preguntas.Count > 0) {
                    supervision.Preguntas = new List<SupervisionPregunta>();
                    foreach(var item in preguntas) {
                        SupervisionPregunta p = new SupervisionPregunta {
                            IdSupervision = item.IdSupervision,
                            IdSeccion = item.IdSeccion,
                            IdPregunta = item.IdPregunta,
                            Descripcion = item.Descripcion,
                            Valor = item.Valor,
                            Observaciones = item.Observaciones
                        };
                        supervision.Preguntas.Add(p);
                    }
                }
            }

            //Asignar checklist
            if(checklist != null) {
                if(checklist.Count > 0) {
                    supervision.ChecklistPreguntas = new List<ChecklistPregunta>();
                    foreach(var item in checklist) {
                        ChecklistPregunta c = new ChecklistPregunta {
                            IdSupervision = item.IdSupervision,
                            IdPregunta = item.IdPregunta,
                            Descripcion = item.Descripcion,
                            Valor = item.Valor,
                            Observaciones = item.Observaciones
                        };
                        supervision.ChecklistPreguntas.Add(c);
                    }
                }
            }

            //Asignar evaluacion
            if(evaluacion != null) {
                if(evaluacion.Count > 0) {
                    supervision.PreguntasEvaluacion = new List<Evaluacion>();
                    foreach(var item in evaluacion) {
                        Evaluacion e = new Evaluacion {
                            IdSupervision = item.IdSupervision,
                            IdPregunta = item.IdPregunta,
                            Descripcion = item.Descripcion,
                            Valor = item._valor,
                            Observaciones = item.Observaciones
                        };
                        supervision.PreguntasEvaluacion.Add(e);
                    }
                }
            }

            //Asignar listados
            if(listados != null) {
                if(listados.Count > 0) {
                    supervision.ListadoMateriales = new List<ListadoMaterial>();
                    foreach(var item in listados) {
                        ListadoMaterial l = new ListadoMaterial {
                            IdListado = item.IdListado,
                            Clave = item.Clave,
                            Descripcion = item.Descripcion,
                            Cantidad = item.Cantidad,
                            Entregado = item.Entregado,
                            Sugerido = item.Sugerido
                        };
                        supervision.ListadoMateriales.Add(l);
                    }
                }
            }
            return supervision;
        } catch(Exception ex) {
            Console.WriteLine(ex.Message);
            throw ex;
        }
    }

    public async Task<bool> MarcarSupervisionEnviada(int idSupervision) {
        const int ESTATUS_ENVIADO = 2;

        try {
            int rowsAffected = await _dbConn.ExecuteAsync(
                "UPDATE SupervisionLocal SET IdStatusLocal = $p0 WHERE IdLocal = $p1",
                ESTATUS_ENVIADO,
                idSupervision);

            return rowsAffected > 0;
        } catch(Exception ex) {
            Debug.WriteLine($"Error al actualizar estatus: {ex.Message}");
            return false;
        }
    }

    //public async Task<List<SupervisionLocal>> GetSupervisionesSinEnviar()
    //{
    //    await EnsureInitialized();
    //    const int ESTATUS_PENDIENTE = 1;
    //    try {
    //        return await _dbConn.Table<SupervisionLocal>()
    //                      .Where(s => s.IdStatusLocal == ESTATUS_PENDIENTE)
    //                      .OrderBy(s => s.Fechaini)
    //                      .ToListAsync();
    //    } catch(Exception ex) {
    //        Console.WriteLine($"Error al obtener supervisiones: {ex.Message}");
    //        return new List<SupervisionLocal>();
    //    }
    //}

    public async Task<List<SupervisionLocal>> GetSupervisionesSinEnviar() {
        await EnsureInitialized();
        const int ESTATUS_PENDIENTE = 1;

        try {
            var supervisiones = new List<SupervisionLocal>();

            using var command = _dbConn.CreateCommand();
            command.CommandText = @"
        SELECT * FROM SupervisionLocal
        WHERE IdStatusLocal = $estatus
        ORDER BY Fechaini";

            command.Parameters.AddWithValue("$estatus", ESTATUS_PENDIENTE);

            using var reader = await command.ExecuteReaderAsync();
            while(await reader.ReadAsync()) {
                supervisiones.Add(new SupervisionLocal {
                    IdLocal = reader.GetInt32(reader.GetOrdinal("IdLocal")),
                    IdStatusLocal = reader.GetInt32(reader.GetOrdinal("IdStatusLocal")),
                    IdOrden = reader.GetInt32(reader.GetOrdinal("IdOrden")),
                    Usuario = reader.GetInt32(reader.GetOrdinal("Usuario")),
                    Fechaini = reader.GetDateTime(reader.GetOrdinal("Fechaini")),
                    Fechafin = reader.GetDateTime(reader.GetOrdinal("Fechafin")),
                    Id_Cliente = reader.GetInt32(reader.GetOrdinal("Id_Cliente")),
                    Id_Inmueble = reader.GetInt32(reader.GetOrdinal("Id_Inmueble")),
                    Cliente = reader.GetString(reader.GetOrdinal("Cliente")),
                    Inmueble = reader.GetString(reader.GetOrdinal("Inmueble")),
                    Latitud = reader.GetString(reader.GetOrdinal("Latitud")),
                    Longitud = reader.GetString(reader.GetOrdinal("Longitud")),

                    // VERIFICAR NULL para campos que pueden ser NULL
                    NombreOperador = reader.IsDBNull(reader.GetOrdinal("NombreOperador"))
                        ? null : reader.GetString(reader.GetOrdinal("NombreOperador")),

                    _clienteentrevista = reader.GetBoolean(reader.GetOrdinal("_clienteentrevista")),
                    Clientenombre = reader.IsDBNull(reader.GetOrdinal("Clientenombre"))
                        ? null : reader.GetString(reader.GetOrdinal("Clientenombre")),
                    Clientecomentario = reader.IsDBNull(reader.GetOrdinal("Clientecomentario"))
                        ? null : reader.GetString(reader.GetOrdinal("Clientecomentario")),
                    Evalua = reader.GetInt32(reader.GetOrdinal("Evalua")),
                    Trabrealizados = reader.GetInt32(reader.GetOrdinal("Trabrealizados")),
                    Tratopersonal = reader.GetInt32(reader.GetOrdinal("Tratopersonal")),
                    Uniformcompleto = reader.GetBoolean(reader.GetOrdinal("Uniformcompleto")),
                    Suprecorrido = reader.GetBoolean(reader.GetOrdinal("Suprecorrido")),
                    Areaoportunidad = reader.GetBoolean(reader.GetOrdinal("Areaoportunidad")),
                    Plancorrectivo = reader.GetBoolean(reader.GetOrdinal("Plancorrectivo")),
                    Calificasup = reader.GetInt32(reader.GetOrdinal("Calificasup")),
                    Reporteasiscgo = reader.GetBoolean(reader.GetOrdinal("Reporteasiscgo")),
                    Matetiquetados = reader.GetBoolean(reader.GetOrdinal("Matetiquetados")),
                    Matrequerimientos = reader.GetBoolean(reader.GetOrdinal("Matrequerimientos")),
                    IdSupervisionGeneradaSinga = reader.GetInt32(reader.GetOrdinal("IdSupervisionGeneradaSinga"))
                });
            }

            return supervisiones;
        } catch(Exception ex) {
            Debug.WriteLine($"Error al obtener supervisiones: {ex.Message}");
            return new List<SupervisionLocal>();
        }
    }

    public async Task<bool> InsertSupervisionProgramadaLocal(List<SupervisionModel> supervisiones) {
        await EnsureInitialized();

        // Usar transacción para asegurar atomicidad
        using var transaction = await _dbConn.BeginTransactionAsync();
        try {
            // 1. Eliminar todos los registros existentes
            await DeleteAllAsync<SupervisionModel>();

            // 2. Insertar todos los nuevos registros
            await InsertAllAsync(supervisiones);

            await transaction.CommitAsync();
            return true;
        } catch(Exception ex) {
            await transaction.RollbackAsync();
            Debug.WriteLine($"Error SQLite InsertSupervisionProgramadaLocal: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> InsertClientesLocal(List<ClientsModel> clientes) {
        try {
            await EnsureInitialized();
            await DeleteAllAsync<ClientsModel>();
            await InsertAllAsync(clientes);
            return true;
        } catch(Exception ex) {
            Console.WriteLine("Error SQLite InsertClienteLocal:" + ex.Message);
            return false;
        }
    }

    public async Task<bool> InsertEstadosLocal(List<EstadoModel> estados) {
        try {
            await EnsureInitialized();
            await DeleteAllAsync<EstadoModel>();
            await InsertAllAsync(estados);
            return true;
        } catch(Exception ex) {
            Console.WriteLine("Error SQLite InsertEstadosLocal:" + ex.Message);
            return false;
        }
    }

    public async Task<bool> InsertInmueblesLocal(List<InmuebleLocal> inmuebles) {
        try {
            await EnsureInitialized();
            await DeleteAllAsync<InmuebleLocal>();
            await InsertAllAsync(inmuebles);
            return true;
        } catch(Exception ex) {
            Console.WriteLine("Error SQLite InsertInmueblesLocal:" + ex.Message);
            return false;
        }
    }

    public async Task<bool> InsertFechaCarga() {
        try {
            PrecargaSupervision pc = new PrecargaSupervision {
                FechaCarga = DateTime.Now
            };
            await InsertAsync(pc);
            return true;
        } catch(Exception ex) {
            Console.WriteLine("Error SQLite InserFechaCarga: " + ex.Message);
            return false;
        }
    }

    public async Task<bool> InsertFechaCargaEntrega() {
        try {
            PrecargaEntrega pc = new PrecargaEntrega {
                FechaCarga = DateTime.Now
            };
            await InsertAsync(pc);
            return true;
        } catch(Exception ex) {
            Console.WriteLine("Error SQLite InsertFechaCargaEntrega: " + ex.Message);
            return false;
        }
    }

    public async Task<List<SupervisionModel>> GetSupervisionesLocal() {
        try {
            await EnsureInitialized();
            return await _dbConn.Table<SupervisionModel>().ToListAsync();
        } catch(Exception ex) {
            Console.WriteLine("Error SQLite GetSupervisionesLocal:" + ex.Message);
            return new List<SupervisionModel>();
        }
    }

    public async Task<List<ClientsModel>> GetClientesLocal() {
        try {
            await EnsureInitialized();
            return await _dbConn.Table<ClientsModel>().ToListAsync();
        } catch(Exception ex) {
            Console.WriteLine("Error SQLite GetSupervisionesLocal:" + ex.Message);
            return new List<ClientsModel>();
        }
    }

    public async Task<List<EstadoModel>> GetEstadosLocal() {
        try {
            await EnsureInitialized();
            return await _dbConn.Table<EstadoModel>().ToListAsync();
        } catch(Exception ex) {
            Console.WriteLine("Error SQLite GetSupervisionesLocal:" + ex.Message);
            return new List<EstadoModel>();
        }
    }

    public async Task<List<Inmueble>> GetinmueblesLocal(int IdCliente, int IdEstado) {
        if(IdEstado == null) {
            IdEstado = 0;
        }
        try {
            await EnsureInitialized();

            //LOGS
            //var todos = await _dbConn.Table<InmuebleLocal>().ToListAsync();
            //Console.WriteLine($"Total ordenes en tabla: {todos.Count}");
            //foreach(var item in todos) {
            //    Console.WriteLine($"Inmueble: {item.IdInmueble}, Cliente: {item.IdCliente}");
            //}

            var query = _dbConn.Table<InmuebleLocal>().Where(x => x.IdCliente == IdCliente);

            if(IdEstado != 0) {
                query = query.Where(x => x.IdEstado == IdEstado);
            }
            var list = await query.ToListAsync();
            var filtrado = list.Where(x => x.IdCliente == IdCliente).ToList();
            if(IdEstado != 0) {
                filtrado = filtrado.Where(x => x.IdEstado == IdEstado).ToList();
            }

            List<Inmueble> inmuebles = new List<Inmueble>();
            if(list != null) {
                foreach(var item in filtrado) {
                    Inmueble inmueble = new Inmueble {
                        IdInmueble = item.IdInmueble,
                        Nombre = item.Nombre,
                        Tipo = item.Tipo
                    };
                    inmuebles.Add(inmueble);
                }
            }
            return inmuebles;
        } catch(Exception ex) {
            Console.WriteLine("Error SQLite GetSupervisionesLocal:" + ex.Message);
            return new List<Inmueble>();
        }
    }

    public async Task<DateTime> GetUltimaCarga() {
        try {
            await EnsureInitialized();
            var carga = await _dbConn.Table<PrecargaSupervision>()
                         .OrderByDescending(x => x.IdCarga)
                         .FirstOrDefaultAsync();

            return carga?.FechaCarga ?? DateTime.MinValue;
        } catch(Exception ex) {
            Console.WriteLine("Error SQLITE GetultimaCarga:" + ex.Message);
            throw;
        }
    }

    public async Task<DateTime> GetUltimaCargaEntregas() {
        try {
            await EnsureInitialized();
            var carga = await _dbConn.Table<PrecargaEntrega>()
                         .OrderByDescending(x => x.IdCarga)
                         .FirstOrDefaultAsync();

            return carga?.FechaCarga ?? DateTime.MinValue;
        } catch(Exception ex) {
            Console.WriteLine("Error SQLITE GetUltimaCargaEntregas:" + ex.Message);
            throw;
        }
    }

    public async Task<bool> VerificarBancoInmueble(int idInmueble) {
        try {
            await EnsureInitialized();

            var inmueble = await _dbConn.Table<InmuebleLocal>()
                         .Where(x => x.IdInmueble == idInmueble)
                         .FirstOrDefaultAsync();

            if(inmueble != null) {
                return inmueble.AreaBanco;
            } else {
                return false; // O decide qué valor tiene sentido si no encuentra nada
            }
        } catch(Exception ex) {
            Console.WriteLine("Error SQLITE VerificarBancoInmueble: " + ex.Message);
            throw;
        }
    }

    public async Task<bool> DeleteSupervisionProgramadaLocal(int idOrden) {
        try {
            int rowsAffected = await _dbConn.ExecuteAsync(
                "DELETE FROM SupervisionModel WHERE Orden = $p0",
                idOrden);

            return rowsAffected > 0;
        } catch(Exception ex) {
            Console.WriteLine($"Error al actualizar estatus: {ex.Message}");
            return false;
        }
    }

    //public async Task<bool> MarcarSupervisionEnviadaCopy(int idSupervision) {
    //    const int ESTATUS_ENVIADO = 2;

    //    try {
    //        int rowsAffected = await _dbConn.ExecuteAsync(
    //            "UPDATE SupervisionLocal SET IdStatusLocal = $p0 WHERE IdLocal = $p1",
    //            ESTATUS_ENVIADO,
    //            idSupervision);

    //        return rowsAffected > 0;
    //    } catch(Exception ex) {
    //        Debug.WriteLine($"Error al actualizar estatus: {ex.Message}");
    //        return false;
    //    }
    //}

    #endregion Supervision

    //    #region Entregas

    //    public async Task<bool> InsertClientesLocal(List<ClientsModelLocal> clientesLocal) {
    //        try {
    //            await EnsureInitialized();
    //            await _dbConn.DeleteAllAsync<ClientsModelLocal>();
    //            await _dbConn.InsertAllAsync(clientesLocal);
    //            return true;
    //        } catch(Exception ex) {
    //            Console.WriteLine("Error SQLite InsertClientesLocal:" + ex.Message);
    //            return false;
    //        }
    //    }
    //    public async Task<bool> InsertInmueblesLocal(List<InmueblesModelLocal> clientesLocal) {
    //        try {
    //            await EnsureInitialized();
    //            await _dbConn.DeleteAllAsync<InmueblesModelLocal>();
    //            await _dbConn.InsertAllAsync(clientesLocal);
    //            return true;
    //        } catch(Exception ex) {
    //            Console.WriteLine("Error SQLite InsertClientesLocal:" + ex.Message);
    //            return false;
    //        }
    //    }
    //    public async Task<bool> InsertListadoHeaders(List<ListApp> listaHeaders) {
    //        try {
    //            await EnsureInitialized();
    //            await _dbConn.DeleteAllAsync<ListApp>();
    //            await _dbConn.InsertAllAsync(listaHeaders);
    //            return true;
    //        } catch(Exception ex) {
    //            Console.WriteLine("Error SQLite InsertListadoHeaders:" + ex.Message);
    //            return false;
    //        }

    //    }
    //    public async Task<bool> InsertListadoDetalle(List<ListadoMaterialesModel> listaDetalle) {
    //        try {
    //            await EnsureInitialized();
    //            await _dbConn.DeleteAllAsync<ListadoMaterialesModel>();
    //            await _dbConn.InsertAllAsync(listaDetalle);
    //            return true;
    //        } catch(Exception ex) {
    //            Console.WriteLine("Error SQLite InsertListadoDetalle:" + ex.Message);
    //            return false;
    //        }
    //    }

    //    public async Task<List<ClientsModelLocal>> GetEntregasClientesLocal(){
    //        try
    //        {
    //            await EnsureInitialized();
    //            return await _dbConn.Table<ClientsModelLocal>().ToListAsync();
    //}
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine("Error SQLite GetSupervisionesLocal:" + ex.Message);
    //            return new List<ClientsModelLocal>();
    //        }
    //    }

    //    public async Task<List<InmueblesModelLocal>> GetEntregasInmueblesLocal() {
    //        try {
    //            await EnsureInitialized();
    //            return await _dbConn.Table<InmueblesModelLocal>().ToListAsync();
    //        } catch(Exception ex) {
    //            Console.WriteLine("Error SQLite GetSupervisionesLocal:" + ex.Message);
    //            return new List<InmueblesModelLocal>();
    //        }
    //    }

    //    public async Task<List<InmueblesModelLocal>> GetEntregasInmueblesLocal() {
    //        try {
    //            await EnsureInitialized();
    //            return await _dbConn.Table<InmueblesModelLocal>().ToListAsync();
    //        } catch(Exception ex) {
    //            Console.WriteLine("Error SQLite GetSupervisionesLocal:" + ex.Message);
    //            return new List<InmueblesModelLocal>();
    //        }
    //    }

    //    #endregion

    private async Task DeleteAllAsync<T>() {
        var tableName = typeof(T).Name;
        using var command = _dbConn.CreateCommand();
        command.CommandText = $"DELETE FROM {tableName}";
        await command.ExecuteNonQueryAsync();
    }

    private async Task InsertAllAsync<T>(List<T> items) where T : class, new() {
        if(items == null || items.Count == 0)
            return;

        await EnsureInitialized();

        var primaryKeyProperty = GetPrimaryKeyProperty(typeof(T));

        var properties = typeof(T).GetProperties()
            .Where(p => p.CanWrite &&
                       (primaryKeyProperty == null ||
                        p != primaryKeyProperty ||
                        !IsDefaultValue(primaryKeyProperty.GetValue(items[0]))))
            .ToList();

        var columnNames = string.Join(", ", properties.Select(p => p.Name));
        var parameterNames = string.Join(", ", properties.Select(p => $"@{p.Name}"));
        var sql = $"INSERT INTO {typeof(T).Name} ({columnNames}) VALUES ({parameterNames});";

        using var transaction = _dbConn.BeginTransaction(); // 💥 Transacción global
        using var command = _dbConn.CreateCommand();
        command.CommandText = sql;

        // Crear parámetros una sola vez (los reusamos)
        foreach(var prop in properties)
            command.Parameters.Add(new SqliteParameter($"@{prop.Name}", DbType.Object));

        foreach(var item in items) {
            foreach(var prop in properties) {
                var value = prop.GetValue(item);
                command.Parameters[$"@{prop.Name}"].Value = value ?? DBNull.Value;
            }

            await command.ExecuteNonQueryAsync(); // 👈 Sin SELECT last_insert_rowid()
        }

        transaction.Commit(); // ✅ Un solo commit al final
    }

    public async Task<int> InsertAsync<T>(T item) where T : class, new() {
        await EnsureInitialized();

        // Obtener la propiedad PK dinámicamente
        var primaryKeyProperty = GetPrimaryKeyProperty(typeof(T));

        // Excluir la PK solo si su valor es 0 (para autoincrement)
        var properties = typeof(T).GetProperties()
            .Where(p => p.CanWrite &&
                       (primaryKeyProperty == null ||
                        p != primaryKeyProperty ||
                        !IsDefaultValue(primaryKeyProperty.GetValue(item))))
            .ToList();

        var columnNames = string.Join(", ", properties.Select(p => p.Name));
        var parameterNames = string.Join(", ", properties.Select(p => $"${p.Name}"));

        var sql = $"INSERT INTO {typeof(T).Name} ({columnNames}) VALUES ({parameterNames}); SELECT last_insert_rowid();";

        using var command = _dbConn.CreateCommand();
        command.CommandText = sql;

        foreach(var prop in properties) {
            var value = prop.GetValue(item);
            command.Parameters.AddWithValue($"${prop.Name}", value ?? DBNull.Value);
        }

        var newId = (long)(await command.ExecuteScalarAsync())!;

        // ¡IMPORTANTE! Actualizar la propiedad PK del objeto con el nuevo ID
        if(primaryKeyProperty != null && primaryKeyProperty.CanWrite) {
            primaryKeyProperty.SetValue(item, Convert.ChangeType(newId, primaryKeyProperty.PropertyType));
        }

        return (int)newId;
    }

    // Método para obtener la propiedad PK dinámicamente
    private PropertyInfo GetPrimaryKeyProperty(Type type) {
        var properties = type.GetProperties();

        // ORDEN DE PRIORIDAD para detectar PK
        var pkPriority = new[] { "Clave","IdCorrectivoLocal", "IdConsec", "IdLocal", "IdCarga", "Id" };

        foreach(var pkName in pkPriority) {
            var property = properties.FirstOrDefault(p =>
                p.Name.Equals(pkName, StringComparison.OrdinalIgnoreCase));
            if(property != null) {
                return property;
            }
        }

        return null;
    }

    public async Task<int> UpdateAsync<T>(T item) where T : class, new() {
        await EnsureInitialized();

        var properties = typeof(T).GetProperties()
            .Where(p => p.CanRead && p.CanWrite)
            .ToList();

        var primaryKeyProperty = GetPrimaryKeyProperty(typeof(T));

        if(primaryKeyProperty == null)
            throw new Exception($"No se encontró PRIMARY KEY para {typeof(T).Name}");

        var setClause = string.Join(", ",
            properties
                .Where(p => p.Name != primaryKeyProperty.Name)
                .Select(p => $"{p.Name} = ${p.Name}"));

        var sql =
            $"UPDATE {typeof(T).Name} " +
            $"SET {setClause} " +
            $"WHERE {primaryKeyProperty.Name} = $PrimaryKey";

        using var command = _dbConn.CreateCommand();

        command.CommandText = sql;

        foreach(var prop in properties) {
            if(prop.Name == primaryKeyProperty.Name)
                continue;

            var value = prop.GetValue(item);

            command.Parameters.AddWithValue(
                $"${prop.Name}",
                value ?? DBNull.Value
            );
        }

        command.Parameters.AddWithValue(
            "$PrimaryKey",
            primaryKeyProperty.GetValue(item)
        );

        return await command.ExecuteNonQueryAsync();
    }

    // Método para verificar si un valor es el valor por defecto
    private bool IsDefaultValue(object value) {
        if(value == null) return true;

        if(value is int intValue) return intValue == 0;
        if(value is long longValue) return longValue == 0;
        if(value is bool boolValue) return boolValue == false;
        if(value is short shortValue) return shortValue == 0;
        if(value is byte byteValue) return byteValue == 0;

        return value.Equals(Activator.CreateInstance(value.GetType()));
    }

    #region ENTREGAS PRECARGA

    //INSERTAR PRECARGA
    public async Task<bool> InsertPrecargaEntregasLocal(EntregaPrecarga precarga) {
        try {
            await EnsureInitialized();

            await DeleteAllAsync<ClienteEntregaPrecarga>();
            await DeleteAllAsync<InmuebleEntregaPrecarga>();
            await DeleteAllAsync<ListadoEntregaPrecarga>();
            await DeleteAllAsync<ListadoMaterialEntregaPrecarga>();
            if(precarga.Clientes != null && precarga.Clientes.Count > 0) {
                await InsertAllAsync(precarga.Clientes);
            }
            if(precarga.Inmuebles != null && precarga.Inmuebles.Count > 0) {
                await InsertAllAsync(precarga.Inmuebles);
            }
            if(precarga.Listados != null && precarga.Listados.Count > 0) {
                await InsertAllAsync(precarga.Listados);
            }
            if(precarga.ListadosDetalle != null && precarga.ListadosDetalle.Count > 0) {
                await InsertAllAsync(precarga.ListadosDetalle);
            }
            return true;
        } catch(Exception ex) {
            Console.WriteLine("Error SQLite, metodo: InsertPrecargaEntregasLocal:" + ex.Message);
            return false;
        }
    }

    public async Task<List<ClienteEntregaPrecarga>> ObtenerClientesEntregaPrecarga() {
        try {
            await EnsureInitialized();
            return await _dbConn.Table<ClienteEntregaPrecarga>().ToListAsync();
        } catch(Exception ex) {
            Console.WriteLine("Error SQLite en DbContext ClienteEntregaLocal:" + ex.Message);
            return new List<ClienteEntregaPrecarga>();
        }
    }

    public async Task<List<InmuebleEntregaPrecarga>> ObtenerInmueblesEntregaPrecargaByIdCliente(int IdCliente) {
        try {
            await EnsureInitialized();
            return await _dbConn.Table<InmuebleEntregaPrecarga>().Where(a => a.IdCliente == IdCliente).ToListAsync();
        } catch(Exception ex) {
            Console.WriteLine("Error SQLite en DbContext ObtenerInmueblesEntregaPrecargaByIdCliente:" + ex.Message);
            return new List<InmuebleEntregaPrecarga>();
        }
    }

    public async Task<List<ListadoEntregaPrecarga>> ObtenerListadosEntregaPrecargaByIdInmueble(int IdInmueble) {
        try {
            await EnsureInitialized();
            return await _dbConn.Table<ListadoEntregaPrecarga>().Where(a => a.IdInmueble == IdInmueble).ToListAsync();
        } catch(Exception ex) {
            Console.WriteLine("Error SQLite en DbContext ObtenerListadosEntregaPrecargaByIdInmueble:" + ex.Message);
            return new List<ListadoEntregaPrecarga>();
        }
    }

    public async Task<List<ListadoMaterialEntregaPrecarga>> ObtenerListadoMaterialEntregaPrecargaByIdListado(int IdListado) {
        try {
            await EnsureInitialized();
            return await _dbConn.Table<ListadoMaterialEntregaPrecarga>().Where(a => a.IdListado == IdListado).ToListAsync();
        } catch(Exception ex) {
            Console.WriteLine("Error SQLite en DbContext ObtenerListadoMaterialEntregaPrecargaByIdListado:" + ex.Message);
            return new List<ListadoMaterialEntregaPrecarga>();
        }
    }

    #endregion ENTREGAS PRECARGA

    #region ENTREGAS CONSULTA

    // METODO PRINCIPAL DE INSERCION DE ENTREGAS OFFLINE
    public async Task InsertarEntrega(EntregaLocal entrega, List<EntregaMaterialLocal> entregaMaterial, List<FotoEntregaLocal> entregaFoto) {
        try {
            await EnsureInitialized();
            if(entrega != null) {
                await InsertAsync(entrega);
                int idEntregaLocal = entrega.IdLocal;

                if(entregaMaterial != null && entregaMaterial.Count > 0) {
                    //ASIGNAR EL ID DE LA ENTREGA LOCAL A CADA MATERIAL
                    foreach(var material in entregaMaterial) {
                        material.IdEntregaLocal = idEntregaLocal;
                    }
                    await InsertAllAsync(entregaMaterial);
                }
                if(entregaFoto != null && entregaFoto.Count > 0) {
                    //ASIGNAR EL ID DE LA ENTREGA LOCAL A CADA MATERIAL
                    foreach(var foto in entregaFoto) {
                        foto.IdEntregaLocal = idEntregaLocal;
                    }
                }
                await InsertAllAsync(entregaFoto);
            }
        } catch(Exception ex) {
            Console.WriteLine("Error SQLite InsertarEntrega:" + ex.Message);
        }
    }

    //OBTENER CATALOGO DE ENTREGAS EN LOCAL
    public async Task<List<EntregaLocal>> ObtenerEntregasLocal() {
        try {
            await EnsureInitialized();
            return await _dbConn.Table<EntregaLocal>().ToListAsync();
        } catch(Exception ex) {
            Console.WriteLine("Error SQLite ObtenerEntregasLocal:" + ex.Message);
            return new List<EntregaLocal>();
        }
    }

    //INSERTA UBICACION DE ENTREGA
    public async Task<bool> InsertarUbicacionesEntrega(EntregaReporteUbicacionLocal reporte) {
        try {
            await EnsureInitialized();
            await InsertAsync(reporte);
            return true;
        } catch(Exception ex) {
            Console.WriteLine("Error SQLite InsertarUbicacionesEntrega:" + ex.Message);
            return false;
        }
    }

    //OBTENER REPORTES DE UBICACION EN LOCAL
    public async Task<List<EntregaReporteUbicacionLocal>> ObtenerReportesUbicacionLocales() {
        try {
            await EnsureInitialized();
            return await _dbConn.Table<EntregaReporteUbicacionLocal>().ToListAsync();
        } catch(Exception ex) {
            Console.WriteLine("Error SQLite ObtenerEntregasLocal:" + ex.Message);
            return new List<EntregaReporteUbicacionLocal>();
        }
    }

    //ELIMINAR REPORTE DE UBICACION DE LOCAL
    public async Task<bool> EliminarReportesUbicacionLocal(int idReporte) {
        try {
            int rowsAffected = await _dbConn.ExecuteAsync(
                "DELETE FROM EntregaReporteUbicacionLocal WHERE IdLocal = $p0",
                idReporte);
            return rowsAffected > 0;
        } catch(Exception ex) {
            Console.WriteLine($"Error al elimnar el reporte de ubcacion: {ex.Message}");
            return false;
        }
    }

    //OBTENER ENTREGA Y DETALLE DE ENTREGA LOCAL
    public async Task<EntregaLocalModel> ObtenerEntregaLocalParaEnvio(int IdLocal) {
        try {
            var entregaLocalModel = new EntregaLocalModel();
            int IdEntregaLocal = IdLocal;
            await EnsureInitialized();
            entregaLocalModel.Header = await _dbConn.Table<EntregaLocal>().Where(e => e.IdLocal == IdLocal).FirstOrDefaultAsync();
            entregaLocalModel.Materiales = await _dbConn.Table<EntregaMaterialLocal>().Where(a => a.IdEntregaLocal == IdEntregaLocal).ToListAsync();
            entregaLocalModel.Archivos = await _dbConn.Table<FotoEntregaLocal>().Where(a => a.IdEntregaLocal == IdEntregaLocal).ToListAsync();
            return entregaLocalModel;
        } catch(Exception ex) {
            Console.WriteLine("Error SQLite en DbContext ObtenerEntregaLocalParaEnvio:" + ex.Message);
            return new EntregaLocalModel();
        }
    }

    //ELIMINAR TODO RELACIONADO A LA ENTREGA ENVIADA
    public async Task<bool> EliminarEntregaLocalEnviada(int IdLocal) {
        try {
            await EnsureInitialized();
            int IdEntregaLocal = IdLocal;
            int rowsAffectedEntrega = await _dbConn.ExecuteAsync("DELETE FROM EntregaLocal WHERE IdLocal = $p0", IdLocal);
            int rowsAffectedMaterial = await _dbConn.ExecuteAsync("DELETE FROM EntregaMaterialLocal WHERE IdEntregaLocal = $p0", IdEntregaLocal);
            int rowsAffectedFoto = await _dbConn.ExecuteAsync("DELETE FROM FotoEntregaLocal WHERE IdEntregaLocal = $p0", IdEntregaLocal);
            return true;
        } catch(Exception ex) {
            Console.WriteLine($"Error al eliminar supervision local enviada: {ex.Message}");
            return false;
        }
    }

    //ELIMINAR LISTADO DE LA PRECARGA
    public async Task<bool> EliminarListadoMaterialPrecarga(int IdListado) {
        try {
            await EnsureInitialized();
            int rowsAffectedListado = await _dbConn.ExecuteAsync("DELETE FROM ListadoEntregaPrecarga WHERE IdListado = $p0", IdListado);
            int rowsAffectedMaterial = await _dbConn.ExecuteAsync("DELETE FROM ListadoMaterialEntregaPrecarga WHERE IdListado = $p0", IdListado);
            return true;
        } catch(Exception ex) {
            Console.WriteLine($"Error al eliminar listado de precarga: {ex.Message}");
            return false;
        }
    }

    #endregion ENTREGAS CONSULTA

    #region Correctivos Mayores

    public async Task DeleteAllDataCorrectivos() {
        await EnsureInitialized();

        // Orden recomendado:
        // Primero datos dependientes, luego catálogos
        await DeleteAllAsync<ListCorrecM>();
        await DeleteAllAsync<InmuebleCmModel.InmuebleCorrec>();
        await DeleteAllAsync<ClienteCmModel.ClienteCorrec>();
    }

    public async Task GuardarClientesLocal(List<ClienteCmModel.ClienteCorrec> clientes) {
        await EnsureInitialized();

        await DeleteAllAsync<ClienteCmModel.ClienteCorrec>();

        foreach(var cliente in clientes) {
            cliente.SyncDate = DateTime.Now;
        }

        await InsertAllAsync(clientes);
    }

    public async Task GuardarInmueblesLocal(List<InmuebleCmModel.InmuebleCorrec> inmuebles) {
        await EnsureInitialized();

        if(inmuebles == null || !inmuebles.Any())
            return;

        foreach(var inmueble in inmuebles) {
            inmueble.SyncDate = DateTime.Now;
        }

        // SOLO INSERTA, NO BORRA TODA LA TABLA
        await InsertAllAsync(inmuebles);
    }

    public async Task GuardarCorrectivosLocal(List<ListCorrecM> correctivos) {
        await EnsureInitialized();

        foreach(var item in correctivos) {
            item.SyncDate = DateTime.Now;
        }

        await InsertAllAsync(correctivos);
    }

    public async Task<List<ClienteCmModel.ClienteCorrec>> ObtenerClientesLocales() {
        await EnsureInitialized();

        return await QueryAsync<ClienteCmModel.ClienteCorrec>(
            "SELECT * FROM ClienteCorrec ORDER BY nombre");
    }

    public async Task<List<InmuebleCmModel.InmuebleCorrec>> ObtenerInmueblesLocales(int idCliente) {
        try {
            await EnsureInitialized();
            return await _dbConn.Table<InmuebleCmModel.InmuebleCorrec>().Where(x => x.id_cliente == idCliente).ToListAsync();
        } catch(Exception ex) {
            Console.WriteLine("Error SQLite en DbContext ClienteEntregaLocal:" + ex.Message);
            return new List<InmuebleCmModel.InmuebleCorrec>();
        }
    }

    public async Task<List<ListCorrecM>> ObtenerCorrectivosPorCliente(int idCliente) {
        try {
            await EnsureInitialized();
            var correctivos = await _dbConn.Table<ListCorrecM>().ToListAsync();
            return correctivos.Where(c => c.idCliente == idCliente).ToList();
        } catch(Exception ex) {
            Console.WriteLine("Error SQLite en DbContext ClienteEntregaLocal:" + ex.Message);
            return new List<ListCorrecM>();
        }

        //TestDB();

        //return await QueryAsync<ListCorrecM>(
        //    "SELECT * FROM ListCorrecM WHERE idCliente = $p0",
        //    idCliente);
    }

    public async Task<List<ListCorrecM>> ObtenerCorrectivosPorClienteInmueble(int cliente, int inmueble) {
        await EnsureInitialized();

        var testCorrec = await QueryAsync<ListCorrecM>(
            "SELECT * FROM ListCorrecM");
        var correctivos = testCorrec.Where(x => x.idCliente == cliente && x.idInmueble == inmueble).ToList();

        return correctivos;
    }

    public async Task GuardarCorrectivoPendienteLocal(
     RegistrosCorrectivosMModel registro,
     ObservableCollection<PhotosModel> fotos,
     string pathFirmaLocal) {
        await EnsureInitialized();

        // =========================
        // 1) GUARDAR CABECERA
        // =========================
        var correctivoLocal = new CorrectivoMPendienteLocal {
            IdClaveCM = registro.IdClaveCM,
            TrabajosGeneral = registro.TrabajosGeneral,
            TecnicosUniforme = registro.TecnicosUniforme,
            TratoTecnicos = registro.TratoTecnicos,
            TrabajosOrden = registro.TrabajosOrden,
            MaterialesAdecuados = registro.MaterialesAdecuados,
            Encuestado = registro.Encuestado,
            FirmaPath = pathFirmaLocal,
            Sincronizado = false,
            FechaRegistro = DateTime.Now
        };

        // InsertAsync actualizará automáticamente IdLocal
        await InsertAsync(correctivoLocal);

        // =========================
        // 2) GUARDAR FOTOS
        // =========================
        if(fotos != null && fotos.Any()) {
            foreach(var foto in fotos) {
                if(string.IsNullOrWhiteSpace(foto.UrlPhoto))
                    continue;

                var fotoLocal = new FotoCorrectivoPendienteLocal {
                    IdCorrectivoLocal = correctivoLocal.IdLocal,
                    PathFoto = foto.UrlPhoto,
                    EsFirma = false,
                    Sincronizado = false,
                    FechaRegistro = DateTime.Now
                };

                await InsertAsync(fotoLocal);
            }
        }

        // =========================
        // 3) GUARDAR FIRMA
        // =========================
        if(!string.IsNullOrWhiteSpace(pathFirmaLocal)) {
            var firmaLocal = new FotoCorrectivoPendienteLocal {
                IdCorrectivoLocal = correctivoLocal.IdLocal,
                PathFoto = pathFirmaLocal,
                EsFirma = true,
                Sincronizado = false,
                FechaRegistro = DateTime.Now
            };

            await InsertAsync(firmaLocal);
        }
    }

    public async Task SincronizarCorrectivosPendientes() {
        try {
            await EnsureInitialized();

            // =========================
            // 1) OBTENER CORRECTIVOS NO SINCRONIZADOS
            // =========================
            var correctivos = await _dbConn.Table<CorrectivoMPendienteLocal>()
                .Where(x => x.Sincronizado == false)
                .ToListAsync();

            if(correctivos == null || !correctivos.Any())
                return;

            using var httpClient = new HttpClient();

            // =========================
            // 2) RECORRER CADA CORRECTIVO
            // =========================
            foreach(var local in correctivos) {
                try {
                    // =========================
                    // 3) OBTENER FOTOS RELACIONADAS
                    // =========================
                    var fotos = await _dbConn.Table<FotoCorrectivoPendienteLocal>()
                        .Where(x =>
                            x.IdCorrectivoLocal == local.IdLocal &&
                            x.Sincronizado == false)
                        .ToListAsync();

                    // =========================
                    // 4) SUBIR FOTOS / FIRMA
                    // =========================
                    foreach(var foto in fotos) {
                        if(string.IsNullOrWhiteSpace(foto.PathFoto) ||
                           !File.Exists(foto.PathFoto))
                            continue;

                        using var content = new MultipartFormDataContent();

                        byte[] fileBytes = File.ReadAllBytes(foto.PathFoto);

                        var fileContent = new ByteArrayContent(fileBytes);

                        content.Add(
                            fileContent,
                            "files",
                            Path.GetFileName(foto.PathFoto)
                        );

                        string url =
                            Constants.API_BASE_URL +
                            $"FilesImagenesCM/CargaMul?folio={local.IdClaveCM}";

                        var fotoResponse = await httpClient.PostAsync(
                            url,
                            content
                        );

                        if(fotoResponse.IsSuccessStatusCode) {
                            foto.Sincronizado = true;

                            await UpdateAsync(foto);
                        }
                    }

                    // =========================
                    // 5) ENVIAR REPORTE
                    // =========================
                    var registro = new RegistrosCorrectivosMModel {
                        IdClaveCM = local.IdClaveCM,
                        TrabajosGeneral = local.TrabajosGeneral,
                        TecnicosUniforme = local.TecnicosUniforme,
                        TratoTecnicos = local.TratoTecnicos,
                        TrabajosOrden = local.TrabajosOrden,
                        MaterialesAdecuados = local.MaterialesAdecuados,
                        Encuestado = local.Encuestado
                    };

                    var json = JsonConvert.SerializeObject(registro);

                    var requestContent = new StringContent(
                        json,
                        Encoding.UTF8,
                        "application/json"
                    );

                    var response = await httpClient.PostAsync(
                        Constants.API_BASE_URL + "CorrectivosMReporte",
                        requestContent
                    );

                    // =========================
                    // 6) MARCAR COMO SINCRONIZADO
                    // =========================
                    if(response.IsSuccessStatusCode) {
                        local.Sincronizado = true;

                        await UpdateAsync(local);
                    }
                } catch(Exception ex) {
                    System.Diagnostics.Debug.WriteLine(
                        $"Error sincronizando correctivo local {local.IdLocal}: {ex.Message}"
                    );
                }
            }
        } catch(Exception ex) {
            System.Diagnostics.Debug.WriteLine(
                $"Error general de sincronización: {ex.Message}"
            );
        }
    }

    public async Task<List<T>> QueryAsync<T>(string sql, params object[] parameters) where T : class, new() {
        await EnsureInitialized();

        var result = new List<T>();

        using var command = _dbConn.CreateCommand();
        command.CommandText = sql;

        // Parámetros dinámicos: $p0, $p1, etc.
        for(int i = 0; i < parameters.Length; i++) {
            command.Parameters.AddWithValue($"$p{i}", parameters[i] ?? DBNull.Value);
        }

        using var reader = await command.ExecuteReaderAsync();

        var properties = typeof(T).GetProperties();

        while(await reader.ReadAsync()) {
            var item = new T();

            foreach(var prop in properties) {
                try {
                    int ordinal = reader.GetOrdinal(prop.Name);

                    if(!reader.IsDBNull(ordinal)) {
                        var dbValue = reader.GetValue(ordinal);

                        // Manejar Nullable<T>
                        var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                        // Manejo especial DateTime
                        if(targetType == typeof(DateTime)) {
                            prop.SetValue(item, DateTime.Parse(dbValue.ToString()));
                        }
                        // Bool SQLite (0/1)
                        else if(targetType == typeof(bool)) {
                            prop.SetValue(item, Convert.ToInt32(dbValue) == 1);
                        } else {
                            prop.SetValue(item, Convert.ChangeType(dbValue, targetType));
                        }
                    }
                } catch {
                    // Ignora columnas faltantes o incompatibles
                }
            }

            result.Add(item);
        }

        return result;
    }

    #endregion Correctivos Mayores

    #region Ordenes de Trabajo

    public async Task GuardarOrdenesTrabajoLocal(List<OrdenTrabajoModel> ordenes) {
        try {
            await EnsureInitialized();
          

            if(ordenes == null || !ordenes.Any())
                return;

            foreach(var inmueble in ordenes) {
                inmueble.SyncDate = DateTime.Now;
            }

            await InsertAllAsync(ordenes);
        } catch(Exception ex) {
            Debug.WriteLine($"Error al guardar órdenes de trabajo local: {ex.Message}");
        }
    }

    public async Task GuardarPersonalLocal(List<PersonalOrdenTrabajoResponse> personal) {
        try {
            await EnsureInitialized();  
            if(personal == null || !personal.Any())
                return;

            foreach(var inmueble in personal) {
                inmueble.SyncDate = DateTime.Now;
            }

            await InsertAllAsync(personal);
        } catch(Exception ex) {
            Debug.WriteLine($"Error al guardar personal local: {ex.Message}");
        }
    }

    public async Task GuardarUnidadMedidaLocal(List<UnidadMedidaModel> unidades) {
        try {
            await EnsureInitialized();  
            if(unidades == null || !unidades.Any())
                return;

            foreach(var unidad in unidades) {
                unidad.SyncDate = DateTime.Now;
            }

            await InsertAllAsync(unidades   );
        } catch(Exception ex) {
            Debug.WriteLine($"Error al guardar personal local: {ex.Message}");
        }
    }
    public async Task GuardarAlmacenesLocal(List<AlmacenModel> materiales) {
        try {
            await EnsureInitialized();  
            if(materiales == null || !materiales.Any())
                return;

            foreach(var material in materiales) {
                material.SyncDate = DateTime.Now;
            }

            await InsertAllAsync(materiales);
        } catch(Exception ex) {
            Debug.WriteLine($"Error al guardar personal local: {ex.Message}");
        }
    }
    public async Task GuardarMaterialFiltradoLocal(List<MaterialResponse> materiales) {
        try {
            await EnsureInitialized();  
            if(materiales == null || !materiales.Any())
                return;

            foreach(var material in materiales) {
                material.SyncDate = DateTime.Now;
            }

            await InsertAllAsync(materiales);
        } catch(Exception ex) {
            Debug.WriteLine($"Error al guardar personal local: {ex.Message}");
        }
    }
    public async Task GuardarMaterialCompletoLocal(List<MaterialResponse> materiales) {
        try {
            await EnsureInitialized();  
            if(materiales == null || !materiales.Any())
                return;

            foreach(var material in materiales) {
                material.SyncDate = DateTime.Now;
            }

            await InsertAllAsync(materiales);
        } catch(Exception ex) {
            Debug.WriteLine($"Error al guardar personal local: {ex.Message}");
        }
    }

    public async Task<List<OrdenTrabajoModel>> ObtenerOrdenesTrabajoLocales() {
        try {
            await EnsureInitialized();
            return await _dbConn.Table<OrdenTrabajoModel>().ToListAsync();
        } catch(Exception ex) {
            Console.WriteLine("Error SQLite en DbContext ClienteEntregaLocal:" + ex.Message);
            return new List<OrdenTrabajoModel>();
        }
    }

    public async Task<List<PersonalOrdenTrabajoResponse>> ObtenerPersonalLocales() {
        try {
            await EnsureInitialized();
            return await _dbConn.Table<PersonalOrdenTrabajoResponse>().ToListAsync();
        } catch(Exception ex) {
            Console.WriteLine("Error SQLite en DbContext ClienteEntregaLocal:" + ex.Message);
            return new List<PersonalOrdenTrabajoResponse>();
        }
    }

    public async Task<List<UnidadMedidaModel>> ObtenerUnidadesMedidaLocales() {
        try {
            await EnsureInitialized();
            return await _dbConn.Table<UnidadMedidaModel>().ToListAsync();
        } catch(Exception ex) {
            Console.WriteLine("Error SQLite en DbContext ClienteEntregaLocal:" + ex.Message);
            return new List<UnidadMedidaModel>();
        }
    }
    public async Task<List<AlmacenModel>> ObtenerAlmacenesLocales() {
        try {
            await EnsureInitialized();
            return await _dbConn.Table<AlmacenModel>().ToListAsync();
        } catch(Exception ex) {
            Console.WriteLine("Error SQLite en DbContext ClienteEntregaLocal:" + ex.Message);
            return new List<AlmacenModel>();
        }
    }
    public async Task<List<MaterialResponse>> ObtenerMaterialFiltradoLocales() {
        try {
            //await EnsureInitialized();
            return await _dbConn.Table<MaterialResponse>().ToListAsync();
        } catch(Exception ex) {
            Console.WriteLine("Error SQLite en DbContext ClienteEntregaLocal:" + ex.Message);
            return new List<MaterialResponse>();
        }
    }

    public async Task<List<MaterialResponse>> ObtenerMaterialCompletoLocales() {
        try {
            await EnsureInitialized();
            TestDB();
            var listaTest = await _dbConn.Table<MaterialResponse>().ToListAsync();
            return listaTest;
        } catch(Exception ex) {
            Console.WriteLine("Error SQLite en DbContext ClienteEntregaLocal:" + ex.Message);
            return new List<MaterialResponse>();
        }
    }

    public async Task DeleteAllDataOrdenesTrabajo() {
        await EnsureInitialized();

        // Orden recomendado:
        // Primero datos dependientes, luego catálogos
        await DeleteAllAsync<OrdenTrabajoModel>();
        await DeleteAllAsync<PersonalOrdenTrabajoResponse>();
        await DeleteAllAsync<UnidadMedidaModel>();
        await DeleteAllAsync<AlmacenModel>();
        await DeleteAllAsync<MaterialResponse>();
    }


    public async Task GuardarDataOrdenesTrabajoLocal() {
        await EnsureInitialized();
        await DeleteAllDataOrdenesTrabajo();

        string url = $"{Constants.ORDENES_TRABAJO_API}?idtecnico={UserSession.IdEmpleado}";
        var ListOrdenes = await _httpHelper.GetAsync<List<OrdenTrabajoModel>>(url);
        await EnsureInitialized();
        await GuardarOrdenesTrabajoLocal(ListOrdenes);

        string urlPersonal = $"{Constants.OT_TECNICO_API}";
        var ListPersonal = await _httpHelper.GetAsync<List<PersonalOrdenTrabajoResponse>>(urlPersonal);
        await GuardarPersonalLocal(ListPersonal);

        string urlUnidadesMedida = Constants.OT_UNIDAD_MEDIDA_API;
        var unidadMedidaList = await _httpHelper.GetAsync<List<UnidadMedidaModel>>(urlUnidadesMedida);
        await GuardarUnidadMedidaLocal(unidadMedidaList);

        string urlAlmacenes = Constants.OT_ALMACEN_API;
        var materialesList = await _httpHelper.GetAsync<List<AlmacenModel>>(urlAlmacenes);
        await GuardarAlmacenesLocal(materialesList);

        string urlMateriales = $"{Constants.OT_MATERIAL_COMPLETO_API}";
        var materialCompletoList = await _httpHelper.GetAsync<List<MaterialResponse>>(urlMateriales);
        await GuardarMaterialCompletoLocal(materialCompletoList);

    }
    #endregion Ordenes de Trabajo

    public async Task TestDB() {
        try {
            await EnsureInitialized();
            using var cmd = _dbConn.CreateCommand();

            cmd.CommandText = @"SELECT * from MaterialResponse";

            using var reader = await cmd.ExecuteReaderAsync();

            StringBuilder sb = new();

            while(await reader.ReadAsync()) {
                string row = $"ID: {reader["idOrden"]} | Status: {reader["status"]}";
                sb.AppendLine(row);

                System.Diagnostics.Debug.WriteLine(row);
            }
        } catch(Exception ex) {
        }
    }

}