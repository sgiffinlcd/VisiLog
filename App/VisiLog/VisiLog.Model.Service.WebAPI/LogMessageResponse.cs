using System;
using System.Text.Json.Serialization;

namespace VisiLog.Model.Service.WebAPI
{
    /// <summary>
    /// Wire-shape DTO returned by the WebAPI for a single log message.
    /// </summary>
    public class LogMessageResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("level")]
        public string? Level { get; set; }

        [JsonPropertyName("timeStamp")]
        public DateTime? TimeStamp { get; set; }

        [JsonPropertyName("exception")]
        public string? Exception { get; set; }

        [JsonPropertyName("environment")]
        public string? Environment { get; set; }

        [JsonPropertyName("machine")]
        public string? Machine { get; set; }

        [JsonPropertyName("thread")]
        public int? Thread { get; set; }

        [JsonPropertyName("logger")]
        public string? Logger { get; set; }

        [JsonPropertyName("memberName")]
        public string? MemberName { get; set; }

        [JsonPropertyName("lineNumber")]
        public int? LineNumber { get; set; }

        [JsonPropertyName("traceId")]
        public string? TraceId { get; set; }
    }
}
