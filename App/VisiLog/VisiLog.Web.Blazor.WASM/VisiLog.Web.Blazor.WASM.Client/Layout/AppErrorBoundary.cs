using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace VisiLog.Web.Blazor.WASM.Client.Layout
{
    /// <summary>
    /// App-wide error boundary that routes any unhandled component exception to the
    /// <c>/error</c> page. Used in <see cref="MainLayout"/> to wrap the routed content
    /// so a single failing page can't take down the whole app.
    /// </summary>
    /// <remarks>
    /// Navigation uses <c>forceLoad: true</c> so the failed component tree is fully torn
    /// down and the error page renders in a fresh boundary — avoiding the
    /// recover/re-render/re-throw loop that occurs when calling <c>Recover()</c>
    /// while the failing component is still in the tree.
    /// </remarks>
    public sealed class AppErrorBoundary : ErrorBoundary
    {
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        protected override Task OnErrorAsync(System.Exception exception)
        {
            NavigationManager.NavigateTo("/error", forceLoad: true);
            return Task.CompletedTask;
        }
    }
}
