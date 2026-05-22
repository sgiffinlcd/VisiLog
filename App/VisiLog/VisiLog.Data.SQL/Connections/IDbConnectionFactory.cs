using System.Data;

namespace VisiLog.Data.SQL.Connections
{
    /// <summary>
    /// Creates a new, unopened <see cref="IDbConnection"/> per call. Dapper opens/closes as needed.
    /// Internal to the SQL data layer — callers outside Data.SQL should depend on repositories, not connections.
    /// </summary>
    internal interface IDbConnectionFactory
    {
        IDbConnection Create();
    }
}
