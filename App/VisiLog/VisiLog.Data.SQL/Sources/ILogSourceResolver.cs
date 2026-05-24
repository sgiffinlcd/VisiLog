namespace VisiLog.Data.SQL.Sources
{
    /// <summary>
    /// Resolves a configured log source by name into the connection string and table name needed to query it.
    /// Internal to the SQL data layer — callers outside Data.SQL should depend on repositories, not source details.
    /// </summary>
    internal interface ILogSourceResolver
    {
        /// <summary>
        /// Looks up the log source whose <c>Name</c> matches <paramref name="logSourceName"/> in the configured
        /// <c>LogSources</c> collection and returns its connection string and table name.
        /// </summary>
        /// <param name="logSourceName">Name of the configured log source.</param>
        /// <returns>A tuple of the SQL Server connection string and the fully-qualified table name for the source.</returns>
        (string ConnectionString, string TableName) Resolve(string logSourceName);
    }
}
