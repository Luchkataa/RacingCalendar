using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RacingCalendar.Data;
using RacingCalendar.Data.Models;
using RacingCalendar.Services.Core;
using RacingCalendar.ViewModels;
using RacingCalendar.ViewModels.Filters;
using Xunit;

public class DriverServiceTests
{
    private RacingCalendarDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<RacingCalendarDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new RacingCalendarDbContext(options);
    }

    private DriverService GetService(RacingCalendarDbContext context)
    {
        return new DriverService(context);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllDrivers()
    {
        var context = GetDbContext();
        var team1 = new Team { Id = 1, Name = "Mercedes", Country = "Germany" };
        var team2 = new Team { Id = 2, Name = "Red Bull", Country = "Austria" };
        context.Teams.AddRange(team1, team2);
        await context.SaveChangesAsync();

        context.Drivers.Add(new Driver { Id = 1, FullName = "Lewis Hamilton", Nationality = "British", TeamId = team1.Id });
        context.Drivers.Add(new Driver { Id = 2, FullName = "Max Verstappen", Nationality = "Dutch", TeamId = team2.Id });
        await context.SaveChangesAsync();

        var service = GetService(context);
        var result = await service.GetAllAsync();

        Assert.Equal(2, result.Count());
        Assert.Contains(result, d => d.FullName == "Lewis Hamilton");
        Assert.Contains(result, d => d.TeamName == "Red Bull");
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsDriver_WhenExists()
    {
        var context = GetDbContext();
        var team = new Team { Id = 3, Name = "Ferrari", Country = "Italy" };
        context.Teams.Add(team);
        await context.SaveChangesAsync();

        var driver = new Driver { Id = 1, FullName = "Charles Leclerc", Nationality = "Monégasque", TeamId = team.Id };
        context.Drivers.Add(driver);
        await context.SaveChangesAsync();

        var service = GetService(context);
        var result = await service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Charles Leclerc", result.FullName);
        Assert.Equal("Ferrari", result.TeamName);
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
    public async Task AddAsync_AddsDriver()
    {
        var context = GetDbContext();
        var team = new Team { Id = 4, Name = "Aston Martin", Country = "UK" };
        context.Teams.Add(team);
        await context.SaveChangesAsync();

        var service = GetService(context);

        var vm = new DriverViewModel
        {
            FullName = "Fernando Alonso",
            Nationality = "Spanish",
            TeamId = team.Id,
            DriverImageUrl = "url"
        };

        await service.AddAsync(vm);

        var driver = await context.Drivers.FirstOrDefaultAsync(d => d.FullName == "Fernando Alonso");
        Assert.NotNull(driver);
        Assert.Equal("Spanish", driver.Nationality);
        Assert.Equal(vm.Id, driver.Id);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesDriver_WhenExists()
    {
        var context = GetDbContext();
        var team1 = new Team { Id = 1, Name = "Old Team", Country = "Old Country" };
        var team2 = new Team { Id = 2, Name = "New Team", Country = "New Country" };
        context.Teams.AddRange(team1, team2);
        await context.SaveChangesAsync();

        var driver = new Driver { Id = 1, FullName = "Old Name", Nationality = "Old", TeamId = team1.Id, DriverImageUrl = "old" };
        context.Drivers.Add(driver);
        await context.SaveChangesAsync();

        var service = GetService(context);
        var vm = new DriverViewModel
        {
            Id = 1,
            FullName = "New Name",
            Nationality = "New",
            TeamId = team2.Id,
            DriverImageUrl = "new"
        };

        await service.UpdateAsync(vm);

        var updated = await context.Drivers.FindAsync(1);
        Assert.Equal("New Name", updated.FullName);
        Assert.Equal("New", updated.Nationality);
        Assert.Equal(team2.Id, updated.TeamId);
        Assert.Equal("new", updated.DriverImageUrl);
    }

    [Fact]
    public async Task UpdateAsync_DoesNothing_WhenDriverNotExists()
    {
        var context = GetDbContext();
        var service = GetService(context);

        var vm = new DriverViewModel { Id = 99, FullName = "Nobody", Nationality = "None" };
        await service.UpdateAsync(vm);

        Assert.Empty(context.Drivers);
    }

    [Fact]
    public async Task DeleteAsync_RemovesDriver_WhenExists()
    {
        var context = GetDbContext();
        var team = new Team { Id = 1, Name = "Mercedes", Country = "Germany" };
        context.Teams.Add(team);
        await context.SaveChangesAsync();

        var driver = new Driver { Id = 1, FullName = "To Delete", Nationality = "British", TeamId = team.Id };
        context.Drivers.Add(driver);
        await context.SaveChangesAsync();

        var service = GetService(context);
        await service.DeleteAsync(1);

        Assert.Empty(context.Drivers);
    }

    [Fact]
    public async Task DeleteAsync_DoesNothing_WhenNotExists()
    {
        var context = GetDbContext();
        var service = GetService(context);

        await service.DeleteAsync(99);

        Assert.Empty(context.Drivers);
    }

    [Fact]
    public async Task GetAllPaginatedAsync_ReturnsPaginatedDrivers()
    {
        var context = GetDbContext();
        for (int i = 1; i <= 10; i++)
        {
            var team = new Team { Id = i, Name = $"Team {i}", Country = $"Country {i}" };
            context.Teams.Add(team);
            await context.SaveChangesAsync();
            context.Drivers.Add(new Driver { Id = i, FullName = $"Driver {i}", Nationality = $"Nationality {i}", TeamId = team.Id });
        }
        await context.SaveChangesAsync();

        var service = GetService(context);
        var result = await service.GetAllPaginatedAsync(2, 3);

        Assert.Equal(3, result.Items.Count());
        Assert.Equal(4, result.TotalPages); // 10 / 3 = 3.33 => 4 pages
        Assert.Equal(2, result.PageIndex);
    }

    [Fact]
    public async Task GetAllPaginatedAsync_FiltersBySearchTerm()
    {
        var context = GetDbContext();
        var teamA = new Team { Id = 1, Name = "A-Team", Country = "Aland" };
        var teamB = new Team { Id = 2, Name = "B-Team", Country = "Belgium" };
        context.Teams.AddRange(teamA, teamB);
        await context.SaveChangesAsync();

        context.Drivers.Add(new Driver { Id = 1, FullName = "Alpha", Nationality = "A", TeamId = teamA.Id });
        context.Drivers.Add(new Driver { Id = 2, FullName = "Beta", Nationality = "B", TeamId = teamB.Id });
        await context.SaveChangesAsync();

        var service = GetService(context);
        var result = await service.GetAllPaginatedAsync(1, 10, "Alpha");

        Assert.Single(result.Items);
        Assert.Equal("Alpha", result.Items.First().FullName);
    }

    [Fact]
    public async Task GetDriversAsync_ReturnsFilteredAndSortedDrivers()
    {
        var context = GetDbContext();
        var teamA = new Team { Id = 1, Name = "TeamA", Country = "CountryA" };
        var teamB = new Team { Id = 2, Name = "TeamB", Country = "CountryB" };
        context.Teams.AddRange(teamA, teamB);
        await context.SaveChangesAsync();

        var driver1 = new Driver { Id = 1, FullName = "Zeta", Nationality = "CountryA", TeamId = teamA.Id };
        var driver2 = new Driver { Id = 2, FullName = "Alpha", Nationality = "CountryB", TeamId = teamB.Id };
        context.Drivers.AddRange(driver1, driver2);
        await context.SaveChangesAsync();

        var service = GetService(context);
        var result = await service.GetDriversAsync("Alpha", "name_desc", 1, 10);

        Assert.Single(result.Drivers);
        Assert.Equal("Alpha", result.Drivers.First().FullName);
        Assert.Equal(1, result.CurrentPage);
        Assert.Equal(1, result.TotalPages);
    }

    [Fact]
    public async Task GetDriversByTeamAsync_ReturnsDriversForTeam()
    {
        var context = GetDbContext();
        var team = new Team { Id = 1, Name = "TeamX", Country = "Xland" };
        context.Teams.Add(team);
        await context.SaveChangesAsync();

        var driver1 = new Driver { Id = 1, FullName = "Driver1", Nationality = "X", DriverImageUrl = "img1", TeamId = team.Id };
        var driver2 = new Driver { Id = 2, FullName = "Driver2", Nationality = "Y", DriverImageUrl = "img2", TeamId = team.Id };
        context.Drivers.AddRange(driver1, driver2);
        await context.SaveChangesAsync();

        var service = GetService(context);
        var result = await service.GetDriversByTeamAsync(1);

        Assert.NotNull(result);
        Assert.Equal("TeamX", result.TeamName);
        Assert.Equal(2, result.Drivers.Count);
        Assert.Contains(result.Drivers, d => d.FullName == "Driver1");
    }

    [Fact]
    public async Task GetDriversByTeamAsync_ReturnsNull_WhenTeamNotExists()
    {
        var context = GetDbContext();
        var service = GetService(context);

        var result = await service.GetDriversByTeamAsync(99);

        Assert.Null(result);
    }
}