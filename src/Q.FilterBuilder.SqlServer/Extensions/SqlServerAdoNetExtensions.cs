using System;
using System.Data.Common;

namespace Q.FilterBuilder.SqlServer.Extensions
{
    /// <summary>
    /// SQL Server-specific ADO.NET extension methods for DbCommand.
    /// These extensions provide parameter handling for SQL Server with ADO.NET.
    /// </summary>
    public static class SqlServerAdoNetExtensions
    {
        /// <summary>
        /// Adds parameters to a DbCommand for SQL Server.
        /// </summary>
        /// <param name="command">The DbCommand to add parameters to</param>
        /// <param name="parameters">The parameter values</param>
        public static void AddParameters(this DbCommand command, object[] parameters)
        {
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = $"@p{i}";
                parameter.Value = parameters[i] ?? DBNull.Value;
                command.Parameters.Add(parameter);
            }
        }
    }
}
