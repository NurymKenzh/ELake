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
    public class EvaporationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EvaporationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Evaporations
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Index(string SortOrder,
            int? LakeId,
            int? Year,
            int? Page)
        {
            var evaporations = _context.Evaporation
                .Where(w => true);

            ViewBag.LakeIdFilter = LakeId;
            ViewBag.YearFilter = Year;

            ViewBag.LakeIdSort = SortOrder == "LakeId" ? "LakeIdDesc" : "LakeId";
            ViewBag.YearSort = SortOrder == "Year" ? "YearDesc" : "Year";
            ViewBag.ValueSort = SortOrder == "Value" ? "ValueDesc" : "Value";

            if (LakeId != null)
            {
                evaporations = evaporations.Where(w => w.LakeId == LakeId);
            }
            if (Year != null)
            {
                evaporations = evaporations.Where(w => w.Year == Year);
            }

            switch (SortOrder)
            {
                case "LakeId":
                    evaporations = evaporations.OrderBy(w => w.LakeId);
                    break;
                case "LakeIdDesc":
                    evaporations = evaporations.OrderByDescending(w => w.LakeId);
                    break;
                case "Year":
                    evaporations = evaporations.OrderBy(w => w.Year);
                    break;
                case "YearDesc":
                    evaporations = evaporations.OrderByDescending(w => w.Year);
                    break;
                case "Value":
                    evaporations = evaporations.OrderBy(w => w.Value);
                    break;
                case "ValueDesc":
                    evaporations = evaporations.OrderByDescending(w => w.Value);
                    break;
                default:
                    evaporations = evaporations.OrderBy(w => w.Id);
                    break;
            }
            ViewBag.SortOrder = SortOrder;

            var pager = new Pager(evaporations.Count(), Page);

            var viewModel = new EvaporationIndexPageViewModel
            {
                Items = evaporations.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };

            return View(viewModel);
        }

        // GET: Evaporations/Details/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surfaceFlow = await _context.Evaporation
                .SingleOrDefaultAsync(m => m.Id == id);
            if (surfaceFlow == null)
            {
                return NotFound();
            }

            return View(surfaceFlow);
        }

        // GET: Evaporations/Create
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Evaporations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Create([Bind("Id,LakeId,Year,Value")] Evaporation surfaceFlow)
        {
            if (ModelState.IsValid)
            {
                _context.Add(surfaceFlow);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(surfaceFlow);
        }

        // GET: Evaporations/Edit/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surfaceFlow = await _context.Evaporation.SingleOrDefaultAsync(m => m.Id == id);
            if (surfaceFlow == null)
            {
                return NotFound();
            }
            return View(surfaceFlow);
        }

        // POST: Evaporations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LakeId,Year,Value")] Evaporation surfaceFlow)
        {
            if (id != surfaceFlow.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(surfaceFlow);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EvaporationExists(surfaceFlow.Id))
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
            return View(surfaceFlow);
        }

        // GET: Evaporations/Delete/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surfaceFlow = await _context.Evaporation
                .SingleOrDefaultAsync(m => m.Id == id);
            if (surfaceFlow == null)
            {
                return NotFound();
            }

            return View(surfaceFlow);
        }

        // POST: Evaporations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var surfaceFlow = await _context.Evaporation.SingleOrDefaultAsync(m => m.Id == id);
            _context.Evaporation.Remove(surfaceFlow);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EvaporationExists(int id)
        {
            return _context.Evaporation.Any(e => e.Id == id);
        }
    }
}
