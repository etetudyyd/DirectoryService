using DirectoryService.Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<CreateLocationHandler>();
        serviceCollection.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        return serviceCollection;
    }
}