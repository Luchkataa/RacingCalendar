using Microsoft.AspNetCore.Mvc;

namespace RacingCalendar.Web.Areas.User.Controllers
{
    [Area("User")]
    public class UserHomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
