using System.Linq;
using Q.FilterBuilder.Core.Providers;

namespace Q.FilterBuilder.MySql;

/// <summary>
/// MySQL query syntax provider implementation.
/// </summary>
public class MySqlFormatProvider : IQueryFormatProvider
{
    /// <inheritdoc />
    public string ParameterPrefix => "?";

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
        return string.Join(".", segments.Select(s => $"`{s}`"));
    }

    /// <inheritdoc />
    public string FormatParameterName(int parameterIndex)
    {
        // MySQL uses positional parameters
        return ParameterPrefix;
    }
}
