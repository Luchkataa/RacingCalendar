using Microsoft.AspNetCore.Mvc;
using RacingCalendar.Services.Core.Contracts;
using RacingCalendar.ViewModels.Filters;

namespace RacingCalendar.Web.Areas.User.Controllers
{
    [Area("User")]
    public class SeriesController : Controller
    {
        private readonly ISeriesService _seriesService;

        public SeriesController(ISeriesService seriesService)
        {
            _seriesService = seriesService;
        }

        public async Task<IActionResult> Index(string? searchTerm, string? sortOrder, int page = 1)
        {
            const int pageSize = 10;

            var series = await _seriesService.GetPaginatedAsync(searchTerm, sortOrder, page, pageSize);

            var model = new SeriesListViewModel
            {
                Series = series,
                SearchTerm = searchTerm,
                SortOrder = sortOrder
            };

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var series = await _seriesService.GetByIdAsync(id);
            if (series == null) return NotFound();
            return View(series);
        }

        public async Task<IActionResult> Races(int id)
        {
            var races = await _seriesService.GetRacesBySeriesIdAsync(id);
            return View(races);
        }
    }
}
