using Microsoft.EntityFrameworkCore;
using ProjectNest.Api.Endpoints;
using ProjectNest.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<ProjectNestDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("ProjectNestDatabase")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "ProjectNest API v1");
    });
}

app.MapStoreEndpoints();

app.Run();