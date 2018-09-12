using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ELake.Data;
using ELake.Models;

namespace ELake.Controllers
{
    public class LakeKATOesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LakeKATOesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: LakeKATOes
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.LakeKATO.Include(l => l.KATO);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: LakeKATOes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lakeKATO = await _context.LakeKATO
                .Include(l => l.KATO)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (lakeKATO == null)
            {
                return NotFound();
            }

            return View(lakeKATO);
        }

        // GET: LakeKATOes/Create
        public IActionResult Create()
        {
            ViewData["KATOId"] = new SelectList(_context.KATO, "Id", "Id");
            return View();
        }

        // POST: LakeKATOes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LakeId,KATOId")] LakeKATO lakeKATO)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lakeKATO);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["KATOId"] = new SelectList(_context.KATO, "Id", "Id", lakeKATO.KATOId);
            return View(lakeKATO);
        }

        // GET: LakeKATOes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lakeKATO = await _context.LakeKATO.SingleOrDefaultAsync(m => m.Id == id);
            if (lakeKATO == null)
            {
                return NotFound();
            }
            ViewData["KATOId"] = new SelectList(_context.KATO, "Id", "Id", lakeKATO.KATOId);
            return View(lakeKATO);
        }

        // POST: LakeKATOes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LakeId,KATOId")] LakeKATO lakeKATO)
        {
            if (id != lakeKATO.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lakeKATO);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LakeKATOExists(lakeKATO.Id))
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
            ViewData["KATOId"] = new SelectList(_context.KATO, "Id", "Id", lakeKATO.KATOId);
            return View(lakeKATO);
        }

        // GET: LakeKATOes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lakeKATO = await _context.LakeKATO
                .Include(l => l.KATO)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (lakeKATO == null)
            {
                return NotFound();
            }

            return View(lakeKATO);
        }

        // POST: LakeKATOes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lakeKATO = await _context.LakeKATO.SingleOrDefaultAsync(m => m.Id == id);
            _context.LakeKATO.Remove(lakeKATO);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LakeKATOExists(int id)
        {
            return _context.LakeKATO.Any(e => e.Id == id);
        }
    }
}
