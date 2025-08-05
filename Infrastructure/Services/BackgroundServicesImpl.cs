using Application.Contracts;
using Dapper;
using Infrastructure.Dapper;

namespace Infrastructure.Services;

public class BackgroundServicesImpl(ISqlConnectionFactory factory) : IBackgroundService
{
    public async Task<List<Domain.Entities.BackgroundServices>> ListOfAvailableServices()
    {
        var connection = await factory.CreateConnectionAsync();

        var sql = @"
            SELECT *
            FROM dbo.BackgroundServices
        ";

        var backgroundServicesList = await connection.QueryAsync<Domain.Entities.BackgroundServices>(new CommandDefinition(sql, null, commandType: System.Data.CommandType.Text, commandTimeout: 60));

        return backgroundServicesList.ToList();
    }
}
