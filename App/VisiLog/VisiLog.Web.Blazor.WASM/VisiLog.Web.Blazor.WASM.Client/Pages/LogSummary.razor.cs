using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using VisiLog.Abstraction.Contracts;
using VisiLog.Model.App;

namespace VisiLog.Web.Blazor.WASM.Client.Pages
{
    public partial class LogSummary : ComponentBase
    {
        private const int MinPageSize = 40;

        private static readonly Dictionary<string, (string Icon, Color Color)> LevelIconMap =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["Trace"]       = (Icons.Material.Filled.Timeline,    Color.Default),
                ["Debug"]       = (Icons.Material.Filled.BugReport,   Color.Secondary),
                ["Information"] = (Icons.Material.Filled.Info,        Color.Info),
                ["Warning"]     = (Icons.Material.Filled.Warning,     Color.Warning),
                ["Error"]       = (Icons.Material.Filled.Error,       Color.Error),
                ["Critical"]    = (Icons.Material.Filled.Dangerous,   Color.Error),
            };

        private static string GetLevelIcon(string? level) =>
            level is not null && LevelIconMap.TryGetValue(level, out var entry)
                ? entry.Icon
                : Icons.Material.Filled.HelpOutline;

        private static Color GetLevelColor(string? level) =>
            level is not null && LevelIconMap.TryGetValue(level, out var entry)
                ? entry.Color
                : Color.Default;

        private static string GetTraceIdShort(string traceId) =>
            traceId.Length <= 6 ? traceId : traceId[..6];

        private static string Truncate(string value, int maxLength) =>
            value.Length <= maxLength ? value : value[..maxLength] + "…";

        // Ordered for stable checkbox display; LevelIconMap keys are unordered.
        private static readonly string[] LevelFilterOrder =
        {
            "Trace", "Debug", "Information", "Warning", "Error", "Critical",
        };

        private readonly HashSet<string> _selectedLevels =
            new(LevelFilterOrder, StringComparer.OrdinalIgnoreCase);

        [Inject] private ILogSourceClient LogSourceClient { get; set; } = default!;
        [Inject] private ILogMessageClient LogMessageClient { get; set; } = default!;
        [Inject] private IDialogService DialogService { get; set; } = default!;

        private MudDataGrid<LogMessage>? _grid;
        private IReadOnlyList<string> _logSources = Array.Empty<string>();
        private string? _selectedSource;
        private string? _selectedTraceId;
        private bool _loadingSources;
        private string? _errorMessage;

        // Set synchronously by trace-id button clicks so the grid's row-click handler
        // (which fires after the button click via DOM bubbling) can skip opening the
        // details dialog. Far more reliable than @onclick:stopPropagation, which
        // MudButton swallows internally before it reaches a wrapping element.
        private bool _suppressNextRowClick;

        // Cached delegate: ensures the grid's Virtualize sees a stable reference across
        // re-renders so it doesn't cancel and re-issue the data-provider call.
        private readonly Func<GridStateVirtualize<LogMessage>, CancellationToken, Task<GridData<LogMessage>>> _serverReload;

        public LogSummary()
        {
            _serverReload = ServerReloadAsync;
        }

        protected override async Task OnInitializedAsync()
        {
            _loadingSources = true;
            try
            {
                _logSources = await LogSourceClient.GetAllLogSourceNamesAsync();
                _selectedSource = _logSources.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _errorMessage = $"Failed to load log sources: {ex.Message}";
            }
            finally
            {
                _loadingSources = false;
            }
        }

        private async Task OnSourceChangedAsync(string source)
        {
            _selectedSource = source;
            // Trace ids are source-specific; clear the trace filter when switching sources
            // so the user isn't silently filtering on an id that doesn't exist in the new source.
            _selectedTraceId = null;
            await ReloadGridAsync();
        }

        private async Task OnLevelFilterChangedAsync(string level, bool selected)
        {
            if (selected)
            {
                _selectedLevels.Add(level);
            }
            else
            {
                _selectedLevels.Remove(level);
            }

            await ReloadGridAsync();
        }

        private async Task OnTraceIdFilterSelectedAsync(string traceId)
        {
            // Set the suppression flag synchronously (before any await) so the row-click
            // handler — which fires in the same dispatch cycle via DOM bubbling — sees it.
            _suppressNextRowClick = true;

            if (string.IsNullOrEmpty(traceId) || string.Equals(_selectedTraceId, traceId, StringComparison.Ordinal))
            {
                return;
            }

            _selectedTraceId = traceId;
            await ReloadGridAsync();
        }

        private async Task ClearTraceIdFilterAsync()
        {
            if (_selectedTraceId is null)
            {
                return;
            }

            _selectedTraceId = null;
            await ReloadGridAsync();
        }

        private Task ReloadGridAsync() =>
            _grid is null ? Task.CompletedTask : _grid.ReloadServerData();

        private Task OnRowClickAsync(DataGridRowClickEventArgs<LogMessage> args)
        {
            if (_suppressNextRowClick)
            {
                _suppressNextRowClick = false;
                return Task.CompletedTask;
            }

            var parameters = new DialogParameters<LogMessageDetailsDialog>
            {
                { x => x.Message, args.Item },
            };
            var options = new DialogOptions
            {
                MaxWidth = MaxWidth.ExtraLarge,
                FullWidth = true,
                CloseButton = true,
                BackdropClick = true,
            };
            return DialogService.ShowAsync<LogMessageDetailsDialog>("Log Message Details", parameters, options);
        }

        private async Task<GridData<LogMessage>> ServerReloadAsync(
            GridStateVirtualize<LogMessage> state,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_selectedSource))
            {
                return new GridData<LogMessage>
                {
                    Items = Array.Empty<LogMessage>(),
                    TotalItems = 0,
                };
            }

            // Honor the "at least 40 records per fetch" expectation — first request seeds
            // the latest 40 even when the visible viewport would ask for fewer.
            var count = Math.Max(state.Count, MinPageSize);
            var levels = _selectedLevels.ToList();

            try
            {
                var page = await LogMessageClient.GetPageAsync(
                    _selectedSource,
                    state.StartIndex,
                    count,
                    levels,
                    _selectedTraceId,
                    cancellationToken);

                _errorMessage = null;

                return new GridData<LogMessage>
                {
                    Items = page.Items,
                    TotalItems = page.TotalCount,
                };
            }
            catch (TaskCanceledException)
            {
                // Terminal swallow point. Dapper throws TaskCanceledException when its
                // CancellationToken fires; the repo, logic, endpoint and HTTP client all
                // propagate it unchanged. Absorb it here so it never surfaces to MudBlazor,
                // Blazor's logger, or the user. Causes: grid superseded this fetch with a
                // newer one, page disposed, or HTTP connection aborted — all routine.
                return new GridData<LogMessage>
                {
                    Items = Array.Empty<LogMessage>(),
                    TotalItems = 0,
                };
            }
            catch (Exception ex)
            {
                _errorMessage = $"Failed to load messages for '{_selectedSource}': {ex.Message}";
                return new GridData<LogMessage>
                {
                    Items = Array.Empty<LogMessage>(),
                    TotalItems = 0,
                };
            }
        }
    }
}
