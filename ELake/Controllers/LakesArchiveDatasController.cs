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
    public class LakesArchiveDatasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LakesArchiveDatasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: LakesArchiveDatas
        public async Task<IActionResult> Index()
        {
            return View(await _context.LakesArchiveData.ToListAsync());
        }

        // GET: LakesArchiveDatas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lakesArchiveData = await _context.LakesArchiveData
                .SingleOrDefaultAsync(m => m.Id == id);
            if (lakesArchiveData == null)
            {
                return NotFound();
            }

            return View(lakesArchiveData);
        }

        // GET: LakesArchiveDatas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LakesArchiveDatas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LakeId,SurveyYear,LakeLength,LakeShorelineLength,LakeMirrorArea,LakeAbsoluteHeight,LakeWidth,LakeMaxDepth,LakeWaterMass,ArchivalInfoSource")] LakesArchiveData lakesArchiveData)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lakesArchiveData);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lakesArchiveData);
        }

        // GET: LakesArchiveDatas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lakesArchiveData = await _context.LakesArchiveData.SingleOrDefaultAsync(m => m.Id == id);
            if (lakesArchiveData == null)
            {
                return NotFound();
            }
            return View(lakesArchiveData);
        }

        // POST: LakesArchiveDatas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LakeId,SurveyYear,LakeLength,LakeShorelineLength,LakeMirrorArea,LakeAbsoluteHeight,LakeWidth,LakeMaxDepth,LakeWaterMass,ArchivalInfoSource")] LakesArchiveData lakesArchiveData)
        {
            if (id != lakesArchiveData.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lakesArchiveData);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LakesArchiveDataExists(lakesArchiveData.Id))
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
            return View(lakesArchiveData);
        }

        // GET: LakesArchiveDatas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lakesArchiveData = await _context.LakesArchiveData
                .SingleOrDefaultAsync(m => m.Id == id);
            if (lakesArchiveData == null)
            {
                return NotFound();
            }

            return View(lakesArchiveData);
        }

        // POST: LakesArchiveDatas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lakesArchiveData = await _context.LakesArchiveData.SingleOrDefaultAsync(m => m.Id == id);
            _context.LakesArchiveData.Remove(lakesArchiveData);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LakesArchiveDataExists(int id)
        {
            return _context.LakesArchiveData.Any(e => e.Id == id);
        }
    }
}
