using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VisiLog.Model.Service.WebAPI
{
    /// <summary>
    /// Wire-shape DTO returned by the WebAPI for a page of log messages plus the total count
    /// of records that match the active filter.
    /// </summary>
    public class LogMessagePageResponse
    {
        [JsonPropertyName("items")]
        public List<LogMessageResponse> Items { get; set; } = new();

        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }
    }
}
