﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RacingCalendar.Services.Core.Contracts;
using RacingCalendar.ViewModels;

namespace RacingCalendar.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class SeriesController : Controller
    {
        private readonly ISeriesService _seriesService;

        public SeriesController(ISeriesService seriesService)
        {
            _seriesService = seriesService;
        }

        public async Task<IActionResult> Index(string searchTerm, string sortOrder,int page = 1)
        {
            int pageSize = 10;
            var paginatedSeries = await _seriesService.GetPaginatedAsync(searchTerm,sortOrder, page, pageSize);
            ViewBag.CurrentSort = sortOrder;
            ViewBag.SearchTerm = searchTerm;
            return View(paginatedSeries);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SeriesViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _seriesService.AddAsync(model);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var series = await _seriesService.GetByIdAsync(id);
            if (series == null) return NotFound();

            return View(series);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SeriesViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _seriesService.UpdateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var series = await _seriesService.GetByIdAsync(id);
            if (series == null) return NotFound();

            return View(series);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _seriesService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
