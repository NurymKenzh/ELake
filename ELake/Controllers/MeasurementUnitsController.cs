using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ELake.Data;
using ELake.Models;
using Microsoft.AspNetCore.Authorization;

namespace ELake.Controllers
{
    public class MeasurementUnitsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MeasurementUnitsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MeasurementUnits
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Index(int? Page)
        {
            var measurementUnits = _context.MeasurementUnit
                .Where(w => true);

            var pager = new Pager(measurementUnits.Count(), Page);

            var viewModel = new MeasurementUnitIndexPageViewModel
            {
                Items = measurementUnits.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };

            return View(viewModel);
        }

        // GET: MeasurementUnits/Details/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var measurementUnit = await _context.MeasurementUnit
                .SingleOrDefaultAsync(m => m.Id == id);
            if (measurementUnit == null)
            {
                return NotFound();
            }

            return View(measurementUnit);
        }

        // GET: MeasurementUnits/Create
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: MeasurementUnits/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Create([Bind("Id,Name")] MeasurementUnit measurementUnit)
        {
            if (ModelState.IsValid)
            {
                _context.Add(measurementUnit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(measurementUnit);
        }

        // GET: MeasurementUnits/Edit/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var measurementUnit = await _context.MeasurementUnit.SingleOrDefaultAsync(m => m.Id == id);
            if (measurementUnit == null)
            {
                return NotFound();
            }
            return View(measurementUnit);
        }

        // POST: MeasurementUnits/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] MeasurementUnit measurementUnit)
        {
            if (id != measurementUnit.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(measurementUnit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MeasurementUnitExists(measurementUnit.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(measurementUnit);
        }

        // GET: MeasurementUnits/Delete/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var measurementUnit = await _context.MeasurementUnit
                .SingleOrDefaultAsync(m => m.Id == id);
            if (measurementUnit == null)
            {
                return NotFound();
            }

            return View(measurementUnit);
        }

        // POST: MeasurementUnits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var measurementUnit = await _context.MeasurementUnit.SingleOrDefaultAsync(m => m.Id == id);
            _context.MeasurementUnit.Remove(measurementUnit);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MeasurementUnitExists(int id)
        {
            return _context.MeasurementUnit.Any(e => e.Id == id);
        }
    }
}
