using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VisiLog.Logic.Contracts
{
    /// <summary>
    /// Business logic contract for working with the catalog of configured log sources.
    /// </summary>
    public interface ILogSourceLogic
    {
        /// <summary>
        /// Retrieves the names of all configured log sources.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A read-only list of configured log source names.</returns>
        Task<IReadOnlyList<string>> GetAllLogSourceNamesAsync(CancellationToken cancellationToken = default);
    }
}
