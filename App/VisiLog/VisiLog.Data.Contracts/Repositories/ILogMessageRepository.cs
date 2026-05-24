using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VisiLog.Model.App;

namespace VisiLog.Data.Contracts.Repositories
{
    /// <summary>
    /// Persistence contract for <see cref="LogMessage"/> records.
    /// </summary>
    public interface ILogMessageRepository
    {
        /// <summary>
        /// Retrieves the most recent log messages from the specified log source.
        /// </summary>
        /// <param name="logSourceName">Name of the configured log source to query.</param>
        /// <param name="count">The maximum number of log messages to retrieve.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A read-only list of the most recent log messages ordered by timestamp in descending order.</returns>
        Task<IReadOnlyList<LogMessage>> GetRecentAsync(string logSourceName, int count, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a log message by its unique identifier from the specified log source.
        /// </summary>
        /// <param name="logSourceName">Name of the configured log source to query.</param>
        /// <param name="id">The unique identifier of the log message.</param>
        /// <param name="cancellationToken">The cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the log message if found;
        /// otherwise, null.</returns>
        Task<LogMessage?> GetByIdAsync(string logSourceName, int id, CancellationToken cancellationToken = default);


    }
}
