using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Moq;
using RacingCalendar.Data;
using RacingCalendar.Data.Models;
using RacingCalendar.Services.Core;
using RacingCalendar.ViewModels;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

public class CircuitServiceTests
{
    private List<Circuit> GetFakeCircuits() => new List<Circuit>
    {
        new Circuit { Id = 1, Name = "Monza", Country = "Italy", LayoutImageUrl = "http://img.com/monza" },
        new Circuit { Id = 2, Name = "Silverstone", Country = "UK", LayoutImageUrl = "http://img.com/silverstone" },
    };

    private DbContextOptions<RacingCalendarDbContext> GetDbOptions()
    {
        return new DbContextOptionsBuilder<RacingCalendarDbContext>()
            .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllCircuits()
    {
        var options = GetDbOptions();
        using (var context = new RacingCalendarDbContext(options))
        {
            context.Circuits.AddRange(GetFakeCircuits());
            context.SaveChanges();
        }

        using (var context = new RacingCalendarDbContext(options))
        {
            var service = new CircuitService(context);

            var result = await service.GetAllAsync();

            Assert.Equal(2, result.Count());
            Assert.Contains(result, c => c.Name == "Monza");
            Assert.Contains(result, c => c.Name == "Silverstone");
        }
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsCorrectCircuit()
    {
        var options = GetDbOptions();
        using (var context = new RacingCalendarDbContext(options))
        {
            context.Circuits.AddRange(GetFakeCircuits());
            context.SaveChanges();
        }

        using (var context = new RacingCalendarDbContext(options))
        {
            var service = new CircuitService(context);

            var result = await service.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("Monza", result!.Name);
        }
    }

    [Fact]
    public async Task AddAsync_AddsCircuitAndSetsId()
    {
        var options = GetDbOptions();
        var viewModel = new CircuitViewModel
        {
            Name = "Spa-Francorchamps",
            Country = "Belgium",
            LayoutImageUrl = "http://img.com/spa"
        };

        using (var context = new RacingCalendarDbContext(options))
        {
            var service = new CircuitService(context);

            await service.AddAsync(viewModel);
        }

        using (var context = new RacingCalendarDbContext(options))
        {
            var added = await context.Circuits.FirstOrDefaultAsync(c => c.Name == "Spa-Francorchamps");

            Assert.NotNull(added);
            Assert.Equal("Belgium", added!.Country);
        }
    }

    [Fact]
    public async Task DeleteAsync_RemovesCircuit_WhenExists()
    {
        var options = GetDbOptions();
        using (var context = new RacingCalendarDbContext(options))
        {
            context.Circuits.AddRange(GetFakeCircuits());
            context.SaveChanges();
        }

        using (var context = new RacingCalendarDbContext(options))
        {
            var service = new CircuitService(context);
            await service.DeleteAsync(1);
        }

        using (var context = new RacingCalendarDbContext(options))
        {
            var deleted = await context.Circuits.FindAsync(1);
            Assert.Null(deleted);
            Assert.Single(context.Circuits);
        }
    }
    [Fact]
    public async Task UpdateAsync_UpdatesCircuit_WhenExists()
    {
        var options = GetDbOptions();

        using (var context = new RacingCalendarDbContext(options))
        {
            context.Circuits.Add(new Circuit { Id = 1, Name = "OldName", Country = "OldCountry", LayoutImageUrl = "old.png" });
            context.SaveChanges();
        }

        using (var context = new RacingCalendarDbContext(options))
        {
            var service = new CircuitService(context);

            var updatedVm = new CircuitViewModel
            {
                Id = 1,
                Name = "NewName",
                Country = "NewCountry",
                LayoutImageUrl = "new.png"
            };

            await service.UpdateAsync(updatedVm);
        }

        using (var context = new RacingCalendarDbContext(options))
        {
            var circuit = await context.Circuits.FindAsync(1);

            Assert.NotNull(circuit);
            Assert.Equal("NewName", circuit!.Name);
            Assert.Equal("NewCountry", circuit.Country);
            Assert.Equal("new.png", circuit.LayoutImageUrl);
        }
    }

    [Fact]
    public async Task GetAllPaginatedAsync_ReturnsCorrectPage()
    {
        var options = new DbContextOptionsBuilder<RacingCalendarDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new RacingCalendarDbContext(options);

        var circuits = Enumerable.Range(1, 10).Select(i => new Circuit
        {
            Name = $"Circuit {i}",
            Country = i % 2 == 0 ? "USA" : "Italy",
            LayoutImageUrl = $"url{i}"
        }).ToList();

        context.Circuits.AddRange(circuits);
        await context.SaveChangesAsync();

        var service = new CircuitService(context);

        var result = await service.GetAllPaginatedAsync(pageIndex: 2, pageSize: 3, searchTerm: null);

        Assert.Equal(3, result.Items.Count());
        Assert.Equal(4, result.TotalPages);

        Assert.Equal("Circuit 3", result.Items.First().Name);
        Assert.Equal("Circuit 5", result.Items.Last().Name);
    }



    [Fact]
    public async Task GetAllPaginatedAsync_AppliesSearchTerm()
    {
        var options = new DbContextOptionsBuilder<RacingCalendarDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new RacingCalendarDbContext(options);

        var circuits = new[]
        {
        new Circuit { Name = "Silverstone", Country = "UK", LayoutImageUrl = "url1" },
        new Circuit { Name = "Monza", Country = "Italy", LayoutImageUrl = "url2" },
        new Circuit { Name = "Suzuka", Country = "Japan", LayoutImageUrl = "url3" },
        new Circuit { Name = "Imola", Country = "Italy", LayoutImageUrl = "url4" }
        };

        context.Circuits.AddRange(circuits);
        await context.SaveChangesAsync();

        var service = new CircuitService(context);

        var result = await service.GetAllPaginatedAsync(pageIndex: 1, pageSize: 10, searchTerm: "italy");

        Assert.Equal(2, result.Items.Count());

        Assert.All(result.Items, c =>
            Assert.True(
                c.Country.Contains("italy", StringComparison.OrdinalIgnoreCase) ||
                c.Name.Contains("italy", StringComparison.OrdinalIgnoreCase)
            )
        );
        Assert.Equal(1, result.TotalPages);
    }

    [Fact]
    public async Task GetAllFilteredAsync_FiltersAndSortsCorrectly()
    {
        var options = GetDbOptions();

        using (var context = new RacingCalendarDbContext(options))
        {
            context.Circuits.AddRange(
                new Circuit { Name = "Bahrain", Country = "Bahrain" },
                new Circuit { Name = "Imola", Country = "Italy" },
                new Circuit { Name = "Monza", Country = "Italy" }
            );

            context.SaveChanges();
        }

        using (var context = new RacingCalendarDbContext(options))
        {
            var service = new CircuitService(context);

            var result = await service.GetAllFilteredAsync(1, 10, country: "Italy", sortOrder: "name_desc");

            Assert.Equal(2, result.Items.Count());
            Assert.Equal("Monza", result.Items.First().Name);
        }
    }

    [Fact]
    public async Task GetDistinctCountriesAsync_ReturnsSortedDistinctCountries()
    {
        var options = GetDbOptions();

        using (var context = new RacingCalendarDbContext(options))
        {
            context.Circuits.AddRange(
                new Circuit { Name = "A", Country = "Germany" },
                new Circuit { Name = "B", Country = "Italy" },
                new Circuit { Name = "C", Country = "Germany" }
            );

            context.SaveChanges();
        }

        using (var context = new RacingCalendarDbContext(options))
        {
            var service = new CircuitService(context);

            var countries = await service.GetDistinctCountriesAsync();

            Assert.Equal(2, countries.Count);
            Assert.Equal(new List<string> { "Germany", "Italy" }, countries);
        }
    }
}
