using RacingCalendar.Data.Models;
using RacingCalendar.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RacingCalendar.Services.Core.Contracts
{
    public interface ICircuitService
    {
        Task<IEnumerable<CircuitViewModel>> GetAllAsync();
        Task<CircuitViewModel?> GetByIdAsync(int id);
        Task AddAsync(CircuitViewModel circuit);
        Task UpdateAsync(CircuitViewModel circuit);
        Task DeleteAsync(int id);
        Task<PaginatedList<CircuitViewModel>> GetAllPaginatedAsync(int pageIndex, int pageSize);

    }
}
