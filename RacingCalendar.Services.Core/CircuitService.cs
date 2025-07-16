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
    }
}
