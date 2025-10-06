using DevQuestions.Web;
using DevQuestions.Web.Middlewares;
using DirectoryService.Application;
using DirectoryService.Infrastructure.Postgresql;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.Debug()
    .WriteTo.Seq(builder.Configuration.GetConnectionString("Seq")
                        ?? throw new ArgumentNullException("Seq NULL!"))
    .CreateLogger();

Log.Information("Test message to Seq");

builder.Services.AddWeb();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddSerilog();

var app = builder.Build();

app.UseExceptionMiddleware();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "DevQuestions"));
}

app.UseSerilogRequestLogging();

app.MapControllers();

app.Run();

namespace DevQuestions.Web
{
public partial class Program { }

}
