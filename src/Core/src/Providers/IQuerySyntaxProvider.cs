namespace Q.FilterBuilder.Core.Providers;

/// <summary>
/// Defines the interface for database-specific query syntax providers.
/// Each database (SqlServer, MySQL, Postgres, etc.) implements this interface to provide
/// database-specific formatting rules for parameters, field names, and logical operators.
/// </summary>
public interface IQuerySyntaxProvider
{
    /// <summary>
    /// Gets the parameter prefix for this database (e.g., "@" for SQL Server, "?" for MySQL).
    /// </summary>
    string ParameterPrefix { get; }

    /// <summary>
    /// Gets the logical operator representation for AND conditions.
    /// </summary>
    string AndOperator { get; }

    /// <summary>
    /// Gets the logical operator representation for OR conditions.
    /// </summary>
    string OrOperator { get; }

    /// <summary>
    /// Formats a field name according to database conventions (e.g., [FieldName] for SQL Server).
    /// </summary>
    /// <param name="fieldName">The field name to format.</param>
    /// <returns>The formatted field name.</returns>
    string FormatFieldName(string fieldName);

    /// <summary>
    /// Formats a parameter name according to database conventions.
    /// </summary>
    /// <param name="parameterIndex">The parameter index.</param>
    /// <returns>The formatted parameter name.</returns>
    string FormatParameterName(int parameterIndex);
}
