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

public class TeamServiceTests
{
    private RacingCalendarDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<RacingCalendarDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new RacingCalendarDbContext(options);
    }

    private TeamService GetService(RacingCalendarDbContext context)
    {
        return new TeamService(context);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllTeams()
    {
        var context = GetDbContext();
        context.Teams.Add(new Team { Id = 1, Name = "Mercedes", Country = "Germany" });
        context.Teams.Add(new Team { Id = 2, Name = "Red Bull", Country = "Austria" });
        await context.SaveChangesAsync();

        var service = GetService(context);
        var result = await service.GetAllAsync();

        Assert.Equal(2, result.Count());
        Assert.Contains(result, t => t.Name == "Mercedes");
        Assert.Contains(result, t => t.Country == "Austria");
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsTeam_WhenExists()
    {
        var context = GetDbContext();
        context.Teams.Add(new Team { Id = 1, Name = "Ferrari", Country = "Italy" });
        await context.SaveChangesAsync();

        var service = GetService(context);
        var result = await service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Ferrari", result.Name);
        Assert.Equal("Italy", result.Country);
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
    public async Task AddAsync_AddsTeam()
    {
        var context = GetDbContext();
        var service = GetService(context);

        var vm = new TeamViewModel
        {
            Name = "Aston Martin",
            Country = "UK"
        };

        await service.AddAsync(vm);

        var team = await context.Teams.FirstOrDefaultAsync(t => t.Name == "Aston Martin");
        Assert.NotNull(team);
        Assert.Equal("UK", team.Country);
        Assert.Equal(vm.Id, team.Id);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesTeam_WhenExists()
    {
        var context = GetDbContext();
        var team = new Team { Id = 1, Name = "Old Name", Country = "Old Country" };
        context.Teams.Add(team);
        await context.SaveChangesAsync();

        var service = GetService(context);
        var vm = new TeamViewModel
        {
            Id = 1,
            Name = "New Name",
            Country = "New Country"
        };

        await service.UpdateAsync(vm);

        var updated = await context.Teams.FindAsync(1);
        Assert.Equal("New Name", updated.Name);
        Assert.Equal("New Country", updated.Country);
    }

    [Fact]
    public async Task UpdateAsync_DoesNothing_WhenTeamNotExists()
    {
        var context = GetDbContext();
        var service = GetService(context);

        var vm = new TeamViewModel { Id = 99, Name = "Nope", Country = "Nope" };
        await service.UpdateAsync(vm);

        Assert.Empty(context.Teams);
    }

    [Fact]
    public async Task DeleteAsync_RemovesTeam_WhenExists()
    {
        var context = GetDbContext();
        context.Teams.Add(new Team { Id = 1, Name = "To Delete", Country = "Desc" });
        await context.SaveChangesAsync();

        var service = GetService(context);
        await service.DeleteAsync(1);

        Assert.Empty(context.Teams);
    }

    [Fact]
    public async Task DeleteAsync_DoesNothing_WhenNotExists()
    {
        var context = GetDbContext();
        var service = GetService(context);

        await service.DeleteAsync(99);

        Assert.Empty(context.Teams);
    }

    [Fact]
    public async Task GetTeamsSelectListAsync_ReturnsTeamsList()
    {
        var context = GetDbContext();
        context.Teams.Add(new Team { Id = 1, Name = "Mercedes", Country = "Germany" });
        context.Teams.Add(new Team { Id = 2, Name = "Red Bull", Country = "Austria" });
        await context.SaveChangesAsync();

        var service = GetService(context);
        var result = await service.GetTeamsSelectListAsync();

        Assert.Equal(2, result.Count());
        Assert.Contains(result, t => t.Text == "Mercedes");
        Assert.Contains(result, t => t.Value == "2");
    }

    [Fact]
    public async Task GetPaginatedTeamsAsync_ReturnsPaginatedTeams()
    {
        var context = GetDbContext();
        for (int i = 1; i <= 10; i++)
        {
            context.Teams.Add(new Team { Id = i, Name = $"Team {i}", Country = $"Country {i}" });
        }
        await context.SaveChangesAsync();

        var service = GetService(context);
        var result = await service.GetPaginatedTeamsAsync(null, null, 2, 3);

        Assert.Equal(3, result.Items.Count());
        Assert.Equal(4, result.TotalPages);
        Assert.Equal(2, result.PageIndex);
    }

    [Fact]
    public async Task GetPaginatedTeamsAsync_FiltersBySearchTerm()
    {
        var context = GetDbContext();
        context.Teams.Add(new Team { Id = 1, Name = "Alpha", Country = "Aland" });
        context.Teams.Add(new Team { Id = 2, Name = "Beta", Country = "Belgium" });
        await context.SaveChangesAsync();

        var service = GetService(context);
        var result = await service.GetPaginatedTeamsAsync("Alpha", null, 1, 10);

        Assert.Single(result.Items);
        Assert.Equal("Alpha", result.Items.First().Name);
    }

    [Fact]
    public async Task GetPaginatedTeamsAsync_SortsByNameDesc()
    {
        var context = GetDbContext();
        context.Teams.Add(new Team { Id = 1, Name = "Alpha", Country = "Aland" });
        context.Teams.Add(new Team { Id = 2, Name = "Beta", Country = "Belgium" });
        await context.SaveChangesAsync();

        var service = GetService(context);
        var result = await service.GetPaginatedTeamsAsync(null, "name_desc", 1, 10);

        Assert.Equal(2, result.Items.Count());
        Assert.Equal("Beta", result.Items.First().Name);
    }

    [Fact]
    public async Task GetPaginatedTeamsAsync_SortsByCountryAsc()
    {
        var context = GetDbContext();
        context.Teams.Add(new Team { Id = 1, Name = "Alpha", Country = "Bland" });
        context.Teams.Add(new Team { Id = 2, Name = "Beta", Country = "Aland" });
        await context.SaveChangesAsync();

        var service = GetService(context);
        var result = await service.GetPaginatedTeamsAsync(null, "country_asc", 1, 10);

        Assert.Equal(2, result.Items.Count());
        Assert.Equal("Aland", result.Items.First().Country);
    }

    [Fact]
    public async Task GetPaginatedTeamsAsync_SortsByCountryDesc()
    {
        var context = GetDbContext();
        context.Teams.Add(new Team { Id = 1, Name = "Alpha", Country = "Aland" });
        context.Teams.Add(new Team { Id = 2, Name = "Beta", Country = "Bland" });
        await context.SaveChangesAsync();

        var service = GetService(context);
        var result = await service.GetPaginatedTeamsAsync(null, "country_desc", 1, 10);

        Assert.Equal(2, result.Items.Count());
        Assert.Equal("Bland", result.Items.First().Country);
    }
}