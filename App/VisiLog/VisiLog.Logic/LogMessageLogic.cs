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
        /// Retrieves the most recent log messages from the specified log source.
        /// </summary>
        /// <param name="logSourceName">Name of the configured log source to query.</param>
        /// <param name="count">The maximum number of log messages to retrieve.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A read-only list of the most recent log messages ordered by timestamp in descending order.</returns>
        public Task<IReadOnlyList<LogMessage>> GetRecentAsync(string logSourceName, int count, CancellationToken cancellationToken = default)
        {
            return _logMessageRepository.GetRecentAsync(logSourceName, count, cancellationToken);
        }
    }
}
