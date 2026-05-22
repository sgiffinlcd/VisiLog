namespace VisiLog.Model.App
{
    /// <summary>
    /// Configuration describing an external SQL-backed source of log messages that VisiLog reads from.
    /// Bound from the "LogSource" section of appsettings and exposed via <c>IOptions&lt;LogSource&gt;</c>.
    /// </summary>
    public class LogSource
    {
        /// <summary>
        /// Human-readable name identifying this logging source (e.g., "Production API", "Staging Worker").
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// SQL Server connection string targeting the database that contains the log table.
        /// </summary>
        public string ConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Fully-qualified name of the table holding the log records (e.g., "dbo.LogMessage").
        /// </summary>
        public string TableName { get; set; } = string.Empty;
    }
}
