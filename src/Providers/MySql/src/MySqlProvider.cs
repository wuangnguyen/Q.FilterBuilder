using Q.FilterBuilder.Core.Providers;

namespace Q.FilterBuilder.MySql;

/// <summary>
/// MySQL query syntax provider implementation.
/// </summary>
public class MySqlProvider : IQuerySyntaxProvider
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
        return $"`{fieldName}`";
    }

    /// <inheritdoc />
    public string FormatParameterName(int parameterIndex)
    {
        // MySQL uses positional parameters
        return ParameterPrefix;
    }
}
