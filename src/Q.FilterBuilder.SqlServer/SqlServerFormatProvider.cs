using Q.FilterBuilder.Core.Providers;

namespace Q.FilterBuilder.SqlServer;

/// <summary>
/// SQL Server query syntax provider implementation.
/// </summary>
public class SqlServerFormatProvider : IQueryFormatProvider
{
    /// <inheritdoc />
    public string ParameterPrefix => "@";

    /// <inheritdoc />
    public string AndOperator => "AND";

    /// <inheritdoc />
    public string OrOperator => "OR";

    /// <inheritdoc />
    public string FormatFieldName(string fieldName)
    {
        return $"[{fieldName}]";
    }

    /// <inheritdoc />
    public string FormatParameterName(int parameterIndex)
    {
        return $"{ParameterPrefix}p{parameterIndex}";
    }
}
