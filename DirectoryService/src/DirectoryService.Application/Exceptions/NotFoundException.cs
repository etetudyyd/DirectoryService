using System.Text.Json;
using DevQuestions.Domain.Shared;

namespace DirectoryService.Application.Exceptions;

public class NotFoundException : Exception
{
    protected NotFoundException(Error[] errors) 
        : base(JsonSerializer.Serialize(errors))
    {
    }
}