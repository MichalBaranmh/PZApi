using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using PZApi.Models;
using PZApi.Controllers;
using System;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var configuration = builder.Configuration;
builder.Services.AddDbContext<CarServiceConext>(options =>
options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
    {
        var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        var db = serviceScope.ServiceProvider.GetRequiredService<CarServiceConext>().Database;

        logger.LogInformation("Migrating database...");

        while (!db.CanConnect())
        {
            logger.LogInformation("Database not ready yet; waiting...");
            Thread.Sleep(1000);
        }

        try
        {
            serviceScope.ServiceProvider.GetRequiredService<CarServiceConext>().Database.Migrate();
            logger.LogInformation("Database migrated successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database.");
        }
    }
}

//Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
