using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VisiLog.Model.App;

namespace VisiLog.Logic.Contracts
{
    /// <summary>
    /// Business logic contract for retrieving <see cref="LogMessage"/> records from a configured log source.
    /// </summary>
    public interface ILogMessageLogic
    {
        /// <summary>
        /// Retrieves the most recent log messages from the specified log source.
        /// </summary>
        /// <param name="logSourceName">Name of the configured log source to query.</param>
        /// <param name="count">The maximum number of log messages to retrieve.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A read-only list of the most recent log messages ordered by timestamp in descending order.</returns>
        Task<IReadOnlyList<LogMessage>> GetRecentAsync(string logSourceName, int count, CancellationToken cancellationToken = default);
    }
}
