using RacingCalendar.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RacingCalendar.Services.Core.Contracts
{
    public interface ISeriesService
    {
        Task<IEnumerable<SeriesViewModel>> GetAllAsync();
        Task<SeriesViewModel?> GetByIdAsync(int id);
        Task AddAsync(SeriesViewModel model);
        Task UpdateAsync(SeriesViewModel model);
        Task DeleteAsync(int id);
        Task<PaginatedList<SeriesViewModel>> GetPaginatedAsync(string? searchTerm, string? sortOrder, int pageIndex, int pageSize);
        Task<IEnumerable<RaceViewModel>> GetRacesBySeriesIdAsync(int seriesId);

    }
}
