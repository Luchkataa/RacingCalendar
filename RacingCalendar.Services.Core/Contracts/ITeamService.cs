using RacingCalendar.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RacingCalendar.Services.Core.Contracts
{
    public interface ITeamService
    {
        Task<IEnumerable<TeamViewModel>> GetAllAsync();
        Task<TeamViewModel?> GetByIdAsync(int id);
        Task AddAsync(TeamViewModel vm);
        Task UpdateAsync(TeamViewModel vm);
        Task DeleteAsync(int id);
        Task<IEnumerable<SelectListItem>> GetTeamsSelectListAsync();
        Task<PaginatedList<TeamViewModel>> GetPaginatedTeamsAsync(string searchTerm, int page, int pageSize);
    }
}
