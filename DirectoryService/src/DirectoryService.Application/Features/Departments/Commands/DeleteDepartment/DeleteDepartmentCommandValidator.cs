using FluentValidation;

namespace DirectoryService.Application.Features.Departments.Commands.DeleteDepartment;

public class DeleteDepartmentCommandValidator : AbstractValidator<DeleteDepartmentCommand>
{
    public DeleteDepartmentCommandValidator() { }
}