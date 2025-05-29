using Q.FilterBuilder.Core.Providers;

namespace Q.FilterBuilder.Linq;

/// <summary>
/// LINQ query format provider that provides LINQ-specific formatting for use with FilterBuilder.
/// </summary>
public class LinqFormatProvider : IQueryFormatProvider
{
    /// <inheritdoc />
    public string ParameterPrefix => "";

    /// <inheritdoc />
    public string AndOperator => "&&";

    /// <inheritdoc />
    public string OrOperator => "||";

    /// <inheritdoc />
    public string FormatFieldName(string fieldName) => fieldName;

    /// <inheritdoc />
    public string FormatParameterName(int parameterIndex) => $"@p{parameterIndex}";
}