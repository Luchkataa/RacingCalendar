using Microsoft.AspNetCore.Mvc;

namespace RacingCalendar.Web.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/404")]
        public IActionResult NotFoundPage() => View("NotFound");

        [Route("Error/500")]
        public IActionResult InternalServerError() => View("Error");

        [Route("Error/403")]
        public IActionResult Forbidden() => View("Forbidden");
    }
}
