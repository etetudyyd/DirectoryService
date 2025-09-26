using CSharpFunctionalExtensions;
using DevQuestions.Domain.Entities;
using DevQuestions.Domain.Shared;
using DevQuestions.Domain.ValueObjects.DepartmentVO;
using DirectoryService.Application.Database.IRepositories;
using DirectoryService.Contracts.Locations;

namespace DirectoryService.Application.Features.Departments.UpdateDepartmentLocations;

public class UpdateDepartmentLocationsHandler
{
    private readonly IDepartmentsRepository _repository;

    public UpdateDepartmentLocationsHandler(IDepartmentsRepository repository)
    {
        _repository = repository;
    }

    /*public async Task<Result<Guid, Error>> Handler(UpdateDepartmentLocationsDto request, CancellationToken cancellationToken)
    {
        var departmentId = new DepartmentId(request.DepartmentId);
        
        await DeleteLocationsByDepartmentId(departmentId, cancellationToken);

        List<Locations> locations = [];

        foreach (var locationRequest in request.DepartmentLocations)
        {
            var location = Location.Create(request.Name, departmentId, );
        }
        
        
        var result = await _repository.Update(request, cancellationToken);

        return result.Id.Value;Id
    }*/
}