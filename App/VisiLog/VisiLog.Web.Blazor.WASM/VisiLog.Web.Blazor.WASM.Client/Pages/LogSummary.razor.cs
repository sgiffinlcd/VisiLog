using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using VisiLog.Abstraction.Contracts;
using VisiLog.Model.App;

namespace VisiLog.Web.Blazor.WASM.Client.Pages
{
    public partial class LogSummary : ComponentBase
    {
        private const int RecentMessageCount = 200;

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

        private static readonly HashSet<string> KnownLevels =
            new(LevelFilterOrder, StringComparer.OrdinalIgnoreCase);

        private readonly HashSet<string> _selectedLevels =
            new(LevelFilterOrder, StringComparer.OrdinalIgnoreCase);

        private IEnumerable<LogMessage> FilteredMessages =>
            _messages.Where(m =>
                string.IsNullOrEmpty(m.Level)
                || !KnownLevels.Contains(m.Level)
                || _selectedLevels.Contains(m.Level));

        private void OnLevelFilterChanged(string level, bool selected)
        {
            if (selected)
            {
                _selectedLevels.Add(level);
            }
            else
            {
                _selectedLevels.Remove(level);
            }
        }

        [Inject] private ILogSourceClient LogSourceClient { get; set; } = default!;
        [Inject] private ILogMessageClient LogMessageClient { get; set; } = default!;
        [Inject] private IDialogService DialogService { get; set; } = default!;

        private IReadOnlyList<string> _logSources = Array.Empty<string>();
        private IReadOnlyList<LogMessage> _messages = Array.Empty<LogMessage>();
        private string? _selectedSource;
        private bool _loadingSources;
        private bool _loadingMessages;
        private string? _errorMessage;

        protected override async Task OnInitializedAsync()
        {
            _loadingSources = true;
            try
            {
                _logSources = await LogSourceClient.GetAllLogSourceNamesAsync();
                _selectedSource = _logSources.FirstOrDefault();
                if (_selectedSource is not null)
                {
                    await LoadMessagesAsync(_selectedSource);
                }
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
            await LoadMessagesAsync(source);
        }

        private Task OnRowClickAsync(DataGridRowClickEventArgs<LogMessage> args)
        {
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

        private async Task LoadMessagesAsync(string source)
        {
            _loadingMessages = true;
            _errorMessage = null;
            try
            {
                _messages = await LogMessageClient.GetRecentAsync(source, RecentMessageCount);
            }
            catch (Exception ex)
            {
                _messages = Array.Empty<LogMessage>();
                _errorMessage = $"Failed to load messages for '{source}': {ex.Message}";
            }
            finally
            {
                _loadingMessages = false;
            }
        }
    }
}
