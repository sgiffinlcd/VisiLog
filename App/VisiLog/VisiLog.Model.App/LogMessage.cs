using System;
using System.Collections.Generic;
using System.Text;

namespace VisiLog.Model.App
{
    /// <summary>
    /// Data class that provides all the details for a log message that is captured and stored in the log. This is used to capture all the details of a log message and provide it for storage and retrieval from the log.
    /// </summary>
    public class LogMessage
    {
        /// <summary>
        /// Seed value based identity for each log entry.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The logging message written to the log.
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// The logging level the log message was assigned to.
        /// </summary>
        public string? Level { get; set; }

        /// <summary>
        /// The date and time in which the log message has occurred.
        /// </summary>
        public DateTime? TimeStamp { get; set; }

        /// <summary>
        /// The exception that was captured as part of the log message.
        /// </summary>
        public string? Exception { get; set; }

        /// <summary>
        /// Which hosting environment was the log message captured in.
        /// </summary>
        public string? Environment { get; set; }

        /// <summary>
        /// The name of the machine in which the log message occurred on.
        /// </summary>
        public string? Machine { get; set; }

        /// <summary>
        /// Which processor thread the log message was captured from.
        /// </summary>
        public int? Thread { get; set; }

        /// <summary>
        /// The full name of the object the logging occurred in.
        /// </summary>
        public string? Logger { get; set; }

        /// <summary>
        /// The name of the member the logging event was captured from.
        /// </summary>
        public string? MemberName { get; set; }

        /// <summary>
        /// The line number in the source code the logging event was captured from.
        /// </summary>
        public int? LineNumber { get; set; }

        /// <summary>
        /// Unique identifier used to trace a complete transaction across logging messages.
        /// </summary>
        public string? TraceId { get; set; }
    }

}
