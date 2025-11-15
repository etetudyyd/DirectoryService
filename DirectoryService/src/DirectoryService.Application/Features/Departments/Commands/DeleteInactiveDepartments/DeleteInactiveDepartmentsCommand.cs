using System.Windows.Input;
using ICommand = DirectoryService.Application.Abstractions.Commands.ICommand;

namespace DirectoryService.Application.Features.Departments.Commands.DeleteInactiveDepartments;

public record DeleteInactiveDepartmentsCommand : ICommand;