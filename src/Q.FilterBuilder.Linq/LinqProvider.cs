using Q.FilterBuilder.Core.Providers;

namespace Q.FilterBuilder.Linq;

/// <summary>
/// LINQ query syntax provider that provides LINQ-specific formatting for use with FilterBuilder.
/// </summary>
public class LinqDatabaseProvider : IQuerySyntaxProvider
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