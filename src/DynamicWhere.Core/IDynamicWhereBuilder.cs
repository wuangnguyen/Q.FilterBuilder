using DynamicWhere.Core.Models;

namespace DynamicWhere.Core;

/// <summary>
/// Defines the interface for dynamic WHERE clause builders.
/// </summary>
public interface IDynamicWhereBuilder
{
    /// <summary>
    /// Builds a WHERE clause based on the provided DynamicGroup.
    /// </summary>
    /// <param name="group">The DynamicGroup to build the WHERE clause from.</param>
    /// <returns>A tuple containing the parsed query string and an array of parameters.</returns>
    (string parsedQuery, object[] parameters) Build(DynamicGroup group);
}
