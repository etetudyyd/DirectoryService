using DirectoryService.Application.Features.Departments.Commands.DeleteInactiveDepartments;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Postgresql.BackgroundServices;

public class InactiveDepartmentsCleanerBackgroundService : BackgroundService
{
    private readonly ILogger<InactiveDepartmentsCleanerBackgroundService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly TimeSpan _period;

    public InactiveDepartmentsCleanerBackgroundService(
        ILogger<InactiveDepartmentsCleanerBackgroundService> logger,
        IConfiguration configuration,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _period = TimeSpan.FromHours(configuration
            .GetValue("InactiveDepartmentsCleanerInterval:IntervalHours", 24));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(_period);

        _logger.LogInformation(
            "InactiveDepartmentsCleaner Background Service started. Running every {Hours} hours", 
            _period.TotalHours);

        await DoWorkAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            await DoWorkAsync(stoppingToken);
        }
    }

    private async Task DoWorkAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Starting deletion of inactive entities at {Time}", DateTime.UtcNow);

            using var scope = _serviceScopeFactory.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<DeleteInactiveDepartmentsHandler>();
            var command = new DeleteInactiveDepartmentsCommand();

            var result = await handler.Handle(command, stoppingToken);
            if (result.IsFailure)
            {
                throw new Exception(result.Error.ToString());
            }

            _logger.LogInformation("Inactive departments deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete inactive departments");
        }
    }
}