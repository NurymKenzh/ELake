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
    public class LakesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LakesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Lakes
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Lake.ToListAsync());
        }

        // GET: Lakes/Details/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lake = await _context.Lake
                .SingleOrDefaultAsync(m => m.Id == id);
            if (lake == null)
            {
                return NotFound();
            }

            return View(lake);
        }

        // GET: Lakes/Create
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Lakes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Create([Bind("Id,LakeId,NameKK,NameRU,NameEN")] Lake lake)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lake);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lake);
        }

        // GET: Lakes/Edit/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lake = await _context.Lake.SingleOrDefaultAsync(m => m.Id == id);
            if (lake == null)
            {
                return NotFound();
            }
            return View(lake);
        }

        // POST: Lakes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LakeId,NameKK,NameRU,NameEN")] Lake lake)
        {
            if (id != lake.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lake);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LakeExists(lake.Id))
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
            return View(lake);
        }

        // GET: Lakes/Delete/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lake = await _context.Lake
                .SingleOrDefaultAsync(m => m.Id == id);
            if (lake == null)
            {
                return NotFound();
            }

            return View(lake);
        }

        // POST: Lakes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lake = await _context.Lake.SingleOrDefaultAsync(m => m.Id == id);
            _context.Lake.Remove(lake);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LakeExists(int id)
        {
            return _context.Lake.Any(e => e.Id == id);
        }
    }
}
