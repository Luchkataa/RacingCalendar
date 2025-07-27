using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RacingCalendar.Services.Core.Contracts;
using RacingCalendar.ViewModels;
using System.Drawing.Printing;

namespace RacingCalendar.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class TeamsController : Controller
    {
        private readonly ITeamService _teamService;

        public TeamsController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        public async Task<IActionResult> Index(string searchTerm, string sortOrder, int page = 1)
        {
            int pageSize = 10;
            var teams = await _teamService.GetPaginatedTeamsAsync(searchTerm, sortOrder, page, pageSize);
            ViewBag.SearchTerm = searchTerm;
            ViewBag.SortOrder = sortOrder;
            return View(teams);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TeamViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            await _teamService.AddAsync(model);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var team = await _teamService.GetByIdAsync(id);
            if (team == null) return NotFound();

            return View(team);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TeamViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            await _teamService.UpdateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var team = await _teamService.GetByIdAsync(id);
            if (team == null) return NotFound();

            return View(team);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _teamService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
