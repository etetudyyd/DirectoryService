using DevQuestions.Web;
using DirectoryService.Application;
using DirectoryService.Infrastructure.Postgresql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWeb();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "DevQuestions"));
}

app.MapControllers();

app.Run();