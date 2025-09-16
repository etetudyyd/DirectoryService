using CSharpFunctionalExtensions;
using DevQuestions.Domain.Shared;
using DevQuestions.Domain.ValueObjects.DepartmentVO;

namespace DevQuestions.Domain.Entities;

public class Department
{
    // ef
    private Department()
    {
    }

    private readonly List<Department> _children;
    private readonly List<DepartmentLocation> _location;
    private readonly List<DepartmentPosition> _position;

    public Guid Id { get; private set; }

    public DepartmentName Name { get; private set; }

    public Identifier Identifier { get; private set; }

    public Guid? ParentId { get; private set; }

    public IReadOnlyList<Department> Children => _children;

    public IReadOnlyList<DepartmentLocation> DepartmentLocations => _location;

    public IReadOnlyList<DepartmentPosition> DepartmentPositions => _position;

    public DepartmentPath Path { get; private set; }

    public short Depth { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public int ChildrenCount => _children.Count;

    private Department(
        Guid departmentId,
        DepartmentName name,
        DepartmentPath path,
        Identifier identifier,
        short depth,
        DateTime createdAt,
        DateTime updatedAt,
        Guid? parentId,
        List<DepartmentLocation> locations,
        List<DepartmentPosition> positions,
        List<Department> children,
        bool isActive)
    {
        Id = departmentId;
        Name = name;
        Path = path;
        Identifier = identifier;
        ParentId = parentId;
        _children = children;
        _location = locations;
        _position = positions;
        Depth = depth;
        IsActive = isActive;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
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

    public static Result<Department, Error> Create(
        DepartmentName name,
        DepartmentPath path,
        Identifier identifier,
        short depth,
        DateTime createdAt,
        DateTime updatedAt,
        Guid? parentId,
        List<DepartmentLocation> locations,
        List<DepartmentPosition> positions,
        List<Department> children,
        bool isActive = true)
    {
        if (locations == null)
            return Error.Validation(null, "Locations is required");

        if (positions == null)
            return Error.Validation(null, "Positions is required");

        string updatedPath = path.Value + "." + identifier.Value;

        if (updatedPath == null)
        {
            return Error.Validation(
                null,
                "Path is required");
        }

        var departmentPath = DepartmentPath.Create(updatedPath);

        short updatedDepth = GetDepth(departmentPath.Value);

        return new Department(
            Guid.NewGuid(),
            name,
            departmentPath.Value,
            identifier,
            updatedDepth,
            createdAt,
            updatedAt,
            parentId,
            locations,
            positions,
            children,
            isActive);
    }

    private static short GetDepth(DepartmentPath path)
    {
        return (short)(path.Value.Split('.').Length + 1);
    }

}