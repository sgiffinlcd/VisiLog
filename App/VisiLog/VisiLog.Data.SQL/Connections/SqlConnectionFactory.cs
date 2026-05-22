using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace VisiLog.Data.SQL.Connections
{
    internal sealed class SqlConnectionFactory : IDbConnectionFactory
    {
        public const string ConnectionStringName = "VisiLog";

        private readonly string _connectionString;

        public SqlConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString(ConnectionStringName)
                ?? throw new InvalidOperationException(
                    $"Connection string '{ConnectionStringName}' is not configured.");
        }

        public IDbConnection Create() => new SqlConnection(_connectionString);
    }
}
