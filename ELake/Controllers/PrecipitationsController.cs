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
    public class PrecipitationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PrecipitationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Precipitations
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Index(string SortOrder,
            int? LakeId,
            int? Year,
            int? Page)
        {
            var srecipitations = _context.Precipitation
                .Where(w => true);

            ViewBag.LakeIdFilter = LakeId;
            ViewBag.YearFilter = Year;

            ViewBag.LakeIdSort = SortOrder == "LakeId" ? "LakeIdDesc" : "LakeId";
            ViewBag.YearSort = SortOrder == "Year" ? "YearDesc" : "Year";
            ViewBag.ValueSort = SortOrder == "Value" ? "ValueDesc" : "Value";

            if (LakeId != null)
            {
                srecipitations = srecipitations.Where(w => w.LakeId == LakeId);
            }
            if (Year != null)
            {
                srecipitations = srecipitations.Where(w => w.Year == Year);
            }

            switch (SortOrder)
            {
                case "LakeId":
                    srecipitations = srecipitations.OrderBy(w => w.LakeId);
                    break;
                case "LakeIdDesc":
                    srecipitations = srecipitations.OrderByDescending(w => w.LakeId);
                    break;
                case "Year":
                    srecipitations = srecipitations.OrderBy(w => w.Year);
                    break;
                case "YearDesc":
                    srecipitations = srecipitations.OrderByDescending(w => w.Year);
                    break;
                case "Value":
                    srecipitations = srecipitations.OrderBy(w => w.Value);
                    break;
                case "ValueDesc":
                    srecipitations = srecipitations.OrderByDescending(w => w.Value);
                    break;
                default:
                    srecipitations = srecipitations.OrderBy(w => w.Id);
                    break;
            }
            ViewBag.SortOrder = SortOrder;

            var pager = new Pager(srecipitations.Count(), Page);

            var viewModel = new PrecipitationIndexPageViewModel
            {
                Items = srecipitations.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };

            return View(viewModel);
        }

        // GET: Precipitations/Details/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surfaceFlow = await _context.Precipitation
                .SingleOrDefaultAsync(m => m.Id == id);
            if (surfaceFlow == null)
            {
                return NotFound();
            }

            return View(surfaceFlow);
        }

        // GET: Precipitations/Create
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Precipitations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Create([Bind("Id,LakeId,Year,Value")] Precipitation surfaceFlow)
        {
            if (ModelState.IsValid)
            {
                _context.Add(surfaceFlow);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(surfaceFlow);
        }

        // GET: Precipitations/Edit/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surfaceFlow = await _context.Precipitation.SingleOrDefaultAsync(m => m.Id == id);
            if (surfaceFlow == null)
            {
                return NotFound();
            }
            return View(surfaceFlow);
        }

        // POST: Precipitations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LakeId,Year,Value")] Precipitation surfaceFlow)
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
                    if (!PrecipitationExists(surfaceFlow.Id))
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

        // GET: Precipitations/Delete/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surfaceFlow = await _context.Precipitation
                .SingleOrDefaultAsync(m => m.Id == id);
            if (surfaceFlow == null)
            {
                return NotFound();
            }

            return View(surfaceFlow);
        }

        // POST: Precipitations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var surfaceFlow = await _context.Precipitation.SingleOrDefaultAsync(m => m.Id == id);
            _context.Precipitation.Remove(surfaceFlow);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PrecipitationExists(int id)
        {
            return _context.Precipitation.Any(e => e.Id == id);
        }
    }
}
