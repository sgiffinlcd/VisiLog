using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VisiLog.Data.Contracts.Repositories;
using VisiLog.Logic.Contracts;
using VisiLog.Model.App;

namespace VisiLog.Logic
{
    internal sealed class LogMessageLogic : ILogMessageLogic
    {
        private readonly ILogMessageRepository _logMessageRepository;

        public LogMessageLogic(ILogMessageRepository logMessageRepository)
        {
            _logMessageRepository = logMessageRepository;
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
        public Task<LogMessagePage> GetPageAsync(
            string logSourceName,
            int offset,
            int count,
            IReadOnlyCollection<string>? levels,
            string? traceId,
            CancellationToken cancellationToken = default)
        {
            return _logMessageRepository.GetPageAsync(logSourceName, offset, count, levels, traceId, cancellationToken);
        }
    }
}
