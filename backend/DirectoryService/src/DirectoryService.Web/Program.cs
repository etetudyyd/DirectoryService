using DirectoryService;
using DirectoryService.Database;
using DirectoryService.HttpCommunication;
using Framework.Endpoints;
using Framework.Middlewares;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.Debug()
    .WriteTo.Seq(builder.Configuration.GetConnectionString("Seq")!)
    .CreateLogger();

builder.Services.AddWeb();
builder.Services.AddFileServiceHttpCommunication(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddDistributedCache(builder.Configuration);
builder.Services.AddSerilog();

string[]? corsOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        policy
            .WithOrigins(corsOrigins ?? [])
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});


WebApplication app = builder.Build();

app.UseCors("DefaultCorsPolicy");

app.UseExceptionMiddleware();
app.UseSerilogRequestLogging();

app.MapOpenApi();

app.MapScalarApiReference();

app.MapControllers();
app.MapEndpoints();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DirectoryServiceDbContext>();
    db.Database.Migrate();
}

app.Run();

namespace DirectoryService
{
public partial class Program { }
}
