using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace RacingCalendar.Web.Areas.User.Controllers
{
    [Area("User")]
    public class UserHomeController : Controller
    {
        private readonly RacingCalendarDbContext _context;
        public UserHomeController(RacingCalendarDbContext context)
        {
            _context = context;
        }
        public async  Task<IActionResult> Index()
        {
            try
            {
                ViewBag.TotalCircuits = await _context.Circuits.CountAsync();
                ViewBag.TotalDrivers = await _context.Drivers.CountAsync();
                ViewBag.TotalTeams = await _context.Teams.CountAsync();
                ViewBag.UpcomingRaces = await _context.Races
                    .Where(r => r.Date >= DateTime.Today)
                    .CountAsync();
            }
            catch
            {
                ViewBag.TotalCircuits = 0;
                ViewBag.TotalDrivers = 0;
                ViewBag.TotalTeams = 0;
                ViewBag.UpcomingRaces = 0;
            }

            return View();
        }
    }
}
