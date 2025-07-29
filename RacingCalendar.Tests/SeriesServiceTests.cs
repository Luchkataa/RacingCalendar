using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RacingCalendar.Data;
using RacingCalendar.Data.Models;
using RacingCalendar.Services.Core;
using RacingCalendar.ViewModels;
using Xunit;

public class SeriesServiceTests
{
    private RacingCalendarDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<RacingCalendarDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new RacingCalendarDbContext(options);
    }

    private SeriesService GetService(RacingCalendarDbContext context)
    {
        return new SeriesService(context);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllSeries()
    {
        var context = GetDbContext();
        context.Series.Add(new Series { Id = 1, Name = "F1", Description = "Formula 1" });
        context.Series.Add(new Series { Id = 2, Name = "F2", Description = "Formula 2" });
        await context.SaveChangesAsync();

        var service = GetService(context);
        var result = await service.GetAllAsync();

        Assert.Equal(2, result.Count());
        Assert.Contains(result, s => s.Name == "F1");
        Assert.Contains(result, s => s.Description == "Formula 2");
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsSeries_WhenExists()
    {
        var context = GetDbContext();
        context.Series.Add(new Series { Id = 1, Name = "F1", Description = "Formula 1" });
        await context.SaveChangesAsync();

        var service = GetService(context);
        var result = await service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("F1", result.Name);
        Assert.Equal("Formula 1", result.Description);
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
    public async Task AddAsync_AddsSeries()
    {
        var context = GetDbContext();
        var service = GetService(context);

        var vm = new SeriesViewModel
        {
            Name = "F3",
            Description = "Formula 3"
        };

        await service.AddAsync(vm);

        var series = await context.Series.FirstOrDefaultAsync(s => s.Name == "F3");
        Assert.NotNull(series);
        Assert.Equal("Formula 3", series.Description);
        Assert.Equal(vm.Id, series.Id);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesSeries_WhenExists()
    {
        var context = GetDbContext();
        var series = new Series { Id = 1, Name = "Old Name", Description = "Old Desc" };
        context.Series.Add(series);
        await context.SaveChangesAsync();

        var service = GetService(context);
        var vm = new SeriesViewModel
        {
            Id = 1,
            Name = "New Name",
            Description = "New Desc"
        };

        await service.UpdateAsync(vm);

        var updated = await context.Series.FindAsync(1);
        Assert.Equal("New Name", updated.Name);
        Assert.Equal("New Desc", updated.Description);
    }

    [Fact]
    public async Task UpdateAsync_DoesNothing_WhenSeriesNotExists()
    {
        var context = GetDbContext();
        var service = GetService(context);

        var vm = new SeriesViewModel { Id = 99, Name = "Nope", Description = "Nope" };
        await service.UpdateAsync(vm);

        Assert.Empty(context.Series);
    }

    [Fact]
    public async Task DeleteAsync_RemovesSeries_WhenExists()
    {
        var context = GetDbContext();
        context.Series.Add(new Series { Id = 1, Name = "To Delete", Description = "Desc" });
        await context.SaveChangesAsync();

        var service = GetService(context);
        await service.DeleteAsync(1);

        Assert.Empty(context.Series);
    }

    [Fact]
    public async Task DeleteAsync_DoesNothing_WhenNotExists()
    {
        var context = GetDbContext();
        var service = GetService(context);

        await service.DeleteAsync(99);

        Assert.Empty(context.Series);
    }

    [Fact]
    public async Task GetPaginatedAsync_ReturnsPaginatedSeries()
    {
        var context = GetDbContext();
        for (int i = 1; i <= 10; i++)
        {
            context.Series.Add(new Series { Id = i, Name = $"Series {i}", Description = $"Desc {i}" });
        }
        await context.SaveChangesAsync();

        var service = GetService(context);
        var result = await service.GetPaginatedAsync(null, null, 2, 3);

        Assert.Equal(3, result.Items.Count());
        Assert.Equal(4, result.TotalPages);
        Assert.Equal(2, result.PageIndex);
    }

    [Fact]
    public async Task GetPaginatedAsync_FiltersBySearchTerm()
    {
        var context = GetDbContext();
        context.Series.Add(new Series { Id = 1, Name = "Alpha", Description = "Desc" });
        context.Series.Add(new Series { Id = 2, Name = "Beta", Description = "Desc" });
        await context.SaveChangesAsync();

        var service = GetService(context);
        var result = await service.GetPaginatedAsync("Alpha", null, 1, 10);

        Assert.Single(result.Items);
        Assert.Equal("Alpha", result.Items.First().Name);
    }

    [Fact]
    public async Task GetPaginatedAsync_SortsByNameDesc()
    {
        var context = GetDbContext();
        context.Series.Add(new Series { Id = 1, Name = "Alpha", Description = "Desc" });
        context.Series.Add(new Series { Id = 2, Name = "Beta", Description = "Desc" });
        await context.SaveChangesAsync();

        var service = GetService(context);
        var result = await service.GetPaginatedAsync(null, "name_desc", 1, 10);

        Assert.Equal(2, result.Items.Count());
        Assert.Equal("Beta", result.Items.First().Name);
    }

    [Fact]
    public async Task GetRacesBySeriesIdAsync_ReturnsRacesForSeries()
    {
        var context = GetDbContext();
        var series = new Series { Id = 1, Name = "F1", Description = "Formula 1" };
        var circuit = new Circuit { Id = 1, Name = "Monza", Country = "Italy" };
        context.Series.Add(series);
        context.Circuits.Add(circuit);
        await context.SaveChangesAsync();

        context.Races.Add(new Race { Id = 1, Name = "Italian GP", Date = DateTime.Today, SeriesId = series.Id, CircuitId = circuit.Id });
        context.Races.Add(new Race { Id = 2, Name = "British GP", Date = DateTime.Today, SeriesId = series.Id, CircuitId = circuit.Id });
        await context.SaveChangesAsync();

        var service = GetService(context);
        var result = await service.GetRacesBySeriesIdAsync(series.Id);

        Assert.Equal(2, result.Count());
        Assert.All(result, r => Assert.Equal(series.Id, r.SeriesId));
        Assert.All(result, r => Assert.Equal("Monza", r.CircuitName));
    }

    [Fact]
    public async Task GetRacesBySeriesIdAsync_ReturnsEmpty_WhenNoRaces()
    {
        var context = GetDbContext();
        var series = new Series { Id = 1, Name = "F1", Description = "Formula 1" };
        context.Series.Add(series);
        await context.SaveChangesAsync();

        var service = GetService(context);
        var result = await service.GetRacesBySeriesIdAsync(series.Id);

        Assert.Empty(result);
    }
}