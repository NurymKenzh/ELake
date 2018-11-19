using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ELake.Data;
using ELake.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Authorization;

namespace ELake.Controllers
{
    public class LakeSystemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;

        public LakeSystemsController(ApplicationDbContext context,
            IHostingEnvironment hostingEnvironment,
            IStringLocalizer<SharedResources> sharedLocalizer)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _sharedLocalizer = sharedLocalizer;
        }

        // GET: LakeSystems
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Index(string SortOrder,
            int? LakeSystemId,
            string NameKK,
            string NameRU,
            string NameEN,
            int? Page)
        {
            var lakeSystems = _context.LakeSystem
                .Where(w => true);

            ViewBag.LakeSystemIdFilter = LakeSystemId;
            ViewBag.NameKKFilter = NameKK;
            ViewBag.NameRUFilter = NameRU;
            ViewBag.NameENFilter = NameEN;

            ViewBag.LakeSystemIdSort = SortOrder == "LakeSystemId" ? "LakeSystemIdDesc" : "LakeSystemId";
            ViewBag.NameKKSort = SortOrder == "NameKK" ? "NameKKDesc" : "NameKK";
            ViewBag.NameRUSort = SortOrder == "NameRU" ? "NameRUDesc" : "NameRU";
            ViewBag.NameENSort = SortOrder == "NameEN" ? "NameENDesc" : "NameEN";

            if (LakeSystemId != null)
            {
                lakeSystems = lakeSystems.Where(w => w.LakeSystemId == LakeSystemId);
            }
            if (!string.IsNullOrEmpty(NameKK))
            {
                lakeSystems = lakeSystems.Where(w => w.NameKK.ToLower().Contains(NameKK.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameRU))
            {
                lakeSystems = lakeSystems.Where(w => w.NameRU.ToLower().Contains(NameRU.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameEN))
            {
                lakeSystems = lakeSystems.Where(w => w.NameEN.ToLower().Contains(NameEN.ToLower()));
            }

            switch (SortOrder)
            {
                case "LakeSystemId":
                    lakeSystems = lakeSystems.OrderBy(w => w.LakeSystemId);
                    break;
                case "LakeSystemIdDesc":
                    lakeSystems = lakeSystems.OrderByDescending(w => w.LakeSystemId);
                    break;
                case "NameKK":
                    lakeSystems = lakeSystems.OrderBy(w => w.NameKK);
                    break;
                case "NameKKDesc":
                    lakeSystems = lakeSystems.OrderByDescending(w => w.NameKK);
                    break;
                case "NameRU":
                    lakeSystems = lakeSystems.OrderBy(w => w.NameRU);
                    break;
                case "NameRUDesc":
                    lakeSystems = lakeSystems.OrderByDescending(w => w.NameRU);
                    break;
                case "NameEN":
                    lakeSystems = lakeSystems.OrderBy(w => w.NameEN);
                    break;
                case "NameENDesc":
                    lakeSystems = lakeSystems.OrderByDescending(w => w.NameEN);
                    break;
                default:
                    lakeSystems = lakeSystems.OrderBy(w => w.Id);
                    break;
            }
            ViewBag.SortOrder = SortOrder;

            var pager = new Pager(lakeSystems.Count(), Page);

            var viewModel = new LakeSystemIndexPageViewModel
            {
                Items = lakeSystems.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };

            return View(viewModel);
        }

        // GET: LakeSystems/Details/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lakeSystem = await _context.LakeSystem
                .SingleOrDefaultAsync(m => m.Id == id);
            if (lakeSystem == null)
            {
                return NotFound();
            }

            return View(lakeSystem);
        }

        // GET: LakeSystems/Create
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: LakeSystems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Create([Bind("Id,LakeSystemId,NameKK,NameRU,NameEN")] LakeSystem lakeSystem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lakeSystem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lakeSystem);
        }

        // GET: LakeSystems/Edit/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lakeSystem = await _context.LakeSystem.SingleOrDefaultAsync(m => m.Id == id);
            if (lakeSystem == null)
            {
                return NotFound();
            }
            return View(lakeSystem);
        }

        // POST: LakeSystems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LakeSystemId,NameKK,NameRU,NameEN")] LakeSystem lakeSystem)
        {
            if (id != lakeSystem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lakeSystem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LakeSystemExists(lakeSystem.Id))
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
            return View(lakeSystem);
        }

        // GET: LakeSystems/Delete/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lakeSystem = await _context.LakeSystem
                .SingleOrDefaultAsync(m => m.Id == id);
            if (lakeSystem == null)
            {
                return NotFound();
            }

            return View(lakeSystem);
        }

        // POST: LakeSystems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lakeSystem = await _context.LakeSystem.SingleOrDefaultAsync(m => m.Id == id);
            _context.LakeSystem.Remove(lakeSystem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LakeSystemExists(int id)
        {
            return _context.LakeSystem.Any(e => e.Id == id);
        }
    }
}
