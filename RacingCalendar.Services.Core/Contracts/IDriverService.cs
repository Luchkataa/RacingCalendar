using RacingCalendar.ViewModels;
using RacingCalendar.ViewModels.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RacingCalendar.Services.Core.Contracts
{
    public interface IDriverService
    {
        Task<IEnumerable<DriverViewModel>> GetAllAsync();
        Task<DriverViewModel?> GetByIdAsync(int id);
        Task AddAsync(DriverViewModel driver);
        Task UpdateAsync(DriverViewModel driver);
        Task DeleteAsync(int id);
        Task<PaginatedList<DriverViewModel>> GetAllPaginatedAsync(int pageIndex, int pageSize, string? searchTerm = null);
        Task<DriverListViewModel> GetDriversAsync(string? searchTerm, string? sortOrder, int page, int pageSize);
    }
}
