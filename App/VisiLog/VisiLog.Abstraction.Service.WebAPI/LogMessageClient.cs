using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VisiLog.Abstraction.Contracts;
using VisiLog.Model.App;
using VisiLog.Model.Service.WebAPI;

namespace VisiLog.Abstraction.Service.WebAPI
{
    internal sealed class LogMessageClient : ILogMessageClient
    {
        private readonly HttpClient _httpClient;

        public LogMessageClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Retrieves a page of log messages from the specified log source via the WebAPI,
        /// optionally filtered by level.
        /// </summary>
        public async Task<LogMessagePage> GetPageAsync(
            string logSourceName,
            int offset,
            int count,
            IReadOnlyCollection<string>? levels,
            string? traceId,
            CancellationToken cancellationToken = default)
        {
            var escapedName = Uri.EscapeDataString(logSourceName);
            var url = new StringBuilder($"api/logsources/{escapedName}/messages?offset={offset}&count={count}");
            if (levels is { Count: > 0 })
            {
                foreach (var level in levels)
                {
                    url.Append("&levels=").Append(Uri.EscapeDataString(level));
                }
            }
            if (!string.IsNullOrEmpty(traceId))
            {
                url.Append("&traceId=").Append(Uri.EscapeDataString(traceId));
            }

            var response = await _httpClient.GetFromJsonAsync(
                url.ToString(),
                VisiLogJsonSerializerContext.Default.LogMessagePageResponse,
                cancellationToken);

            if (response is null)
            {
                return new LogMessagePage();
            }

            return new LogMessagePage
            {
                Items = response.Items.Select(ToDomain).ToList(),
                TotalCount = response.TotalCount,
            };
        }

        private static LogMessage ToDomain(LogMessageResponse response) => new()
        {
            Id = response.Id,
            Message = response.Message,
            Level = response.Level,
            TimeStamp = response.TimeStamp,
            Exception = response.Exception,
            Environment = response.Environment,
            Machine = response.Machine,
            Thread = response.Thread,
            Logger = response.Logger,
            MemberName = response.MemberName,
            LineNumber = response.LineNumber,
            TraceId = response.TraceId,
        };
    }
}
