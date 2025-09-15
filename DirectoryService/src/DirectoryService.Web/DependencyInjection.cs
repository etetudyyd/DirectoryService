using DirectoryService.Presentation.Controllers.Locations;

namespace DevQuestions.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddWeb(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddControllers()
            .AddApplicationPart(typeof(LocationController).Assembly);
        serviceCollection.AddOpenApi();
        return serviceCollection;
    }
}