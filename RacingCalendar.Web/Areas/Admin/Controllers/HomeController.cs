using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RacingCalendar.Data;

namespace RacingCalendar.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class HomeController : Controller
    {
        private readonly RacingCalendarDbContext _context;

        public HomeController(RacingCalendarDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Dashboard");
        }
        public async Task<IActionResult> Dashboard()
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