using Q.FilterBuilder.Core.Models;
using Q.FilterBuilder.Core.Providers;

namespace Q.FilterBuilder.Core;

/// <summary>
/// Defines the interface for filter builders that generate database queries.
/// This is the main interface that should be injected into controllers and services.
/// </summary>
public interface IFilterBuilder
{
    /// <summary>
    /// Builds a WHERE clause based on the provided FilterGroup.
    /// </summary>
    /// <param name="group">The FilterGroup to build the WHERE clause from.</param>
    /// <returns>A tuple containing the parsed query string and an array of parameters.</returns>
    (string parsedQuery, object[] parameters) Build(FilterGroup group);

    /// <summary>
    /// Gets the query format provider used by this filter builder.
    /// This allows extension methods to access provider-specific formatting capabilities.
    /// </summary>
    IQueryFormatProvider QueryFormatProvider { get; }
}