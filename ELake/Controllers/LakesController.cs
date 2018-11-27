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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using OfficeOpenXml;

namespace ELake.Controllers
{
    public class LakesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;

        public LakesController(ApplicationDbContext context,
            IHostingEnvironment hostingEnvironment,
            IStringLocalizer<SharedResources> sharedLocalizer)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _sharedLocalizer = sharedLocalizer;
        }

        // GET: Lakes
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Index(string SortOrder,
            int? LakeId,
            string NameKK,
            string NameRU,
            string NameEN,
            string VHBKK,
            string VHBRU,
            string VHBEN,
            int? Page)
        {
            var lakes = _context.Lake
                .Where(w => true);

            ViewBag.LakeIdFilter = LakeId;
            ViewBag.NameKKFilter = NameKK;
            ViewBag.NameRUFilter = NameRU;
            ViewBag.NameENFilter = NameEN;
            ViewBag.VHBKKFilter = VHBKK;
            ViewBag.VHBRUFilter = VHBRU;
            ViewBag.VHBENFilter = VHBEN;

            ViewBag.LakeIdSort = SortOrder == "LakeId" ? "LakeIdDesc" : "LakeId";
            ViewBag.NameKKSort = SortOrder == "NameKK" ? "NameKKDesc" : "NameKK";
            ViewBag.NameRUSort = SortOrder == "NameRU" ? "NameRUDesc" : "NameRU";
            ViewBag.NameENSort = SortOrder == "NameEN" ? "NameENDesc" : "NameEN";
            ViewBag.VHBKKSort = SortOrder == "VHBKK" ? "VHBKKDesc" : "VHBKK";
            ViewBag.VHBRUSort = SortOrder == "VHBRU" ? "VHBRUDesc" : "VHBRU";
            ViewBag.VHBENSort = SortOrder == "VHBEN" ? "VHBENDesc" : "VHBEN";

            if (LakeId != null)
            {
                lakes = lakes.Where(w => w.LakeId == LakeId);
            }
            if (!string.IsNullOrEmpty(NameKK))
            {
                lakes = lakes.Where(w => w.NameKK.ToLower().Contains(NameKK.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameRU))
            {
                lakes = lakes.Where(w => w.NameRU.ToLower().Contains(NameRU.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameEN))
            {
                lakes = lakes.Where(w => w.NameEN.ToLower().Contains(NameEN.ToLower()));
            }
            if (!string.IsNullOrEmpty(VHBKK))
            {
                lakes = lakes.Where(w => w.VHBKK.ToLower().Contains(VHBKK.ToLower()));
            }
            if (!string.IsNullOrEmpty(VHBRU))
            {
                lakes = lakes.Where(w => w.VHBRU.ToLower().Contains(VHBRU.ToLower()));
            }
            if (!string.IsNullOrEmpty(VHBEN))
            {
                lakes = lakes.Where(w => w.VHBEN.ToLower().Contains(VHBEN.ToLower()));
            }

            switch (SortOrder)
            {
                case "LakeId":
                    lakes = lakes.OrderBy(w => w.LakeId);
                    break;
                case "LakeIdDesc":
                    lakes = lakes.OrderByDescending(w => w.LakeId);
                    break;
                case "NameKK":
                    lakes = lakes.OrderBy(w => w.NameKK);
                    break;
                case "NameKKDesc":
                    lakes = lakes.OrderByDescending(w => w.NameKK);
                    break;
                case "NameRU":
                    lakes = lakes.OrderBy(w => w.NameRU);
                    break;
                case "NameRUDesc":
                    lakes = lakes.OrderByDescending(w => w.NameRU);
                    break;
                case "NameEN":
                    lakes = lakes.OrderBy(w => w.NameEN);
                    break;
                case "NameENDesc":
                    lakes = lakes.OrderByDescending(w => w.NameEN);
                    break;
                case "VHBKK":
                    lakes = lakes.OrderBy(w => w.VHBKK);
                    break;
                case "VHBKKDesc":
                    lakes = lakes.OrderByDescending(w => w.VHBKK);
                    break;
                case "VHBRU":
                    lakes = lakes.OrderBy(w => w.VHBRU);
                    break;
                case "VHBRUDesc":
                    lakes = lakes.OrderByDescending(w => w.VHBRU);
                    break;
                case "VHBEN":
                    lakes = lakes.OrderBy(w => w.VHBEN);
                    break;
                case "VHBENDesc":
                    lakes = lakes.OrderByDescending(w => w.VHBEN);
                    break;
                default:
                    lakes = lakes.OrderBy(w => w.Id);
                    break;
            }
            ViewBag.SortOrder = SortOrder;

            var pager = new Pager(lakes.Count(), Page);

            var viewModel = new LakeIndexPageViewModel
            {
                Items = lakes.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };

            return View(viewModel);
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,LakeId,NameKK,NameRU,NameEN,VHBKK,VHBRU,VHBEN,VHU,LakeSystemId,Area2015,LakeShorelineLength2015,Longitude,Latitude")] Lake lake)
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
            ViewBag.LakeId = id;
            ViewBag.Lakes = new SelectList(
                _context.Lake
                    .Where(l => !string.IsNullOrEmpty(l.Name)).OrderBy(l => l.Name),
                "LakeId",
                "Name",
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
            ViewBag.LakeName = _context.Lake.FirstOrDefault(l => l.LakeId == id)?.Name;

            /////////////////////

            ViewBag.Adm1 = new SelectList(_context.KATO.Where(k => k.Level == 1).OrderBy(k => k.Name), "Id", "Name");
            ViewBag.VHB = new SelectList(_context.Lake.Select(l => l.VHB).Distinct().OrderBy(v => v).ToArray());
            ViewBag.System = new SelectList(_context.LakeSystem.OrderBy(s => s.Name), "LakeSystemId", "Name");

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
        public async Task<IActionResult> Create([Bind("Id,LakeId,NameKK,NameRU,NameEN,VHBKK,VHBRU,VHBEN,VHU,LakeSystemId,Area2015,LakeShorelineLength2015,Longitude,Latitude")] Lake lake)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lake);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lake);
        }

        [HttpPost]
        public ActionResult GetLakeInfo(int LakeId)
        {
            // Основная информация об озере
            Lake lake = _context.Lake.FirstOrDefault(l => l.LakeId == LakeId);
            string name = lake?.Name,
                longitude = lake?.Longitude,
                latitude = lake?.Latitude;
            List<KATO> katoes = new List<KATO>();
            foreach (var lakeKATO in _context.LakeKATO.Where(l => l.LakeId == LakeId))
            {
                katoes.Add(_context.KATO.FirstOrDefault(k => k.Id == lakeKATO.KATOId));
            }
            string adm1 = string.Join(", ", katoes.Where(k => k.Level == 1).Select(k => k.Name)),
                adm2 = string.Join(", ", katoes.Where(k => k.Level == 2).Select(k => k.Name));
            bool twoparts = false;
            // Общие гидрохимические показатели
            GeneralHydrochemicalIndicator[] ghitable1 = null,
                ghitable2 = null;
            if (LakeId > 0 && _context.GeneralHydrochemicalIndicator.Count(g => g.LakeId == LakeId)>0)
            {
                List<GeneralHydrochemicalIndicator> ghis = _context.GeneralHydrochemicalIndicator
                    .Where(g => g.LakeId == LakeId)
                    .ToList();
                if(ghis.Count(g => g.LakePart == LakePart.FreshPart) > 0 &&
                    ghis.Count(g => g.LakePart == LakePart.SaltyPart) > 0)
                {
                    twoparts = true;
                }
                ghitable1 = _context.GeneralHydrochemicalIndicator
                    .Where(g => g.LakeId == LakeId)
                    .Where(g => g.LakePart == LakePart.FullyPart || g.LakePart == LakePart.FreshPart)
                    .OrderBy(g => g.Year)
                    .ToArray();
                ghitable2 = _context.GeneralHydrochemicalIndicator
                    .Where(g => g.LakeId == LakeId)
                    .Where(g => g.LakePart == LakePart.SaltyPart)
                    .OrderBy(g => g.Year)
                    .ToArray();
            }
            // Ионно-солевой состав воды
            IonsaltWaterComposition[] iwctable1 = null,
                iwctable2 = null;
            if (LakeId > 0 && _context.IonsaltWaterComposition.Count(i => i.LakeId == LakeId) > 0)
            {
                List<IonsaltWaterComposition> iwcs = _context.IonsaltWaterComposition
                    .Where(i => i.LakeId == LakeId)
                    .ToList();
                if (iwcs.Count(i => i.LakePart == LakePart.FreshPart) > 0 &&
                    iwcs.Count(i => i.LakePart == LakePart.SaltyPart) > 0)
                {
                    twoparts = true;
                }
                iwctable1 = _context.IonsaltWaterComposition
                    .Where(i => i.LakeId == LakeId)
                    .Where(i => i.LakePart == LakePart.FullyPart || i.LakePart == LakePart.FreshPart)
                    .OrderBy(i => i.Year)
                    .ToArray();
                iwctable2 = _context.IonsaltWaterComposition
                    .Where(i => i.LakeId == LakeId)
                    .Where(i => i.LakePart == LakePart.SaltyPart)
                    .OrderBy(i => i.Year)
                    .ToArray();
            }
            // Токсикологические показатели
            ToxicologicalIndicator[] titable1 = null,
                titable2 = null;
            if (LakeId > 0 && _context.ToxicologicalIndicator.Count(i => i.LakeId == LakeId) > 0)
            {
                List<ToxicologicalIndicator> tis = _context.ToxicologicalIndicator
                    .Where(i => i.LakeId == LakeId)
                    .ToList();
                if (tis.Count(i => i.LakePart == LakePart.FreshPart) > 0 &&
                    tis.Count(i => i.LakePart == LakePart.SaltyPart) > 0)
                {
                    twoparts = true;
                }
                titable1 = _context.ToxicologicalIndicator
                    .Where(i => i.LakeId == LakeId)
                    .Where(i => i.LakePart == LakePart.FullyPart || i.LakePart == LakePart.FreshPart)
                    .OrderBy(i => i.Year)
                    .ToArray();
                titable2 = _context.ToxicologicalIndicator
                    .Where(i => i.LakeId == LakeId)
                    .Where(i => i.LakePart == LakePart.SaltyPart)
                    .OrderBy(i => i.Year)
                    .ToArray();
            }
            // Водный баланс
            WaterBalance[] wbtable = null;
            if (LakeId > 0 && _context.WaterBalance.Count(w => w.LakeId == LakeId) > 0)
            {
                List<WaterBalance> wbs = _context.WaterBalance
                    .Where(w => w.LakeId == LakeId)
                    .ToList();
                wbtable = _context.WaterBalance
                    .Where(w => w.LakeId == LakeId)
                    .OrderBy(w => w.Year)
                    .ToArray();
            }
            return Json(new
            {
                // Основная информация об озере
                name,
                longitude,
                latitude,
                adm1,
                adm2,
                // Общие гидрохимические показатели
                twoparts,
                ghitable1,
                ghitable2,
                // Ионно-солевой состав воды
                iwctable1,
                iwctable2,
                // Токсикологические показатели
                titable1,
                titable2,
                // Водный баланс
                wbtable
            });
        }

        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Upload(bool FirstRowHeader, IFormFile File)
        {
            try
            {
                string sContentRootPath = _hostingEnvironment.WebRootPath;
                sContentRootPath = Path.Combine(sContentRootPath, "Uploads");
                DirectoryInfo di = new DirectoryInfo(sContentRootPath);
                foreach (FileInfo filed in di.GetFiles())
                {
                    try
                    {
                        filed.Delete();
                    }
                    catch
                    {
                    }
                }
                string path_filename = Path.Combine(sContentRootPath, Path.GetFileName(File.FileName));
                using (var stream = new FileStream(Path.GetFullPath(path_filename), FileMode.Create))
                {
                    await File.CopyToAsync(stream);
                }
                FileInfo fileinfo = new FileInfo(Path.Combine(sContentRootPath, Path.GetFileName(path_filename)));
                using (ExcelPackage package = new ExcelPackage(fileinfo))
                {
                    int start_row = 1;
                    if (FirstRowHeader)
                    {
                        start_row++;
                    }
                    List<Lake> lakes = new List<Lake>();
                    for (int i = start_row; ; i++)
                    {
                        if (package.Workbook.Worksheets.FirstOrDefault().Cells[i, 1].Value == null)
                        {
                            break;
                        }
                        Lake lake = new Lake();

                        try
                        {
                            lake.LakeId = Convert.ToInt32(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 1].Value);
                            lake.NameKK = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 2].Value?.ToString();
                            lake.NameRU = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 3].Value?.ToString();
                            lake.NameEN = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 4].Value?.ToString();
                            lake.VHBKK = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 5].Value?.ToString();
                            lake.VHBRU = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 6].Value?.ToString();
                            lake.VHBEN = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 7].Value?.ToString();
                            lake.VHU = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 8].Value?.ToString();
                            lake.LakeSystemId = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 9].Value == null ?
                                (int?)null :
                                Convert.ToInt32(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 9].Value);
                            lake.Area2015 = Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 10].Value);
                            lake.LakeShorelineLength2015 = Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 11].Value);
                            lake.Longitude = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 12].Value?.ToString();
                            lake.Latitude = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 13].Value?.ToString();
                        }
                        catch (Exception e)
                        {
                            ViewBag.Error = $"{_sharedLocalizer["Row"]} {i.ToString()}: " + e.Message + (e.InnerException == null ? "" : ": " + e.InnerException.Message);
                            break;
                        }

                        lakes.Add(lake);
                        _context.Add(lakes.LastOrDefault());
                    }
                    if (string.IsNullOrEmpty(ViewBag.Error))
                    {
                        _context.SaveChanges();
                        ViewBag.Report = $"{_sharedLocalizer["UploadedCount"]}: {lakes.Count()}";
                    }
                }
                foreach (FileInfo filed in di.GetFiles())
                {
                    try
                    {
                        filed.Delete();
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception e)
            {
                if (File != null)
                {
                    ViewBag.Error = e.Message + (e.InnerException == null ? "" : ": " + e.InnerException.Message);
                }
            }
            return View();
        }

        //[HttpPost]
        //public ActionResult something(string userGuid)
        //{
        //    var p = _context.Lake.Where(l => !string.IsNullOrEmpty(l.Name)).Take(4).ToArray();
        //    return Json(new
        //    {
        //        p
        //    });
        //}

        [HttpPost]
        public ActionResult GetLakes(string Search, int? Adm1KATOId, int? Adm2KATOId, string VHB, string VHU, int? LakeSystem)
        {
            if (Search == null)
            {
                Search = "";
            }
            var lakes = _context.Lake
                .Where(l => l.Name.ToLower().Contains(Search.ToLower()))
                .OrderBy(l => l.Name)
                .ToArray();
            if(Adm1KATOId != null)
            {
                lakes = lakes.Where(l => _context.LakeKATO.Where(lk => lk.KATOId == Adm1KATOId).Select(lk => lk.LakeId).Contains(l.LakeId)).ToArray();
            }
            if (Adm2KATOId != null)
            {
                lakes = lakes.Where(l => _context.LakeKATO.Where(lk => lk.KATOId == Adm2KATOId).Select(lk => lk.LakeId).Contains(l.LakeId)).ToArray();
            }
            if (!string.IsNullOrEmpty(VHB))
            {
                lakes = lakes.Where(l => l.VHB == VHB).ToArray();
            }
            if (!string.IsNullOrEmpty(VHU))
            {
                lakes = lakes.Where(l => l.VHU == VHU).ToArray();
            }
            if (LakeSystem != null)
            {
                lakes = lakes.Where(l => l.LakeSystemId == LakeSystem).ToArray();
            }
            return Json(new
            {
                lakes
            });
        }
        
        public ActionResult GetLakeGroup(int LakeId)
        {
            int max = Convert.ToInt32(LakeId.ToString().Substring(0, LakeId.ToString().Length - 1) + "9"),
                min = Convert.ToInt32(LakeId.ToString().Substring(0, LakeId.ToString().Length - 1) + "1");
            var lakes = _context.Lake
                .Where(l => l.LakeId >= min && l.LakeId <= max)
                .OrderBy(l => l.Name)
                .ToArray();
            return Json(new
            {
                lakes
            });
        }

        [HttpPost]
        public ActionResult GetAdm1(string Search)
        {
            if(!string.IsNullOrEmpty(Search))
            {
                List<int> lakeIds = _context.Lake
                    .Where(l => l.Name.ToLower().Contains(Search.ToLower()))
                    .Select(l => l.LakeId)
                    .Distinct()
                    .ToList();
                List<int> katoIds = _context.LakeKATO
                    .Where(lk => lakeIds.Contains(lk.LakeId))
                    .Select(lk => lk.KATOId)
                    .Distinct()
                    .ToList();
                var adm1 = _context.KATO
                    .Where(k => k.Level == 1)
                    .Where(k => katoIds.Contains(k.Id))
                    .OrderBy(k => k.Name)
                    .ToArray();
                return Json(new
                {
                    adm1
                });
            }
            else
            {
                var adm1 = _context.KATO
                    .Where(k => k.Level == 1)
                    .OrderBy(k => k.Name)
                    .ToArray();
                return Json(new
                {
                    adm1
                });
            }            
        }

        [HttpPost]
        public ActionResult GetAdm2(string Search, int? Adm1KATOId)
        {
            if (!string.IsNullOrEmpty(Search))
            {
                List<int> lakeIds = _context.Lake
                    .Where(l => l.Name.ToLower().Contains(Search.ToLower()))
                    .Select(l => l.LakeId)
                    .Distinct()
                    .ToList();
                List<int> katoIds = _context.LakeKATO
                    .Where(lk => lakeIds.Contains(lk.LakeId))
                    .Select(lk => lk.KATOId)
                    .Distinct()
                    .ToList();
                KATO adm1 = _context.KATO.FirstOrDefault(k => k.Id == Adm1KATOId);
                string adm1Number = adm1 == null ? null : adm1?.Number.Substring(0, 2);
                var adm2 = _context.KATO
                    .Where(k => katoIds.Contains(k.Id))
                    .Where(k => k.Level == 2 && k.Number.Substring(0, 2) == adm1Number)
                    .OrderBy(k => k.Name)
                    .ToArray();
                return Json(new
                {
                    adm2
                });
            }
            else
            {
                KATO adm1 = _context.KATO.FirstOrDefault(k => k.Id == Adm1KATOId);
                string adm1Number = adm1 == null ? null : adm1?.Number.Substring(0, 2);
                var adm2 = _context.KATO
                    .Where(k => k.Level == 2 && k.Number.Substring(0, 2) == adm1Number)
                    .OrderBy(k => k.Name)
                    .ToArray();
                return Json(new
                {
                    adm2
                });
            }
        }

        [HttpPost]
        public ActionResult GetVHB(string Search)
        {
            if (!string.IsNullOrEmpty(Search))
            {
                var vhb = _context.Lake
                    .Where(l => l.Name.ToLower().Contains(Search.ToLower()))
                    .Select(l => l.VHB)
                    .Distinct()
                    .OrderBy(v => v)
                    .ToArray();
                return Json(new
                {
                    vhb
                });
            }
            else
            {
                var vhb = _context.Lake
                    .Select(l => l.VHB)
                    .Distinct()
                    .OrderBy(v => v)
                    .ToArray();
                return Json(new
                {
                    vhb
                });
            }
        }

        [HttpPost]
        public ActionResult GetVHU(string Search, string VHB)
        {
            if(!string.IsNullOrEmpty(Search))
            {
                var vhu = _context.Lake
                .Where(l => l.Name.ToLower().Contains(Search.ToLower()))
                .Where(l => l.VHB == VHB && !string.IsNullOrEmpty(l.VHU))
                .Select(l => l.VHU)
                .Distinct()
                .OrderBy(v => v)
                .ToArray();
                return Json(new
                {
                    vhu
                });
            }
            else
            {
                var vhu = _context.Lake
                    .Where(l => l.VHB == VHB && !string.IsNullOrEmpty(l.VHU))
                    .Select(l => l.VHU)
                    .Distinct()
                    .OrderBy(v => v)
                    .ToArray();
                return Json(new
                {
                    vhu
                });
            }
        }

        [HttpPost]
        public ActionResult GetSystem(string Search)
        {
            if (!string.IsNullOrEmpty(Search))
            {
                List<int?> lakeSystemIds = _context.Lake
                    .Where(l => l.LakeSystemId != null)
                    .Where(l => l.Name.ToLower().Contains(Search.ToLower()))
                    .Select(l => l.LakeSystemId)
                    .Distinct()
                    .ToList();
                var system = _context.LakeSystem
                    .Where(s => lakeSystemIds.Contains(s.LakeSystemId))
                    .OrderBy(s => s.Name)
                    .ToArray();
                return Json(new
                {
                    system
                });
            }
            else
            {
                var system = _context.LakeSystem
                    .OrderBy(k => k.Name)
                    .ToArray();
                return Json(new
                {
                    system
                });
            }
        }
    }
}