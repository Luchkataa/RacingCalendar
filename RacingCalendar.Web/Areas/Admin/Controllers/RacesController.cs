﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RacingCalendar.Data.Models;
using RacingCalendar.Services.Core.Contracts;
using RacingCalendar.ViewModels;

namespace RacingCalendar.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class RacesController : Controller
    {
        private readonly IRaceService _raceService;

        public RacesController(IRaceService raceService)
        {
            _raceService = raceService;
        }

        public async Task<IActionResult> Index(string? searchTerm, int? seriesId, int page = 1)
        {
            int pageSize = 10;

            var races = await _raceService.GetAllPaginatedAsync(page, pageSize, searchTerm, seriesId);

            ViewBag.SearchTerm = searchTerm;
            ViewBag.SeriesId = seriesId;
            ViewBag.SeriesOptions = await _raceService.GetSeriesSelectListAsync();

            return View(races);
        }

        public async Task<IActionResult> Create()
        {
            var model = new RaceViewModel
            {
                SeriesOptions = await _raceService.GetSeriesSelectListAsync(),
                CircuitOptions = await _raceService.GetCircuitsSelectListAsync()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RaceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.SeriesOptions = await _raceService.GetSeriesSelectListAsync();
                model.CircuitOptions = await _raceService.GetCircuitsSelectListAsync();
                return View(model);
            }

            await _raceService.AddAsync(model);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var model = await _raceService.GetByIdAsync(id);
            if (model == null) return NotFound();

            model.SeriesOptions = await _raceService.GetSeriesSelectListAsync();
            model.CircuitOptions = await _raceService.GetCircuitsSelectListAsync();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RaceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.SeriesOptions = await _raceService.GetSeriesSelectListAsync();
                model.CircuitOptions = await _raceService.GetCircuitsSelectListAsync();
                return View(model);
            }

            await _raceService.UpdateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _raceService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
