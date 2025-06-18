using System.Text.RegularExpressions;
using Q.FilterBuilder.Core;
using Q.FilterBuilder.Core.Models;

namespace Q.FilterBuilder.PostgreSql.Extensions
{
    /// <summary>
    /// PostgreSQL-specific extension methods for Entity Framework integration.
    /// These extensions provide optimized parameter handling for PostgreSQL with Entity Framework.
    /// </summary>
    public static class PostgreSqlEfExtensions
    {
        /// <summary>
        /// Builds a query result specifically formatted for Entity Framework and Entity Framework Core.
        /// </summary>
        /// <param name="filterBuilder">The filter builder instance.</param>
        /// <param name="group">The FilterGroup to build the query from.</param>
        /// <returns>A tuple with the query and parameters</returns>
        /// <example>
        /// <code>
        /// var (query, parameters) = filterBuilder.BuildForEf(filterGroup);
        /// var users = context.Users.FromSqlRaw($"SELECT * FROM Users WHERE {query}", parameters);
        /// </code>
        /// </example>
        public static (string query, object[] parameters) BuildForEf(this IFilterBuilder filterBuilder, FilterGroup group)
        {
            var (query, parameters) = filterBuilder.Build(group);
            // PostgreSQL: $1, $2, etc. -> {0}, {1}, etc.
            var formattedQuery = Regex.Replace(query, @"\$(\d+)", match =>
                {
                    var paramIndex = int.Parse(match.Groups[1].Value) - 1; // Convert 1-based to 0-based
                    return $"{{{paramIndex}}}";
                });
            return (formattedQuery, parameters);
        }
    }
}
