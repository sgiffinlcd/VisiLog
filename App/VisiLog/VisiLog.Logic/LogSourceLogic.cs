using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VisiLog.Data.Contracts.Repositories;
using VisiLog.Logic.Contracts;

namespace VisiLog.Logic
{
    internal sealed class LogSourceLogic : ILogSourceLogic
    {
        private readonly ILogSourceRepository _logSourceRepository;

        public LogSourceLogic(ILogSourceRepository logSourceRepository)
        {
            _logSourceRepository = logSourceRepository;
        }

        /// <summary>
        /// Retrieves the names of all configured log sources.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A read-only list of configured log source names.</returns>
        public Task<IReadOnlyList<string>> GetAllLogSourceNamesAsync(CancellationToken cancellationToken = default)
        {
            return _logSourceRepository.GetAllLogSourceNamesAsync(cancellationToken);
        }
    }
}
