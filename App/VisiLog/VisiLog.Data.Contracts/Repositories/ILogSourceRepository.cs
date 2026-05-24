using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VisiLog.Model.App;

namespace VisiLog.Data.Contracts.Repositories
{
    /// <summary>
    /// Persistence contract for the catalog of configured <see cref="LogSource"/> entries available to query.
    /// </summary>
    public interface ILogSourceRepository
    {
        /// <summary>
        /// Retrieves the names of all configured log sources that have a non-blank <see cref="LogSource.Name"/>.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A read-only list of the configured log source names.</returns>
        Task<IReadOnlyList<string>> GetAllLogSourceNamesAsync(CancellationToken cancellationToken = default);
    }
}
