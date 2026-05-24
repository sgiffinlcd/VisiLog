using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VisiLog.Abstraction.Contracts
{
    /// <summary>
    /// Client-side HTTP abstraction for the log-source catalog WebAPI.
    /// </summary>
    public interface ILogSourceClient
    {
        /// <summary>
        /// Retrieves the names of all configured log sources from the WebAPI.
        /// </summary>
        Task<IReadOnlyList<string>> GetAllLogSourceNamesAsync(CancellationToken cancellationToken = default);
    }
}
