using System.Data;
using Microsoft.Data.SqlClient;

namespace IdentityAuthService.DbContext;

public interface IDbProvider
{
    public Task<IDbConnection> CreateConnectionAsync();
}

public class DbProvider(string connectionString) : IDbProvider
{
    public async Task<IDbConnection> CreateConnectionAsync()
    {
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        return connection;
    }
}