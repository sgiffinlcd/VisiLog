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
                int? count,
                ILogMessageLogic logMessageLogic,
                CancellationToken cancellationToken) =>
            {
                var requestedCount = count is > 0 ? count.Value : 200;
                var messages = await logMessageLogic.GetRecentAsync(name, requestedCount, cancellationToken);
                var response = messages.Select(ToResponse).ToList();
                return Results.Ok(response);
            })
            .WithName("GetRecentLogMessages");

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
