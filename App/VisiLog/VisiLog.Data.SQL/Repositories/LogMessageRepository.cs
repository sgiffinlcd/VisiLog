using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CodeFactory.NDF.SQL;
using CodeFactory.NDF;
using Dapper;
using Microsoft.Data.SqlClient;
using VisiLog.Data.Contracts.Repositories;
using VisiLog.Data.SQL.Connections;
using VisiLog.Model.App;

namespace VisiLog.Data.SQL.Repositories
{
    internal sealed class LogMessageRepository : ILogMessageRepository
    {
        private const string SelectColumns =
            "Id, Message, Level, TimeStamp, Exception, Environment, Machine, Thread, Logger, MemberName, LineNumber, TraceId";

        private readonly IDbConnectionFactory _connectionFactory;

        public LogMessageRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        /// <summary>
        /// Retrieves the most recent log messages from the database.
        /// </summary>
        /// <param name="count">The maximum number of log messages to retrieve.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A read-only list of the most recent log messages ordered by timestamp in descending order.</returns>
        public async Task<IReadOnlyList<LogMessage>> GetRecentAsync(int count, CancellationToken cancellationToken = default)
        {

            var result = new List<LogMessage>();

            try
            {
                using var connection = _connectionFactory.Create();
                var command = new CommandDefinition(
                    $"SELECT TOP (@count) {SelectColumns} FROM dbo.LogMessage ORDER BY TimeStamp DESC",
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
        /// Retrieves a log message by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the log message.</param>
        /// <param name="cancellationToken">The cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the log message if found;
        /// otherwise, null.</returns>
        public async Task<LogMessage?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            LogMessage? result = null;

            try
            {
                using var connection = _connectionFactory.Create();
                var command = new CommandDefinition(
                    $"SELECT {SelectColumns} FROM dbo.LogMessage WHERE Id = @id",
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
