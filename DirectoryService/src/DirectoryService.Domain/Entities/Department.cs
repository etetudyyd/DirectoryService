using CSharpFunctionalExtensions;
using DevQuestions.Domain.Shared;
using DevQuestions.Domain.ValueObjects.DepartmentVO;
using Path = DevQuestions.Domain.ValueObjects.DepartmentVO.Path;

namespace DevQuestions.Domain.Entities;

public sealed class Department
{
    // ef
    private Department() { }

    private readonly List<Department> _childrenDepartments = [];
    private readonly List<DepartmentLocation> _departmentLocations = [];
    private readonly List<DepartmentPosition> _departmentPositions = [];

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

    public IReadOnlyList<Department> ChildrenDepartments => _childrenDepartments;

    public IReadOnlyList<DepartmentLocation> DepartmentLocations => _departmentLocations;

    public IReadOnlyList<DepartmentPosition> DepartmentPositions => _departmentPositions;

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
        _departmentLocations = departmentLocations.ToList();
    }

    public Result Rename(DepartmentName name, Identifier identifier)
    {
        if(string.IsNullOrWhiteSpace(name.Value) || name.Value.Length > 150)
            return Result.Failure<Department>("Name is required");

        Name = name;
        Identifier = identifier;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success(this);
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
}