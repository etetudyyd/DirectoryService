using CSharpFunctionalExtensions;
using DevQuestions.Domain.Shared;

namespace DirectoryService.Application.Abstractions.Queries;

public interface IQueryHandler<TResponse, in TQuery>
    where TQuery : IQuery
{
    Task<Result<TResponse, Errors>> Handle(TQuery query, CancellationToken cancellationToken);
}