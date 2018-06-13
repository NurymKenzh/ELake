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
    public class WaterLevelsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WaterLevelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: WaterLevels
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Index(string SortOrder,
            int? LakeId,
            int? Year,
            int? Page)
        {
            var waterLevels = _context.WaterLevel
                .Where(w => true);

            ViewBag.LakeIdFilter = LakeId;
            ViewBag.YearFilter = Year;

            ViewBag.LakeIdSort = SortOrder == "LakeId" ? "LakeIdDesc" : "LakeId";
            ViewBag.YearSort = SortOrder == "Year" ? "YearDesc" : "Year";
            ViewBag.WaterLavelMSort = SortOrder == "WaterLavelM" ? "WaterLavelMDesc" : "WaterLavelM";

            if (LakeId!=null)
            {
                waterLevels = waterLevels.Where(w => w.LakeId == LakeId);
            }
            if (Year != null)
            {
                waterLevels = waterLevels.Where(w => w.Year == Year);
            }

            switch (SortOrder)
            {
                case "LakeId":
                    waterLevels = waterLevels.OrderBy(w => w.LakeId);
                    break;
                case "LakeIdDesc":
                    waterLevels = waterLevels.OrderByDescending(w => w.LakeId);
                    break;
                case "Year":
                    waterLevels = waterLevels.OrderBy(w => w.Year);
                    break;
                case "YearDesc":
                    waterLevels = waterLevels.OrderByDescending(w => w.Year);
                    break;
                case "WaterLavelM":
                    waterLevels = waterLevels.OrderBy(w => w.WaterLavelM);
                    break;
                case "WaterLavelMDesc":
                    waterLevels = waterLevels.OrderByDescending(w => w.WaterLavelM);
                    break;
                default:
                    waterLevels = waterLevels.OrderBy(w => w.Id);
                    break;
            }
            ViewBag.SortOrder = SortOrder;

            var pager = new Pager(waterLevels.Count(), Page);

            var viewModel = new WaterLevelIndexPageViewModel
            {
                Items = waterLevels.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };

            return View(viewModel);
        }

        // GET: WaterLevels/Details/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var waterLevel = await _context.WaterLevel
                .SingleOrDefaultAsync(m => m.Id == id);
            if (waterLevel == null)
            {
                return NotFound();
            }

            return View(waterLevel);
        }

        // GET: WaterLevels/Create
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: WaterLevels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Create([Bind("Id,LakeId,Year,WaterLavelM")] WaterLevel waterLevel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(waterLevel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(waterLevel);
        }

        // GET: WaterLevels/Edit/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var waterLevel = await _context.WaterLevel.SingleOrDefaultAsync(m => m.Id == id);
            if (waterLevel == null)
            {
                return NotFound();
            }
            return View(waterLevel);
        }

        // POST: WaterLevels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LakeId,Year,WaterLavelM")] WaterLevel waterLevel)
        {
            if (id != waterLevel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(waterLevel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WaterLevelExists(waterLevel.Id))
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
            return View(waterLevel);
        }

        // GET: WaterLevels/Delete/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var waterLevel = await _context.WaterLevel
                .SingleOrDefaultAsync(m => m.Id == id);
            if (waterLevel == null)
            {
                return NotFound();
            }

            return View(waterLevel);
        }

        // POST: WaterLevels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var waterLevel = await _context.WaterLevel.SingleOrDefaultAsync(m => m.Id == id);
            _context.WaterLevel.Remove(waterLevel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WaterLevelExists(int id)
        {
            return _context.WaterLevel.Any(e => e.Id == id);
        }
    }
}
