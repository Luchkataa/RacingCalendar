﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RacingCalendar.Services.Core.Contracts;
using RacingCalendar.ViewModels;

namespace RacingCalendar.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class CircuitsController : Controller
    {
        private readonly ICircuitService _circuitService;

        public CircuitsController(ICircuitService circuitService)
        {
            _circuitService = circuitService;
        }

        public async Task<IActionResult> Index(int page = 1, string? searchTerm = null)
        {
            int pageSize = 8;
            var circuits = await _circuitService.GetAllPaginatedAsync(page, pageSize, searchTerm);
            ViewData["CurrentSearch"] = searchTerm;
            return View(circuits);
        }
            
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CircuitViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _circuitService.AddAsync(model);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var circuit = await _circuitService.GetByIdAsync(id);
            if (circuit == null)
                return NotFound();

            return View(circuit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CircuitViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _circuitService.UpdateAsync(model);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var circuit = await _circuitService.GetByIdAsync(id);
            if (circuit == null)
                return NotFound();

            return View(circuit);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _circuitService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
