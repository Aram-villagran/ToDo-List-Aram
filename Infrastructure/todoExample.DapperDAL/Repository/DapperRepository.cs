using System.Globalization;
using System.Reflection;
using Dapper;
using todoExample.DapperDAL.DbContext;
using todoExample.domain.Data;
using todoExample.domain.Models;
using todoExample.domain.Models.Identity;
using todoExample.domain.Respository.Base;

namespace todoExample.DapperDAL.Repository;

public class DapperRepository<T> : IDapperRepository<T> where T : BaseEntity
{
    private static readonly IReadOnlyDictionary<Type, string> TableNames = new Dictionary<Type, string>
    {
        [typeof(TodoListModel)] = "todolists",
        [typeof(TodoTaskModel)] = "todotasks",
        [typeof(TagModel)] = "tags",
        [typeof(TodoTaskTagModel)] = "todotasktags"
    };

    private static readonly HashSet<string> AllowedOperators = new(StringComparer.OrdinalIgnoreCase)
    {
        "=", "!=", "<>", ">", "<", ">=", "<=", "LIKE", "BETWEEN", "IS", "IS NOT"
    };

    private static readonly HashSet<string> AllowedOrderDirections = new(StringComparer.OrdinalIgnoreCase)
    {
        "ASC", "DESC"
    };

    private readonly IConnectionFactory _connectionFactory;
    private readonly string _tableName = ResolveTableName(typeof(T));

    public dynamic ConnectionFactory => _connectionFactory;

    public DapperRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        var sql = $"SELECT * FROM {_tableName} WHERE Id = @Id AND Enabled = 1;";

        using var connection = _connectionFactory.CreateConnection();
        var result = await connection.QueryAsync<T>(sql, new { Id = id });
        return result.FirstOrDefault();
    }

    public async Task<IEnumerable<T>> GetAllAsync(List<Where>? where = null, List<OrderBy>? orderBy = null, int limit = 20, int offset = 0)
    {
        var (whereSql, parameters) = BuildWhereClause(where);
        var orderBySql = BuildOrderByClause(orderBy);

        var sql = $"SELECT * FROM {_tableName}";
        sql += string.IsNullOrEmpty(whereSql)
            ? " WHERE Enabled = 1"
            : $" WHERE {whereSql} AND Enabled = 1";

        if (!string.IsNullOrEmpty(orderBySql))
        {
            sql += $" ORDER BY {orderBySql}";
        }

        limit = Math.Max(limit, 0);
        offset = Math.Max(offset, 0);

        if (limit != 0)
        {
            sql += offset != 0 ? $" LIMIT {offset}, {limit}" : $" LIMIT {limit}";
        }

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<T>(sql, parameters);
    }

    public async Task<T?> AddAsync(T entity)
    {
        entity.Id = entity.Id == Guid.Empty ? Guid.NewGuid() : entity.Id;
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;

        var properties = GetColumnProperties()
            .Select(property => new { Property = property, Value = property.GetValue(entity) })
            .Where(x => x.Value is not null)
            .ToList();

        if (properties.Count == 0)
        {
            throw new InvalidOperationException($"No hay campos para insertar en {typeof(T).Name}.");
        }

        var columns = string.Join(", ", properties.Select(x => x.Property.Name));
        var values = string.Join(", ", properties.Select(x => $"@{x.Property.Name}"));

        var sql = $"INSERT INTO {_tableName} ({columns}) VALUES ({values});";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, entity);
        return entity;
    }

    public async Task<T?> UpdateAsync(T entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;

        var properties = GetColumnProperties()
     .Where(property => property.Name is not "Id" and not "CreatedAt" and not "DeletedAt")
     .Select(property => new { Property = property, Value = property.GetValue(entity) })
     .ToList();

        if (properties.Count == 0)
        {
            throw new InvalidOperationException($"No hay campos para actualizar en {typeof(T).Name}.");
        }

        var setValues = string.Join(", ", properties.Select(x => $"{x.Property.Name} = @{x.Property.Name}"));

        var sql = $"UPDATE {_tableName} SET {setValues} WHERE Id = @Id;";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, entity);
        return entity;
    }

    public async Task DeleteAsync(T entity)
    {
        entity.Enabled = false;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.DeletedAt = entity.UpdatedAt;

        var sql = $"UPDATE {_tableName} SET Enabled = @Enabled, UpdatedAt = @UpdatedAt, DeletedAt = @DeletedAt WHERE Id = @Id;";

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(sql, entity);
    }

    public async Task<long> CountAsync(List<Where>? where = null)
    {
        var (whereSql, parameters) = BuildWhereClause(where);

        var sql = $"SELECT COUNT(*) FROM {_tableName}";
        sql += string.IsNullOrEmpty(whereSql)
            ? " WHERE Enabled = 1"
            : $" WHERE {whereSql} AND Enabled = 1";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<long>(sql, parameters);
    }

    #region Helpers

    private static string ResolveTableName(Type type)
    {
        if (TableNames.TryGetValue(type, out var tableName))
        {
            return tableName;
        }

        return type.Name.Replace("Model", string.Empty).ToLowerInvariant() + "s";
    }

    private static IEnumerable<PropertyInfo> GetColumnProperties()
    {
        return typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => property.CanRead && IsScalar(property.PropertyType));
    }

    private static PropertyInfo? GetColumnProperty(Type type, string field)
    {
        var property = type.GetProperty(field, BindingFlags.Public | BindingFlags.Instance);
        return property is not null && IsScalar(property.PropertyType) ? property : null;
    }

    private static bool IsScalar(Type type)
    {
        var underlying = Nullable.GetUnderlyingType(type) ?? type;

        return underlying.IsPrimitive
               || underlying.IsEnum
               || underlying == typeof(string)
               || underlying == typeof(Guid)
               || underlying == typeof(DateTime)
               || underlying == typeof(DateTimeOffset)
               || underlying == typeof(decimal);
    }

    private static (string Sql, DynamicParameters Parameters) BuildWhereClause(List<Where>? where)
    {
        var parameters = new DynamicParameters();

        if (where is null || where.Count == 0)
        {
            return (string.Empty, parameters);
        }

        var clauses = new List<string>();
        var parameterIndex = 0;

        for (var i = 0; i < where.Count; i++)
        {
            var filter = where[i];

            var property = GetColumnProperty(typeof(T), filter.Field)
                           ?? throw new ArgumentException(
                               $"El campo '{filter.Field}' no existe en {typeof(T).Name} o no es un campo escalar.",
                               nameof(where));

            var op = string.IsNullOrWhiteSpace(filter.Op)
                ? "="
                : filter.Op.Trim().ToUpperInvariant();

            if (!AllowedOperators.Contains(op))
            {
                throw new ArgumentException($"Operador no permitido: '{filter.Op}'.", nameof(where));
            }

            var prefix = i == 0 ? string.Empty : $"{ResolveAndOr(filter.AndOr)} ";
            var field = property.Name;
            string clause;

            if (op is "IS" or "IS NOT")
            {
                clause = $"{field} {op} NULL";
            }
            else if (op == "BETWEEN")
            {
                if (string.IsNullOrWhiteSpace(filter.Value2))
                {
                    throw new ArgumentException($"El filtro BETWEEN de '{field}' requiere Value2.", nameof(where));
                }

                parameters.Add($"w{parameterIndex}", ConvertValue(property.PropertyType, filter.Value));
                parameters.Add($"w{parameterIndex + 1}", ConvertValue(property.PropertyType, filter.Value2));

                clause = $"{field} BETWEEN @w{parameterIndex} AND @w{parameterIndex + 1}";
                parameterIndex += 2;
            }
            else
            {
                parameters.Add($"w{parameterIndex}", ConvertValue(property.PropertyType, filter.Value));
                clause = $"{field} {op} @w{parameterIndex}";
                parameterIndex++;
            }

            clauses.Add(prefix + clause);
        }

        return ($"({string.Join(" ", clauses)})", parameters);
    }

    private static string BuildOrderByClause(List<OrderBy>? orderBy)
    {
        if (orderBy is null || orderBy.Count == 0)
        {
            return string.Empty;
        }

        var fields = new List<string>();

        foreach (var element in orderBy)
        {
            var property = GetColumnProperty(typeof(T), element.Field)
                           ?? throw new ArgumentException(
                               $"El campo '{element.Field}' no existe en {typeof(T).Name} o no es un campo escalar.",
                               nameof(orderBy));

            var direction = string.IsNullOrWhiteSpace(element.Order)
                ? "ASC"
                : element.Order.Trim().ToUpperInvariant();

            if (!AllowedOrderDirections.Contains(direction))
            {
                throw new ArgumentException($"Dirección de orden no permitida: '{element.Order}'.", nameof(orderBy));
            }

            fields.Add($"{property.Name} {direction}");
        }

        return string.Join(", ", fields);
    }

    private static string ResolveAndOr(string? andOr)
    {
        if (string.IsNullOrWhiteSpace(andOr))
        {
            return "AND";
        }

        var normalized = andOr.Trim().ToUpperInvariant();

        return normalized is "AND" or "OR"
            ? normalized
            : throw new ArgumentException($"Operador lógico no permitido: '{andOr}'.");
    }

    private static object? ConvertValue(Type propertyType, string? value)
    {
        if (value is null || string.Equals(value, "NULL", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var underlying = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

        if (underlying == typeof(string))
        {
            return value;
        }

        if (underlying == typeof(Guid))
        {
            return Guid.Parse(value);
        }

        if (underlying == typeof(bool))
        {
            return bool.Parse(value);
        }

        if (underlying == typeof(DateTime))
        {
            return DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        }

        if (underlying == typeof(DateTimeOffset))
        {
            return DateTimeOffset.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        }

        if (underlying.IsEnum)
        {
            return Enum.Parse(underlying, value, ignoreCase: true);
        }

        return Convert.ChangeType(value, underlying, CultureInfo.InvariantCulture);
    }

    #endregion
}
