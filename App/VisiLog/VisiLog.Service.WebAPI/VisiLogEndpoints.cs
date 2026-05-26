using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using VisiLog.Logic.Contracts;
using VisiLog.Model.App;
using VisiLog.Model.Service.WebAPI;

namespace VisiLog.Service.WebAPI
{
    /// <summary>
    /// Minimal-API endpoint registrations exposed by this library.
    /// Called once from the host's <c>Program.cs</c> after <c>WebApplication</c> has been built.
    /// </summary>
    public static class VisiLogEndpoints
    {
        private const int DefaultPageSize = 40;
        private const int MaxPageSize = 500;

        /// <summary>
        /// Maps the VisiLog HTTP endpoints onto the supplied route builder.
        /// </summary>
        public static IEndpointRouteBuilder MapVisiLogEndpoints(this IEndpointRouteBuilder endpoints)
        {
            var group = endpoints.MapGroup("/api");

            group.MapGet("/logsources", async (
                ILogSourceLogic logSourceLogic,
                CancellationToken cancellationToken) =>
            {
                var names = await logSourceLogic.GetAllLogSourceNamesAsync(cancellationToken);
                return Results.Ok(names);
            })
            .WithName("GetLogSourceNames");

            group.MapGet("/logsources/{name}/messages", async (
                string name,
                int? offset,
                int? count,
                string[]? levels,
                string? traceId,
                ILogMessageLogic logMessageLogic,
                CancellationToken cancellationToken) =>
            {
                var requestedOffset = offset is > 0 ? offset.Value : 0;
                var requestedCount = count is > 0 ? Math.Min(count.Value, MaxPageSize) : DefaultPageSize;
                var levelFilter = levels is { Length: > 0 } ? levels : null;
                var traceIdFilter = string.IsNullOrEmpty(traceId) ? null : traceId;

                var page = await logMessageLogic.GetPageAsync(
                    name,
                    requestedOffset,
                    requestedCount,
                    levelFilter,
                    traceIdFilter,
                    cancellationToken);

                var response = new LogMessagePageResponse
                {
                    Items = page.Items.Select(ToResponse).ToList(),
                    TotalCount = page.TotalCount,
                };
                return Results.Ok(response);
            })
            .WithName("GetLogMessagePage");

            return endpoints;
        }

        private static LogMessageResponse ToResponse(LogMessage message) => new()
        {
            Id = message.Id,
            Message = message.Message,
            Level = message.Level,
            TimeStamp = message.TimeStamp,
            Exception = message.Exception,
            Environment = message.Environment,
            Machine = message.Machine,
            Thread = message.Thread,
            Logger = message.Logger,
            MemberName = message.MemberName,
            LineNumber = message.LineNumber,
            TraceId = message.TraceId,
        };
    }
}
