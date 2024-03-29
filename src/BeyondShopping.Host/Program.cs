using BeyondShopping.Application;
using BeyondShopping.Host.Middlewares;
using BeyondShopping.Infrastructure;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

string? envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
if (!string.IsNullOrEmpty(envName))
{
    builder.Configuration.AddJsonFile($"appsettings.{envName}.json", optional: true, reloadOnChange: true);
    if (envName == "Development")
    {
        builder.Configuration.AddUserSecrets<Program>();
    }
}

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo()
    {
        Title = builder.Configuration.GetValue<string>("SwaggerTitle") ?? string.Empty,
        Description = builder.Configuration.GetValue<string>("SwaggerDescription") ?? string.Empty
    });

    c.ExampleFilters();

    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration.GetValue<string>("PostgreConnection")!);

var app = builder.Build();

// TODO: modify is Production environment name changes
if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseExceptionHandlingMiddleware();

app.Run();
