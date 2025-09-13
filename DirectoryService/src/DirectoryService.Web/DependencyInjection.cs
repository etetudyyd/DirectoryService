namespace DevQuestions.Web;

public static class DependencyInjection
{
    public static IServiceCollection AddWeb(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddControllers();
        serviceCollection.AddOpenApi();
        return serviceCollection;
    }
}