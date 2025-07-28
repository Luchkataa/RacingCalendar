using Microsoft.EntityFrameworkCore;
using RacingCalendar.Data.Models;
using RacingCalendar.Services.Core.Contracts;
using RacingCalendar.ViewModels;

namespace RacingCalendar.Services.Core
{
    public class CircuitService : ICircuitService
    {
        private readonly RacingCalendarDbContext _context;

        public CircuitService(RacingCalendarDbContext context)
        {
            _context = context;
        }

        private static CircuitViewModel ToViewModel(Circuit circuit) => new CircuitViewModel
        {
            Id = circuit.Id,
            Name = circuit.Name,
            Country = circuit.Country,
            LayoutImageUrl = circuit.LayoutImageUrl
        };

        private static Circuit ToEntity(CircuitViewModel vm) => new Circuit
        {
            Id = vm.Id,
            Name = vm.Name,
            Country = vm.Country,
            LayoutImageUrl = vm.LayoutImageUrl
        };

        public async Task<IEnumerable<CircuitViewModel>> GetAllAsync()
        {
            var circuits = await _context.Circuits.ToListAsync();
            return circuits.Select(c => ToViewModel(c));
        }

        public async Task<CircuitViewModel?> GetByIdAsync(int id)
        {
            var circuit = await _context.Circuits.FindAsync(id);
            if (circuit == null) return null;

            return ToViewModel(circuit);
        }

        public async Task AddAsync(CircuitViewModel vm)
        {
            var circuit = ToEntity(vm);
            _context.Circuits.Add(circuit);
            await _context.SaveChangesAsync();

            vm.Id = circuit.Id;
        }

        public async Task UpdateAsync(CircuitViewModel vm)
        {
            var circuit = await _context.Circuits.FindAsync(vm.Id);
            if (circuit == null) return;

            circuit.Name = vm.Name;
            circuit.Country = vm.Country;
            circuit.LayoutImageUrl = vm.LayoutImageUrl;

            _context.Circuits.Update(circuit);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var circuit = await _context.Circuits.FindAsync(id);
            if (circuit == null) return;

            _context.Circuits.Remove(circuit);
            await _context.SaveChangesAsync();
        }

        public async Task<PaginatedList<CircuitViewModel>> GetAllPaginatedAsync(int pageIndex, int pageSize, string? searchTerm = null)
        {
            var query = _context.Circuits.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c => c.Name.Contains(searchTerm) || c.Country.Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();

            var circuits = await query
                .OrderBy(c => c.Name)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var items = circuits.Select(c => new CircuitViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Country = c.Country,
                LayoutImageUrl = c.LayoutImageUrl
            });

            return new PaginatedList<CircuitViewModel>(items, totalCount, pageIndex, pageSize);
        }

        public async Task<PaginatedList<CircuitViewModel>> GetAllFilteredAsync(int pageIndex, int pageSize, string? country = null, string? sortOrder = null)
        {
            var query = _context.Circuits.AsQueryable();

            if (!string.IsNullOrWhiteSpace(country))
            {
                query = query.Where(c => c.Country == country);
            }

            query = sortOrder switch
            {
                "name_desc" => query.OrderByDescending(c => c.Name),
                _ => query.OrderBy(c => c.Name),
            };

            var totalCount = await query.CountAsync();

            var circuits = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var items = circuits.Select(c => new CircuitViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Country = c.Country,
                LayoutImageUrl = c.LayoutImageUrl
            });

            return new PaginatedList<CircuitViewModel>(items, totalCount, pageIndex, pageSize);
        }


        public async Task<List<string>> GetDistinctCountriesAsync()
        {
            return await _context.Circuits
                .Select(c => c.Country)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
        }
    }
}
