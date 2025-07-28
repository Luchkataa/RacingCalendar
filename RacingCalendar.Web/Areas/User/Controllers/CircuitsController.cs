using Microsoft.AspNetCore.Mvc;
using RacingCalendar.Services.Core.Contracts;
using RacingCalendar.ViewModels.Filters;

namespace RacingCalendar.Web.Areas.User.Controllers
{
    [Area("User")]
    public class CircuitsController : Controller
    {
        private readonly ICircuitService _circuitService;

        public CircuitsController(ICircuitService circuitService)
        {
            _circuitService = circuitService;
        }

        public async Task<IActionResult> Index(string? countryFilter, string? sortOrder, int page = 1)
        {
            int pageSize = 6;

            var circuits = await _circuitService.GetAllFilteredAsync(page, pageSize, countryFilter, sortOrder);
            var countries = await _circuitService.GetDistinctCountriesAsync();

            var viewModel = new CircuitListViewModel
            {
                Circuits = circuits,
                CountryFilter = countryFilter,
                SortOrder = sortOrder,
                CountryOptions = countries
            };

            return View(viewModel);
        }
    }

}
