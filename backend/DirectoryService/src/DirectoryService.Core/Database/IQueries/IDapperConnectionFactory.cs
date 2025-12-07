using System.Data;

namespace DirectoryService.Database.IQueries;

public interface IDapperConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken);
}