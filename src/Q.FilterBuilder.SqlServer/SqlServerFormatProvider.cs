using Q.FilterBuilder.Core.Providers;
using System.Linq;

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
        if (string.IsNullOrEmpty(fieldName))
            throw new System.ArgumentException("Field name cannot be null or empty.", nameof(fieldName));
        
        var segments = fieldName.Split('.');
        return string.Join(".", segments.Select(s => $"[{s}]").ToArray());
    }

    /// <inheritdoc />
    public string FormatParameterName(int parameterIndex)
    {
        return $"{ParameterPrefix}p{parameterIndex}";
    }
}
