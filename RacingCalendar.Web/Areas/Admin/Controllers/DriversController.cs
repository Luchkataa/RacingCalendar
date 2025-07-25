using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RacingCalendar.Services.Core.Contracts;
using RacingCalendar.ViewModels;

namespace RacingCalendar.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class DriversController : Controller
    {
        private readonly IDriverService _driverService;
        private readonly ITeamService _teamService;

        public DriversController(IDriverService driverService, ITeamService teamService)
        {
            _driverService = driverService;
            _teamService = teamService;
        }

        public async Task<IActionResult> Index(int page = 1, string? searchTerm = null)
        {
            int pageSize = 8;
            var paginatedDrivers = await _driverService.GetAllPaginatedAsync(page, pageSize, searchTerm);
            return View(paginatedDrivers);
        }

        public async Task<IActionResult> Create()
        {
            var model = new DriverViewModel();
            model.Teams = await _teamService.GetTeamsSelectListAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DriverViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Teams = await _teamService.GetTeamsSelectListAsync();
                return View(model);
            }

            await _driverService.AddAsync(model);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var driver = await _driverService.GetByIdAsync(id);
            if (driver == null)
                return NotFound();

            driver.Teams = await _teamService.GetTeamsSelectListAsync();
            return View(driver);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DriverViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Teams = await _teamService.GetTeamsSelectListAsync();
                return View(model);
            }

            await _driverService.UpdateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _driverService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

    }
}
