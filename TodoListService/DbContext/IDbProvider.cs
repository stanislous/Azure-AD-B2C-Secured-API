using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace TodoListService.DbContext;

public interface IDbProvider
{
    public Task<IDbConnection> CreateConnectionAsync();
}

public class DbProvider : IDbProvider
{
    private readonly string _connectionString;

    public DbProvider(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IDbConnection> CreateConnectionAsync()
    {
        var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }
}