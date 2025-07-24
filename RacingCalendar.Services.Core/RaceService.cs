using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RacingCalendar.Data;
using RacingCalendar.Data.Models;
using RacingCalendar.Services.Core.Contracts;
using RacingCalendar.ViewModels;

namespace RacingCalendar.Services.Core
{
    public class RaceService : IRaceService
    {
        private readonly RacingCalendarDbContext _context;

        public RaceService(RacingCalendarDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RaceViewModel>> GetAllAsync()
        {
            var races = await _context.Races
                .Include(r => r.Series)
                .Include(r => r.Circuit)
                .ToListAsync();

            return races.Select(r => new RaceViewModel
            {
                Id = r.Id,
                Name = r.Name,
                Date = r.Date,
                SeriesId = r.SeriesId,
                SeriesName = r.Series.Name,
                CircuitId = r.CircuitId,
                CircuitName = r.Circuit.Name
            });
        }

        public async Task<RaceViewModel?> GetByIdAsync(int id)
        {
            var race = await _context.Races
                .Include(r => r.Series)
                .Include(r => r.Circuit)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (race == null) return null;

            return new RaceViewModel
            {
                Id = race.Id,
                Name = race.Name,
                Date = race.Date,
                SeriesId = race.SeriesId,
                SeriesName = race.Series?.Name,
                CircuitId = race.CircuitId,
                CircuitName = race.Circuit?.Name
            };
        }

        public async Task AddAsync(RaceViewModel model)
        {
            var race = new Race
            {
                Name = model.Name,
                Date = model.Date,
                SeriesId = model.SeriesId,
                CircuitId = model.CircuitId
            };

            _context.Races.Add(race);
            await _context.SaveChangesAsync();

            model.Id = race.Id;
        }

        public async Task UpdateAsync(RaceViewModel model)
        {
            var race = await _context.Races.FindAsync(model.Id);
            if (race == null) return;

            race.Name = model.Name;
            race.Date = model.Date;
            race.SeriesId = model.SeriesId;
            race.CircuitId = model.CircuitId;

            _context.Races.Update(race);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var race = await _context.Races.FindAsync(id);
            if (race == null) return;

            _context.Races.Remove(race);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<SelectListItem>> GetSeriesSelectListAsync()
        {
            return await _context.Series
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                }).ToListAsync();
        }

        public async Task<IEnumerable<SelectListItem>> GetCircuitsSelectListAsync()
        {
            return await _context.Circuits
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToListAsync();
        }
        public async Task<PaginatedList<RaceViewModel>> GetAllPaginatedAsync(int pageIndex, int pageSize, string? searchTerm = null, int? seriesId = null)
        {
            var query = _context.Races
                .Include(r => r.Series)
                .Include(r => r.Circuit)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(r =>
                    r.Name.Contains(searchTerm) ||
                    r.Series.Name.Contains(searchTerm) ||
                    r.Circuit.Name.Contains(searchTerm));
            }

            if (seriesId.HasValue)
            {
                query = query.Where(r => r.SeriesId == seriesId.Value);
            }

            var totalCount = await query.CountAsync();

            var races = await query
                .OrderBy(r => r.Date)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var items = races.Select(r => new RaceViewModel
            {
                Id = r.Id,
                Name = r.Name,
                Date = r.Date,
                SeriesId = r.SeriesId,
                CircuitId = r.CircuitId,
                SeriesName = r.Series.Name,
                CircuitName = r.Circuit.Name
            });

            return new PaginatedList<RaceViewModel>(items, totalCount, pageIndex, pageSize);
        }
    }
}
