using RacingCalendar.ViewModels;

namespace RacingCalendar.Services.Core.Contracts
{
    public interface ICircuitService
    {
        Task<IEnumerable<CircuitViewModel>> GetAllAsync();
        Task<CircuitViewModel?> GetByIdAsync(int id);
        Task AddAsync(CircuitViewModel circuit);
        Task UpdateAsync(CircuitViewModel circuit);
        Task DeleteAsync(int id);
        Task<PaginatedList<CircuitViewModel>> GetAllPaginatedAsync(int pageIndex, int pageSize, string? searchTerm = null);
        Task<PaginatedList<CircuitViewModel>> GetAllFilteredAsync(int pageIndex, int pageSize, string? country = null, string? sortOrder = null);
        Task<List<string>> GetDistinctCountriesAsync();
    }
}
