using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Zadanie5;
using Zadanie5.DataTransferObjects;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("Sql")));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var apiGroup = app.MapGroup("/api");

apiGroup.MapGet("/trips", async (AppDbContext dbContext) =>
{
    var countries = dbContext.Countries.Select(x => x.IdTrips);
    var result = await dbContext.Trips
        .Include(x=> x.IdCountries)
        .Include(x => x.ClientTrips)
        .ThenInclude(x => x.IdClientNavigation)
        .Select(x => new TripDto
        {
            Name = x.Name,
            DateFrom = x.DateFrom,
            DateTo = x.DateTo,
            Description = x.Description,
            MaxPeople = x.MaxPeople,
            Countries = x.IdCountries.Select(x => new CountryDto { Name = x.Name }).ToList(),
            Clients = x.ClientTrips.Select(x => x.IdClientNavigation).Select(x => new ClientDto
            {
                FirstName = x.FirstName,
                LastName = x.LastName
            }).ToList()
        }).ToListAsync();

    return result;
})
.WithName("GetTrips")
.WithOpenApi();

apiGroup.MapDelete("/clients/{idClient:int}", async (AppDbContext dbContext, int idClient) =>
{
    var client = await dbContext.Clients.Include(x => x.ClientTrips).FirstOrDefaultAsync(x => x.IdClient == idClient);
    if (client is null || !client.ClientTrips.Any())
    {
        return Results.BadRequest("Klient posiada przypisane wycieczki.");
    }

    await dbContext.SaveChangesAsync();
    return Results.Ok();
})
.WithName("DeleteCLient")
.WithOpenApi();

apiGroup.MapPost("/trips/{idTrip:int}/clients", async (AppDbContext dbContext, int idTrip, AddTripClientDto requestModel) =>
{
    var client = await dbContext.Clients.Include(x => x.ClientTrips).FirstOrDefaultAsync(x => x.Pesel == requestModel.Pesel);
    if (client is null || !client.ClientTrips.Any())
    {
        return Results.BadRequest("Klient o takim numerze pesel istnieje.");
    }

    var isTripExist = client.ClientTrips.Any(x => x.IdTrip == idTrip);
    if (!isTripExist)
    {
        return Results.BadRequest("Klient jest już zapisany na taką wycieczke.");
    }

    var trip = await dbContext.Trips.FirstOrDefaultAsync(x => x.IdTrip == idTrip);
    if (trip is null)
    {
        return Results.BadRequest("Taka wycieczka nie istnieje.");
    }

    await dbContext.SaveChangesAsync();
    return Results.Ok();
})
.WithName("AddTrip")
.WithOpenApi();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}