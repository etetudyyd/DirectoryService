using DirectoryService.Infrastructure.Postgresql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddScoped<DirectoryServiceDbContext>(
    _ => new DirectoryServiceDbContext(
    builder.Configuration
        .GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "DevQuestions"));
}

app.MapControllers();

app.Run();