using Microsoft.AspNetCore.Mvc;
using RacingCalendar.Services.Core.Contracts;

namespace RacingCalendar.Web.Areas.User.Controllers
{
    [Area("User")]
    public class DriversController : Controller
    {
        private readonly IDriverService _driverService;

        public DriversController(IDriverService driverService)
        {
            _driverService = driverService;
        }

        public async Task<IActionResult> Index(string? searchTerm, string? sortOrder, int page = 1)
        {
            const int pageSize = 8;

            var model = await _driverService.GetDriversAsync(searchTerm, sortOrder, page, pageSize);
            return View(model);
        }
    }
}
