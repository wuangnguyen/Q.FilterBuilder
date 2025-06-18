using System;
using System.Data.Common;

namespace Q.FilterBuilder.PostgreSql.Extensions
{
    /// <summary>
    /// PostgreSQL-specific ADO.NET extension methods for DbCommand.
    /// These extensions provide parameter handling for PostgreSQL with ADO.NET.
    /// </summary>
    public static class PostgreSqlAdoNetExtensions
    {
        /// <summary>
        /// Adds parameters to a DbCommand for PostgreSQL.
        /// </summary>
        /// <param name="command">The DbCommand to add parameters to</param>
        /// <param name="parameters">The parameter values</param>
        public static void AddParameters(this DbCommand command, object[] parameters)
        {
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = command.CreateParameter();
                parameter.Value = parameters[i] ?? DBNull.Value;
                command.Parameters.Add(parameter);
            }
        }
    }
}
