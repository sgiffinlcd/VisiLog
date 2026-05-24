using Microsoft.Extensions.Options;
using VisiLog.Model.App;

namespace VisiLog.Data.SQL.Sources
{
    internal sealed class LogSourceResolver : ILogSourceResolver
    {
        private readonly IReadOnlyDictionary<string, LogSource> _sourcesByName;

        public LogSourceResolver(IOptions<List<LogSource>> logSources)
        {
            _sourcesByName = (logSources?.Value ?? new List<LogSource>())
                .Where(source => !string.IsNullOrWhiteSpace(source.Name))
                .ToDictionary(source => source.Name, StringComparer.OrdinalIgnoreCase);
        }

        public (string ConnectionString, string TableName) Resolve(string logSourceName)
        {
            if (string.IsNullOrWhiteSpace(logSourceName))
            {
                throw new ArgumentException("A log source name must be provided.", nameof(logSourceName));
            }

            if (!_sourcesByName.TryGetValue(logSourceName, out var source))
            {
                var available = _sourcesByName.Count == 0
                    ? "(none configured)"
                    : string.Join(", ", _sourcesByName.Keys);
                throw new InvalidOperationException(
                    $"No log source named '{logSourceName}' is configured. Available: {available}.");
            }

            if (string.IsNullOrWhiteSpace(source.ConnectionString))
            {
                throw new InvalidOperationException(
                    $"Log source '{logSourceName}' has no connection string configured.");
            }

            if (string.IsNullOrWhiteSpace(source.TableName))
            {
                throw new InvalidOperationException(
                    $"Log source '{logSourceName}' has no table name configured.");
            }

            return (source.ConnectionString, source.TableName);
        }
    }
}
