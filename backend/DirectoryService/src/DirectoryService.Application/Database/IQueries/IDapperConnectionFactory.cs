using System.Data;

namespace DirectoryService.Application.Database.IQueries;

public interface IDapperConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken);
}