using CSharpFunctionalExtensions;
using DevQuestions.Domain.Shared;
using DevQuestions.Domain.ValueObjects.DepartmentVO;
using Path = DevQuestions.Domain.ValueObjects.DepartmentVO.Path;

namespace DevQuestions.Domain.Entities;

public sealed class Department : ISoftDeletable
{
    // ef
    private Department() { }

    private List<Department> _childrenDepartments = [];
    private List<DepartmentLocation> _departmentLocations = [];
    private List<DepartmentPosition> _departmentPositions = [];

    public DepartmentId Id { get; private set; } = null!;

    public DepartmentName Name { get; private set; } = null!;

    public Identifier Identifier { get; private set; } = null!;

    public Path Path { get; private set; } = null!;

    public DepartmentId? ParentId { get; private set; }

    public int Depth { get; private set; }

    public int ChildrenCount { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public DateTime? DeletedAt { get; private set; }

    public IReadOnlyList<Department> ChildrenDepartments => _childrenDepartments;

    public ICollection<DepartmentLocation> DepartmentLocations => _departmentLocations;

    public ICollection<DepartmentPosition> DepartmentPositions => _departmentPositions;

    private Department(
        DepartmentId id,
        DepartmentName name,
        Identifier identifier,
        Path path,
        int depth,
        DepartmentId? parentId,
        IEnumerable<DepartmentLocation> departmentLocations)
    {
        Id = id;
        Name = name;
        Path = path;
        Identifier = identifier;
        ParentId = parentId;
        Depth = depth;
        ChildrenCount = ChildrenDepartments.Count;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        DeletedAt = null;
        _departmentLocations = departmentLocations.ToList();
    }

    private Department(
        DepartmentId id,
        DepartmentName name,
        Identifier identifier,
        Path path,
        int depth,
        DepartmentId? parentId,
        bool isActive,
        IEnumerable<DepartmentLocation> departmentLocations)
    {
        Id = id;
        Name = name;
        Path = path;
        Identifier = identifier;
        ParentId = parentId;
        Depth = depth;
        ChildrenCount = ChildrenDepartments.Count;
        IsActive = isActive;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        DeletedAt = DateTime.UtcNow - TimeSpan.FromDays(7);
        _departmentLocations = departmentLocations.ToList();
    }

    public void AddLocations(IEnumerable<DepartmentLocation> departmentLocation)
    {
        var departmentLocations = departmentLocation
            .Select(l => DepartmentLocation.Create(l.LocationId, l.DepartmentId).Value);
        _departmentLocations.AddRange(departmentLocations);
    }

    public void UpdateLocations(IEnumerable<DepartmentLocation> departmentLocations)
    {
        _departmentLocations.Clear();
        AddLocations(departmentLocations);
    }

    public Result<int, Errors> SetParent(Department? parent)
    {
        if (parent == this)
        {
            return Error
                .Failure("department.not.found","department was not founded")
                .ToErrors();
        }

        // пересчитываем path + depth
        var newPathResult = Path.CalculatePath(parent?.Path, Identifier);
        if (!newPathResult.IsSuccess)
        {
            return newPathResult.Error.ToErrors();
        }

        int newDepth = 1;
        if (parent != null)
            newDepth = parent.Depth + 1;

        ParentId = parent?.Id;
        Depth = newDepth;
        Path = newPathResult.Value;
        return Result.Success<int, Errors>(newDepth);
    }


    public static Result<Department, Error> CreateParent(
        DepartmentName name,
        Identifier identifier,
        IEnumerable<DepartmentLocation> departmentLocations,
        DepartmentId? departmentId = null!)
    {
       var departmentLocationsList = departmentLocations.ToList();

       if(departmentLocationsList.Count == 0)
           return Error.Validation("department.location", "Department locations must contain at least one location");

       var path = Path.CreateParent(identifier);

       return new Department(
           departmentId ?? new DepartmentId(Guid.NewGuid()),
           name,
           identifier,
           path,
           0,
           null,
           departmentLocationsList);
    }

    public static Result<Department, Error> CreateChild(
        DepartmentName name,
        Identifier identifier,
        Department parent,
        IEnumerable<DepartmentLocation> departmentLocations,
        DepartmentId? departmentId = null!)
    {
        var departmentLocationsList = departmentLocations.ToList();

        if(departmentLocationsList.Count == 0)
            return Error.Validation("department.location", "Department locations must contain at least one location");

        var path = parent.Path.CreateChild(identifier);

        return new Department(
            departmentId ?? new DepartmentId(Guid.NewGuid()),
            name,
            identifier,
            path,
            parent.Depth + 1,
            parent.Id,
            departmentLocationsList);
    }

    public static Result<Department, Error> CreateInactiveParent(
        DepartmentName name,
        Identifier identifier,
        IEnumerable<DepartmentLocation> departmentLocations,
        DepartmentId? departmentId = null!)
    {
        var departmentLocationsList = departmentLocations.ToList();

        if(departmentLocationsList.Count == 0)
            return Error.Validation("department.location", "Department locations must contain at least one location");

        var path = Path.CreateParent(identifier);

        return new Department(
            departmentId ?? new DepartmentId(Guid.NewGuid()),
            name,
            identifier,
            path,
            0,
            null,
            false,
            departmentLocationsList);
    }

    public static Result<Department, Error> CreateInactiveChild(
        DepartmentName name,
        Identifier identifier,
        Department parent,
        IEnumerable<DepartmentLocation> departmentLocations,
        DepartmentId? departmentId = null!)
    {
        var departmentLocationsList = departmentLocations.ToList();

        if(departmentLocationsList.Count == 0)
            return Error.Validation("department.location", "Department locations must contain at least one location");

        var path = parent.Path.CreateChild(identifier);

        return new Department(
            departmentId ?? new DepartmentId(Guid.NewGuid()),
            name,
            identifier,
            path,
            parent.Depth + 1,
            parent.Id,
            false,
            departmentLocationsList);
    }

    public UnitResult<Error> Deactivate()
    {
        if(!IsActive)
            return Error.Failure("department.error.delete", "department is already not active");

        Path = Path.CreateDeleted(Identifier.Value, Path);
        Identifier = Identifier.CreateDeleted(Identifier).Value;
        IsActive = false;
        DeletedAt = DateTime.UtcNow;

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> Activate()
    {
        if(IsActive)
            return Error.Failure("department.error.delete", "department is already active");

        Path = Path.CreateRestored(Identifier.Value, Path);
        Identifier = Identifier.CreateRestored(Identifier).Value;
        IsActive = true;
        DeletedAt = null;

        return UnitResult.Success<Error>();
    }
}