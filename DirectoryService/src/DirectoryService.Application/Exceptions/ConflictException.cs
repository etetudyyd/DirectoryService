using System.Text.Json;
using DevQuestions.Domain.Shared;

namespace DirectoryService.Application.Exceptions;

public class ConflictException : Exception
{
    protected ConflictException(Error[] errors) 
        : base(JsonSerializer.Serialize(errors))
    {
    }
}