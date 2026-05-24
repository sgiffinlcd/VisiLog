using CodeFactory.NDF;
using Microsoft.Extensions.Options;
using VisiLog.Data.Contracts.Repositories;
using VisiLog.Model.App;

namespace VisiLog.Data.SQL.Repositories
{
    /// <summary>
    /// Configuration-backed implementation of <see cref="ILogSourceRepository"/>.
    /// Returns the <see cref="LogSource"/> entries bound from the <c>LogSources</c> section of <c>appsettings.json</c>.
    /// Lives in Data.SQL alongside the other repositories despite not touching SQL itself.
    /// </summary>
    internal sealed class LogSourceRepository : ILogSourceRepository
    {
        private readonly IOptions<List<LogSource>> _logSources;

        public LogSourceRepository(IOptions<List<LogSource>> logSources)
        {
            _logSources = logSources;
        }

        /// <summary>
        /// Retrieves the names of all configured log sources that have a non-blank <see cref="LogSource.Name"/>.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A read-only list of the configured log source names.</returns>
        public Task<IReadOnlyList<string>> GetAllLogSourceNamesAsync(CancellationToken cancellationToken = default)
        {
            IReadOnlyList<string> result = Array.Empty<string>();

            try
            {
                result = (_logSources?.Value ?? new List<LogSource>())
                    .Where(source => !string.IsNullOrWhiteSpace(source.Name))
                    .Select(source => source.Name)
                    .ToList();
            }
            catch (Exception)
            {
                // No SqlException catch — this repository never hits SQL.
                return Task.FromException<IReadOnlyList<string>>(new UnhandledException());
            }

            return Task.FromResult(result);
        }

    }
}
