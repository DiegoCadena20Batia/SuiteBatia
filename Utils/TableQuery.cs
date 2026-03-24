using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.Utils {
    public class TableQuery<T> where T : class, new() {
        private readonly SqliteConnection _connection;
        private string _whereClause;
        private Dictionary<string, object> _parameters;
        private string _orderBy;

        public TableQuery(SqliteConnection connection) {
            _connection = connection;
            _parameters = new Dictionary<string, object>();
        }

        public TableQuery<T> Where(Expression<Func<T, bool>> predicate) {
            var visitor = new SqliteExpressionVisitor();
            visitor.Visit(predicate);
            _whereClause = visitor.WhereClause;
            _parameters = visitor.Parameters;
            return this;
        }

        public TableQuery<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector) {
            var member = keySelector.Body as MemberExpression;
            if(member != null) {
                _orderBy = $"{member.Member.Name} ASC";
            }
            return this;
        }

        public TableQuery<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> keySelector) {
            var member = keySelector.Body as MemberExpression;
            if(member != null) {
                _orderBy = $"{member.Member.Name} DESC";
            }
            return this;
        }

        public async Task<List<T>> ToListAsync() {
            var tableName = typeof(T).Name;
            var sql = $"SELECT * FROM {tableName}";

            if(!string.IsNullOrEmpty(_whereClause)) {
                sql += $" WHERE {_whereClause}";
            }

            if(!string.IsNullOrEmpty(_orderBy)) {
                sql += $" ORDER BY {_orderBy}";
            }

            var items = new List<T>();

            using var command = _connection.CreateCommand();
            command.CommandText = sql;

            foreach(var param in _parameters) {
                command.Parameters.AddWithValue($"${param.Key}", param.Value);
            }

            using var reader = await command.ExecuteReaderAsync();
            while(await reader.ReadAsync()) {
                items.Add(MapToObject<T>(reader));
            }

            return items;
        }

        public async Task<T> FirstOrDefaultAsync() {
            try {
                if(_connection == null) {
                    throw new InvalidOperationException("Database connection is null in TableQuery");
                }

                var tableName = typeof(T).Name;
            var sql = $"SELECT * FROM {tableName}";

            if(!string.IsNullOrEmpty(_whereClause)) {
                sql += $" WHERE {_whereClause}";
            }

            if(!string.IsNullOrEmpty(_orderBy)) {
                sql += $" ORDER BY {_orderBy}";
            }

            sql += " LIMIT 1";

            using var command = _connection.CreateCommand();
            command.CommandText = sql;

            foreach(var param in _parameters) {
                command.Parameters.AddWithValue($"${param.Key}", param.Value);
            }

            using var reader = await command.ExecuteReaderAsync();
            if(await reader.ReadAsync()) {
                return MapToObject<T>(reader);
            }

            return default(T);
            }
            catch(Exception ex) {
                Console.WriteLine($"Error en FirstOrDefaultAsync: {ex.Message}");
                throw ex;
            }
        }

        private static T MapToObject<T>(SqliteDataReader reader) where T : class, new() {
            var obj = new T();
            var properties = typeof(T).GetProperties();

            foreach(var prop in properties) {
                try {
                    var ordinal = reader.GetOrdinal(prop.Name);
                    if(!reader.IsDBNull(ordinal)) {
                        var value = reader.GetValue(ordinal);
                        value = ConvertValue(value, prop.PropertyType);
                        prop.SetValue(obj, value);
                    } else {
                        // Manejar valores NULL para tipos nullable
                        if(prop.PropertyType == typeof(float?) ||
                            prop.PropertyType == typeof(double?) ||
                            prop.PropertyType == typeof(int?) ||
                            prop.PropertyType == typeof(bool?)) {
                            prop.SetValue(obj, null);
                        }
                    }
                } catch(Exception ex) {
                    Console.WriteLine($"Error mapeando propiedad {prop.Name}: {ex.Message}");
                }
            }

            return obj;
        }

        private static object ConvertValue(object value, Type targetType) {
            if(value == null) return null;

            try {
                // Manejar float?
                if(targetType == typeof(float?) && value is double doubleValue) {
                    return (float?)doubleValue;
                }
                if(targetType == typeof(float) && value is double doubleValue2) {
                    return (float)doubleValue2;
                }

                // Manejar otros tipos como antes...
                if(targetType == typeof(DateTime) && value is string dateString) {
                    return DateTime.Parse(dateString);
                } else if(targetType.IsEnum && value is int intValue) {
                    return Enum.ToObject(targetType, intValue);
                } else if(targetType == typeof(bool) && value is long longValue) {
                    return longValue != 0;
                } else if(value.GetType() != targetType) {
                    return Convert.ChangeType(value, targetType);
                }

                return value;
            } catch {
                return value;
            }
        }
    }

    // Visitor para convertir expresiones Lambda a SQL
    public class SqliteExpressionVisitor : ExpressionVisitor {
        public string WhereClause { get; private set; } = string.Empty;
        public Dictionary<string, object> Parameters { get; private set; } = new Dictionary<string, object>();
        private int _parameterCount = 0;

        protected override Expression VisitBinary(BinaryExpression node) {
            Visit(node.Left);
            WhereClause += $" {GetOperator(node.NodeType)} ";
            Visit(node.Right);
            return node;
        }

        protected override Expression VisitMember(MemberExpression node) {
            // Si es una propiedad del entity (ej: e.IdLocal)
            if(node.Expression != null && node.Expression.NodeType == ExpressionType.Parameter) {
                WhereClause += node.Member.Name;
            } else {
                // Si es una variable local (ej: IdLocal del método), obtener su valor
                var value = GetValue(node);
                var paramName = $"p{_parameterCount++}";
                WhereClause += $"${paramName}";
                Parameters[paramName] = value ?? DBNull.Value;
            }
            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node) {
            var paramName = $"p{_parameterCount++}";
            WhereClause += $"${paramName}";
            Parameters[paramName] = node.Value ?? DBNull.Value;
            return node;
        }

        // NUEVO MÉTODO: Para obtener el valor de variables locales
        private object GetValue(MemberExpression member) {
            try {
                // Para propiedades estáticas
                if(member.Member is System.Reflection.PropertyInfo propertyInfo) {
                    if(member.Expression == null) // Propiedad estática
                    {
                        return propertyInfo.GetValue(null);
                    } else // Propiedad de instancia
                      {
                        var objectMember = Expression.Convert(member, typeof(object));
                        var getterLambda = Expression.Lambda<Func<object>>(objectMember);
                        var getter = getterLambda.Compile();
                        return getter();
                    }
                }
                // Para campos
                else if(member.Member is System.Reflection.FieldInfo fieldInfo) {
                    if(member.Expression == null) // Campo estático
                    {
                        return fieldInfo.GetValue(null);
                    } else // Campo de instancia
                      {
                        var objectMember = Expression.Convert(member, typeof(object));
                        var getterLambda = Expression.Lambda<Func<object>>(objectMember);
                        var getter = getterLambda.Compile();
                        return getter();
                    }
                }
            } catch(Exception ex) {
                Console.WriteLine($"Error obteniendo valor de {member.Member.Name}: {ex.Message}");
            }

            return null;
        }

        private string GetOperator(ExpressionType nodeType) {
            return nodeType switch {
                ExpressionType.Equal => "=",
                ExpressionType.NotEqual => "!=",
                ExpressionType.GreaterThan => ">",
                ExpressionType.GreaterThanOrEqual => ">=",
                ExpressionType.LessThan => "<",
                ExpressionType.LessThanOrEqual => "<=",
                ExpressionType.AndAlso => "AND",
                ExpressionType.OrElse => "OR",
                _ => throw new NotSupportedException($"Operador {nodeType} no soportado")
            };
        }
    }

    //// Visitor para convertir expresiones Lambda a SQL
    //public class SqliteExpressionVisitor : ExpressionVisitor {
    //    public string WhereClause { get; private set; }
    //    public Dictionary<string, object> Parameters { get; private set; } = new Dictionary<string, object>();
    //    private int _parameterCount = 0;

    //    protected override Expression VisitBinary(BinaryExpression node) {
    //        Visit(node.Left);
    //        WhereClause += $" {GetOperator(node.NodeType)} ";
    //        Visit(node.Right);
    //        return node;
    //    }

    //    protected override Expression VisitMember(MemberExpression node) {
    //        WhereClause += node.Member.Name;
    //        return node;
    //    }

    //    protected override Expression VisitConstant(ConstantExpression node) {
    //        var paramName = $"p{_parameterCount++}";
    //        WhereClause += $"${paramName}";
    //        Parameters[paramName] = node.Value;
    //        return node;
    //    }

    //    private string GetOperator(ExpressionType nodeType) {
    //        return nodeType switch {
    //            ExpressionType.Equal => "=",
    //            ExpressionType.NotEqual => "!=",
    //            ExpressionType.GreaterThan => ">",
    //            ExpressionType.GreaterThanOrEqual => ">=",
    //            ExpressionType.LessThan => "<",
    //            ExpressionType.LessThanOrEqual => "<=",
    //            ExpressionType.AndAlso => "AND",
    //            ExpressionType.OrElse => "OR",
    //            _ => throw new NotSupportedException($"Operador {nodeType} no soportado")
    //        };
    //    }
    //}
}
