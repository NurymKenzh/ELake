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
    public class UndergroundOutflowsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UndergroundOutflowsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: UndergroundOutflows
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Index(string SortOrder,
            int? LakeId,
            int? Year,
            int? Page)
        {
            var undergroundOutflows = _context.UndergroundOutflow
                .Where(w => true);

            ViewBag.LakeIdFilter = LakeId;
            ViewBag.YearFilter = Year;

            ViewBag.LakeIdSort = SortOrder == "LakeId" ? "LakeIdDesc" : "LakeId";
            ViewBag.YearSort = SortOrder == "Year" ? "YearDesc" : "Year";
            ViewBag.ValueSort = SortOrder == "Value" ? "ValueDesc" : "Value";

            if (LakeId != null)
            {
                undergroundOutflows = undergroundOutflows.Where(w => w.LakeId == LakeId);
            }
            if (Year != null)
            {
                undergroundOutflows = undergroundOutflows.Where(w => w.Year == Year);
            }

            switch (SortOrder)
            {
                case "LakeId":
                    undergroundOutflows = undergroundOutflows.OrderBy(w => w.LakeId);
                    break;
                case "LakeIdDesc":
                    undergroundOutflows = undergroundOutflows.OrderByDescending(w => w.LakeId);
                    break;
                case "Year":
                    undergroundOutflows = undergroundOutflows.OrderBy(w => w.Year);
                    break;
                case "YearDesc":
                    undergroundOutflows = undergroundOutflows.OrderByDescending(w => w.Year);
                    break;
                case "Value":
                    undergroundOutflows = undergroundOutflows.OrderBy(w => w.Value);
                    break;
                case "ValueDesc":
                    undergroundOutflows = undergroundOutflows.OrderByDescending(w => w.Value);
                    break;
                default:
                    undergroundOutflows = undergroundOutflows.OrderBy(w => w.Id);
                    break;
            }
            ViewBag.SortOrder = SortOrder;

            var pager = new Pager(undergroundOutflows.Count(), Page);

            var viewModel = new UndergroundOutflowIndexPageViewModel
            {
                Items = undergroundOutflows.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };

            return View(viewModel);
        }

        // GET: UndergroundOutflows/Details/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surfaceFlow = await _context.UndergroundOutflow
                .SingleOrDefaultAsync(m => m.Id == id);
            if (surfaceFlow == null)
            {
                return NotFound();
            }

            return View(surfaceFlow);
        }

        // GET: UndergroundOutflows/Create
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: UndergroundOutflows/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Create([Bind("Id,LakeId,Year,Value")] UndergroundOutflow surfaceFlow)
        {
            if (ModelState.IsValid)
            {
                _context.Add(surfaceFlow);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(surfaceFlow);
        }

        // GET: UndergroundOutflows/Edit/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surfaceFlow = await _context.UndergroundOutflow.SingleOrDefaultAsync(m => m.Id == id);
            if (surfaceFlow == null)
            {
                return NotFound();
            }
            return View(surfaceFlow);
        }

        // POST: UndergroundOutflows/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LakeId,Year,Value")] UndergroundOutflow surfaceFlow)
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
                    if (!UndergroundOutflowExists(surfaceFlow.Id))
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

        // GET: UndergroundOutflows/Delete/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surfaceFlow = await _context.UndergroundOutflow
                .SingleOrDefaultAsync(m => m.Id == id);
            if (surfaceFlow == null)
            {
                return NotFound();
            }

            return View(surfaceFlow);
        }

        // POST: UndergroundOutflows/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var surfaceFlow = await _context.UndergroundOutflow.SingleOrDefaultAsync(m => m.Id == id);
            _context.UndergroundOutflow.Remove(surfaceFlow);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UndergroundOutflowExists(int id)
        {
            return _context.UndergroundOutflow.Any(e => e.Id == id);
        }
    }
}
