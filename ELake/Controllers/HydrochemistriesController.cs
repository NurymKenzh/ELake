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
    public class HydrochemistriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HydrochemistriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Hydrochemistries
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Index(string SortOrder,
            int? LakeId,
            int? Year,
            int? Page)
        {
            var hydrochemistries = _context.Hydrochemistry
                .Where(w => true);

            ViewBag.LakeIdFilter = LakeId;
            ViewBag.YearFilter = Year;

            ViewBag.LakeIdSort = SortOrder == "LakeId" ? "LakeIdDesc" : "LakeId";
            ViewBag.YearSort = SortOrder == "Year" ? "YearDesc" : "Year";

            if (LakeId != null)
            {
                hydrochemistries = hydrochemistries.Where(w => w.LakeId == LakeId);
            }
            if (Year != null)
            {
                hydrochemistries = hydrochemistries.Where(w => w.Year == Year);
            }

            switch (SortOrder)
            {
                case "LakeId":
                    hydrochemistries = hydrochemistries.OrderBy(w => w.LakeId);
                    break;
                case "LakeIdDesc":
                    hydrochemistries = hydrochemistries.OrderByDescending(w => w.LakeId);
                    break;
                case "Year":
                    hydrochemistries = hydrochemistries.OrderBy(w => w.Year);
                    break;
                case "YearDesc":
                    hydrochemistries = hydrochemistries.OrderByDescending(w => w.Year);
                    break;
                default:
                    hydrochemistries = hydrochemistries.OrderBy(w => w.Id);
                    break;
            }
            ViewBag.SortOrder = SortOrder;

            var pager = new Pager(hydrochemistries.Count(), Page);

            var viewModel = new HydrochemistryIndexPageViewModel
            {
                Items = hydrochemistries.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };

            return View(viewModel);
        }

        // GET: Hydrochemistries/Details/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hydrochemistry = await _context.Hydrochemistry
                .SingleOrDefaultAsync(m => m.Id == id);
            if (hydrochemistry == null)
            {
                return NotFound();
            }

            return View(hydrochemistry);
        }

        // GET: Hydrochemistries/Create
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Hydrochemistries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Create([Bind("Id,LakeId,Year,Mineralization,TotalHardness,DissOxygWater,PercentOxygWater,pH,OrganicSubstances,Ca,Mg,NaK,Cl,HCO,SO,NH,NO2,NO3,PPO,Cu,Zn,Mn,Pb,Ni,Cd,Co,CIWP")] Hydrochemistry hydrochemistry)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hydrochemistry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(hydrochemistry);
        }

        // GET: Hydrochemistries/Edit/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hydrochemistry = await _context.Hydrochemistry.SingleOrDefaultAsync(m => m.Id == id);
            if (hydrochemistry == null)
            {
                return NotFound();
            }
            return View(hydrochemistry);
        }

        // POST: Hydrochemistries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LakeId,Year,Mineralization,TotalHardness,DissOxygWater,PercentOxygWater,pH,OrganicSubstances,Ca,Mg,NaK,Cl,HCO,SO,NH,NO2,NO3,PPO,Cu,Zn,Mn,Pb,Ni,Cd,Co,CIWP")] Hydrochemistry hydrochemistry)
        {
            if (id != hydrochemistry.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hydrochemistry);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HydrochemistryExists(hydrochemistry.Id))
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
            return View(hydrochemistry);
        }

        // GET: Hydrochemistries/Delete/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hydrochemistry = await _context.Hydrochemistry
                .SingleOrDefaultAsync(m => m.Id == id);
            if (hydrochemistry == null)
            {
                return NotFound();
            }

            return View(hydrochemistry);
        }

        // POST: Hydrochemistries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hydrochemistry = await _context.Hydrochemistry.SingleOrDefaultAsync(m => m.Id == id);
            _context.Hydrochemistry.Remove(hydrochemistry);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HydrochemistryExists(int id)
        {
            return _context.Hydrochemistry.Any(e => e.Id == id);
        }
    }
}
