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
    public class KATOesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KATOesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: KATOes
        public async Task<IActionResult> Index()
        {
            return View(await _context.KATO.ToListAsync());
        }

        // GET: KATOes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kATO = await _context.KATO
                .SingleOrDefaultAsync(m => m.Id == id);
            if (kATO == null)
            {
                return NotFound();
            }

            return View(kATO);
        }

        // GET: KATOes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: KATOes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Number,NameKK,NameRU")] KATO kATO)
        {
            if (ModelState.IsValid)
            {
                _context.Add(kATO);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(kATO);
        }

        // GET: KATOes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kATO = await _context.KATO.SingleOrDefaultAsync(m => m.Id == id);
            if (kATO == null)
            {
                return NotFound();
            }
            return View(kATO);
        }

        // POST: KATOes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Number,NameKK,NameRU")] KATO kATO)
        {
            if (id != kATO.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(kATO);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KATOExists(kATO.Id))
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
            return View(kATO);
        }

        // GET: KATOes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kATO = await _context.KATO
                .SingleOrDefaultAsync(m => m.Id == id);
            if (kATO == null)
            {
                return NotFound();
            }

            return View(kATO);
        }

        // POST: KATOes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var kATO = await _context.KATO.SingleOrDefaultAsync(m => m.Id == id);
            _context.KATO.Remove(kATO);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KATOExists(int id)
        {
            return _context.KATO.Any(e => e.Id == id);
        }
    }
}
