using Microsoft.AspNetCore.Mvc;
using RacingCalendar.Services.Core.Contracts;
using RacingCalendar.ViewModels;

namespace RacingCalendar.Web.Areas.User.Controllers
{
    [Area("User")]
    public class TeamController : Controller
    {
        private readonly ITeamService _teamService;
        private readonly IDriverService _driverService;

        public TeamController(ITeamService teamService, IDriverService driverService)
        {
            _teamService = teamService;
            _driverService = driverService;
        }

        public async Task<IActionResult> Index(string? searchTerm, string? sortOrder, int page = 1)
        {
            const int pageSize = 12;
            var teams = await _teamService.GetPaginatedTeamsAsync(searchTerm ?? "", sortOrder ?? "name_asc", page, pageSize);

            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParam"] = sortOrder == "name_asc" ? "name_desc" : "name_asc";
            ViewData["CountrySortParam"] = sortOrder == "country_asc" ? "country_desc" : "country_asc";
            ViewData["CurrentSearch"] = searchTerm;

            return View(teams);
        }

        public async Task<IActionResult> Drivers(int id)
        {
            var model = await _driverService.GetDriversByTeamAsync(id);

            if (model == null)
                return NotFound();

            return View(model);
        }
    }
}
