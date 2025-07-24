using Microsoft.EntityFrameworkCore;
using RacingCalendar.Data.Models;
using RacingCalendar.Services.Core.Contracts;
using RacingCalendar.ViewModels;


namespace RacingCalendar.Services.Core
{
    public class DriverService : IDriverService
    {
        private readonly RacingCalendarDbContext _context;

        public DriverService(RacingCalendarDbContext context)
        {
            _context = context;
        }

        private static DriverViewModel ToViewModel(Driver driver) => new DriverViewModel
        {
            Id = driver.Id,
            FullName = driver.FullName,
            Nationality = driver.Nationality,
            TeamId = driver.TeamId,
            TeamName = driver.Team?.Name,
            DriverImageUrl = driver.DriverImageUrl
        };

        private static Driver ToEntity(DriverViewModel vm) => new Driver
        {
            Id = vm.Id,
            FullName = vm.FullName,
            Nationality = vm.Nationality,
            TeamId = vm.TeamId,
            DriverImageUrl = vm.DriverImageUrl
        };

        public async Task<IEnumerable<DriverViewModel>> GetAllAsync()
        {
            var drivers = await _context.Drivers.Include(d => d.Team).ToListAsync();
            return drivers.Select(ToViewModel);
        }

        public async Task<DriverViewModel?> GetByIdAsync(int id)
        {
            var driver = await _context.Drivers.Include(d => d.Team).FirstOrDefaultAsync(d => d.Id == id);
            return driver == null ? null : ToViewModel(driver);
        }

        public async Task AddAsync(DriverViewModel vm)
        {
            var driver = ToEntity(vm);
            _context.Drivers.Add(driver);
            await _context.SaveChangesAsync();

            vm.Id = driver.Id;
        }

        public async Task UpdateAsync(DriverViewModel vm)
        {
            var driver = await _context.Drivers.FindAsync(vm.Id);
            if (driver == null) return;

            driver.FullName = vm.FullName;
            driver.Nationality = vm.Nationality;
            driver.TeamId = vm.TeamId;
            driver.DriverImageUrl = vm.DriverImageUrl;

            _context.Drivers.Update(driver);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var driver = await _context.Drivers.FindAsync(id);
            if (driver == null) return;

            _context.Drivers.Remove(driver);
            await _context.SaveChangesAsync();
        }

        public async Task<PaginatedList<DriverViewModel>> GetAllPaginatedAsync(int pageIndex, int pageSize, string? searchTerm = null)
        {
            var query = _context.Drivers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(d => d.FullName.Contains(searchTerm) || d.Team.Name.Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();

            var drivers = await query
                .Include(d => d.Team)
                .OrderBy(d => d.FullName)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var items = drivers.Select(d => new DriverViewModel
            {
                Id = d.Id,
                FullName = d.FullName,
                TeamName = d.Team?.Name,
                DriverImageUrl = d.DriverImageUrl
            }); 

            return new PaginatedList<DriverViewModel>(items, totalCount, pageIndex, pageSize);
        }

    }
}
