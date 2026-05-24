using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
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
        /// Retrieves the most recent log messages from the specified log source via the WebAPI.
        /// </summary>
        public async Task<IReadOnlyList<LogMessage>> GetRecentAsync(string logSourceName, int count, CancellationToken cancellationToken = default)
        {
            var escapedName = Uri.EscapeDataString(logSourceName);
            var url = $"api/logsources/{escapedName}/messages?count={count}";
            var responses = await _httpClient.GetFromJsonAsync(
                url,
                VisiLogJsonSerializerContext.Default.ListLogMessageResponse,
                cancellationToken);
            if (responses is null)
            {
                return Array.Empty<LogMessage>();
            }

            return responses.Select(ToDomain).ToList();
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
