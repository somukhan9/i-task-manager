using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Dapper;

public interface ISqlConnectionFactory
{
    public Task<IDbConnection> CreateConnectionAsync();
}

public class SqlConnectionFactory(IConfiguration config) : ISqlConnectionFactory
{
    private readonly string _connectionString = config.GetConnectionString("TaskManagerDbConnectionString")!;


    public async Task<IDbConnection> CreateConnectionAsync()
    {
        if (string.IsNullOrEmpty(_connectionString))
        {
            throw new ArgumentNullException(nameof(_connectionString), "Connection string cannot be null or empty.");
        }

        var connection = new SqlConnection(_connectionString);

        await connection.OpenAsync();

        return connection;
    }
}