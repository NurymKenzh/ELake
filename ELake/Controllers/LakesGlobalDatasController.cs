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
    public class LakesGlobalDatasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LakesGlobalDatasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: LakesGlobalDatas
        public async Task<IActionResult> Index()
        {
            return View(await _context.LakesGlobalData.ToListAsync());
        }

        // GET: LakesGlobalDatas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lakesGlobalData = await _context.LakesGlobalData
                .SingleOrDefaultAsync(m => m.Id == id);
            if (lakesGlobalData == null)
            {
                return NotFound();
            }

            return View(lakesGlobalData);
        }

        // GET: LakesGlobalDatas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LakesGlobalDatas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LakeId,Hylak_id,Lake_name_ENG,Lake_name_RU,Lake_name_KZ,Country_ENG,Country_RU,Country_KZ,Continent_ENG,Continent_RU,Continent_KZ,Lake_area,Shore_len,Shore_dev,Vol_total,Depth_avg,Dis_avg,Res_time,Elevation,Slope_100,Wshd_area,Pour_long,Pour_lat")] LakesGlobalData lakesGlobalData)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lakesGlobalData);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lakesGlobalData);
        }

        // GET: LakesGlobalDatas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lakesGlobalData = await _context.LakesGlobalData.SingleOrDefaultAsync(m => m.Id == id);
            if (lakesGlobalData == null)
            {
                return NotFound();
            }
            return View(lakesGlobalData);
        }

        // POST: LakesGlobalDatas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LakeId,Hylak_id,Lake_name_ENG,Lake_name_RU,Lake_name_KZ,Country_ENG,Country_RU,Country_KZ,Continent_ENG,Continent_RU,Continent_KZ,Lake_area,Shore_len,Shore_dev,Vol_total,Depth_avg,Dis_avg,Res_time,Elevation,Slope_100,Wshd_area,Pour_long,Pour_lat")] LakesGlobalData lakesGlobalData)
        {
            if (id != lakesGlobalData.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lakesGlobalData);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LakesGlobalDataExists(lakesGlobalData.Id))
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
            return View(lakesGlobalData);
        }

        // GET: LakesGlobalDatas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lakesGlobalData = await _context.LakesGlobalData
                .SingleOrDefaultAsync(m => m.Id == id);
            if (lakesGlobalData == null)
            {
                return NotFound();
            }

            return View(lakesGlobalData);
        }

        // POST: LakesGlobalDatas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lakesGlobalData = await _context.LakesGlobalData.SingleOrDefaultAsync(m => m.Id == id);
            _context.LakesGlobalData.Remove(lakesGlobalData);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LakesGlobalDataExists(int id)
        {
            return _context.LakesGlobalData.Any(e => e.Id == id);
        }
    }
}
