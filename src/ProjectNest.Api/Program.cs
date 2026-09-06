using Microsoft.EntityFrameworkCore;
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

app.Run();