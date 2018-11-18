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
    public class GeneralHydrochemicalIndicatorStandardsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GeneralHydrochemicalIndicatorStandardsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: GeneralHydrochemicalIndicatorStandards
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Index(string SortOrder,
            string Indicator,
            int? Page)
        {
            var generalHydrochemicalIndicatorStandards = _context.GeneralHydrochemicalIndicatorStandard
                .Include(r => r.MeasurementUnit)
                .Include(r => r.RegulatoryDocument)
                .Where(r => true);

            ViewBag.IndicatorFilter = Indicator;

            ViewBag.IndicatorSort = SortOrder == "Indicator" ? "IndicatorDesc" : "Indicator";

            if (!string.IsNullOrEmpty(Indicator))
            {
                generalHydrochemicalIndicatorStandards = generalHydrochemicalIndicatorStandards.Where(w => w.Indicator.ToLower().Contains(Indicator.ToLower()));
            }

            switch (SortOrder)
            {
                case "Indicator":
                    generalHydrochemicalIndicatorStandards = generalHydrochemicalIndicatorStandards.OrderBy(w => w.Indicator);
                    break;
                case "IndicatorDesc":
                    generalHydrochemicalIndicatorStandards = generalHydrochemicalIndicatorStandards.OrderByDescending(w => w.Indicator);
                    break;
                default:
                    generalHydrochemicalIndicatorStandards = generalHydrochemicalIndicatorStandards.OrderBy(w => w.Id);
                    break;
            }
            ViewBag.SortOrder = SortOrder;

            var pager = new Pager(generalHydrochemicalIndicatorStandards.Count(), Page);

            var viewModel = new GeneralHydrochemicalIndicatorStandardIndexPageViewModel
            {
                Items = generalHydrochemicalIndicatorStandards.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };

            return View(viewModel);
        }

        // GET: GeneralHydrochemicalIndicatorStandards/Details/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var generalHydrochemicalIndicatorStandard = await _context.GeneralHydrochemicalIndicatorStandard
                .Include(g => g.MeasurementUnit)
                .Include(g => g.RegulatoryDocument)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (generalHydrochemicalIndicatorStandard == null)
            {
                return NotFound();
            }

            return View(generalHydrochemicalIndicatorStandard);
        }

        // GET: GeneralHydrochemicalIndicatorStandards/Create
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult Create()
        {
            ViewData["MeasurementUnitId"] = new SelectList(_context.MeasurementUnit, "Id", "Name");
            ViewData["RegulatoryDocumentId"] = new SelectList(_context.RegulatoryDocument.Where(r => !r.Archival), "Id", "Name");
            return View();
        }

        // POST: GeneralHydrochemicalIndicatorStandards/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Create([Bind("Id,Indicator,Value,LowerLimit,UpperLimit,MeasurementUnitId,RegulatoryDocumentId")] GeneralHydrochemicalIndicatorStandard generalHydrochemicalIndicatorStandard)
        {
            if (ModelState.IsValid)
            {
                _context.Add(generalHydrochemicalIndicatorStandard);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MeasurementUnitId"] = new SelectList(_context.MeasurementUnit, "Id", "Name", generalHydrochemicalIndicatorStandard.MeasurementUnitId);
            ViewData["RegulatoryDocumentId"] = new SelectList(_context.RegulatoryDocument.Where(r => !r.Archival), "Id", "Name", generalHydrochemicalIndicatorStandard.RegulatoryDocumentId);
            return View(generalHydrochemicalIndicatorStandard);
        }

        // GET: GeneralHydrochemicalIndicatorStandards/Edit/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var generalHydrochemicalIndicatorStandard = await _context.GeneralHydrochemicalIndicatorStandard.SingleOrDefaultAsync(m => m.Id == id);
            if (generalHydrochemicalIndicatorStandard == null)
            {
                return NotFound();
            }
            ViewData["MeasurementUnitId"] = new SelectList(_context.MeasurementUnit, "Id", "Name", generalHydrochemicalIndicatorStandard.MeasurementUnitId);
            ViewData["RegulatoryDocumentId"] = new SelectList(_context.RegulatoryDocument.Where(r => !r.Archival), "Id", "Name", generalHydrochemicalIndicatorStandard.RegulatoryDocumentId);
            return View(generalHydrochemicalIndicatorStandard);
        }

        // POST: GeneralHydrochemicalIndicatorStandards/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Indicator,Value,LowerLimit,UpperLimit,MeasurementUnitId,RegulatoryDocumentId")] GeneralHydrochemicalIndicatorStandard generalHydrochemicalIndicatorStandard)
        {
            if (id != generalHydrochemicalIndicatorStandard.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(generalHydrochemicalIndicatorStandard);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GeneralHydrochemicalIndicatorStandardExists(generalHydrochemicalIndicatorStandard.Id))
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
            ViewData["MeasurementUnitId"] = new SelectList(_context.MeasurementUnit, "Id", "Name", generalHydrochemicalIndicatorStandard.MeasurementUnitId);
            ViewData["RegulatoryDocumentId"] = new SelectList(_context.RegulatoryDocument.Where(r => !r.Archival), "Id", "Name", generalHydrochemicalIndicatorStandard.RegulatoryDocumentId);
            return View(generalHydrochemicalIndicatorStandard);
        }

        // GET: GeneralHydrochemicalIndicatorStandards/Delete/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var generalHydrochemicalIndicatorStandard = await _context.GeneralHydrochemicalIndicatorStandard
                .Include(g => g.MeasurementUnit)
                .Include(g => g.RegulatoryDocument)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (generalHydrochemicalIndicatorStandard == null)
            {
                return NotFound();
            }

            return View(generalHydrochemicalIndicatorStandard);
        }

        // POST: GeneralHydrochemicalIndicatorStandards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var generalHydrochemicalIndicatorStandard = await _context.GeneralHydrochemicalIndicatorStandard.SingleOrDefaultAsync(m => m.Id == id);
            _context.GeneralHydrochemicalIndicatorStandard.Remove(generalHydrochemicalIndicatorStandard);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GeneralHydrochemicalIndicatorStandardExists(int id)
        {
            return _context.GeneralHydrochemicalIndicatorStandard.Any(e => e.Id == id);
        }
    }
}
