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
    public class SurfaceFlowsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SurfaceFlowsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SurfaceFlows
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Index(string SortOrder,
            int? LakeId,
            int? Year,
            int? Page)
        {
            var surfaceFlows = _context.SurfaceFlow
                .Where(w => true);

            ViewBag.LakeIdFilter = LakeId;
            ViewBag.YearFilter = Year;

            ViewBag.LakeIdSort = SortOrder == "LakeId" ? "LakeIdDesc" : "LakeId";
            ViewBag.YearSort = SortOrder == "Year" ? "YearDesc" : "Year";
            ViewBag.ValueSort = SortOrder == "Value" ? "ValueDesc" : "Value";

            if (LakeId != null)
            {
                surfaceFlows = surfaceFlows.Where(w => w.LakeId == LakeId);
            }
            if (Year != null)
            {
                surfaceFlows = surfaceFlows.Where(w => w.Year == Year);
            }

            switch (SortOrder)
            {
                case "LakeId":
                    surfaceFlows = surfaceFlows.OrderBy(w => w.LakeId);
                    break;
                case "LakeIdDesc":
                    surfaceFlows = surfaceFlows.OrderByDescending(w => w.LakeId);
                    break;
                case "Year":
                    surfaceFlows = surfaceFlows.OrderBy(w => w.Year);
                    break;
                case "YearDesc":
                    surfaceFlows = surfaceFlows.OrderByDescending(w => w.Year);
                    break;
                case "Value":
                    surfaceFlows = surfaceFlows.OrderBy(w => w.Value);
                    break;
                case "ValueDesc":
                    surfaceFlows = surfaceFlows.OrderByDescending(w => w.Value);
                    break;
                default:
                    surfaceFlows = surfaceFlows.OrderBy(w => w.Id);
                    break;
            }
            ViewBag.SortOrder = SortOrder;

            var pager = new Pager(surfaceFlows.Count(), Page);

            var viewModel = new SurfaceFlowIndexPageViewModel
            {
                Items = surfaceFlows.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };

            return View(viewModel);
        }

        // GET: SurfaceFlows/Details/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surfaceFlow = await _context.SurfaceFlow
                .SingleOrDefaultAsync(m => m.Id == id);
            if (surfaceFlow == null)
            {
                return NotFound();
            }

            return View(surfaceFlow);
        }

        // GET: SurfaceFlows/Create
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: SurfaceFlows/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Create([Bind("Id,LakeId,Year,Value")] SurfaceFlow surfaceFlow)
        {
            if (ModelState.IsValid)
            {
                _context.Add(surfaceFlow);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(surfaceFlow);
        }

        // GET: SurfaceFlows/Edit/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surfaceFlow = await _context.SurfaceFlow.SingleOrDefaultAsync(m => m.Id == id);
            if (surfaceFlow == null)
            {
                return NotFound();
            }
            return View(surfaceFlow);
        }

        // POST: SurfaceFlows/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LakeId,Year,Value")] SurfaceFlow surfaceFlow)
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
                    if (!SurfaceFlowExists(surfaceFlow.Id))
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

        // GET: SurfaceFlows/Delete/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surfaceFlow = await _context.SurfaceFlow
                .SingleOrDefaultAsync(m => m.Id == id);
            if (surfaceFlow == null)
            {
                return NotFound();
            }

            return View(surfaceFlow);
        }

        // POST: SurfaceFlows/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var surfaceFlow = await _context.SurfaceFlow.SingleOrDefaultAsync(m => m.Id == id);
            _context.SurfaceFlow.Remove(surfaceFlow);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SurfaceFlowExists(int id)
        {
            return _context.SurfaceFlow.Any(e => e.Id == id);
        }
    }
}
