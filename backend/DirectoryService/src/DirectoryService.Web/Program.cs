using DevQuestions.Web;
using DevQuestions.Web.Middlewares;
using DirectoryService.Application;
using DirectoryService.Infrastructure.Postgresql;
using DirectoryService.Infrastructure.Postgresql.Database;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "DirectoryService",
        Version = "v1",
    });
});

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.Debug()
    .WriteTo.Seq(builder.Configuration.GetConnectionString("Seq")
                        ?? throw new ArgumentNullException("Seq NULL!"))
    .CreateLogger();

Log.Information("Test message to Seq");

builder.Services.AddWeb();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddDistributedCache(builder.Configuration);
builder.Services.AddSerilog();

var app = builder.Build();

app.UseExceptionMiddleware();
app.UseSerilogRequestLogging();

app.MapOpenApi();

app.UseSwagger();
app.UseSwaggerUI(options => options.SwaggerEndpoint(
    "/openapi/v1.json", "DirectoryService"));

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DirectoryServiceDbContext>();
    db.Database.Migrate();
}

app.Run();

namespace DevQuestions.Web
{
public partial class Program { }
}
