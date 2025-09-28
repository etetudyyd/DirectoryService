using CSharpFunctionalExtensions;
using DevQuestions.Domain.Entities;
using DevQuestions.Domain.Shared;

namespace DirectoryService.Application.Database.IRepositories;

public interface IDepartmentsRepository
{
    Task<Result<Guid, Error>> AddAsync(Department department, CancellationToken cancellationToken);

    Task<Result<Department, Error>> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> IsIdentifierExistAsync(string identifier, CancellationToken cancellationToken);

    Task<bool> AllDepartmentLocationsExistsAndActiveAsync(List<Guid> ids, CancellationToken cancellationToken);
}