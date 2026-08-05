using Core.Abstractions;
using FluentValidation;
using Framework.Endpoints;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService;

/// <summary>
/// DependencyInjection - service for adding scopes in DI business-logic services.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        var assembly = typeof(DependencyInjection).Assembly;

        serviceCollection.AddEndpoints(assembly);

        serviceCollection.Scan(scan => scan.FromAssemblies(assembly)
            .AddClasses(classes => classes
                .AssignableToAny(
                    typeof(ICommandHandler<,>),
                    typeof(ICommandHandler<>),
                    typeof(IQueryHandler<,>)))
            .AsSelfWithInterfaces()
            .WithScopedLifetime());

        return serviceCollection;
    }
}
