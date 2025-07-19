using RacingCalendar.ViewModels;

namespace RacingCalendar.Services.Core.Contracts
{
    public interface ITeamService
    {
        Task<IEnumerable<TeamViewModel>> GetAllAsync();
        Task<TeamViewModel?> GetByIdAsync(int id);
        Task AddAsync(TeamViewModel vm);
        Task UpdateAsync(TeamViewModel vm);
        Task DeleteAsync(int id);
    }
}
