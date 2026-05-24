using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using VisiLog.Model.App;

namespace VisiLog.Web.Blazor.WASM.Client.Pages
{
    public partial class LogMessageDetailsDialog : ComponentBase
    {
        private static readonly JsonSerializerOptions CopyJsonOptions = new()
        {
            WriteIndented = true,
        };

        [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = default!;

        [Inject] private IJSRuntime JS { get; set; } = default!;

        [Parameter, EditorRequired] public LogMessage Message { get; set; } = default!;

        private void Close() => MudDialog.Close();

        private async Task CopyMessageAsync()
        {
            if (!string.IsNullOrEmpty(Message.Message))
            {
                await JS.InvokeVoidAsync("navigator.clipboard.writeText", Message.Message);
            }
        }

        private async Task CopyExceptionAsync()
        {
            if (!string.IsNullOrEmpty(Message.Exception))
            {
                await JS.InvokeVoidAsync("navigator.clipboard.writeText", Message.Exception);
            }
        }

        private async Task CopyAllAsync()
        {
            // Anonymous object preserves declaration order in the JSON output, so the copied
            // payload mirrors the field order shown in the dialog table.
            var ordered = new
            {
                Message.Id,
                Message.TimeStamp,
                Message.Level,
                Message.TraceId,
                Message.Machine,
                Message.Environment,
                Message.Thread,
                Message.Logger,
                Message.MemberName,
                Message.LineNumber,
                Message.Message,
                Message.Exception,
            };
            var json = JsonSerializer.Serialize(ordered, CopyJsonOptions);
            await JS.InvokeVoidAsync("navigator.clipboard.writeText", json);
        }
    }
}
