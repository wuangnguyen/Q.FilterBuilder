using System.Text.RegularExpressions;
using Q.FilterBuilder.Core;
using Q.FilterBuilder.Core.Models;

namespace Q.FilterBuilder.MySql.Extensions
{
    /// <summary>
    /// MySQL-specific extension methods for Entity Framework integration.
    /// These extensions provide optimized parameter handling for MySQL with Entity Framework.
    /// </summary>
    public static class MySqlEfExtensions
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
            // MySQL: ? -> {0}, {1}, etc. (handled by counting occurrences)
            var paramIndex = 0;
            var formattedQuery = Regex.Replace(query, @"\?", _ => $"{{{paramIndex++}}}");
            return (formattedQuery, parameters);
        }
    }
}
