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
        /// Retrieves a page of log messages from the specified log source, ordered newest to oldest.
        /// </summary>
        /// <param name="logSourceName">Name of the configured log source to query.</param>
        /// <param name="offset">Zero-based offset of the first record to return.</param>
        /// <param name="count">Maximum number of log messages to return in this page.</param>
        /// <param name="levels">
        /// Optional filter — when non-null and non-empty, only messages whose <c>Level</c>
        /// matches one of the supplied values are returned. <c>null</c> or empty disables the filter.
        /// </param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>The page of matching log messages plus the total count across all pages.</returns>
        public async Task<LogMessagePage> GetPageAsync(
            string logSourceName,
            int offset,
            int count,
            IReadOnlyCollection<string>? levels,
            string? traceId,
            CancellationToken cancellationToken = default)
        {
            var result = new LogMessagePage();

            try
            {
                var (connectionString, tableName) = _sourceResolver.Resolve(logSourceName);
                using var connection = new SqlConnection(connectionString);

                var clauses = new List<string>();
                if (levels is { Count: > 0 })
                {
                    clauses.Add("Level IN @levels");
                }
                if (!string.IsNullOrEmpty(traceId))
                {
                    clauses.Add("TraceId = @traceId");
                }
                var whereClause = clauses.Count > 0
                    ? " WHERE " + string.Join(" AND ", clauses)
                    : string.Empty;

                // No COUNT(*) — on a large logs table the filtered count is the
                // dominant cost and we don't need it for virtualization. TotalCount
                // is derived below from the page size to drive an infinite-scroll
                // virtualization total.
                var sql =
                    $"SELECT {SelectColumns} FROM {tableName}{whereClause} " +
                    "ORDER BY TimeStamp DESC " +
                    "OFFSET @offset ROWS FETCH NEXT @count ROWS ONLY;";

                var command = new CommandDefinition(
                    sql,
                    new { offset, count, levels, traceId },
                    cancellationToken: cancellationToken);

                var rows = (await connection.QueryAsync<LogMessage>(command)).AsList();

                result.Items = rows;
                result.TotalCount = rows.Count < count
                    ? offset + rows.Count
                    : offset + rows.Count + count * 5;
            }
            catch (TaskCanceledException)
            {
                // Dapper raises TaskCanceledException when the supplied CancellationToken
                // fires (e.g. virtualized grid issued a newer fetch, client disconnected).
                // Propagate as-is so callers can distinguish cancellation from a fault.
                throw;
            }
            catch (SqlException ex)
            {
                // Using the extension method to throw a more specific managed exception based on the SQL error.
                ex.ThrowManagedException();

            }
            catch (Exception unhandledException)
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
