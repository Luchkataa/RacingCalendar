using RacingCalendar.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RacingCalendar.Services.Core.Contracts
{
    public interface IRaceService
    {
        Task<IEnumerable<RaceViewModel>> GetAllAsync();
        Task<RaceViewModel?> GetByIdAsync(int id);
        Task AddAsync(RaceViewModel model);
        Task UpdateAsync(RaceViewModel model);
        Task DeleteAsync(int id);
        Task<PaginatedList<RaceViewModel>> GetAllPaginatedAsync(int pageIndex, int pageSize, string? searchTerm = null, int? seriesId = null);

        Task<IEnumerable<SelectListItem>> GetSeriesSelectListAsync();
        Task<IEnumerable<SelectListItem>> GetCircuitsSelectListAsync();
        Task<IEnumerable<RaceViewModel>> GetUpcomingRacesAsync();
        Task<IEnumerable<RaceViewModel>> GetPastRacesAsync();
    }
}
