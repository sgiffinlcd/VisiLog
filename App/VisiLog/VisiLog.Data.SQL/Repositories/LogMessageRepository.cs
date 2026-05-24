using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CodeFactory.NDF.SQL;
using CodeFactory.NDF;
using Dapper;
using Microsoft.Data.SqlClient;
using VisiLog.Data.Contracts.Repositories;
using VisiLog.Data.SQL.Sources;
using VisiLog.Model.App;

namespace VisiLog.Data.SQL.Repositories
{
    internal sealed class LogMessageRepository : ILogMessageRepository
    {
        private const string SelectColumns =
            "Id, Message, Level, TimeStamp, Exception, Environment, Machine, Thread, Logger, MemberName, LineNumber, TraceId";

        private readonly ILogSourceResolver _sourceResolver;

        public LogMessageRepository(ILogSourceResolver sourceResolver)
        {
            _sourceResolver = sourceResolver;
        }

        /// <summary>
        /// Retrieves the most recent log messages from the specified log source.
        /// </summary>
        /// <param name="logSourceName">Name of the configured log source to query.</param>
        /// <param name="count">The maximum number of log messages to retrieve.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A read-only list of the most recent log messages ordered by timestamp in descending order.</returns>
        public async Task<IReadOnlyList<LogMessage>> GetRecentAsync(string logSourceName, int count, CancellationToken cancellationToken = default)
        {

            var result = new List<LogMessage>();

            try
            {
                var (connectionString, tableName) = _sourceResolver.Resolve(logSourceName);
                using var connection = new SqlConnection(connectionString);
                var command = new CommandDefinition(
                    $"SELECT TOP (@count) {SelectColumns} FROM {tableName} ORDER BY TimeStamp DESC",
                    new { count },
                    cancellationToken: cancellationToken);
                var rows = await connection.QueryAsync<LogMessage>(command);
                result =  rows.AsList();
            }
            catch (SqlException ex)
            {
                // Using the extension method to throw a more specific managed exception based on the SQL error.
                ex.ThrowManagedException();

            }
            catch (Exception)
            {
                // This catch is to handle any unexpected exceptions that may occur during the database operation.
                throw new UnhandledException();
            }

            return result;
        }

        /// <summary>
        /// Retrieves a log message by its unique identifier from the specified log source.
        /// </summary>
        /// <param name="logSourceName">Name of the configured log source to query.</param>
        /// <param name="id">The unique identifier of the log message.</param>
        /// <param name="cancellationToken">The cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the log message if found;
        /// otherwise, null.</returns>
        public async Task<LogMessage?> GetByIdAsync(string logSourceName, int id, CancellationToken cancellationToken = default)
        {
            LogMessage? result = null;

            try
            {
                var (connectionString, tableName) = _sourceResolver.Resolve(logSourceName);
                using var connection = new SqlConnection(connectionString);
                var command = new CommandDefinition(
                    $"SELECT {SelectColumns} FROM {tableName} WHERE Id = @id",
                    new { id },
                    cancellationToken: cancellationToken);
                result = await connection.QueryFirstOrDefaultAsync<LogMessage>(command);
            }
            catch (SqlException ex)
            {
                // Using the extension method to throw a more specific managed exception based on the SQL error.
                ex.ThrowManagedException();
                
            }
            catch (Exception)
            {
                // This catch is to handle any unexpected exceptions that may occur during the database operation.
                throw new UnhandledException();
            }

            return result;
        }

        
    }
}
