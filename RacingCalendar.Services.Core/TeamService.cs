using Microsoft.EntityFrameworkCore;
using RacingCalendar.Data;
using RacingCalendar.Data.Models;
using RacingCalendar.Services.Core.Contracts;
using RacingCalendar.ViewModels;

namespace RacingCalendar.Services.Core
{
    public class TeamService : ITeamService
    {
        private readonly RacingCalendarDbContext _context;

        public TeamService(RacingCalendarDbContext context)
        {
            _context = context;
        }

        private static TeamViewModel ToViewModel(Team team) => new TeamViewModel
        {
            Id = team.Id,
            Name = team.Name,
            Country = team.Country
        };

        private static Team ToEntity(TeamViewModel vm) => new Team
        {
            Id = vm.Id,
            Name = vm.Name,
            Country = vm.Country
        };

        public async Task<IEnumerable<TeamViewModel>> GetAllAsync()
        {
            var teams = await _context.Teams.ToListAsync();
            return teams.Select(ToViewModel);
        }

        public async Task<TeamViewModel?> GetByIdAsync(int id)
        {
            var team = await _context.Teams.FindAsync(id);
            return team == null ? null : ToViewModel(team);
        }

        public async Task AddAsync(TeamViewModel vm)
        {
            var team = ToEntity(vm);
            _context.Teams.Add(team);
            await _context.SaveChangesAsync();
            vm.Id = team.Id;
        }

        public async Task UpdateAsync(TeamViewModel vm)
        {
            var team = await _context.Teams.FindAsync(vm.Id);
            if (team == null) return;

            team.Name = vm.Name;
            team.Country = vm.Country;

            _context.Teams.Update(team);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team == null) return;

            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
        }
    }
}
