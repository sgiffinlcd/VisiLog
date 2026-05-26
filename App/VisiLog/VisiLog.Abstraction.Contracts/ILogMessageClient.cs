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
        /// Retrieves a page of log messages from the specified log source via the WebAPI,
        /// optionally filtered by level.
        /// </summary>
        /// <param name="logSourceName">Name of the configured log source to query.</param>
        /// <param name="offset">Zero-based offset of the first record to return.</param>
        /// <param name="count">Maximum number of log messages to return in this page.</param>
        /// <param name="levels">
        /// Optional filter — when non-null and non-empty, only messages whose <c>Level</c>
        /// matches one of the supplied values are returned. <c>null</c> or empty disables the filter.
        /// </param>
        /// <param name="traceId">
        /// Optional filter — when non-null and non-empty, only messages whose <c>TraceId</c>
        /// equals the supplied value are returned. <c>null</c> or empty disables the filter.
        /// </param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>The page of matching log messages plus the total count across all pages.</returns>
        Task<LogMessagePage> GetPageAsync(
            string logSourceName,
            int offset,
            int count,
            IReadOnlyCollection<string>? levels,
            string? traceId,
            CancellationToken cancellationToken = default);
    }
}
