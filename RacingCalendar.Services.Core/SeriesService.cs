using Microsoft.EntityFrameworkCore;
using RacingCalendar.Data;
using RacingCalendar.Data.Models;
using RacingCalendar.Services.Core.Contracts;
using RacingCalendar.ViewModels;

namespace RacingCalendar.Services.Core
{
    public class SeriesService : ISeriesService
    {
        private readonly RacingCalendarDbContext _context;

        public SeriesService(RacingCalendarDbContext context)
        {
            _context = context;
        }

        private static SeriesViewModel ToViewModel(Series series) => new SeriesViewModel
        {
            Id = series.Id,
            Name = series.Name,
            Description = series.Description
        };

        private static Series ToEntity(SeriesViewModel vm) => new Series
        {
            Id = vm.Id,
            Name = vm.Name,
            Description = vm.Description
        };

        public async Task<IEnumerable<SeriesViewModel>> GetAllAsync()
        {
            var seriesList = await _context.Series.ToListAsync();
            return seriesList.Select(ToViewModel);
        }

        public async Task<SeriesViewModel?> GetByIdAsync(int id)
        {
            var series = await _context.Series.FindAsync(id);
            return series == null ? null : ToViewModel(series);
        }

        public async Task AddAsync(SeriesViewModel model)
        {
            var entity = ToEntity(model);
            _context.Series.Add(entity);
            await _context.SaveChangesAsync();
            model.Id = entity.Id;
        }

        public async Task UpdateAsync(SeriesViewModel model)
        {
            var entity = await _context.Series.FindAsync(model.Id);
            if (entity == null) return;

            entity.Name = model.Name;
            entity.Description = model.Description;

            _context.Series.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Series.FindAsync(id);
            if (entity == null) return;

            _context.Series.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
