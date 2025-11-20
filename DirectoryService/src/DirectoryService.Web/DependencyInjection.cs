using DevQuestions.Web.Controllers.Locations;

namespace DevQuestions.Web;

/// <summary>
/// DependencyInjection - service for adding scopes in DI web services.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddWeb(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddControllers()
            .AddApplicationPart(typeof(LocationsController).Assembly);
        serviceCollection.AddOpenApi();
        return serviceCollection;
    }
}