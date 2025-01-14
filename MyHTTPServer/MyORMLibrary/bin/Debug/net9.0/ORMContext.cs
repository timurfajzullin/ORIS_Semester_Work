using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Data.SqlClient;
using MyHttttpServer.Models;

namespace MyORMLibrary;
public class ORMContext<T> where T : class, new()
{
    private readonly IDbConnection _dbconnection;

    public ORMContext(IDbConnection dbconnection)
    {
        _dbconnection = dbconnection;
    }

    public T Create(T entity)
    {
        var properties = typeof(T).GetProperties();

        // Генерация списка столбцов и параметров
        var columns = string.Join(", ", properties.Select(p => p.Name));
        var values = string.Join(", ", properties.Select(p => $"@{p.Name}"));

        // Формирование SQL-запроса
        var query = $"INSERT INTO {typeof(T).Name}s ({columns}) VALUES ({values});";

        using (var connection = _dbconnection)
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = query;

                // Добавление параметров
                foreach (var property in properties)
                {
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = $"@{property.Name}";
                    parameter.Value = property.GetValue(entity) ?? DBNull.Value;
                    command.Parameters.Add(parameter);
                }

                command.ExecuteNonQuery();
            }
            connection.Close();
        }

        return entity;
    }
    
    public T ReadById(int id)
    {
        using (var connection = _dbconnection)
        {
            connection.Open();
            string tableName = typeof(T).Name;
            string queryRequest = $"SELECT * FROM {tableName} WHERE id = @id";

            using (var command = connection.CreateCommand())
            {
                command.CommandText = queryRequest;

                var parametr = command.CreateParameter();
                parametr.ParameterName = "@id";
                parametr.Value = id;
                command.Parameters.Add(parametr);


                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return Map(reader);
                    }
                }
            }
        }

        return null;
    }

    private T Map(IDataReader reader)
    {
        var entity = new T();
        var properties = typeof(T).GetProperties();

        foreach(var property in properties)
        {
            if (!reader.IsDBNull(reader.GetOrdinal(property.Name))) 
            {
                property.SetValue(entity, reader[property.Name]);
            }
        }
        return entity;
    }


    public T CheckUserByData(string email)
    {
        var tableName = typeof(T).Name + "s"; // Обычно имя таблицы во множественном числе
        using (var connection = _dbconnection)
        {
            connection.Open();
            string queryRequest = $"SELECT * FROM {tableName} WHERE email = @email;";
            using (var command = connection.CreateCommand())
            {
                command.CommandText = queryRequest;

                // Добавление параметра для email
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@email";
                parameter.Value = email;
                command.Parameters.Add(parameter);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return Map(reader); // Если запись найдена, возвращаем объект
                    }
                }
            }
        }
        return null; // Если пользователь не найден, возвращаем null
    }


    public string GetNameByUrl(string url)
    {
        using (var connection = _dbconnection)
        {
            connection.Open();
            string queryRequest = $"SELECT Name FROM MoviePageInformation where url = '{url}';";
            using (var command = connection.CreateCommand())
            {
                using (var reader = command.ExecuteReader())
                {
                    return reader["Name"].ToString();
                }
            }
        }
    }

    public T ReadByAll<T>(string name) where T : class, new()
    {
        var tableName = typeof(T).Name;
        using (var connection = _dbconnection)
        {
            connection.Open();
            string queryRequest = $"SELECT * FROM MoviePageInformation WHERE Name = N'{name}'";
            using (var command = connection.CreateCommand())
            {
                command.CommandText = queryRequest;
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return MapToEntity<T>(reader);
                    }
                }
            }
        }

        return null;
    }

    public T ReadByName<T>(string Name) where T : class, new()
    {
        var tableName = typeof(T).Name;
        using (var connection = _dbconnection)
        {
            connection.Open();
            string queryRequest = $"SELECT * FROM {tableName} WHERE name = @Name ";
            using (var command = connection.CreateCommand())
            {
                command.CommandText = queryRequest;
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@name";
                parameter.Value = Name;

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return MapToEntity<T>(reader);
                    }
                }
            }
        }

        return null;
    }

    public string TakeFieldByData(string query)
    {
        var result = "";
        using (var command = _dbconnection.CreateCommand())
        {
            command.CommandText = query;
            _dbconnection.Open();

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    result = reader[0].ToString();
                }
            }
        }
        
        _dbconnection.Close();
        return result;
    }
    
    
    public List<string> TakeURLByData(string query)
    {
        var results = new List<string>(); // Изменяем тип списка на string
        using (var command = _dbconnection.CreateCommand())
        {
            command.CommandText = query;
            _dbconnection.Open();
        
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    results.Add(reader[0].ToString()); // Считываем первое значение из строки и добавляем в список
                }
            }

            _dbconnection.Close();
        }
        return results; // Возвращаем список строк
    }
    
    public List<Movie> GetMovies(string query)
    {
        List<Movie> movies = new List<Movie>();
        using (var command = _dbconnection.CreateCommand())
        {
            command.CommandText = query;
            _dbconnection.Open();
            using (var reader = command.ExecuteReader())
            {
               
                while (reader.Read())
                {
                    movies.Add(new Movie
                    {
                        PosterURl = reader["PosterURL"].ToString(),
                        Name = reader["Name"].ToString(),
                        Genre = reader["Genre"].ToString(),
                        Country = reader["Country"].ToString(),
                        Privacy = reader["Privacy"].ToString(),
                        Year = reader["Year"].ToString()
                    });
                }
            }
            _dbconnection.Close();
        }
        

        return movies;
    }


    public void AddMovieInfo(string query)
    {
        using (var command = _dbconnection.CreateCommand())
        {
            // Параметры для предотвращения SQL-инъекций
            command.CommandText = query;
            _dbconnection.Open();
            command.ExecuteNonQuery();
            _dbconnection.Close();
        }
    }
    public Movie AddMovie(string query)
    {
        using (var command = _dbconnection.CreateCommand())
        {
            // Параметры для предотвращения SQL-инъекций
            command.CommandText = query;
            _dbconnection.Open();
            command.ExecuteNonQuery();
            var values = ParseInsertQuery(query);
            _dbconnection.Close();
            return new Movie
            {
                Name = values["name"],
                PosterURl = values["posterurl"],
                Country = values["country"],
                Year = values["year"],
                Genre = values["genre"],
                Privacy = values["privacy"]
            };
        }
    }

    public List<Movie> DeleteMovie(string query)
    {
        List<Movie> deletedMovies = new List<Movie>();
    
        // Формируем SELECT запрос, чтобы получить удаляемые записи
        string selectQuery = query.Replace("DELETE", "SELECT *");

        using (var command = _dbconnection.CreateCommand())
        {
            // Выполняем SELECT для получения удаляемых данных
            command.CommandText = selectQuery;
            _dbconnection.Open();

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    // Предполагается, что у Movie есть свойства Title, Year и т. д.
                    deletedMovies.Add(new Movie
                    {
                        Name = reader["Name"].ToString(),
                        PosterURl = reader["PosterURL"].ToString(),
                        Country = reader["Country"].ToString(),
                        Year = reader["Year"].ToString(),
                        Genre = reader["Genre"].ToString(),
                        Privacy = reader["Privacy"].ToString(),
                        // Заполняем остальные свойства объекта Movie
                    });
                }
            }

            // Выполняем DELETE запрос
            command.CommandText = query;
            command.ExecuteNonQuery();

            _dbconnection.Close();
        }

        return deletedMovies;
    }
    
    public void AddComment(string query)
    {
        using (var command = _dbconnection.CreateCommand())
        {
            // Параметры для предотвращения SQL-инъекций
            command.CommandText = query;
            _dbconnection.Open();
            command.ExecuteNonQuery();
            _dbconnection.Close();
        }
    }


    public void Update(int id, T entity)
    {
        var properties = typeof(T).GetProperties()
            .Where(p => p.Name != "Id")
            .ToList();

        var setClause = string.Join(", ", properties.Select(p => $"{p.Name} = @{p.Name}"));
        var query = $"UPDATE {typeof(T).Name}s SET {setClause} WHERE Id = @id";

        using (var command = _dbconnection.CreateCommand())
        {
            command.CommandText = query;

            foreach (var property in properties)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@" + property.Name;
                parameter.Value = property.GetValue(entity) ?? DBNull.Value;
                command.Parameters.Add(parameter);
            }

            var idParameter = command.CreateParameter();
            idParameter.ParameterName = "@id";
            idParameter.Value = id;
            command.Parameters.Add(idParameter);

            _dbconnection.Open();
            command.ExecuteNonQuery();
            _dbconnection.Close();
        }
    }

    private Dictionary<string, string> ParseInsertQuery(string query)
    {
        var values = new Dictionary<string, string>();

        // Извлекаем часть после `VALUES`
        var startIndex = query.IndexOf("VALUES", StringComparison.OrdinalIgnoreCase);
        if (startIndex == -1)
            throw new ArgumentException("Некорректный SQL-запрос: не найдена часть VALUES.");

        var valuesPart = query.Substring(startIndex + "VALUES".Length).Trim().Trim('(', ')');
        var parts = valuesPart.Split(',');

        // Соответствие с порядком столбцов в таблице
        var keys = new[] { "name", "posterurl", "country", "year", "genre", "privacy" };
        for (int i = 0; i < keys.Length && i < parts.Length; i++)
        {
            // Убираем лишние пробелы и кавычки
            values[keys[i]] = parts[i].Trim().Trim('\'', 'N');
        }

        return values;
    }


    public void Delete(int id)
    {
        var query = $"DELETE FROM {typeof(T).Name}s WHERE Id = @id";
        using (var command = _dbconnection.CreateCommand())
        {
            command.CommandText = query;
            var parametr = command.CreateParameter();
            parametr.ParameterName = "@id";
            parametr.Value = id;
            command.Parameters.Add(parametr);

            _dbconnection.Open();
            command.ExecuteNonQuery();
            _dbconnection.Close();
        }
    }

    private T MapToEntity<T>(IDataReader reader) where T : class, new()
    {
        T entity = new T();
        Type entityType = typeof(T);
        PropertyInfo[] properties = entityType.GetProperties();

        //Получаем схему таблицы для проверки существования столбцов
        DataTable schemaTable = reader.GetSchemaTable();

        foreach (PropertyInfo property in properties)
        {
            string columnName = property.Name;
            //Проверяем наличие столбца в схеме
            DataRow[] rows = schemaTable.Select($"ColumnName = '" + columnName + "'");
            if (rows.Length > 0)
            {
                try
                {
                    int ordinal = reader.GetOrdinal(columnName);
                    object value = reader.GetValue(ordinal);
                    if (value != DBNull.Value)
                    {
                        property.SetValue(entity, Convert.ChangeType(value, property.PropertyType));
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    //Обработка ситуации, когда столбец неожиданно пропал
                    Console.WriteLine($"Column '{columnName}' not found in result set.");
                }
            }
        }
        return entity;
    }
    
    


    public T FirstOrDefault(Expression<Func<T, bool>> predicate)
    {
        var sqlQuery = BuildSqlQuery(predicate, singleResult: true);
        return ExecuteQuerySingle(sqlQuery);
    }

    public IEnumerable<T> Where(Expression<Func<T, bool>> predicate)
    {
        var sqlQuery = BuildSqlQuery(predicate, singleResult: false);
        return ExecuteQueryMultiple(sqlQuery);
    }

    private T ExecuteQuerySingle(string query)
    {
        using (var command = _dbconnection.CreateCommand())
        {
            command.CommandText = query;
            _dbconnection.Open();
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return Map(reader);
                }
            }
            _dbconnection.Close();
        }
        return null;
    }

    private IEnumerable<T> ExecuteQueryMultiple(string query)
    {
        var results = new List<T>();
        using (var command = _dbconnection.CreateCommand())
        {
            command.CommandText = query;
            _dbconnection.Open();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    results.Add(Map(reader));
                }
            }
            _dbconnection.Close();
        }
        return results;
    }

    private Tuple<string, Dictionary<string, object>> BuildQuery<T>(Expression<Func<T, bool>> predicate)
   {
        var tableName = typeof(T).Name + "s";
        var parameters = new Dictionary<string, object>();
        var whereClause = BuildWhereClause(predicate.Body, parameters);

        var query = $"SELECT TOP 1 * FROM {tableName} {whereClause}";
        return new Tuple<string, Dictionary<string, object>>(query, parameters);
   }

   private string BuildWhereClause(Expression expression, Dictionary<string, object> parameters)
   {
        if (expression is BinaryExpression binaryExpression)
        {
            var left = BuildWhereClause(binaryExpression.Left, parameters);
            var right = BuildWhereClause(binaryExpression.Right, parameters);
            var operatorString = binaryExpression.NodeType switch
            {
                ExpressionType.Equal => "=",
                ExpressionType.NotEqual => "<>",
                ExpressionType.GreaterThan => ">",
                ExpressionType.GreaterThanOrEqual => ">=",
                ExpressionType.LessThan => "<",
                ExpressionType.LessThanOrEqual => "<=",
                ExpressionType.AndAlso => "AND",
                ExpressionType.OrElse => "OR",
                _ => throw new NotSupportedException($"Operator {binaryExpression.NodeType} is not supported")
            };
            return $"{left} {operatorString} {right}";
        }
        else if (expression is MemberExpression memberExpression)
        {
            return memberExpression.Member.Name;
        }
        else if (expression is ConstantExpression constantExpression)
        {
            var parameterName = $"@p{parameters.Count}";
            parameters.Add(parameterName, constantExpression.Value);
            return parameterName;
        }
        else
        {
            throw new NotSupportedException($"Expression type {expression.GetType().Name} is not supported");
        }
   }

    private string ParseExpression(Expression expression)
    {
        if (expression is BinaryExpression binary)
        {
            // разбираем выражение на составляющие
            var left = ParseExpression(binary.Left);  // Левая часть выражения
            var right = ParseExpression(binary.Right); // Правая часть выражения
            var op = GetSqlOperator(binary.NodeType);  // Оператор (например, > или =)
            return $"({left} {op} {right})";
        }
        else if (expression is MemberExpression member)
        {
            return member.Member.Name; // Название свойства
        }
        else if (expression is ConstantExpression constant)
        {
            return FormatConstant(constant.Value); // Значение константы
        }
        throw new NotSupportedException($"Unsupported expression type: {expression.GetType().Name}");
    }

    private string GetSqlOperator(ExpressionType nodeType)
    {
        return nodeType switch
        {
            ExpressionType.Equal => "=",
            ExpressionType.AndAlso => "AND",
            ExpressionType.NotEqual => "<>",
            ExpressionType.GreaterThan => ">",
            ExpressionType.LessThan => "<",
            ExpressionType.GreaterThanOrEqual => ">=",
            ExpressionType.LessThanOrEqual => "<=",
            _ => throw new NotSupportedException($"Unsupported node type: {nodeType}")
        };
    }

    private string FormatConstant(object value)
    {
        return value is string ? $"'{value}'" : value.ToString();
    }

    private string BuildSqlQuery(Expression<Func<T, bool>> predicate, bool singleResult)
    {
        var tableName = typeof(T).Name + "s"; // Имя таблицы, основанное на имени класса
        var whereClause = ParseExpression(predicate.Body);
        var limitClause = singleResult ? "LIMIT 1" : string.Empty;

        return $"SELECT * FROM {tableName} WHERE {whereClause}".Trim();
    }
}