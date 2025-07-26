using Microsoft.AspNetCore.Mvc;
using RacingCalendar.Services.Core.Contracts;

namespace RacingCalendar.Areas.User.Controllers
{
    [Area("User")]
    public class RaceController : Controller
    {
        private readonly IRaceService _raceService;

        public RaceController(IRaceService raceService)
        {
            _raceService = raceService;
        }

        public async Task<IActionResult> Upcoming()
        {
            var races = await _raceService.GetUpcomingRacesAsync();
            return View(races);
        }

        public async Task<IActionResult> Past()
        {
            var races = await _raceService.GetPastRacesAsync();
            return View(races);
        }
    }
}
