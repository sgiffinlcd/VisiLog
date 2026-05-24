using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VisiLog.Model.App;

namespace VisiLog.Abstraction.Contracts
{
    /// <summary>
    /// Client-side HTTP abstraction for the log-message WebAPI.
    /// </summary>
    public interface ILogMessageClient
    {
        /// <summary>
        /// Retrieves the most recent log messages from the specified log source via the WebAPI.
        /// </summary>
        /// <param name="logSourceName">Name of the configured log source to query.</param>
        /// <param name="count">The maximum number of log messages to retrieve.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        Task<IReadOnlyList<LogMessage>> GetRecentAsync(string logSourceName, int count, CancellationToken cancellationToken = default);
    }
}
