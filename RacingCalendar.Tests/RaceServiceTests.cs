using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RacingCalendar.Data;
using RacingCalendar.Data.Models;
using RacingCalendar.Services.Core;
using RacingCalendar.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Xunit;

public class RaceServiceTests
{
    private RacingCalendarDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<RacingCalendarDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new RacingCalendarDbContext(options);
    }

    private RaceService GetService(RacingCalendarDbContext context)
    {
        return new RaceService(context);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllRaces()
    {
        var context = GetDbContext();
        var series = new Series { Id = 1, Name = "F1", Description = "Formula 1 World Championship" };
        var circuit = new Circuit { Id = 1, Name = "Monza", Country = "Italy" };
        context.Series.Add(series);
        context.Circuits.Add(circuit);
        await context.SaveChangesAsync();

        context.Races.Add(new Race { Id = 1, Name = "Italian GP", Date = DateTime.Today, SeriesId = series.Id, CircuitId = circuit.Id });
        await context.SaveChangesAsync();

        var service = GetService(context);
        var result = await service.GetAllAsync();

        Assert.Single(result);
        Assert.Equal("Italian GP", result.First().Name);
        Assert.Equal("F1", result.First().SeriesName);
        Assert.Equal("Monza", result.First().CircuitName);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsRace_WhenExists()
    {
        var context = GetDbContext();
        var series = new Series { Id = 2, Name = "F2", Description = "Formula 2 Championship" };
        var circuit = new Circuit { Id = 2, Name = "Silverstone", Country = "UK" };
        context.Series.Add(series);
        context.Circuits.Add(circuit);
        await context.SaveChangesAsync();

        var race = new Race { Id = 2, Name = "British GP", Date = DateTime.Today, SeriesId = series.Id, CircuitId = circuit.Id };
        context.Races.Add(race);
        await context.SaveChangesAsync();

        var service = GetService(context);
        var result = await service.GetByIdAsync(2);

        Assert.NotNull(result);
        Assert.Equal("British GP", result.Name);
        Assert.Equal("F2", result.SeriesName);
        Assert.Equal("Silverstone", result.CircuitName);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
    {
        var context = GetDbContext();
        var service = GetService(context);

        var result = await service.GetByIdAsync(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_AddsRace()
    {
        var context = GetDbContext();
        var series = new Series { Id = 3, Name = "F3", Description = "Formula 3 Championship" };
        var circuit = new Circuit { Id = 3, Name = "Spa", Country = "Belgium" };
        context.Series.Add(series);
        context.Circuits.Add(circuit);
        await context.SaveChangesAsync();

        var service = GetService(context);

        var vm = new RaceViewModel
        {
            Name = "Belgian GP",
            Date = DateTime.Today,
            SeriesId = series.Id,
            CircuitId = circuit.Id
        };

        await service.AddAsync(vm);

        var race = await context.Races.FirstOrDefaultAsync(r => r.Name == "Belgian GP");
        Assert.NotNull(race);
        Assert.Equal(series.Id, race.SeriesId);
        Assert.Equal(circuit.Id, race.CircuitId);
        Assert.Equal(vm.Id, race.Id);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesRace_WhenExists()
    {
        var context = GetDbContext();
        var series1 = new Series { Id = 4, Name = "Old Series", Description = "Old Desc" };
        var series2 = new Series { Id = 5, Name = "New Series", Description = "New Desc" };
        var circuit1 = new Circuit { Id = 4, Name = "Old Circuit", Country = "Oldland" };
        var circuit2 = new Circuit { Id = 5, Name = "New Circuit", Country = "Newland" };
        context.Series.AddRange(series1, series2);
        context.Circuits.AddRange(circuit1, circuit2);
        await context.SaveChangesAsync();

        var race = new Race { Id = 4, Name = "Old Race", Date = DateTime.Today, SeriesId = series1.Id, CircuitId = circuit1.Id };
        context.Races.Add(race);
        await context.SaveChangesAsync();

        var service = GetService(context);
        var vm = new RaceViewModel
        {
            Id = 4,
            Name = "New Race",
            Date = DateTime.Today.AddDays(1),
            SeriesId = series2.Id,
            CircuitId = circuit2.Id
        };

        await service.UpdateAsync(vm);

        var updated = await context.Races.FindAsync(4);
        Assert.Equal("New Race", updated.Name);
        Assert.Equal(series2.Id, updated.SeriesId);
        Assert.Equal(circuit2.Id, updated.CircuitId);
        Assert.Equal(DateTime.Today.AddDays(1), updated.Date);
    }

    [Fact]
    public async Task UpdateAsync_DoesNothing_WhenRaceNotExists()
    {
        var context = GetDbContext();
        var service = GetService(context);

        var vm = new RaceViewModel { Id = 99, Name = "No Race", Date = DateTime.Today, SeriesId = 1, CircuitId = 1 };
        await service.UpdateAsync(vm);

        Assert.Empty(context.Races);
    }

    [Fact]
    public async Task DeleteAsync_RemovesRace_WhenExists()
    {
        var context = GetDbContext();
        var series = new Series { Id = 6, Name = "Delete Series", Description = "Delete Desc" };
        var circuit = new Circuit { Id = 6, Name = "Delete Circuit", Country = "DeleteLand" };
        context.Series.Add(series);
        context.Circuits.Add(circuit);
        await context.SaveChangesAsync();

        var race = new Race { Id = 6, Name = "To Delete", Date = DateTime.Today, SeriesId = series.Id, CircuitId = circuit.Id };
        context.Races.Add(race);
        await context.SaveChangesAsync();

        var service = GetService(context);
        await service.DeleteAsync(6);

        Assert.Empty(context.Races);
    }

    [Fact]
    public async Task DeleteAsync_DoesNothing_WhenNotExists()
    {
        var context = GetDbContext();
        var service = GetService(context);

        await service.DeleteAsync(99);

        Assert.Empty(context.Races);
    }

    [Fact]
    public async Task GetSeriesSelectListAsync_ReturnsSeriesList()
    {
        var context = GetDbContext();
        context.Series.Add(new Series { Id = 1, Name = "F1", Description = "F1 Desc" });
        context.Series.Add(new Series { Id = 2, Name = "F2", Description = "F2 Desc" });
        await context.SaveChangesAsync();

        var service = GetService(context);
        var result = await service.GetSeriesSelectListAsync();

        Assert.Equal(2, result.Count());
        Assert.Contains(result, s => s.Text == "F1");
        Assert.Contains(result, s => s.Value == "2");
    }

    [Fact]
    public async Task GetCircuitsSelectListAsync_ReturnsCircuitList()
    {
        var context = GetDbContext();
        context.Circuits.Add(new Circuit { Id = 1, Name = "Monza", Country = "Italy" });
        context.Circuits.Add(new Circuit { Id = 2, Name = "Spa", Country = "Belgium" });
        await context.SaveChangesAsync();

        var service = GetService(context);
        var result = await service.GetCircuitsSelectListAsync();

        Assert.Equal(2, result.Count());
        Assert.Contains(result, c => c.Text == "Monza");
        Assert.Contains(result, c => c.Value == "2");
    }

    [Fact]
    public async Task GetAllPaginatedAsync_ReturnsPaginatedRaces()
    {
        var context = GetDbContext();
        var series = new Series { Id = 1, Name = "F1", Description = "F1 Desc" };
        var circuit = new Circuit { Id = 1, Name = "Monza", Country = "Italy" };
        context.Series.Add(series);
        context.Circuits.Add(circuit);
        await context.SaveChangesAsync();

        for (int i = 1; i <= 10; i++)
        {
            context.Races.Add(new Race
            {
                Id = i,
                Name = $"Race {i}",
                Date = DateTime.Today.AddDays(i),
                SeriesId = series.Id,
                CircuitId = circuit.Id
            });
        }
        await context.SaveChangesAsync();

        var service = GetService(context);
        var result = await service.GetAllPaginatedAsync(2, 3);

        Assert.Equal(3, result.Items.Count());
        Assert.Equal(4, result.TotalPages);
        Assert.Equal(2, result.PageIndex);
    }

    [Fact]
    public async Task GetAllPaginatedAsync_FiltersBySearchTerm_AndSeriesId()
    {
        var context = GetDbContext();
        var series1 = new Series { Id = 1, Name = "F1", Description = "F1 Desc" };
        var series2 = new Series { Id = 2, Name = "F2", Description = "F2 Desc" };
        var circuit = new Circuit { Id = 1, Name = "Monza", Country = "Italy" };
        context.Series.AddRange(series1, series2);
        context.Circuits.Add(circuit);
        await context.SaveChangesAsync();

        context.Races.Add(new Race { Id = 1, Name = "Alpha Race", Date = DateTime.Today, SeriesId = series1.Id, CircuitId = circuit.Id });
        context.Races.Add(new Race { Id = 2, Name = "Beta Race", Date = DateTime.Today, SeriesId = series2.Id, CircuitId = circuit.Id });
        await context.SaveChangesAsync();

        var service = GetService(context);
        var result = await service.GetAllPaginatedAsync(1, 10, "Alpha", series1.Id);

        Assert.Single(result.Items);
        Assert.Equal("Alpha Race", result.Items.First().Name);
    }

    [Fact]
    public async Task GetUpcomingRacesAsync_ReturnsFutureRaces()
    {
        var context = GetDbContext();
        var series = new Series { Id = 1, Name = "F1", Description = "F1 Desc" };
        var circuit = new Circuit { Id = 1, Name = "Monza", Country = "Italy" };
        context.Series.Add(series);
        context.Circuits.Add(circuit);
        await context.SaveChangesAsync();

        context.Races.Add(new Race { Id = 1, Name = "Future Race", Date = DateTime.Now.AddDays(5), SeriesId = series.Id, CircuitId = circuit.Id });
        context.Races.Add(new Race { Id = 2, Name = "Past Race", Date = DateTime.Now.AddDays(-5), SeriesId = series.Id, CircuitId = circuit.Id });
        await context.SaveChangesAsync();

        var service = GetService(context);
        var result = await service.GetUpcomingRacesAsync();

        Assert.Single(result);
        Assert.Equal("Future Race", result.First().Name);
    }

    [Fact]
    public async Task GetPastRacesAsync_ReturnsPastRaces()
    {
        var context = GetDbContext();
        var series = new Series { Id = 1, Name = "F1", Description = "F1 Desc" };
        var circuit = new Circuit { Id = 1, Name = "Monza", Country = "Italy" };
        context.Series.Add(series);
        context.Circuits.Add(circuit);
        await context.SaveChangesAsync();

        context.Races.Add(new Race { Id = 1, Name = "Future Race", Date = DateTime.Now.AddDays(5), SeriesId = series.Id, CircuitId = circuit.Id });
        context.Races.Add(new Race { Id = 2, Name = "Past Race", Date = DateTime.Now.AddDays(-5), SeriesId = series.Id, CircuitId = circuit.Id });
        await context.SaveChangesAsync();

        var service = GetService(context);
        var result = await service.GetPastRacesAsync();

        Assert.Single(result);
        Assert.Equal("Past Race", result.First().Name);
    }
}