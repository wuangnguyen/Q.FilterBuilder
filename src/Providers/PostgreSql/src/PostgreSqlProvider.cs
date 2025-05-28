using Q.FilterBuilder.Core.Providers;

namespace Q.FilterBuilder.PostgreSql;

/// <summary>
/// PostgreSQL query syntax provider implementation.
/// </summary>
public class PostgreSqlProvider : IQuerySyntaxProvider
{
    /// <inheritdoc />
    public string ParameterPrefix => "$";

    /// <inheritdoc />
    public string AndOperator => "AND";

    /// <inheritdoc />
    public string OrOperator => "OR";

    /// <inheritdoc />
    public string FormatFieldName(string fieldName)
    {
        return $"\"{fieldName}\"";
    }

    /// <inheritdoc />
    public string FormatParameterName(int parameterIndex)
    {
        return $"{ParameterPrefix}{parameterIndex + 1}"; // PostgreSQL uses $1, $2, etc.
    }
}
