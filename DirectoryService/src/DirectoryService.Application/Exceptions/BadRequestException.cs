using System.Text.Json;
using DevQuestions.Domain.Shared;

namespace DirectoryService.Application.Exceptions;

public class BadRequestException : Exception
{
    protected BadRequestException(Error[] errors) 
        : base(JsonSerializer.Serialize(errors))
    {
    }
}