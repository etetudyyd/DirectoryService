using CSharpFunctionalExtensions;
using DevQuestions.Domain.Entities;
using DevQuestions.Domain.Shared;
using DirectoryService.Application.Features.Departments.CreateDepartment;
using DirectoryService.Application.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Postgresql.Repositories;

public class DepartmentsRepository : IDepartmentsRepository
{
    private readonly DirectoryServiceDbContext _dbContext;

    private readonly ILogger<CreateDepartmentHandler> _logger;

    public DepartmentsRepository(
        DirectoryServiceDbContext dbContext,
        ILogger<CreateDepartmentHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<Guid, Error>> AddAsync(Department department, CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.Departments.AddAsync(department, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return department.Id.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while adding department");
            return Error.Failure(nameof(ErrorType.FAILURE), ex.Message);
        }
    }

    public async Task<Result<Department, Error>> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var department = await _dbContext.Departments
            .FirstOrDefaultAsync(d => d.Id.Value == id, cancellationToken);

        if (department is null)
        {
            return Error.Failure(nameof(ErrorType.NOT_FOUND), "Department not found");
        }

        return Result.Success<Department, Error>(department);
    }


    public async Task<bool> IsIdentifierExistAsync(string identifier, CancellationToken cancellationToken)
    {
        return await _dbContext.Departments
            .AnyAsync(
                d => d.Identifier.Value == identifier,
                cancellationToken: cancellationToken);
    }

    public async Task<bool> AllExistsAsync(List<Guid> ids, CancellationToken cancellationToken)
    {
        int count = await _dbContext.Departments
            .Where(d => ids.Contains(d.Id.Value))
            .Where(d => d.IsActive)
            .CountAsync(cancellationToken);

        return count == ids.Count;
    }
}