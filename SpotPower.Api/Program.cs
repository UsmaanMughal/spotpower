using Microsoft.EntityFrameworkCore;
using SpotPower.Api.Endpoints;
using SpotPower.Infrastructure;
using SpotPower.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSpotPowerInfrastructure(builder.Configuration);

var app = builder.Build();

// Apply pending EF Core migrations on startup so a fresh clone / restart just works.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SpotPowerDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapAuctionPricesEndpoints();

app.Run();
