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
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Builder;

namespace ELake.Controllers
{
    public class LakesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;

        public LakesController(ApplicationDbContext context,
            IStringLocalizer<SharedResources> sharedLocalizer)
        {
            _context = context;
            _sharedLocalizer = sharedLocalizer;
        }

        // GET: Lakes
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Lake.ToListAsync());
        }

        // GET: Lakes/Details/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lake = await _context.Lake
                .SingleOrDefaultAsync(m => m.Id == id);
            if (lake == null)
            {
                return NotFound();
            }

            return View(lake);
        }

        public static int Max(params int[] values)
        {
            return Enumerable.Max(values);
        }

        public static int Min(params int[] values)
        {
            return Enumerable.Min(values);
        }

        // GET: Lakes/Details/5
        //[Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Map(int? id)
        {
            //if (id == null)
            //{
            //    return NotFound();
            //}

            //var lake = await _context.Lake
            //    .SingleOrDefaultAsync(m => m.LakeId == id);
            //if (lake == null)
            //{
            //    return NotFound();
            //}

            ViewBag.LakeId = id;
            ViewBag.Lakes = new SelectList(
                _context.Lake
                    .Where(l => !string.IsNullOrEmpty(l.NameRU)).OrderBy(l => l.NameRU),
                "LakeId",
                "NameRU",
                id);
            ViewBag.MinYear = Min(_context.WaterLevel.Min(w => w.Year),
                _context.SurfaceFlow.Min(w => w.Year),
                _context.Precipitation.Min(w => w.Year),
                _context.UndergroundFlow.Min(w => w.Year),
                _context.SurfaceOutflow.Min(w => w.Year),
                _context.Evaporation.Min(w => w.Year),
                _context.UndergroundOutflow.Min(w => w.Year),
                _context.Hydrochemistry.Min(w => w.Year));
            ViewBag.MaxYear = Max(_context.WaterLevel.Max(w => w.Year),
                _context.SurfaceFlow.Max(w => w.Year),
                _context.Precipitation.Max(w => w.Year),
                _context.UndergroundFlow.Max(w => w.Year),
                _context.SurfaceOutflow.Max(w => w.Year),
                _context.Evaporation.Max(w => w.Year),
                _context.UndergroundOutflow.Max(w => w.Year),
                _context.Hydrochemistry.Max(w => w.Year));
            //ViewBag.LakesLayer = _context.Layer.FirstOrDefault(l => l.Lake).GeoServerName;
            string[] DataTypes = new string[] {
                "WaterLevels",
                "SurfaceFlows",
                "Precipitations",
                "UndergroundFlows",
                "SurfaceOutflows",
                "Evaporations",
                "UndergroundOutflows",
                "HydrochemistryMineralizations",
                "HydrochemistryTotalHardnesss",
                "HydrochemistryDissOxygWaters",
                "HydrochemistryPercentOxygWaters",
                "HydrochemistrypHs",
                "HydrochemistryOrganicSubstancess",
                "HydrochemistryCas",
                "HydrochemistryMgs",
                "HydrochemistryNaKs",
                "HydrochemistryCls",
                "HydrochemistryHCOs",
                "HydrochemistrySOs",
                "HydrochemistryNHs",
                "HydrochemistryNO2s",
                "HydrochemistryNO3s",
                "HydrochemistryPPOs",
                "HydrochemistryCus",
                "HydrochemistryZns",
                "HydrochemistryMns",
                "HydrochemistryPbs",
                "HydrochemistryNis",
                "HydrochemistryCds",
                "HydrochemistryCos",
                "HydrochemistryCIWPs"
            };
            ViewBag.DataType = DataTypes.Select(r => new SelectListItem { Text = _sharedLocalizer[r], Value = r });
            ViewBag.LakesLayer = _context.Layer.FirstOrDefault(l => l.Lake);

            return View();
        }

        // GET: Lakes/Create
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Lakes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Create([Bind("Id,LakeId,NameKK,NameRU,NameEN")] Lake lake)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lake);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lake);
        }

        // GET: Lakes/Edit/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lake = await _context.Lake.SingleOrDefaultAsync(m => m.Id == id);
            if (lake == null)
            {
                return NotFound();
            }
            return View(lake);
        }

        // POST: Lakes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LakeId,NameKK,NameRU,NameEN")] Lake lake)
        {
            if (id != lake.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lake);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LakeExists(lake.Id))
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
            return View(lake);
        }

        // GET: Lakes/Delete/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lake = await _context.Lake
                .SingleOrDefaultAsync(m => m.Id == id);
            if (lake == null)
            {
                return NotFound();
            }

            return View(lake);
        }

        // POST: Lakes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lake = await _context.Lake.SingleOrDefaultAsync(m => m.Id == id);
            _context.Lake.Remove(lake);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LakeExists(int id)
        {
            return _context.Lake.Any(e => e.Id == id);
        }

        //[HttpPost]
        //public JsonResult GetLakeInfo(int LayerId)
        //{
        //    string s1 = "s1",
        //        s2 = "s2";

        //    JsonResult result = new JsonResult(link);
        //    return result;
        //}

        [HttpPost]
        public ActionResult GetLakeInfo(int LakeId)
        {
            Lake lake = _context.Lake.FirstOrDefault(l => l.LakeId == LakeId);
            string NameKK = lake?.NameKK,
                NameEN = lake?.NameEN,
                NameRU = lake?.NameRU,
                vhb = lake?.VHBKK,
                vhu = lake?.VHU,
                longitude = lake?.Longitude,
                latitude = lake?.Latitude;
            decimal? area = lake?.Area,
                lakeshorelinelength2015 = lake?.LakeShorelineLength2015;
            string language = new RequestLocalizationOptions().DefaultRequestCulture.Culture.Name;
            if (language == "ru")
            {
                vhb = lake?.VHBRU;
            }
            if (language == "en")
            {
                vhb = lake?.VHBEN;
            }
            List<KATO> katoes = new List<KATO>();
            foreach (var lakeKATO in _context.LakeKATO.Where(l => l.LakeId == LakeId))
            {
                katoes.Add(_context.KATO.FirstOrDefault(k => k.Id == lakeKATO.KATOId));
            }
            string adm1 = "",
                adm2 = "",
                adm3 = "";
            if (katoes.Count(k => k.Level == 3) > 0)
            {
                adm3 = katoes.FirstOrDefault(k => k.Level == 3).Name;

                if (katoes.Count(k => k.Level == 2) > 0)
                {
                    adm2 = katoes.FirstOrDefault(k => k.Level == 2).Name;
                }
                else
                {
                    //adm2 = _context.KATO.FirstOrDefault(k => k.Level == 2 && k.Number.Substring(1, 4) == katoes.FirstOrDefault(kk => kk.Level == 3).Number.Substring(1, 4)).Name;
                }

                if (katoes.Count(k => k.Level == 1) > 0)
                {
                    adm1 = katoes.FirstOrDefault(k => k.Level == 1).Name;
                }
            }
            return Json(new
            {
                NameKK,
                NameEN,
                NameRU,
                vhb,
                vhu,
                area,
                lakeshorelinelength2015,
                longitude,
                latitude,
                adm1,
                adm2,
                adm3
            });
        }
    }
}