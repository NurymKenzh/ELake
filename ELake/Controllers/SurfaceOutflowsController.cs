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
    public class SurfaceOutflowsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SurfaceOutflowsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SurfaceOutflows
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Index(string SortOrder,
            int? LakeId,
            int? Year,
            int? Page)
        {
            var surfaceOutflows = _context.SurfaceOutflow
                .Where(w => true);

            ViewBag.LakeIdFilter = LakeId;
            ViewBag.YearFilter = Year;

            ViewBag.LakeIdSort = SortOrder == "LakeId" ? "LakeIdDesc" : "LakeId";
            ViewBag.YearSort = SortOrder == "Year" ? "YearDesc" : "Year";
            ViewBag.ValueSort = SortOrder == "Value" ? "ValueDesc" : "Value";

            if (LakeId != null)
            {
                surfaceOutflows = surfaceOutflows.Where(w => w.LakeId == LakeId);
            }
            if (Year != null)
            {
                surfaceOutflows = surfaceOutflows.Where(w => w.Year == Year);
            }

            switch (SortOrder)
            {
                case "LakeId":
                    surfaceOutflows = surfaceOutflows.OrderBy(w => w.LakeId);
                    break;
                case "LakeIdDesc":
                    surfaceOutflows = surfaceOutflows.OrderByDescending(w => w.LakeId);
                    break;
                case "Year":
                    surfaceOutflows = surfaceOutflows.OrderBy(w => w.Year);
                    break;
                case "YearDesc":
                    surfaceOutflows = surfaceOutflows.OrderByDescending(w => w.Year);
                    break;
                case "Value":
                    surfaceOutflows = surfaceOutflows.OrderBy(w => w.Value);
                    break;
                case "ValueDesc":
                    surfaceOutflows = surfaceOutflows.OrderByDescending(w => w.Value);
                    break;
                default:
                    surfaceOutflows = surfaceOutflows.OrderBy(w => w.Id);
                    break;
            }
            ViewBag.SortOrder = SortOrder;

            var pager = new Pager(surfaceOutflows.Count(), Page);

            var viewModel = new SurfaceOutflowIndexPageViewModel
            {
                Items = surfaceOutflows.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };

            return View(viewModel);
        }

        // GET: SurfaceOutflows/Details/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surfaceFlow = await _context.SurfaceOutflow
                .SingleOrDefaultAsync(m => m.Id == id);
            if (surfaceFlow == null)
            {
                return NotFound();
            }

            return View(surfaceFlow);
        }

        // GET: SurfaceOutflows/Create
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: SurfaceOutflows/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Create([Bind("Id,LakeId,Year,Value")] SurfaceOutflow surfaceFlow)
        {
            if (ModelState.IsValid)
            {
                _context.Add(surfaceFlow);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(surfaceFlow);
        }

        // GET: SurfaceOutflows/Edit/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surfaceFlow = await _context.SurfaceOutflow.SingleOrDefaultAsync(m => m.Id == id);
            if (surfaceFlow == null)
            {
                return NotFound();
            }
            return View(surfaceFlow);
        }

        // POST: SurfaceOutflows/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LakeId,Year,Value")] SurfaceOutflow surfaceFlow)
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
                    if (!SurfaceOutflowExists(surfaceFlow.Id))
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

        // GET: SurfaceOutflows/Delete/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surfaceFlow = await _context.SurfaceOutflow
                .SingleOrDefaultAsync(m => m.Id == id);
            if (surfaceFlow == null)
            {
                return NotFound();
            }

            return View(surfaceFlow);
        }

        // POST: SurfaceOutflows/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var surfaceFlow = await _context.SurfaceOutflow.SingleOrDefaultAsync(m => m.Id == id);
            _context.SurfaceOutflow.Remove(surfaceFlow);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SurfaceOutflowExists(int id)
        {
            return _context.SurfaceOutflow.Any(e => e.Id == id);
        }
    }
}
