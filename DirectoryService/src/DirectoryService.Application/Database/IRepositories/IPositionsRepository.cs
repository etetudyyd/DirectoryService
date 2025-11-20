using CSharpFunctionalExtensions;
using DevQuestions.Domain.Entities;
using DevQuestions.Domain.Shared;
using DevQuestions.Domain.ValueObjects.DepartmentVO;

namespace DirectoryService.Application.Database.IRepositories;

public interface IPositionsRepository
{
    Task<Guid> AddAsync(Position position, CancellationToken cancellationToken);

    Task<UnitResult<Error>> BulkDeleteInactivePositionsAsync(List<DepartmentId> departmentIds, CancellationToken cancellationToken);
}