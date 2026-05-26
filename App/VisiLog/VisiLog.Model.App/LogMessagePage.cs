using System.Collections.Generic;

namespace VisiLog.Model.App
{
    /// <summary>
    /// A page of log messages plus the total count of records that match the active filter.
    /// </summary>
    public class LogMessagePage
    {
        /// <summary>
        /// The log messages contained in this page, ordered from newest to oldest.
        /// </summary>
        public IReadOnlyList<LogMessage> Items { get; set; } = new List<LogMessage>();

        /// <summary>
        /// A virtualization hint indicating the lower bound of the total record count.
        /// While more pages may exist this overshoots the loaded amount so a UI scrollbar
        /// can extend; once the final page is reached this is the exact total.
        /// </summary>
        public int TotalCount { get; set; }
    }
}
