using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<IActionResult> Index()
        {
            var drivers = await _driverService.GetAllAsync();
            return View(drivers);
        }

        public async Task<IActionResult> Create()
        {
            var model = new DriverViewModel();
            model.Teams = await _teamService.GetTeamsSelectListAsync();
            return View(model);
        }

        [HttpPost]
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
        public async Task<IActionResult> Delete(int id)
        {
            await _driverService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
