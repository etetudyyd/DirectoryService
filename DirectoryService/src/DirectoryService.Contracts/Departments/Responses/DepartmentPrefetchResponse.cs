namespace DirectoryService.Contracts.Departments.Responses;

public sealed record DepartmentPrefetchResponse
{
    /*public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Identifier { get; init; } = string.Empty;

    public string Path { get; init; } = string.Empty;

    public Guid? ParentId { get; init; }

    public bool IsActive { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime UpdateAt { get; init; }

    public List<DepartmentPrefetchResponse> Children { get; init; } = [];

    public bool HasMoreChildren { get; init; }*/


        public DepartmentPrefetchResponse(
            Guid id,
            string name,
            string identifier,
            DateTime createdAt,
            bool hasMoreChildren,
            List<ChildDepartmentDto> childs)
        {
            Id = id;
            Name = name;
            Identifier = identifier;
            CreatedAt = createdAt;
            HasMoreChildren = hasMoreChildren;
            Childs = childs;
        }

        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Identifier { get; init; }
        public DateTime CreatedAt { get; init; }
        public bool HasMoreChildren { get; init; }
        public List<ChildDepartmentDto> Childs { get; init; } = [];
    }

public sealed record ChildDepartmentDto
{
    public ChildDepartmentDto(
        Guid id,
        string name,
        string identifier,
        DateTime createdAt)
    {
        Id = id;
        Name = name;
        Identifier = identifier;
        CreatedAt = createdAt;
    }

    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Identifier { get; init; }
    public DateTime CreatedAt { get; init; }
}
