using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using VisiLog.Abstraction.Contracts;
using VisiLog.Model.Service.WebAPI;

namespace VisiLog.Abstraction.Service.WebAPI
{
    internal sealed class LogSourceClient : ILogSourceClient
    {
        private readonly HttpClient _httpClient;

        public LogSourceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Retrieves the names of all configured log sources from the WebAPI.
        /// </summary>
        public async Task<IReadOnlyList<string>> GetAllLogSourceNamesAsync(CancellationToken cancellationToken = default)
        {
            var names = await _httpClient.GetFromJsonAsync(
                "api/logsources",
                VisiLogJsonSerializerContext.Default.ListString,
                cancellationToken);
            return names ?? new List<string>();
        }
    }
}
