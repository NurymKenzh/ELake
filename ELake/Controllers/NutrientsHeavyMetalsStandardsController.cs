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
    public class NutrientsHeavyMetalsStandardsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NutrientsHeavyMetalsStandardsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: NutrientsHeavyMetalsStandards
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Index(int? Page)
        {
            var nutrientsHeavyMetalsStandards = _context.NutrientsHeavyMetalsStandard
                .Include(r => r.RegulatoryDocument)
                .Where(r => true);

            var pager = new Pager(nutrientsHeavyMetalsStandards.Count(), Page);

            var viewModel = new NutrientsHeavyMetalsStandardIndexPageViewModel
            {
                Items = nutrientsHeavyMetalsStandards.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };

            return View(viewModel);
        }

        // GET: NutrientsHeavyMetalsStandards/Details/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nutrientsHeavyMetalsStandard = await _context.NutrientsHeavyMetalsStandard
                .Include(n => n.RegulatoryDocument)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (nutrientsHeavyMetalsStandard == null)
            {
                return NotFound();
            }

            return View(nutrientsHeavyMetalsStandard);
        }

        // GET: NutrientsHeavyMetalsStandards/Create
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult Create()
        {
            ViewData["RegulatoryDocumentId"] = new SelectList(_context.RegulatoryDocument.Where(r => !r.Archival), "Id", "Name");
            return View();
        }

        // POST: NutrientsHeavyMetalsStandards/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Create([Bind("Id,RegulatoryDocumentId,MPCNH4,MPCNO2,MPCNO3,MPCPPO4,MPCCu,MPCZn,MPCMn,MPCPb,MPCNi,MPCCd,MPCCo,HazardClassNH4,HazardClassNO2,HazardClassNO3,HazardClassPPO4,HazardClassCu,HazardClassZn,HazardClassMn,HazardClassPb,HazardClassNi,HazardClassCd,HazardClassCo")] NutrientsHeavyMetalsStandard nutrientsHeavyMetalsStandard)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nutrientsHeavyMetalsStandard);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RegulatoryDocumentId"] = new SelectList(_context.RegulatoryDocument.Where(r => !r.Archival), "Id", "Name", nutrientsHeavyMetalsStandard.RegulatoryDocumentId);
            return View(nutrientsHeavyMetalsStandard);
        }

        // GET: NutrientsHeavyMetalsStandards/Edit/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nutrientsHeavyMetalsStandard = await _context.NutrientsHeavyMetalsStandard.SingleOrDefaultAsync(m => m.Id == id);
            if (nutrientsHeavyMetalsStandard == null)
            {
                return NotFound();
            }
            ViewData["RegulatoryDocumentId"] = new SelectList(_context.RegulatoryDocument.Where(r => !r.Archival), "Id", "Name", nutrientsHeavyMetalsStandard.RegulatoryDocumentId);
            return View(nutrientsHeavyMetalsStandard);
        }

        // POST: NutrientsHeavyMetalsStandards/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RegulatoryDocumentId,MPCNH4,MPCNO2,MPCNO3,MPCPPO4,MPCCu,MPCZn,MPCMn,MPCPb,MPCNi,MPCCd,MPCCo,HazardClassNH4,HazardClassNO2,HazardClassNO3,HazardClassPPO4,HazardClassCu,HazardClassZn,HazardClassMn,HazardClassPb,HazardClassNi,HazardClassCd,HazardClassCo")] NutrientsHeavyMetalsStandard nutrientsHeavyMetalsStandard)
        {
            if (id != nutrientsHeavyMetalsStandard.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nutrientsHeavyMetalsStandard);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NutrientsHeavyMetalsStandardExists(nutrientsHeavyMetalsStandard.Id))
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
            ViewData["RegulatoryDocumentId"] = new SelectList(_context.RegulatoryDocument.Where(r => !r.Archival), "Id", "Name", nutrientsHeavyMetalsStandard.RegulatoryDocumentId);
            return View(nutrientsHeavyMetalsStandard);
        }

        // GET: NutrientsHeavyMetalsStandards/Delete/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nutrientsHeavyMetalsStandard = await _context.NutrientsHeavyMetalsStandard
                .Include(n => n.RegulatoryDocument)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (nutrientsHeavyMetalsStandard == null)
            {
                return NotFound();
            }

            return View(nutrientsHeavyMetalsStandard);
        }

        // POST: NutrientsHeavyMetalsStandards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nutrientsHeavyMetalsStandard = await _context.NutrientsHeavyMetalsStandard.SingleOrDefaultAsync(m => m.Id == id);
            _context.NutrientsHeavyMetalsStandard.Remove(nutrientsHeavyMetalsStandard);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NutrientsHeavyMetalsStandardExists(int id)
        {
            return _context.NutrientsHeavyMetalsStandard.Any(e => e.Id == id);
        }
    }
}
