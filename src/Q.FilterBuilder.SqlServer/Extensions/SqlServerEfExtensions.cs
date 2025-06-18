using System.Text.RegularExpressions;
using Q.FilterBuilder.Core;
using Q.FilterBuilder.Core.Models;

namespace Q.FilterBuilder.SqlServer.Extensions
{
    /// <summary>
    /// SQL Server-specific extension methods for Entity Framework integration.
    /// These extensions provide optimized parameter handling for SQL Server with Entity Framework.
    /// </summary>
    public static class SqlServerEfExtensions
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
            // SQL Server: @p0, @p1, etc. -> {0}, {1}, etc.
            var parameterPattern = Regex.Escape("@") + @"p(\d+)";
            var formattedQuery = Regex.Replace(query, parameterPattern, "{$1}");
            return (formattedQuery, parameters);
        }
    }
}
