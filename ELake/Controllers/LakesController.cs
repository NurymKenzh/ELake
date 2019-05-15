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
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;
using Microsoft.CodeAnalysis.Emit;
using System.Runtime.Loader;

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
            // Глобальная база озер
            LakesGlobalData lakesGlobalData = _context.LakesGlobalData.FirstOrDefault(l => l.LakeId == LakeId);
            // Архивные данные озер
            LakesArchiveData lakesArchiveData = _context.LakesArchiveData.FirstOrDefault(l => l.LakeId == LakeId);
            // Основная информация об озере 2
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
                wbtable = _context.WaterBalance
                    .Where(w => w.LakeId == LakeId)
                    .OrderBy(w => w.Year)
                    .ToArray();
            }
            // Уровни воды озер
            WaterLevel[] wltable = null;
            if (LakeId > 0 && _context.WaterLevel.Count(w => w.LakeId == LakeId) > 0)
            {
                wltable = _context.WaterLevel
                    .Where(w => w.LakeId == LakeId)
                    .OrderBy(w => w.Year)
                    .ToArray();
            }
            // Данные по батиграфической и объемной кривой
            BathigraphicAndVolumetricCurveData[] bavcdtable = null;
            if (LakeId > 0 && _context.BathigraphicAndVolumetricCurveData.Count(w => w.LakeId == LakeId) > 0)
            {
                bavcdtable = _context.BathigraphicAndVolumetricCurveData
                    .Where(w => w.LakeId == LakeId)
                    .OrderBy(w => w.WaterLevel)
                    .ToArray();
            }
            // Переходы
            //Transition[] trtable = null;
            //if (LakeId > 0 && _context.Transition.Count(w => w.LakeId == LakeId) > 0)
            //{
            //    List<Transition> trs = _context.Transition
            //        .Where(w => w.LakeId == LakeId)
            //        .ToList();
            //    trtable = _context.Transition
            //        .Where(w => w.LakeId == LakeId)
            //        .OrderBy(w => w.)
            //        .ToArray();
            //}
            // Димнамика площадей
            DynamicsLakeArea[] pasdtable = null;
            if (LakeId > 0 && _context.DynamicsLakeArea.Count(w => w.LakeId == LakeId) > 0)
            {
                pasdtable = _context.DynamicsLakeArea
                    .Where(w => w.LakeId == LakeId)
                    .OrderBy(w => w.Year)
                    .ToArray();
            }
            // Изменение состояние воды
            Transition transition = null;
            if (LakeId > 0 && _context.Transition.Count(w => w.LakeId == LakeId) > 0)
            {
                transition = _context.Transition
                    .FirstOrDefault(w => w.LakeId == LakeId);
            }
            // Присутствие воды в месяцах
            Seasonalit seasonalit = null;
            if (LakeId > 0 && _context.Seasonalit.Count(w => w.LakeId == LakeId) > 0)
            {
                seasonalit = _context.Seasonalit
                    .FirstOrDefault(w => w.LakeId == LakeId);
            }
            return Json(new
            {
                // Основная информация об озере
                lake,
                // Глобальная база озер
                lakesGlobalData,
                // Архивные данные озер
                lakesArchiveData,
                // Основная информация об озере 2
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
                wbtable,
                // Уровни воды озер
                wltable,
                // Данные по батиграфической и объемной кривой
                bavcdtable,
                // Димнамика площадей
                pasdtable,
                // Изменение состояние воды
                transition,
                // Присутствие воды в месяцах
                seasonalit
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

        // GET: Lakes/Details/5
        [Authorize]
        public async Task<IActionResult> Analytics()
        {
            ViewBag.Adm1 = new SelectList(_context.KATO.Where(k => k.Level == 1).OrderBy(k => k.Name), "Id", "Name");
            return View();
        }

        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> CreateFileToAnalytics()
        {
            DateTime start = DateTime.Now;

            string sContentRootPath = _hostingEnvironment.WebRootPath;
            sContentRootPath = Path.Combine(sContentRootPath, "Analytics");
            string assemblyName = "lakes.txt";
            string filePopulateLakes = Path.Combine(sContentRootPath, assemblyName);
            using (StreamWriter file = new StreamWriter(filePopulateLakes))
            {
                List<int> years = _context.WaterBalance.Select(w => w.Year).Distinct().ToList();
                years.AddRange(_context.WaterBalance.Select(w => w.Year).Distinct().ToList());
                years.AddRange(_context.WaterLevel.Select(w => w.Year).Distinct().ToList());
                years.AddRange(_context.GeneralHydrochemicalIndicator.Select(w => w.Year).Distinct().ToList());
                years.AddRange(_context.IonsaltWaterComposition.Select(w => w.Year).Distinct().ToList());
                years.AddRange(_context.ToxicologicalIndicator.Select(w => w.Year).Distinct().ToList());
                years.AddRange(_context.DynamicsLakeArea.Select(w => w.Year).Distinct().ToList());
                years = years.OrderBy(y => y).Distinct().ToList();

                foreach (Lake lake in _context.Lake)
                {
                    LakesArchiveData lakesArchiveData = _context.LakesArchiveData
                        .FirstOrDefault(l => l.LakeId == lake.LakeId);
                    LakesGlobalData lakesGlobalData = _context.LakesGlobalData
                        .FirstOrDefault(l => l.LakeId == lake.LakeId);
                    Transition transition = _context.Transition
                        .FirstOrDefault(l => l.LakeId == lake.LakeId);
                    Seasonalit seasonalit = _context.Seasonalit
                        .FirstOrDefault(l => l.LakeId == lake.LakeId);
                    string line = lake.Id.ToString();
                    // CreateFile 1
                    line += "\t" + PopulateDecimal(lake.LakeId);
                    line += "\t" + 0.ToString();
                    line += "\t" + PopulateDecimal(lake.Area2015);
                    line += "\t" + PopulateDecimal(lake.LakeShorelineLength2015);

                    line += "\t" + PopulateDecimal(lakesArchiveData?.LakeLength);
                    line += "\t" + PopulateDecimal(lakesArchiveData?.LakeShorelineLength);
                    line += "\t" + PopulateDecimal(lakesArchiveData?.LakeMirrorArea);
                    line += "\t" + PopulateDecimal(lakesArchiveData?.LakeAbsoluteHeight);
                    line += "\t" + PopulateDecimal(lakesArchiveData?.LakeWidth);
                    line += "\t" + PopulateDecimal(lakesArchiveData?.LakeMaxDepth);
                    line += "\t" + PopulateDecimal(lakesArchiveData?.LakeWaterMass);
                    //12
                    line += "\t" + PopulateDecimal(lakesGlobalData?.Lake_area);
                    line += "\t" + PopulateDecimal(lakesGlobalData?.Shore_len);
                    line += "\t" + PopulateDecimal(lakesGlobalData?.Shore_dev);
                    line += "\t" + PopulateDecimal(lakesGlobalData?.Vol_total);
                    line += "\t" + PopulateDecimal(lakesGlobalData?.Depth_avg);
                    line += "\t" + PopulateDecimal(lakesGlobalData?.Dis_avg);
                    line += "\t" + PopulateDecimal(lakesGlobalData?.Res_time);
                    line += "\t" + PopulateDecimal(lakesGlobalData?.Elevation);
                    line += "\t" + PopulateDecimal(lakesGlobalData?.Slope_100);
                    line += "\t" + PopulateDecimal(lakesGlobalData?.Wshd_area);
                    //22
                    line += "\t" + PopulateDecimal(transition?.NoСhange);
                    line += "\t" + PopulateDecimal(transition?.Permanent);
                    line += "\t" + PopulateDecimal(transition?.NewPermanent);
                    line += "\t" + PopulateDecimal(transition?.LostPermanent);
                    line += "\t" + PopulateDecimal(transition?.Seasonal);
                    line += "\t" + PopulateDecimal(transition?.NewSeasonal);
                    line += "\t" + PopulateDecimal(transition?.LostSeasonal);
                    line += "\t" + PopulateDecimal(transition?.SeasonalToPermanent);
                    line += "\t" + PopulateDecimal(transition?.PermanentToDeasonal);
                    line += "\t" + PopulateDecimal(transition?.EphemeralPermanent);
                    line += "\t" + PopulateDecimal(transition?.EphemeralSeasonal);
                    line += "\t" + PopulateDecimal(transition?.MaximumWater);
                    line += "\t" + PopulateDecimal(transition?.PermanentPer);
                    line += "\t" + PopulateDecimal(transition?.SeasonalPer);
                    //36
                    line += "\t" + PopulateDecimal(seasonalit?.NoData);
                    line += "\t" + PopulateDecimal(seasonalit?.January);
                    line += "\t" + PopulateDecimal(seasonalit?.February);
                    line += "\t" + PopulateDecimal(seasonalit?.March);
                    line += "\t" + PopulateDecimal(seasonalit?.April);
                    line += "\t" + PopulateDecimal(seasonalit?.May);
                    line += "\t" + PopulateDecimal(seasonalit?.June);
                    line += "\t" + PopulateDecimal(seasonalit?.July);
                    line += "\t" + PopulateDecimal(seasonalit?.August);
                    line += "\t" + PopulateDecimal(seasonalit?.September);
                    line += "\t" + PopulateDecimal(seasonalit?.October);
                    line += "\t" + PopulateDecimal(seasonalit?.November);
                    line += "\t" + PopulateDecimal(seasonalit?.December);
                    //49
                    List<WaterBalance> waterBalances = _context.WaterBalance.Where(i => i.LakeId == lake.LakeId).ToList();
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.SurfaceFlow).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.SurfaceOutflow).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.UndergroundFlow).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.UndergroundOutflow).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.Precipitation).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.Evaporation).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.WaterBalanceReceipt).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.WaterBalanceExpenditure).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.Discrepancy).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.SurfaceFlowPer).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.UndergroundFlowPer).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.PrecipitationPer).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.SurfaceOutflowPer).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.UndergroundOutflowPer).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.EvaporationPer).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.WaterBalanceReceiptPer).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.WaterBalanceExpenditurePer).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.SurfaceFlow).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.SurfaceOutflow).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.UndergroundFlow).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.UndergroundOutflow).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.Precipitation).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.Evaporation).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.WaterBalanceReceipt).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.WaterBalanceExpenditure).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.Discrepancy).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.SurfaceFlowPer).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.UndergroundFlowPer).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.PrecipitationPer).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.SurfaceOutflowPer).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.UndergroundOutflowPer).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.EvaporationPer).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.WaterBalanceReceiptPer).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.WaterBalanceExpenditurePer).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.SurfaceFlow).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.SurfaceOutflow).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.UndergroundFlow).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.UndergroundOutflow).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.Precipitation).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.Evaporation).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.WaterBalanceReceipt).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.WaterBalanceExpenditure).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.Discrepancy).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.SurfaceFlowPer).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.UndergroundFlowPer).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.PrecipitationPer).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.SurfaceOutflowPer).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.UndergroundOutflowPer).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.EvaporationPer).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.WaterBalanceReceiptPer).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(waterBalances.Select(i => i.WaterBalanceExpenditurePer).Where(i => i != null).DefaultIfEmpty(0).Min());
                    //100
                    List<WaterLevel> waterLevels = _context.WaterLevel.Where(i => i.LakeId == lake.LakeId).ToList();
                    line += "\t" + PopulateDecimal(waterLevels.Select(i => i.WaterLavelM).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(waterLevels.Select(i => i.WaterLavelM).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(waterLevels.Select(i => i.WaterLavelM).Where(i => i != null).DefaultIfEmpty(0).Min());
                    //103
                    List<BathigraphicAndVolumetricCurveData> bathigraphicAndVolumetricCurveDatas = _context.BathigraphicAndVolumetricCurveData.Where(i => i.LakeId == lake.LakeId).ToList();
                    line += "\t" + PopulateDecimal(bathigraphicAndVolumetricCurveDatas.Select(i => i.WaterLevel).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(bathigraphicAndVolumetricCurveDatas.Select(i => i.LakeArea).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(bathigraphicAndVolumetricCurveDatas.Select(i => i.WaterMassVolume).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(bathigraphicAndVolumetricCurveDatas.Select(i => i.WaterLevel).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(bathigraphicAndVolumetricCurveDatas.Select(i => i.LakeArea).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(bathigraphicAndVolumetricCurveDatas.Select(i => i.WaterMassVolume).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(bathigraphicAndVolumetricCurveDatas.Select(i => i.WaterLevel).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(bathigraphicAndVolumetricCurveDatas.Select(i => i.LakeArea).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(bathigraphicAndVolumetricCurveDatas.Select(i => i.WaterMassVolume).Where(i => i != null).DefaultIfEmpty(0).Min());
                    //112
                    List<GeneralHydrochemicalIndicator> generalHydrochemicalIndicators = _context.GeneralHydrochemicalIndicator.Where(i => i.LakeId == lake.LakeId).ToList();
                    line += "\t" + PopulateDecimal(generalHydrochemicalIndicators.Select(i => i.Mineralization).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(generalHydrochemicalIndicators.Select(i => i.TotalHardness).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(generalHydrochemicalIndicators.Select(i => i.DissOxygWater).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(generalHydrochemicalIndicators.Select(i => i.PercentOxygWater).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(generalHydrochemicalIndicators.Select(i => i.pH).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(generalHydrochemicalIndicators.Select(i => i.OrganicSubstances).Where(i => i != null).DefaultIfEmpty(0).Average());
                    if(lake.LakeId == 705110)
                    {
                        int yet = 4;
                    }
                    line += "\t" + PopulateDecimal(generalHydrochemicalIndicators.Select(i => i.Mineralization).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(generalHydrochemicalIndicators.Select(i => i.TotalHardness).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(generalHydrochemicalIndicators.Select(i => i.DissOxygWater).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(generalHydrochemicalIndicators.Select(i => i.PercentOxygWater).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(generalHydrochemicalIndicators.Select(i => i.pH).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(generalHydrochemicalIndicators.Select(i => i.OrganicSubstances).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(generalHydrochemicalIndicators.Select(i => i.Mineralization).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(generalHydrochemicalIndicators.Select(i => i.TotalHardness).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(generalHydrochemicalIndicators.Select(i => i.DissOxygWater).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(generalHydrochemicalIndicators.Select(i => i.PercentOxygWater).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(generalHydrochemicalIndicators.Select(i => i.pH).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(generalHydrochemicalIndicators.Select(i => i.OrganicSubstances).Where(i => i != null).DefaultIfEmpty(0).Min());

                    List<IonsaltWaterComposition> ionsaltWaterCompositions = _context.IonsaltWaterComposition.Where(i => i.LakeId == lake.LakeId).ToList();
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.CaMgEq).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.MgMgEq).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.NaKMgEq).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.ClMgEq).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.HCOMgEq).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.SOMgEq).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.CaMg).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.MgMg).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.NaKMg).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.ClMg).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.HCOMg).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.SOMg).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.CationsSumMgEq).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.AnionsSumMgEq).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.IonsSumMgEq).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.CaPerEq).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.MgPerEq).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.NaKPerEq).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.ClPerEq).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.HCOPerEq).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.SOPerEq).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.CaMgEq).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.MgMgEq).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.NaKMgEq).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.ClMgEq).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.HCOMgEq).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.SOMgEq).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.CaMg).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.MgMg).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.NaKMg).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.ClMg).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.HCOMg).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.SOMg).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.CationsSumMgEq).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.AnionsSumMgEq).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.IonsSumMgEq).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.CaPerEq).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.MgPerEq).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.NaKPerEq).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.ClPerEq).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.HCOPerEq).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.SOPerEq).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.CaMgEq).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.MgMgEq).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.NaKMgEq).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.ClMgEq).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.HCOMgEq).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.SOMgEq).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.CaMg).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.MgMg).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.NaKMg).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.ClMg).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.HCOMg).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.SOMg).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.CationsSumMgEq).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.AnionsSumMgEq).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.IonsSumMgEq).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.CaPerEq).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.MgPerEq).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.NaKPerEq).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.ClPerEq).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.HCOPerEq).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(ionsaltWaterCompositions.Select(i => i.SOPerEq).Where(i => i != null).DefaultIfEmpty(0).Min());

                    List<ToxicologicalIndicator> toxicologicalIndicators = _context.ToxicologicalIndicator.Where(i => i.LakeId == lake.LakeId).ToList();
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.NH4).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.NO2).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.NO3).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.PPO4).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.Cu).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.Zn).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.Mn).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.Pb).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.Ni).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.Cd).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.Co).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.IZVNH4).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.IZVNO2).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.IZVNO3).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.IZVPPO4).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.IZVCu).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.IZVZn).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.IZVMn).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.IZVPb).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.IZVNi).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.IZVCd).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.IZVCo).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.KIZVkoNH4).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.KIZVkoNO2).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.KIZVkoNO3).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.KIZVkoPPO4).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.KIZVkoCu).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.KIZVkoZn).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.KIZVkoMn).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.KIZVkoPb).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.KIZVkoNi).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.KIZVkoCd).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.KIZVkoCo).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.KIZV).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.NH4).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.NO2).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.NO3).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.PPO4).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.Cu).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.Zn).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.Mn).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.Pb).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.Ni).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.Cd).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.Co).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.IZVNH4).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.IZVNO2).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.IZVNO3).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.IZVPPO4).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.IZVCu).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.IZVZn).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.IZVMn).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.IZVPb).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.IZVNi).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.IZVCd).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.IZVCo).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.KIZVkoNH4).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.KIZVkoNO2).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.KIZVkoNO3).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.KIZVkoPPO4).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.KIZVkoCu).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.KIZVkoZn).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.KIZVkoMn).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.KIZVkoPb).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.KIZVkoNi).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.KIZVkoCd).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.KIZVkoCo).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.KIZV).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.NH4).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.NO2).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.NO3).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.PPO4).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.Cu).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.Zn).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.Mn).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.Pb).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.Ni).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.Cd).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.Co).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.IZVNH4).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.IZVNO2).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.IZVNO3).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.IZVPPO4).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.IZVCu).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.IZVZn).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.IZVMn).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.IZVPb).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.IZVNi).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.IZVCd).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.IZVCo).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.KIZVkoNH4).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.KIZVkoNO2).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.KIZVkoNO3).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.KIZVkoPPO4).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.KIZVkoCu).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.KIZVkoZn).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.KIZVkoMn).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.KIZVkoPb).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.KIZVkoNi).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.KIZVkoCd).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.KIZVkoCo).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(toxicologicalIndicators.Select(i => i.KIZV).Where(i => i != null).DefaultIfEmpty(0).Min());

                    

                    List<DynamicsLakeArea> dynamicsLakeAreas = _context.DynamicsLakeArea.Where(i => i.LakeId == lake.LakeId).ToList();
                    line += "\t" + PopulateDecimal(dynamicsLakeAreas.Select(i => i.NoDataPers).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(dynamicsLakeAreas.Select(i => i.NotWater).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(dynamicsLakeAreas.Select(i => i.SeasonalWaterArea).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(dynamicsLakeAreas.Select(i => i.PermanentWaterArea).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(dynamicsLakeAreas.Select(i => i.MaximumWaterArea).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(dynamicsLakeAreas.Select(i => i.SeasonalWaterAreaPer).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(dynamicsLakeAreas.Select(i => i.PermanentWaterAreaPer).Where(i => i != null).DefaultIfEmpty(0).Average());
                    line += "\t" + PopulateDecimal(dynamicsLakeAreas.Select(i => i.NoDataPers).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(dynamicsLakeAreas.Select(i => i.NotWater).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(dynamicsLakeAreas.Select(i => i.SeasonalWaterArea).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(dynamicsLakeAreas.Select(i => i.PermanentWaterArea).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(dynamicsLakeAreas.Select(i => i.MaximumWaterArea).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(dynamicsLakeAreas.Select(i => i.SeasonalWaterAreaPer).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(dynamicsLakeAreas.Select(i => i.PermanentWaterAreaPer).Where(i => i != null).DefaultIfEmpty(0).Max());
                    line += "\t" + PopulateDecimal(dynamicsLakeAreas.Select(i => i.NoDataPers).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(dynamicsLakeAreas.Select(i => i.NotWater).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(dynamicsLakeAreas.Select(i => i.SeasonalWaterArea).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(dynamicsLakeAreas.Select(i => i.PermanentWaterArea).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(dynamicsLakeAreas.Select(i => i.MaximumWaterArea).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(dynamicsLakeAreas.Select(i => i.SeasonalWaterAreaPer).Where(i => i != null).DefaultIfEmpty(0).Min());
                    line += "\t" + PopulateDecimal(dynamicsLakeAreas.Select(i => i.PermanentWaterAreaPer).Where(i => i != null).DefaultIfEmpty(0).Min());

                    file.WriteLine(line);
                }
            }

            TimeSpan timeSpan = start - DateTime.Now;
            ViewBag.Time = timeSpan.ToString();

            return View();
        }

        public bool CheckFormula(string Formula)
        {
            bool r = true;
            string formula_test = Formula;
            // CheckFormula 1
            formula_test = formula_test.Replace("_Area2015_", "");
            formula_test = formula_test.Replace("_LakeShorelineLength2015_", "");

            formula_test = formula_test.Replace("_LakesArchiveDataLakeLength_", "");
            formula_test = formula_test.Replace("_LakesArchiveDataLakeShorelineLength_", "");
            formula_test = formula_test.Replace("_LakesArchiveDataLakeMirrorArea_", "");
            formula_test = formula_test.Replace("_LakesArchiveDataLakeAbsoluteHeight_", "");
            formula_test = formula_test.Replace("_LakesArchiveDataLakeWidth_", "");
            formula_test = formula_test.Replace("_LakesArchiveDataLakeMaxDepth_", "");
            formula_test = formula_test.Replace("_LakesArchiveDataLakeWaterMass_", "");

            formula_test = formula_test.Replace("_LakesGlobalDataLake_area_", "");
            formula_test = formula_test.Replace("_LakesGlobalDataShore_len_", "");
            formula_test = formula_test.Replace("_LakesGlobalDataShore_dev_", "");
            formula_test = formula_test.Replace("_LakesGlobalDataVol_total_", "");
            formula_test = formula_test.Replace("_LakesGlobalDataDepth_avg_", "");
            formula_test = formula_test.Replace("_LakesGlobalDataDis_avg_", "");
            formula_test = formula_test.Replace("_LakesGlobalDataRes_time_", "");
            formula_test = formula_test.Replace("_LakesGlobalDataElevation_", "");
            formula_test = formula_test.Replace("_LakesGlobalDataSlope_100_", "");
            formula_test = formula_test.Replace("_LakesGlobalDataWshd_area_", "");

            formula_test = formula_test.Replace("_WaterBalanceSurfaceFlowAvg_", "");
            formula_test = formula_test.Replace("_WaterBalanceSurfaceOutflowAvg_", "");
            formula_test = formula_test.Replace("_WaterBalanceUndergroundFlowAvg_", "");
            formula_test = formula_test.Replace("_WaterBalanceUndergroundOutflowAvg_", "");
            formula_test = formula_test.Replace("_WaterBalancePrecipitationAvg_", "");
            formula_test = formula_test.Replace("_WaterBalanceEvaporationAvg_", "");
            formula_test = formula_test.Replace("_WaterBalanceWaterBalanceReceiptAvg_", "");
            formula_test = formula_test.Replace("_WaterBalanceWaterBalanceExpenditureAvg_", "");
            formula_test = formula_test.Replace("_WaterBalanceDiscrepancyAvg_", "");
            formula_test = formula_test.Replace("_WaterBalanceSurfaceFlowPerAvg_", "");
            formula_test = formula_test.Replace("_WaterBalanceUndergroundFlowPerAvg_", "");
            formula_test = formula_test.Replace("_WaterBalancePrecipitationPerAvg_", "");
            formula_test = formula_test.Replace("_WaterBalanceSurfaceOutflowPerAvg_", "");
            formula_test = formula_test.Replace("_WaterBalanceUndergroundOutflowPerAvg_", "");
            formula_test = formula_test.Replace("_WaterBalanceEvaporationPerAvg_", "");
            formula_test = formula_test.Replace("_WaterBalanceWaterBalanceReceiptPerAvg_", "");
            formula_test = formula_test.Replace("_WaterBalanceWaterBalanceExpenditurePerAvg_", "");
            formula_test = formula_test.Replace("_WaterBalanceSurfaceFlowMax_", "");
            formula_test = formula_test.Replace("_WaterBalanceSurfaceOutflowMax_", "");
            formula_test = formula_test.Replace("_WaterBalanceUndergroundFlowMax_", "");
            formula_test = formula_test.Replace("_WaterBalanceUndergroundOutflowMax_", "");
            formula_test = formula_test.Replace("_WaterBalancePrecipitationMax_", "");
            formula_test = formula_test.Replace("_WaterBalanceEvaporationMax_", "");
            formula_test = formula_test.Replace("_WaterBalanceWaterBalanceReceiptMax_", "");
            formula_test = formula_test.Replace("_WaterBalanceWaterBalanceExpenditureMax_", "");
            formula_test = formula_test.Replace("_WaterBalanceDiscrepancyMax_", "");
            formula_test = formula_test.Replace("_WaterBalanceSurfaceFlowPerMax_", "");
            formula_test = formula_test.Replace("_WaterBalanceUndergroundFlowPerMax_", "");
            formula_test = formula_test.Replace("_WaterBalancePrecipitationPerMax_", "");
            formula_test = formula_test.Replace("_WaterBalanceSurfaceOutflowPerMax_", "");
            formula_test = formula_test.Replace("_WaterBalanceUndergroundOutflowPerMax_", "");
            formula_test = formula_test.Replace("_WaterBalanceEvaporationPerMax_", "");
            formula_test = formula_test.Replace("_WaterBalanceWaterBalanceReceiptPerMax_", "");
            formula_test = formula_test.Replace("_WaterBalanceWaterBalanceExpenditurePerMax_", "");
            formula_test = formula_test.Replace("_WaterBalanceSurfaceFlowMin_", "");
            formula_test = formula_test.Replace("_WaterBalanceSurfaceOutflowMin_", "");
            formula_test = formula_test.Replace("_WaterBalanceUndergroundFlowMin_", "");
            formula_test = formula_test.Replace("_WaterBalanceUndergroundOutflowMin_", "");
            formula_test = formula_test.Replace("_WaterBalancePrecipitationMin_", "");
            formula_test = formula_test.Replace("_WaterBalanceEvaporationMin_", "");
            formula_test = formula_test.Replace("_WaterBalanceWaterBalanceReceiptMin_", "");
            formula_test = formula_test.Replace("_WaterBalanceWaterBalanceExpenditureMin_", "");
            formula_test = formula_test.Replace("_WaterBalanceDiscrepancyMin_", "");
            formula_test = formula_test.Replace("_WaterBalanceSurfaceFlowPerMin_", "");
            formula_test = formula_test.Replace("_WaterBalanceUndergroundFlowPerMin_", "");
            formula_test = formula_test.Replace("_WaterBalancePrecipitationPerMin_", "");
            formula_test = formula_test.Replace("_WaterBalanceSurfaceOutflowPerMin_", "");
            formula_test = formula_test.Replace("_WaterBalanceUndergroundOutflowPerMin_", "");
            formula_test = formula_test.Replace("_WaterBalanceEvaporationPerMin_", "");
            formula_test = formula_test.Replace("_WaterBalanceWaterBalanceReceiptPerMin_", "");
            formula_test = formula_test.Replace("_WaterBalanceWaterBalanceExpenditurePerMin_", "");

            formula_test = formula_test.Replace("_WaterLevelWaterLavelMAvg_", "");
            formula_test = formula_test.Replace("_WaterLevelWaterLavelMMax_", "");
            formula_test = formula_test.Replace("_WaterLevelWaterLavelMMin_", "");

            formula_test = formula_test.Replace("_BathigraphicAndVolumetricCurveDataWaterLevelAvg_", "");
            formula_test = formula_test.Replace("_BathigraphicAndVolumetricCurveDataLakeAreaAvg_", "");
            formula_test = formula_test.Replace("_BathigraphicAndVolumetricCurveDataWaterMassVolumeAvg_", "");
            formula_test = formula_test.Replace("_BathigraphicAndVolumetricCurveDataWaterLevelMax_", "");
            formula_test = formula_test.Replace("_BathigraphicAndVolumetricCurveDataLakeAreaMax_", "");
            formula_test = formula_test.Replace("_BathigraphicAndVolumetricCurveDataWaterMassVolumeMax_", "");
            formula_test = formula_test.Replace("_BathigraphicAndVolumetricCurveDataWaterLevelMin_", "");
            formula_test = formula_test.Replace("_BathigraphicAndVolumetricCurveDataLakeAreaMin_", "");
            formula_test = formula_test.Replace("_BathigraphicAndVolumetricCurveDataWaterMassVolumeMin_", "");

            formula_test = formula_test.Replace("_GeneralHydrochemicalIndicatorMineralizationAvg_", "");
            formula_test = formula_test.Replace("_GeneralHydrochemicalIndicatorTotalHardnessAvg_", "");
            formula_test = formula_test.Replace("_GeneralHydrochemicalIndicatorDissOxygWaterAvg_", "");
            formula_test = formula_test.Replace("_GeneralHydrochemicalIndicatorPercentOxygWaterAvg_", "");
            formula_test = formula_test.Replace("_GeneralHydrochemicalIndicatorpHAvg_", "");
            formula_test = formula_test.Replace("_GeneralHydrochemicalIndicatorOrganicSubstancesAvg_", "");
            formula_test = formula_test.Replace("_GeneralHydrochemicalIndicatorMineralizationMax_", "");
            formula_test = formula_test.Replace("_GeneralHydrochemicalIndicatorTotalHardnessMax_", "");
            formula_test = formula_test.Replace("_GeneralHydrochemicalIndicatorDissOxygWaterMax_", "");
            formula_test = formula_test.Replace("_GeneralHydrochemicalIndicatorPercentOxygWaterMax_", "");
            formula_test = formula_test.Replace("_GeneralHydrochemicalIndicatorpHMax_", "");
            formula_test = formula_test.Replace("_GeneralHydrochemicalIndicatorOrganicSubstancesMax_", "");
            formula_test = formula_test.Replace("_GeneralHydrochemicalIndicatorMineralizationMin_", "");
            formula_test = formula_test.Replace("_GeneralHydrochemicalIndicatorTotalHardnessMin_", "");
            formula_test = formula_test.Replace("_GeneralHydrochemicalIndicatorDissOxygWaterMin_", "");
            formula_test = formula_test.Replace("_GeneralHydrochemicalIndicatorPercentOxygWaterMin_", "");
            formula_test = formula_test.Replace("_GeneralHydrochemicalIndicatorpHMin_", "");
            formula_test = formula_test.Replace("_GeneralHydrochemicalIndicatorOrganicSubstancesMin_", "");

            formula_test = formula_test.Replace("_IonsaltWaterCompositionCaMgEqAvg_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionMgMgEqAvg_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionNaKMgEqAvg_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionClMgEqAvg_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionHCOMgEqAvg_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionSOMgEqAvg_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionCaMgAvg_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionMgMgAvg_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionNaKMgAvg_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionClMgAvg_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionHCOMgAvg_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionSOMgAvg_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionCationsSumMgEqAvg_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionAnionsSumMgEqAvg_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionIonsSumMgEqAvg_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionCaPerEqAvg_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionMgPerEqAvg_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionNaKPerEqAvg_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionClPerEqAvg_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionHCOPerEqAvg_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionSOPerEqAvg_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionCaMgEqMax_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionMgMgEqMax_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionNaKMgEqMax_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionClMgEqMax_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionHCOMgEqMax_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionSOMgEqMax_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionCaMgMax_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionMgMgMax_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionNaKMgMax_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionClMgMax_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionHCOMgMax_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionSOMgMax_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionCationsSumMgEqMax_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionAnionsSumMgEqMax_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionIonsSumMgEqMax_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionCaPerEqMax_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionMgPerEqMax_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionNaKPerEqMax_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionClPerEqMax_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionHCOPerEqMax_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionSOPerEqMax_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionCaMgEqMin_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionMgMgEqMin_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionNaKMgEqMin_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionClMgEqMin_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionHCOMgEqMin_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionSOMgEqMin_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionCaMgMin_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionMgMgMin_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionNaKMgMin_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionClMgMin_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionHCOMgMin_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionSOMgMin_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionCationsSumMgEqMin_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionAnionsSumMgEqMin_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionIonsSumMgEqMin_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionCaPerEqMin_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionMgPerEqMin_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionNaKPerEqMin_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionClPerEqMin_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionHCOPerEqMin_", "");
            formula_test = formula_test.Replace("_IonsaltWaterCompositionSOPerEqMin_", "");

            formula_test = formula_test.Replace("_ToxicologicalIndicatorNH4Avg_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorNO2Avg_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorNO3Avg_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorPPO4Avg_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorCuAvg_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorZnAvg_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorMnAvg_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorPbAvg_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorNiAvg_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorCdAvg_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorCoAvg_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorIZVNH4Avg_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorIZVNO2Avg_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorIZVNO3Avg_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorIZVPPO4Avg_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorIZVCuAvg_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorIZVZnAvg_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorIZVMnAvg_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorIZVPbAvg_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorIZVNiAvg_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorIZVCdAvg_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorIZVCoAvg_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorKIZVkoNH4Avg_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorKIZVkoNO2Avg_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorKIZVkoNO3Avg_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorKIZVkoPPO4Avg_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorKIZVkoCuAvg_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorKIZVkoZnAvg_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorKIZVkoMnAvg_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorKIZVkoPbAvg_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorKIZVkoNiAvg_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorKIZVkoCdAvg_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorKIZVkoCoAvg_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorKIZVAvg_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorNH4Max_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorNO2Max_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorNO3Max_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorPPO4Max_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorCuMax_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorZnMax_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorMnMax_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorPbMax_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorNiMax_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorCdMax_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorCoMax_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorIZVNH4Max_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorIZVNO2Max_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorIZVNO3Max_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorIZVPPO4Max_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorIZVCuMax_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorIZVZnMax_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorIZVMnMax_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorIZVPbMax_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorIZVNiMax_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorIZVCdMax_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorIZVCoMax_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorKIZVkoNH4Max_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorKIZVkoNO2Max_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorKIZVkoNO3Max_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorKIZVkoPPO4Max_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorKIZVkoCuMax_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorKIZVkoZnMax_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorKIZVkoMnMax_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorKIZVkoPbMax_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorKIZVkoNiMax_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorKIZVkoCdMax_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorKIZVkoCoMax_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorKIZVMax_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorNH4Min_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorNO2Min_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorNO3Min_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorPPO4Min_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorCuMin_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorZnMin_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorMnMin_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorPbMin_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorNiMin_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorCdMin_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorCoMin_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorIZVNH4Min_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorIZVNO2Min_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorIZVNO3Min_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorIZVPPO4Min_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorIZVCuMin_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorIZVZnMin_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorIZVMnMin_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorIZVPbMin_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorIZVNiMin_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorIZVCdMin_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorIZVCoMin_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorKIZVkoNH4Min_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorKIZVkoNO2Min_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorKIZVkoNO3Min_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorKIZVkoPPO4Min_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorKIZVkoCuMin_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorKIZVkoZnMin_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorKIZVkoMnMin_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorKIZVkoPbMin_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorKIZVkoNiMin_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorKIZVkoCdMin_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorKIZVkoCoMin_", "");
            formula_test = formula_test.Replace("_ToxicologicalIndicatorKIZVMin_", "");

            formula_test = formula_test.Replace("_TransitionNoСhange_", "");
            formula_test = formula_test.Replace("_TransitionPermanent_", "");
            formula_test = formula_test.Replace("_TransitionNewPermanent_", "");
            formula_test = formula_test.Replace("_TransitionLostPermanent_", "");
            formula_test = formula_test.Replace("_TransitionSeasonal_", "");
            formula_test = formula_test.Replace("_TransitionNewSeasonal_", "");
            formula_test = formula_test.Replace("_TransitionLostSeasonal_", "");
            formula_test = formula_test.Replace("_TransitionSeasonalToPermanent_", "");
            formula_test = formula_test.Replace("_TransitionPermanentToDeasonal_", "");
            formula_test = formula_test.Replace("_TransitionEphemeralPermanent_", "");
            formula_test = formula_test.Replace("_TransitionEphemeralSeasonal_", "");
            formula_test = formula_test.Replace("_TransitionMaximumWater_", "");
            formula_test = formula_test.Replace("_TransitionPermanentPer_", "");
            formula_test = formula_test.Replace("_TransitionSeasonalPer_", "");

            formula_test = formula_test.Replace("_SeasonalitNoData_", "");
            formula_test = formula_test.Replace("_SeasonalitJanuary_", "");
            formula_test = formula_test.Replace("_SeasonalitFebruary_", "");
            formula_test = formula_test.Replace("_SeasonalitMarch_", "");
            formula_test = formula_test.Replace("_SeasonalitApril_", "");
            formula_test = formula_test.Replace("_SeasonalitMay_", "");
            formula_test = formula_test.Replace("_SeasonalitJune_", "");
            formula_test = formula_test.Replace("_SeasonalitJuly_", "");
            formula_test = formula_test.Replace("_SeasonalitAugust_", "");
            formula_test = formula_test.Replace("_SeasonalitSeptember_", "");
            formula_test = formula_test.Replace("_SeasonalitOctober_", "");
            formula_test = formula_test.Replace("_SeasonalitNovember_", "");
            formula_test = formula_test.Replace("_SeasonalitDecember_", "");

            formula_test = formula_test.Replace("_DynamicsLakeAreaNoDataPersAvg_", "");
            formula_test = formula_test.Replace("_DynamicsLakeAreaNotWaterAvg_", "");
            formula_test = formula_test.Replace("_DynamicsLakeAreaSeasonalWaterAreaAvg_", "");
            formula_test = formula_test.Replace("_DynamicsLakeAreaPermanentWaterAreaAvg_", "");
            formula_test = formula_test.Replace("_DynamicsLakeAreaMaximumWaterAreaAvg_", "");
            formula_test = formula_test.Replace("_DynamicsLakeAreaSeasonalWaterAreaPerAvg_", "");
            formula_test = formula_test.Replace("_DynamicsLakeAreaPermanentWaterAreaPerAvg_", "");
            formula_test = formula_test.Replace("_DynamicsLakeAreaNoDataPersMax_", "");
            formula_test = formula_test.Replace("_DynamicsLakeAreaNotWaterMax_", "");
            formula_test = formula_test.Replace("_DynamicsLakeAreaSeasonalWaterAreaMax_", "");
            formula_test = formula_test.Replace("_DynamicsLakeAreaPermanentWaterAreaMax_", "");
            formula_test = formula_test.Replace("_DynamicsLakeAreaMaximumWaterAreaMax_", "");
            formula_test = formula_test.Replace("_DynamicsLakeAreaSeasonalWaterAreaPerMax_", "");
            formula_test = formula_test.Replace("_DynamicsLakeAreaPermanentWaterAreaPerMax_", "");
            formula_test = formula_test.Replace("_DynamicsLakeAreaNoDataPersMin_", "");
            formula_test = formula_test.Replace("_DynamicsLakeAreaNotWaterMin_", "");
            formula_test = formula_test.Replace("_DynamicsLakeAreaSeasonalWaterAreaMin_", "");
            formula_test = formula_test.Replace("_DynamicsLakeAreaPermanentWaterAreaMin_", "");
            formula_test = formula_test.Replace("_DynamicsLakeAreaMaximumWaterAreaMin_", "");
            formula_test = formula_test.Replace("_DynamicsLakeAreaSeasonalWaterAreaPerMin_", "");
            formula_test = formula_test.Replace("_DynamicsLakeAreaPermanentWaterAreaPerMin_", "");
            
            formula_test = formula_test.Replace("M", "");
            formula_test = formula_test.Replace(" ", "");
            formula_test = formula_test.Replace("(", "");
            formula_test = formula_test.Replace(")", "");
            formula_test = formula_test.Replace("+", "");
            formula_test = formula_test.Replace("-", "");
            formula_test = formula_test.Replace("*", "");
            formula_test = formula_test.Replace("/", "");
            formula_test = formula_test.Replace(",", ".");
            formula_test = formula_test.Replace(".", "");
            formula_test = formula_test.Replace(">", "");
            formula_test = formula_test.Replace("<", "");
            formula_test = formula_test.Replace("=", "");
            formula_test = formula_test.Replace("AND", "");
            formula_test = formula_test.Replace("OR", "");
            formula_test = formula_test.Replace("NOT", "");

            for (int n = 0; n <= 9; n++)
            {
                formula_test = formula_test.Replace(n.ToString(), "");
            }
            if (formula_test.Trim() != "")
            {
                r = false;
            }
            return r;
        }

        private string PopulateDecimal(decimal? Value)
        {
            //if(Value == null)
            //{
            //    return "0,";
            //}
            //else
            //{
            //    return Value.ToString().Replace(',', '.') + "M,";
            //}
            if (Value == null)
            {
                return "0";
            }
            else
            {
                return Value.ToString();
            }
        }

        [HttpPost]
        public ActionResult Analyze(string Formula, int? Adm1KATOId, int? Adm2KATOId)
        {
            var lakesToSearch = _context.Lake
                .ToArray();


            string assemblyName = Path.GetRandomFileName();
            string sContentRootPath = _hostingEnvironment.WebRootPath;
            sContentRootPath = Path.Combine(sContentRootPath, "Analytics");
            string filePopulateLakes = Path.Combine(sContentRootPath, "lakes.txt");

            string codeFilter = Formula;
            // Analyze 1
            //codeFilter = codeFilter.Replace(",", ".");
            codeFilter = codeFilter.Replace("AND", "&&");
            codeFilter = codeFilter.Replace("OR", "||");
            codeFilter = codeFilter.Replace("NOT", "!");

            codeFilter = codeFilter.Replace("_Area2015_", "lake.Area2015");
            codeFilter = codeFilter.Replace("_LakeShorelineLength2015_", "lake.LakeShorelineLength2015");

            codeFilter = codeFilter.Replace("_LakesArchiveDataLakeLength_", "lake.LakesArchiveDataLakeLength");
            codeFilter = codeFilter.Replace("_LakesArchiveDataLakeShorelineLength_", "lake.LakesArchiveDataLakeShorelineLength");
            codeFilter = codeFilter.Replace("_LakesArchiveDataLakeMirrorArea_", "lake.LakesArchiveDataLakeMirrorArea");
            codeFilter = codeFilter.Replace("_LakesArchiveDataLakeAbsoluteHeight_", "lake.LakesArchiveDataLakeAbsoluteHeight");
            codeFilter = codeFilter.Replace("_LakesArchiveDataLakeWidth_", "lake.LakesArchiveDataLakeWidth");
            codeFilter = codeFilter.Replace("_LakesArchiveDataLakeMaxDepth_", "lake.LakesArchiveDataLakeMaxDepth");
            codeFilter = codeFilter.Replace("_LakesArchiveDataLakeWaterMass_", "lake.LakesArchiveDataLakeWaterMass");

            codeFilter = codeFilter.Replace("_LakesGlobalDataLake_area_", "lake.LakesGlobalDataLake_area");
            codeFilter = codeFilter.Replace("_LakesGlobalDataShore_len_", "lake.LakesGlobalDataShore_len");
            codeFilter = codeFilter.Replace("_LakesGlobalDataShore_dev_", "lake.LakesGlobalDataShore_dev");
            codeFilter = codeFilter.Replace("_LakesGlobalDataVol_total_", "lake.LakesGlobalDataVol_total");
            codeFilter = codeFilter.Replace("_LakesGlobalDataDepth_avg_", "lake.LakesGlobalDataDepth_avg");
            codeFilter = codeFilter.Replace("_LakesGlobalDataDis_avg_", "lake.LakesGlobalDataDis_avg");
            codeFilter = codeFilter.Replace("_LakesGlobalDataRes_time_", "lake.LakesGlobalDataRes_time");
            codeFilter = codeFilter.Replace("_LakesGlobalDataElevation_", "lake.LakesGlobalDataElevation");
            codeFilter = codeFilter.Replace("_LakesGlobalDataSlope_100_", "lake.LakesGlobalDataSlope_100");
            codeFilter = codeFilter.Replace("_LakesGlobalDataWshd_area_", "lake.LakesGlobalDataWshd_area");

            codeFilter = codeFilter.Replace("_WaterBalanceSurfaceFlowAvg_", "lake.WaterBalanceSurfaceFlowAvg");
            codeFilter = codeFilter.Replace("_WaterBalanceSurfaceOutflowAvg_", "lake.WaterBalanceSurfaceOutflowAvg");
            codeFilter = codeFilter.Replace("_WaterBalanceUndergroundFlowAvg_", "lake.WaterBalanceUndergroundFlowAvg");
            codeFilter = codeFilter.Replace("_WaterBalanceUndergroundOutflowAvg_", "lake.WaterBalanceUndergroundOutflowAvg");
            codeFilter = codeFilter.Replace("_WaterBalancePrecipitationAvg_", "lake.WaterBalancePrecipitationAvg");
            codeFilter = codeFilter.Replace("_WaterBalanceEvaporationAvg_", "lake.WaterBalanceEvaporationAvg");
            codeFilter = codeFilter.Replace("_WaterBalanceWaterBalanceReceiptAvg_", "lake.WaterBalanceWaterBalanceReceiptAvg");
            codeFilter = codeFilter.Replace("_WaterBalanceWaterBalanceExpenditureAvg_", "lake.WaterBalanceWaterBalanceExpenditureAvg");
            codeFilter = codeFilter.Replace("_WaterBalanceDiscrepancyAvg_", "lake.WaterBalanceDiscrepancyAvg");
            codeFilter = codeFilter.Replace("_WaterBalanceSurfaceFlowPerAvg_", "lake.WaterBalanceSurfaceFlowPerAvg");
            codeFilter = codeFilter.Replace("_WaterBalanceUndergroundFlowPerAvg_", "lake.WaterBalanceUndergroundFlowPerAvg");
            codeFilter = codeFilter.Replace("_WaterBalancePrecipitationPerAvg_", "lake.WaterBalancePrecipitationPerAvg");
            codeFilter = codeFilter.Replace("_WaterBalanceSurfaceOutflowPerAvg_", "lake.WaterBalanceSurfaceOutflowPerAvg");
            codeFilter = codeFilter.Replace("_WaterBalanceUndergroundOutflowPerAvg_", "lake.WaterBalanceUndergroundOutflowPerAvg");
            codeFilter = codeFilter.Replace("_WaterBalanceEvaporationPerAvg_", "lake.WaterBalanceEvaporationPerAvg");
            codeFilter = codeFilter.Replace("_WaterBalanceWaterBalanceReceiptPerAvg_", "lake.WaterBalanceWaterBalanceReceiptPerAvg");
            codeFilter = codeFilter.Replace("_WaterBalanceWaterBalanceExpenditurePerAvg_", "lake.WaterBalanceWaterBalanceExpenditurePerAvg");
            codeFilter = codeFilter.Replace("_WaterBalanceSurfaceFlowMax_", "lake.WaterBalanceSurfaceFlowMax");
            codeFilter = codeFilter.Replace("_WaterBalanceSurfaceOutflowMax_", "lake.WaterBalanceSurfaceOutflowMax");
            codeFilter = codeFilter.Replace("_WaterBalanceUndergroundFlowMax_", "lake.WaterBalanceUndergroundFlowMax");
            codeFilter = codeFilter.Replace("_WaterBalanceUndergroundOutflowMax_", "lake.WaterBalanceUndergroundOutflowMax");
            codeFilter = codeFilter.Replace("_WaterBalancePrecipitationMax_", "lake.WaterBalancePrecipitationMax");
            codeFilter = codeFilter.Replace("_WaterBalanceEvaporationMax_", "lake.WaterBalanceEvaporationMax");
            codeFilter = codeFilter.Replace("_WaterBalanceWaterBalanceReceiptMax_", "lake.WaterBalanceWaterBalanceReceiptMax");
            codeFilter = codeFilter.Replace("_WaterBalanceWaterBalanceExpenditureMax_", "lake.WaterBalanceWaterBalanceExpenditureMax");
            codeFilter = codeFilter.Replace("_WaterBalanceDiscrepancyMax_", "lake.WaterBalanceDiscrepancyMax");
            codeFilter = codeFilter.Replace("_WaterBalanceSurfaceFlowPerMax_", "lake.WaterBalanceSurfaceFlowPerMax");
            codeFilter = codeFilter.Replace("_WaterBalanceUndergroundFlowPerMax_", "lake.WaterBalanceUndergroundFlowPerMax");
            codeFilter = codeFilter.Replace("_WaterBalancePrecipitationPerMax_", "lake.WaterBalancePrecipitationPerMax");
            codeFilter = codeFilter.Replace("_WaterBalanceSurfaceOutflowPerMax_", "lake.WaterBalanceSurfaceOutflowPerMax");
            codeFilter = codeFilter.Replace("_WaterBalanceUndergroundOutflowPerMax_", "lake.WaterBalanceUndergroundOutflowPerMax");
            codeFilter = codeFilter.Replace("_WaterBalanceEvaporationPerMax_", "lake.WaterBalanceEvaporationPerMax");
            codeFilter = codeFilter.Replace("_WaterBalanceWaterBalanceReceiptPerMax_", "lake.WaterBalanceWaterBalanceReceiptPerMax");
            codeFilter = codeFilter.Replace("_WaterBalanceWaterBalanceExpenditurePerMax_", "lake.WaterBalanceWaterBalanceExpenditurePerMax");
            codeFilter = codeFilter.Replace("_WaterBalanceSurfaceFlowMin_", "lake.WaterBalanceSurfaceFlowMin");
            codeFilter = codeFilter.Replace("_WaterBalanceSurfaceOutflowMin_", "lake.WaterBalanceSurfaceOutflowMin");
            codeFilter = codeFilter.Replace("_WaterBalanceUndergroundFlowMin_", "lake.WaterBalanceUndergroundFlowMin");
            codeFilter = codeFilter.Replace("_WaterBalanceUndergroundOutflowMin_", "lake.WaterBalanceUndergroundOutflowMin");
            codeFilter = codeFilter.Replace("_WaterBalancePrecipitationMin_", "lake.WaterBalancePrecipitationMin");
            codeFilter = codeFilter.Replace("_WaterBalanceEvaporationMin_", "lake.WaterBalanceEvaporationMin");
            codeFilter = codeFilter.Replace("_WaterBalanceWaterBalanceReceiptMin_", "lake.WaterBalanceWaterBalanceReceiptMin");
            codeFilter = codeFilter.Replace("_WaterBalanceWaterBalanceExpenditureMin_", "lake.WaterBalanceWaterBalanceExpenditureMin");
            codeFilter = codeFilter.Replace("_WaterBalanceDiscrepancyMin_", "lake.WaterBalanceDiscrepancyMin");
            codeFilter = codeFilter.Replace("_WaterBalanceSurfaceFlowPerMin_", "lake.WaterBalanceSurfaceFlowPerMin");
            codeFilter = codeFilter.Replace("_WaterBalanceUndergroundFlowPerMin_", "lake.WaterBalanceUndergroundFlowPerMin");
            codeFilter = codeFilter.Replace("_WaterBalancePrecipitationPerMin_", "lake.WaterBalancePrecipitationPerMin");
            codeFilter = codeFilter.Replace("_WaterBalanceSurfaceOutflowPerMin_", "lake.WaterBalanceSurfaceOutflowPerMin");
            codeFilter = codeFilter.Replace("_WaterBalanceUndergroundOutflowPerMin_", "lake.WaterBalanceUndergroundOutflowPerMin");
            codeFilter = codeFilter.Replace("_WaterBalanceEvaporationPerMin_", "lake.WaterBalanceEvaporationPerMin");
            codeFilter = codeFilter.Replace("_WaterBalanceWaterBalanceReceiptPerMin_", "lake.WaterBalanceWaterBalanceReceiptPerMin");
            codeFilter = codeFilter.Replace("_WaterBalanceWaterBalanceExpenditurePerMin_", "lake.WaterBalanceWaterBalanceExpenditurePerMin");

            codeFilter = codeFilter.Replace("_WaterLevelWaterLavelMAvg_", "lake.WaterLevelWaterLavelMAvg");
            codeFilter = codeFilter.Replace("_WaterLevelWaterLavelMMax_", "lake.WaterLevelWaterLavelMMax");
            codeFilter = codeFilter.Replace("_WaterLevelWaterLavelMMin_", "lake.WaterLevelWaterLavelMMin");

            codeFilter = codeFilter.Replace("_BathigraphicAndVolumetricCurveDataWaterLevelAvg_", "lake.BathigraphicAndVolumetricCurveDataWaterLevelAvg");
            codeFilter = codeFilter.Replace("_BathigraphicAndVolumetricCurveDataLakeAreaAvg_", "lake.BathigraphicAndVolumetricCurveDataLakeAreaAvg");
            codeFilter = codeFilter.Replace("_BathigraphicAndVolumetricCurveDataWaterMassVolumeAvg_", "lake.BathigraphicAndVolumetricCurveDataWaterMassVolumeAvg");
            codeFilter = codeFilter.Replace("_BathigraphicAndVolumetricCurveDataWaterLevelMax_", "lake.BathigraphicAndVolumetricCurveDataWaterLevelMax");
            codeFilter = codeFilter.Replace("_BathigraphicAndVolumetricCurveDataLakeAreaMax_", "lake.BathigraphicAndVolumetricCurveDataLakeAreaMax");
            codeFilter = codeFilter.Replace("_BathigraphicAndVolumetricCurveDataWaterMassVolumeMax_", "lake.BathigraphicAndVolumetricCurveDataWaterMassVolumeMax");
            codeFilter = codeFilter.Replace("_BathigraphicAndVolumetricCurveDataWaterLevelMin_", "lake.BathigraphicAndVolumetricCurveDataWaterLevelMin");
            codeFilter = codeFilter.Replace("_BathigraphicAndVolumetricCurveDataLakeAreaMin_", "lake.BathigraphicAndVolumetricCurveDataLakeAreaMin");
            codeFilter = codeFilter.Replace("_BathigraphicAndVolumetricCurveDataWaterMassVolumeMin_", "lake.BathigraphicAndVolumetricCurveDataWaterMassVolumeMin");

            codeFilter = codeFilter.Replace("_GeneralHydrochemicalIndicatorMineralizationAvg_", "lake.GeneralHydrochemicalIndicatorMineralizationAvg");
            codeFilter = codeFilter.Replace("_GeneralHydrochemicalIndicatorTotalHardnessAvg_", "lake.GeneralHydrochemicalIndicatorTotalHardnessAvg");
            codeFilter = codeFilter.Replace("_GeneralHydrochemicalIndicatorDissOxygWaterAvg_", "lake.GeneralHydrochemicalIndicatorDissOxygWaterAvg");
            codeFilter = codeFilter.Replace("_GeneralHydrochemicalIndicatorPercentOxygWaterAvg_", "lake.GeneralHydrochemicalIndicatorPercentOxygWaterAvg");
            codeFilter = codeFilter.Replace("_GeneralHydrochemicalIndicatorpHAvg_", "lake.GeneralHydrochemicalIndicatorpHAvg");
            codeFilter = codeFilter.Replace("_GeneralHydrochemicalIndicatorOrganicSubstancesAvg_", "lake.GeneralHydrochemicalIndicatorOrganicSubstancesAvg");
            codeFilter = codeFilter.Replace("_GeneralHydrochemicalIndicatorMineralizationMax_", "lake.GeneralHydrochemicalIndicatorMineralizationMax");
            codeFilter = codeFilter.Replace("_GeneralHydrochemicalIndicatorTotalHardnessMax_", "lake.GeneralHydrochemicalIndicatorTotalHardnessMax");
            codeFilter = codeFilter.Replace("_GeneralHydrochemicalIndicatorDissOxygWaterMax_", "lake.GeneralHydrochemicalIndicatorDissOxygWaterMax");
            codeFilter = codeFilter.Replace("_GeneralHydrochemicalIndicatorPercentOxygWaterMax_", "lake.GeneralHydrochemicalIndicatorPercentOxygWaterMax");
            codeFilter = codeFilter.Replace("_GeneralHydrochemicalIndicatorpHMax_", "lake.GeneralHydrochemicalIndicatorpHMax");
            codeFilter = codeFilter.Replace("_GeneralHydrochemicalIndicatorOrganicSubstancesMax_", "lake.GeneralHydrochemicalIndicatorOrganicSubstancesMax");
            codeFilter = codeFilter.Replace("_GeneralHydrochemicalIndicatorMineralizationMin_", "lake.GeneralHydrochemicalIndicatorMineralizationMin");
            codeFilter = codeFilter.Replace("_GeneralHydrochemicalIndicatorTotalHardnessMin_", "lake.GeneralHydrochemicalIndicatorTotalHardnessMin");
            codeFilter = codeFilter.Replace("_GeneralHydrochemicalIndicatorDissOxygWaterMin_", "lake.GeneralHydrochemicalIndicatorDissOxygWaterMin");
            codeFilter = codeFilter.Replace("_GeneralHydrochemicalIndicatorPercentOxygWaterMin_", "lake.GeneralHydrochemicalIndicatorPercentOxygWaterMin");
            codeFilter = codeFilter.Replace("_GeneralHydrochemicalIndicatorpHMin_", "lake.GeneralHydrochemicalIndicatorpHMin");
            codeFilter = codeFilter.Replace("_GeneralHydrochemicalIndicatorOrganicSubstancesMin_", "lake.GeneralHydrochemicalIndicatorOrganicSubstancesMin");

            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionCaMgEqAvg_", "lake.IonsaltWaterCompositionCaMgEqAvg");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionMgMgEqAvg_", "lake.IonsaltWaterCompositionMgMgEqAvg");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionNaKMgEqAvg_", "lake.IonsaltWaterCompositionNaKMgEqAvg");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionClMgEqAvg_", "lake.IonsaltWaterCompositionClMgEqAvg");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionHCOMgEqAvg_", "lake.IonsaltWaterCompositionHCOMgEqAvg");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionSOMgEqAvg_", "lake.IonsaltWaterCompositionSOMgEqAvg");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionCaMgAvg_", "lake.IonsaltWaterCompositionCaMgAvg");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionMgMgAvg_", "lake.IonsaltWaterCompositionMgMgAvg");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionNaKMgAvg_", "lake.IonsaltWaterCompositionNaKMgAvg");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionClMgAvg_", "lake.IonsaltWaterCompositionClMgAvg");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionHCOMgAvg_", "lake.IonsaltWaterCompositionHCOMgAvg");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionSOMgAvg_", "lake.IonsaltWaterCompositionSOMgAvg");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionCationsSumMgEqAvg_", "lake.IonsaltWaterCompositionCationsSumMgEqAvg");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionAnionsSumMgEqAvg_", "lake.IonsaltWaterCompositionAnionsSumMgEqAvg");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionIonsSumMgEqAvg_", "lake.IonsaltWaterCompositionIonsSumMgEqAvg");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionCaPerEqAvg_", "lake.IonsaltWaterCompositionCaPerEqAvg");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionMgPerEqAvg_", "lake.IonsaltWaterCompositionMgPerEqAvg");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionNaKPerEqAvg_", "lake.IonsaltWaterCompositionNaKPerEqAvg");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionClPerEqAvg_", "lake.IonsaltWaterCompositionClPerEqAvg");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionHCOPerEqAvg_", "lake.IonsaltWaterCompositionHCOPerEqAvg");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionSOPerEqAvg_", "lake.IonsaltWaterCompositionSOPerEqAvg");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionCaMgEqMax_", "lake.IonsaltWaterCompositionCaMgEqMax");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionMgMgEqMax_", "lake.IonsaltWaterCompositionMgMgEqMax");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionNaKMgEqMax_", "lake.IonsaltWaterCompositionNaKMgEqMax");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionClMgEqMax_", "lake.IonsaltWaterCompositionClMgEqMax");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionHCOMgEqMax_", "lake.IonsaltWaterCompositionHCOMgEqMax");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionSOMgEqMax_", "lake.IonsaltWaterCompositionSOMgEqMax");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionCaMgMax_", "lake.IonsaltWaterCompositionCaMgMax");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionMgMgMax_", "lake.IonsaltWaterCompositionMgMgMax");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionNaKMgMax_", "lake.IonsaltWaterCompositionNaKMgMax");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionClMgMax_", "lake.IonsaltWaterCompositionClMgMax");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionHCOMgMax_", "lake.IonsaltWaterCompositionHCOMgMax");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionSOMgMax_", "lake.IonsaltWaterCompositionSOMgMax");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionCationsSumMgEqMax_", "lake.IonsaltWaterCompositionCationsSumMgEqMax");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionAnionsSumMgEqMax_", "lake.IonsaltWaterCompositionAnionsSumMgEqMax");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionIonsSumMgEqMax_", "lake.IonsaltWaterCompositionIonsSumMgEqMax");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionCaPerEqMax_", "lake.IonsaltWaterCompositionCaPerEqMax");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionMgPerEqMax_", "lake.IonsaltWaterCompositionMgPerEqMax");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionNaKPerEqMax_", "lake.IonsaltWaterCompositionNaKPerEqMax");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionClPerEqMax_", "lake.IonsaltWaterCompositionClPerEqMax");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionHCOPerEqMax_", "lake.IonsaltWaterCompositionHCOPerEqMax");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionSOPerEqMax_", "lake.IonsaltWaterCompositionSOPerEqMax");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionCaMgEqMin_", "lake.IonsaltWaterCompositionCaMgEqMin");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionMgMgEqMin_", "lake.IonsaltWaterCompositionMgMgEqMin");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionNaKMgEqMin_", "lake.IonsaltWaterCompositionNaKMgEqMin");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionClMgEqMin_", "lake.IonsaltWaterCompositionClMgEqMin");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionHCOMgEqMin_", "lake.IonsaltWaterCompositionHCOMgEqMin");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionSOMgEqMin_", "lake.IonsaltWaterCompositionSOMgEqMin");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionCaMgMin_", "lake.IonsaltWaterCompositionCaMgMin");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionMgMgMin_", "lake.IonsaltWaterCompositionMgMgMin");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionNaKMgMin_", "lake.IonsaltWaterCompositionNaKMgMin");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionClMgMin_", "lake.IonsaltWaterCompositionClMgMin");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionHCOMgMin_", "lake.IonsaltWaterCompositionHCOMgMin");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionSOMgMin_", "lake.IonsaltWaterCompositionSOMgMin");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionCationsSumMgEqMin_", "lake.IonsaltWaterCompositionCationsSumMgEqMin");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionAnionsSumMgEqMin_", "lake.IonsaltWaterCompositionAnionsSumMgEqMin");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionIonsSumMgEqMin_", "lake.IonsaltWaterCompositionIonsSumMgEqMin");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionCaPerEqMin_", "lake.IonsaltWaterCompositionCaPerEqMin");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionMgPerEqMin_", "lake.IonsaltWaterCompositionMgPerEqMin");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionNaKPerEqMin_", "lake.IonsaltWaterCompositionNaKPerEqMin");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionClPerEqMin_", "lake.IonsaltWaterCompositionClPerEqMin");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionHCOPerEqMin_", "lake.IonsaltWaterCompositionHCOPerEqMin");
            codeFilter = codeFilter.Replace("_IonsaltWaterCompositionSOPerEqMin_", "lake.IonsaltWaterCompositionSOPerEqMin");

            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorNH4Avg_", "lake.ToxicologicalIndicatorNH4Avg");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorNO2Avg_", "lake.ToxicologicalIndicatorNO2Avg");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorNO3Avg_", "lake.ToxicologicalIndicatorNO3Avg");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorPPO4Avg_", "lake.ToxicologicalIndicatorPPO4Avg");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorCuAvg_", "lake.ToxicologicalIndicatorCuAvg");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorZnAvg_", "lake.ToxicologicalIndicatorZnAvg");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorMnAvg_", "lake.ToxicologicalIndicatorMnAvg");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorPbAvg_", "lake.ToxicologicalIndicatorPbAvg");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorNiAvg_", "lake.ToxicologicalIndicatorNiAvg");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorCdAvg_", "lake.ToxicologicalIndicatorCdAvg");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorCoAvg_", "lake.ToxicologicalIndicatorCoAvg");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorIZVNH4Avg_", "lake.ToxicologicalIndicatorIZVNH4Avg");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorIZVNO2Avg_", "lake.ToxicologicalIndicatorIZVNO2Avg");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorIZVNO3Avg_", "lake.ToxicologicalIndicatorIZVNO3Avg");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorIZVPPO4Avg_", "lake.ToxicologicalIndicatorIZVPPO4Avg");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorIZVCuAvg_", "lake.ToxicologicalIndicatorIZVCuAvg");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorIZVZnAvg_", "lake.ToxicologicalIndicatorIZVZnAvg");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorIZVMnAvg_", "lake.ToxicologicalIndicatorIZVMnAvg");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorIZVPbAvg_", "lake.ToxicologicalIndicatorIZVPbAvg");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorIZVNiAvg_", "lake.ToxicologicalIndicatorIZVNiAvg");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorIZVCdAvg_", "lake.ToxicologicalIndicatorIZVCdAvg");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorIZVCoAvg_", "lake.ToxicologicalIndicatorIZVCoAvg");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorKIZVkoNH4Avg_", "lake.ToxicologicalIndicatorKIZVkoNH4Avg");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorKIZVkoNO2Avg_", "lake.ToxicologicalIndicatorKIZVkoNO2Avg");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorKIZVkoNO3Avg_", "lake.ToxicologicalIndicatorKIZVkoNO3Avg");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorKIZVkoPPO4Avg_", "lake.ToxicologicalIndicatorKIZVkoPPO4Avg");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorKIZVkoCuAvg_", "lake.ToxicologicalIndicatorKIZVkoCuAvg");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorKIZVkoZnAvg_", "lake.ToxicologicalIndicatorKIZVkoZnAvg");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorKIZVkoMnAvg_", "lake.ToxicologicalIndicatorKIZVkoMnAvg");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorKIZVkoPbAvg_", "lake.ToxicologicalIndicatorKIZVkoPbAvg");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorKIZVkoNiAvg_", "lake.ToxicologicalIndicatorKIZVkoNiAvg");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorKIZVkoCdAvg_", "lake.ToxicologicalIndicatorKIZVkoCdAvg");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorKIZVkoCoAvg_", "lake.ToxicologicalIndicatorKIZVkoCoAvg");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorKIZVAvg_", "lake.ToxicologicalIndicatorKIZVAvg");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorNH4Max_", "lake.ToxicologicalIndicatorNH4Max");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorNO2Max_", "lake.ToxicologicalIndicatorNO2Max");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorNO3Max_", "lake.ToxicologicalIndicatorNO3Max");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorPPO4Max_", "lake.ToxicologicalIndicatorPPO4Max");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorCuMax_", "lake.ToxicologicalIndicatorCuMax");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorZnMax_", "lake.ToxicologicalIndicatorZnMax");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorMnMax_", "lake.ToxicologicalIndicatorMnMax");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorPbMax_", "lake.ToxicologicalIndicatorPbMax");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorNiMax_", "lake.ToxicologicalIndicatorNiMax");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorCdMax_", "lake.ToxicologicalIndicatorCdMax");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorCoMax_", "lake.ToxicologicalIndicatorCoMax");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorIZVNH4Max_", "lake.ToxicologicalIndicatorIZVNH4Max");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorIZVNO2Max_", "lake.ToxicologicalIndicatorIZVNO2Max");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorIZVNO3Max_", "lake.ToxicologicalIndicatorIZVNO3Max");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorIZVPPO4Max_", "lake.ToxicologicalIndicatorIZVPPO4Max");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorIZVCuMax_", "lake.ToxicologicalIndicatorIZVCuMax");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorIZVZnMax_", "lake.ToxicologicalIndicatorIZVZnMax");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorIZVMnMax_", "lake.ToxicologicalIndicatorIZVMnMax");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorIZVPbMax_", "lake.ToxicologicalIndicatorIZVPbMax");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorIZVNiMax_", "lake.ToxicologicalIndicatorIZVNiMax");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorIZVCdMax_", "lake.ToxicologicalIndicatorIZVCdMax");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorIZVCoMax_", "lake.ToxicologicalIndicatorIZVCoMax");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorKIZVkoNH4Max_", "lake.ToxicologicalIndicatorKIZVkoNH4Max");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorKIZVkoNO2Max_", "lake.ToxicologicalIndicatorKIZVkoNO2Max");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorKIZVkoNO3Max_", "lake.ToxicologicalIndicatorKIZVkoNO3Max");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorKIZVkoPPO4Max_", "lake.ToxicologicalIndicatorKIZVkoPPO4Max");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorKIZVkoCuMax_", "lake.ToxicologicalIndicatorKIZVkoCuMax");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorKIZVkoZnMax_", "lake.ToxicologicalIndicatorKIZVkoZnMax");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorKIZVkoMnMax_", "lake.ToxicologicalIndicatorKIZVkoMnMax");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorKIZVkoPbMax_", "lake.ToxicologicalIndicatorKIZVkoPbMax");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorKIZVkoNiMax_", "lake.ToxicologicalIndicatorKIZVkoNiMax");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorKIZVkoCdMax_", "lake.ToxicologicalIndicatorKIZVkoCdMax");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorKIZVkoCoMax_", "lake.ToxicologicalIndicatorKIZVkoCoMax");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorKIZVMax_", "lake.ToxicologicalIndicatorKIZVMax");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorNH4Min_", "lake.ToxicologicalIndicatorNH4Min");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorNO2Min_", "lake.ToxicologicalIndicatorNO2Min");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorNO3Min_", "lake.ToxicologicalIndicatorNO3Min");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorPPO4Min_", "lake.ToxicologicalIndicatorPPO4Min");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorCuMin_", "lake.ToxicologicalIndicatorCuMin");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorZnMin_", "lake.ToxicologicalIndicatorZnMin");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorMnMin_", "lake.ToxicologicalIndicatorMnMin");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorPbMin_", "lake.ToxicologicalIndicatorPbMin");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorNiMin_", "lake.ToxicologicalIndicatorNiMin");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorCdMin_", "lake.ToxicologicalIndicatorCdMin");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorCoMin_", "lake.ToxicologicalIndicatorCoMin");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorIZVNH4Min_", "lake.ToxicologicalIndicatorIZVNH4Min");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorIZVNO2Min_", "lake.ToxicologicalIndicatorIZVNO2Min");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorIZVNO3Min_", "lake.ToxicologicalIndicatorIZVNO3Min");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorIZVPPO4Min_", "lake.ToxicologicalIndicatorIZVPPO4Min");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorIZVCuMin_", "lake.ToxicologicalIndicatorIZVCuMin");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorIZVZnMin_", "lake.ToxicologicalIndicatorIZVZnMin");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorIZVMnMin_", "lake.ToxicologicalIndicatorIZVMnMin");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorIZVPbMin_", "lake.ToxicologicalIndicatorIZVPbMin");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorIZVNiMin_", "lake.ToxicologicalIndicatorIZVNiMin");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorIZVCdMin_", "lake.ToxicologicalIndicatorIZVCdMin");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorIZVCoMin_", "lake.ToxicologicalIndicatorIZVCoMin");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorKIZVkoNH4Min_", "lake.ToxicologicalIndicatorKIZVkoNH4Min");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorKIZVkoNO2Min_", "lake.ToxicologicalIndicatorKIZVkoNO2Min");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorKIZVkoNO3Min_", "lake.ToxicologicalIndicatorKIZVkoNO3Min");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorKIZVkoPPO4Min_", "lake.ToxicologicalIndicatorKIZVkoPPO4Min");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorKIZVkoCuMin_", "lake.ToxicologicalIndicatorKIZVkoCuMin");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorKIZVkoZnMin_", "lake.ToxicologicalIndicatorKIZVkoZnMin");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorKIZVkoMnMin_", "lake.ToxicologicalIndicatorKIZVkoMnMin");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorKIZVkoPbMin_", "lake.ToxicologicalIndicatorKIZVkoPbMin");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorKIZVkoNiMin_", "lake.ToxicologicalIndicatorKIZVkoNiMin");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorKIZVkoCdMin_", "lake.ToxicologicalIndicatorKIZVkoCdMin");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorKIZVkoCoMin_", "lake.ToxicologicalIndicatorKIZVkoCoMin");
            codeFilter = codeFilter.Replace("_ToxicologicalIndicatorKIZVMin_", "lake.ToxicologicalIndicatorKIZVMin");

            codeFilter = codeFilter.Replace("_TransitionNoСhange_", "lake.TransitionNoСhange");
            codeFilter = codeFilter.Replace("_TransitionPermanent_", "lake.TransitionPermanent");
            codeFilter = codeFilter.Replace("_TransitionNewPermanent_", "lake.TransitionNewPermanent");
            codeFilter = codeFilter.Replace("_TransitionLostPermanent_", "lake.TransitionLostPermanent");
            codeFilter = codeFilter.Replace("_TransitionSeasonal_", "lake.TransitionSeasonal");
            codeFilter = codeFilter.Replace("_TransitionNewSeasonal_", "lake.TransitionNewSeasonal");
            codeFilter = codeFilter.Replace("_TransitionLostSeasonal_", "lake.TransitionLostSeasonal");
            codeFilter = codeFilter.Replace("_TransitionSeasonalToPermanent_", "lake.TransitionSeasonalToPermanent");
            codeFilter = codeFilter.Replace("_TransitionPermanentToDeasonal_", "lake.TransitionPermanentToDeasonal");
            codeFilter = codeFilter.Replace("_TransitionEphemeralPermanent_", "lake.TransitionEphemeralPermanent");
            codeFilter = codeFilter.Replace("_TransitionEphemeralSeasonal_", "lake.TransitionEphemeralSeasonal");
            codeFilter = codeFilter.Replace("_TransitionMaximumWater_", "lake.TransitionMaximumWater");
            codeFilter = codeFilter.Replace("_TransitionPermanentPer_", "lake.TransitionPermanentPer");
            codeFilter = codeFilter.Replace("_TransitionSeasonalPer_", "lake.TransitionSeasonalPer");

            codeFilter = codeFilter.Replace("_SeasonalitNoData_", "lake.SeasonalitNoData");
            codeFilter = codeFilter.Replace("_SeasonalitJanuary_", "lake.SeasonalitJanuary");
            codeFilter = codeFilter.Replace("_SeasonalitFebruary_", "lake.SeasonalitFebruary");
            codeFilter = codeFilter.Replace("_SeasonalitMarch_", "lake.SeasonalitMarch");
            codeFilter = codeFilter.Replace("_SeasonalitApril_", "lake.SeasonalitApril");
            codeFilter = codeFilter.Replace("_SeasonalitMay_", "lake.SeasonalitMay");
            codeFilter = codeFilter.Replace("_SeasonalitJune_", "lake.SeasonalitJune");
            codeFilter = codeFilter.Replace("_SeasonalitJuly_", "lake.SeasonalitJuly");
            codeFilter = codeFilter.Replace("_SeasonalitAugust_", "lake.SeasonalitAugust");
            codeFilter = codeFilter.Replace("_SeasonalitSeptember_", "lake.SeasonalitSeptember");
            codeFilter = codeFilter.Replace("_SeasonalitOctober_", "lake.SeasonalitOctober");
            codeFilter = codeFilter.Replace("_SeasonalitNovember_", "lake.SeasonalitNovember");
            codeFilter = codeFilter.Replace("_SeasonalitDecember_", "lake.SeasonalitDecember");

            codeFilter = codeFilter.Replace("_DynamicsLakeAreaNoDataPersAvg_", "lake.DynamicsLakeAreaNoDataPersAvg");
            codeFilter = codeFilter.Replace("_DynamicsLakeAreaNotWaterAvg_", "lake.DynamicsLakeAreaNotWaterAvg");
            codeFilter = codeFilter.Replace("_DynamicsLakeAreaSeasonalWaterAreaAvg_", "lake.DynamicsLakeAreaSeasonalWaterAreaAvg");
            codeFilter = codeFilter.Replace("_DynamicsLakeAreaPermanentWaterAreaAvg_", "lake.DynamicsLakeAreaPermanentWaterAreaAvg");
            codeFilter = codeFilter.Replace("_DynamicsLakeAreaMaximumWaterAreaAvg_", "lake.DynamicsLakeAreaMaximumWaterAreaAvg");
            codeFilter = codeFilter.Replace("_DynamicsLakeAreaSeasonalWaterAreaPerAvg_", "lake.DynamicsLakeAreaSeasonalWaterAreaPerAvg");
            codeFilter = codeFilter.Replace("_DynamicsLakeAreaPermanentWaterAreaPerAvg_", "lake.DynamicsLakeAreaPermanentWaterAreaPerAvg");
            codeFilter = codeFilter.Replace("_DynamicsLakeAreaNoDataPersMax_", "lake.DynamicsLakeAreaNoDataPersMax");
            codeFilter = codeFilter.Replace("_DynamicsLakeAreaNotWaterMax_", "lake.DynamicsLakeAreaNotWaterMax");
            codeFilter = codeFilter.Replace("_DynamicsLakeAreaSeasonalWaterAreaMax_", "lake.DynamicsLakeAreaSeasonalWaterAreaMax");
            codeFilter = codeFilter.Replace("_DynamicsLakeAreaPermanentWaterAreaMax_", "lake.DynamicsLakeAreaPermanentWaterAreaMax");
            codeFilter = codeFilter.Replace("_DynamicsLakeAreaMaximumWaterAreaMax_", "lake.DynamicsLakeAreaMaximumWaterAreaMax");
            codeFilter = codeFilter.Replace("_DynamicsLakeAreaSeasonalWaterAreaPerMax_", "lake.DynamicsLakeAreaSeasonalWaterAreaPerMax");
            codeFilter = codeFilter.Replace("_DynamicsLakeAreaPermanentWaterAreaPerMax_", "lake.DynamicsLakeAreaPermanentWaterAreaPerMax");
            codeFilter = codeFilter.Replace("_DynamicsLakeAreaNoDataPersMin_", "lake.DynamicsLakeAreaNoDataPersMin");
            codeFilter = codeFilter.Replace("_DynamicsLakeAreaNotWaterMin_", "lake.DynamicsLakeAreaNotWaterMin");
            codeFilter = codeFilter.Replace("_DynamicsLakeAreaSeasonalWaterAreaMin_", "lake.DynamicsLakeAreaSeasonalWaterAreaMin");
            codeFilter = codeFilter.Replace("_DynamicsLakeAreaPermanentWaterAreaMin_", "lake.DynamicsLakeAreaPermanentWaterAreaMin");
            codeFilter = codeFilter.Replace("_DynamicsLakeAreaMaximumWaterAreaMin_", "lake.DynamicsLakeAreaMaximumWaterAreaMin");
            codeFilter = codeFilter.Replace("_DynamicsLakeAreaSeasonalWaterAreaPerMin_", "lake.DynamicsLakeAreaSeasonalWaterAreaPerMin");
            codeFilter = codeFilter.Replace("_DynamicsLakeAreaPermanentWaterAreaPerMin_", "lake.DynamicsLakeAreaPermanentWaterAreaPerMin");

            bool checkFormula = CheckFormula(Formula);

            string codePopulateLakes = "";

            
            //DirectoryInfo di = new DirectoryInfo(sContentRootPath);
            //foreach (FileInfo filed in di.GetFiles())
            //{
            //    try
            //    {
            //        filed.Delete();
            //    }
            //    catch
            //    {
            //    }
            //}
            

            // Analyze 2
            string codeToCompile = @"using System;
                using System.Collections.Generic;
                using System.IO;
                using System.Linq;
                //using System.Linq;
                namespace RoslynCompile
                {
                    // lake for analytics
                    public class Lake
                    {
                        public int Id { get; set; }
                        public int LakeId { get; set; }
                        public int Year { get; set; }
                        public decimal Area2015 { get; set; }
                        public decimal LakeShorelineLength2015 { get; set; }

                        public decimal LakesArchiveDataLakeLength { get; set; }
                        public decimal LakesArchiveDataLakeShorelineLength { get; set; }
                        public decimal LakesArchiveDataLakeMirrorArea { get; set; }
                        public decimal LakesArchiveDataLakeAbsoluteHeight { get; set; }
                        public decimal LakesArchiveDataLakeWidth { get; set; }
                        public decimal LakesArchiveDataLakeMaxDepth { get; set; }
                        public decimal LakesArchiveDataLakeWaterMass { get; set; }

                        public decimal LakesGlobalDataLake_area { get; set; }
                        public decimal LakesGlobalDataShore_len { get; set; }
                        public decimal LakesGlobalDataShore_dev { get; set; }
                        public decimal LakesGlobalDataVol_total { get; set; }
                        public decimal LakesGlobalDataDepth_avg { get; set; }
                        public decimal LakesGlobalDataDis_avg { get; set; }
                        public decimal LakesGlobalDataRes_time { get; set; }
                        public decimal LakesGlobalDataElevation { get; set; }
                        public decimal LakesGlobalDataSlope_100 { get; set; }
                        public decimal LakesGlobalDataWshd_area { get; set; }

                        public decimal WaterBalanceSurfaceFlowAvg { get; set; }
                        public decimal WaterBalanceSurfaceOutflowAvg { get; set; }
                        public decimal WaterBalanceUndergroundFlowAvg { get; set; }
                        public decimal WaterBalanceUndergroundOutflowAvg { get; set; }
                        public decimal WaterBalancePrecipitationAvg { get; set; }
                        public decimal WaterBalanceEvaporationAvg { get; set; }
                        public decimal WaterBalanceWaterBalanceReceiptAvg { get; set; }
                        public decimal WaterBalanceWaterBalanceExpenditureAvg { get; set; }
                        public decimal WaterBalanceDiscrepancyAvg { get; set; }
                        public decimal WaterBalanceSurfaceFlowPerAvg { get; set; }
                        public decimal WaterBalanceUndergroundFlowPerAvg { get; set; }
                        public decimal WaterBalancePrecipitationPerAvg { get; set; }
                        public decimal WaterBalanceSurfaceOutflowPerAvg { get; set; }
                        public decimal WaterBalanceUndergroundOutflowPerAvg { get; set; }
                        public decimal WaterBalanceEvaporationPerAvg { get; set; }
                        public decimal WaterBalanceWaterBalanceReceiptPerAvg { get; set; }
                        public decimal WaterBalanceWaterBalanceExpenditurePerAvg { get; set; }
                        public decimal WaterBalanceSurfaceFlowMax { get; set; }
                        public decimal WaterBalanceSurfaceOutflowMax { get; set; }
                        public decimal WaterBalanceUndergroundFlowMax { get; set; }
                        public decimal WaterBalanceUndergroundOutflowMax { get; set; }
                        public decimal WaterBalancePrecipitationMax { get; set; }
                        public decimal WaterBalanceEvaporationMax { get; set; }
                        public decimal WaterBalanceWaterBalanceReceiptMax { get; set; }
                        public decimal WaterBalanceWaterBalanceExpenditureMax { get; set; }
                        public decimal WaterBalanceDiscrepancyMax { get; set; }
                        public decimal WaterBalanceSurfaceFlowPerMax { get; set; }
                        public decimal WaterBalanceUndergroundFlowPerMax { get; set; }
                        public decimal WaterBalancePrecipitationPerMax { get; set; }
                        public decimal WaterBalanceSurfaceOutflowPerMax { get; set; }
                        public decimal WaterBalanceUndergroundOutflowPerMax { get; set; }
                        public decimal WaterBalanceEvaporationPerMax { get; set; }
                        public decimal WaterBalanceWaterBalanceReceiptPerMax { get; set; }
                        public decimal WaterBalanceWaterBalanceExpenditurePerMax { get; set; }
                        public decimal WaterBalanceSurfaceFlowMin { get; set; }
                        public decimal WaterBalanceSurfaceOutflowMin { get; set; }
                        public decimal WaterBalanceUndergroundFlowMin { get; set; }
                        public decimal WaterBalanceUndergroundOutflowMin { get; set; }
                        public decimal WaterBalancePrecipitationMin { get; set; }
                        public decimal WaterBalanceEvaporationMin { get; set; }
                        public decimal WaterBalanceWaterBalanceReceiptMin { get; set; }
                        public decimal WaterBalanceWaterBalanceExpenditureMin { get; set; }
                        public decimal WaterBalanceDiscrepancyMin { get; set; }
                        public decimal WaterBalanceSurfaceFlowPerMin { get; set; }
                        public decimal WaterBalanceUndergroundFlowPerMin { get; set; }
                        public decimal WaterBalancePrecipitationPerMin { get; set; }
                        public decimal WaterBalanceSurfaceOutflowPerMin { get; set; }
                        public decimal WaterBalanceUndergroundOutflowPerMin { get; set; }
                        public decimal WaterBalanceEvaporationPerMin { get; set; }
                        public decimal WaterBalanceWaterBalanceReceiptPerMin { get; set; }
                        public decimal WaterBalanceWaterBalanceExpenditurePerMin { get; set; }

                        public decimal WaterLevelWaterLavelMAvg { get; set; }
                        public decimal WaterLevelWaterLavelMMax { get; set; }
                        public decimal WaterLevelWaterLavelMMin { get; set; }

                        public decimal BathigraphicAndVolumetricCurveDataWaterLevelAvg { get; set; }
                        public decimal BathigraphicAndVolumetricCurveDataLakeAreaAvg { get; set; }
                        public decimal BathigraphicAndVolumetricCurveDataWaterMassVolumeAvg { get; set; }
                        public decimal BathigraphicAndVolumetricCurveDataWaterLevelMax { get; set; }
                        public decimal BathigraphicAndVolumetricCurveDataLakeAreaMax { get; set; }
                        public decimal BathigraphicAndVolumetricCurveDataWaterMassVolumeMax { get; set; }
                        public decimal BathigraphicAndVolumetricCurveDataWaterLevelMin { get; set; }
                        public decimal BathigraphicAndVolumetricCurveDataLakeAreaMin { get; set; }
                        public decimal BathigraphicAndVolumetricCurveDataWaterMassVolumeMin { get; set; }

                        public decimal GeneralHydrochemicalIndicatorMineralizationAvg { get; set; }
                        public decimal GeneralHydrochemicalIndicatorTotalHardnessAvg { get; set; }
                        public decimal GeneralHydrochemicalIndicatorDissOxygWaterAvg { get; set; }
                        public decimal GeneralHydrochemicalIndicatorPercentOxygWaterAvg { get; set; }
                        public decimal GeneralHydrochemicalIndicatorpHAvg { get; set; }
                        public decimal GeneralHydrochemicalIndicatorOrganicSubstancesAvg { get; set; }
                        public decimal GeneralHydrochemicalIndicatorMineralizationMax { get; set; }
                        public decimal GeneralHydrochemicalIndicatorTotalHardnessMax { get; set; }
                        public decimal GeneralHydrochemicalIndicatorDissOxygWaterMax { get; set; }
                        public decimal GeneralHydrochemicalIndicatorPercentOxygWaterMax { get; set; }
                        public decimal GeneralHydrochemicalIndicatorpHMax { get; set; }
                        public decimal GeneralHydrochemicalIndicatorOrganicSubstancesMax { get; set; }
                        public decimal GeneralHydrochemicalIndicatorMineralizationMin { get; set; }
                        public decimal GeneralHydrochemicalIndicatorTotalHardnessMin { get; set; }
                        public decimal GeneralHydrochemicalIndicatorDissOxygWaterMin { get; set; }
                        public decimal GeneralHydrochemicalIndicatorPercentOxygWaterMin { get; set; }
                        public decimal GeneralHydrochemicalIndicatorpHMin { get; set; }
                        public decimal GeneralHydrochemicalIndicatorOrganicSubstancesMin { get; set; }

                        public decimal IonsaltWaterCompositionCaMgEqAvg { get; set; }
                        public decimal IonsaltWaterCompositionMgMgEqAvg { get; set; }
                        public decimal IonsaltWaterCompositionNaKMgEqAvg { get; set; }
                        public decimal IonsaltWaterCompositionClMgEqAvg { get; set; }
                        public decimal IonsaltWaterCompositionHCOMgEqAvg { get; set; }
                        public decimal IonsaltWaterCompositionSOMgEqAvg { get; set; }
                        public decimal IonsaltWaterCompositionCaMgAvg { get; set; }
                        public decimal IonsaltWaterCompositionMgMgAvg { get; set; }
                        public decimal IonsaltWaterCompositionNaKMgAvg { get; set; }
                        public decimal IonsaltWaterCompositionClMgAvg { get; set; }
                        public decimal IonsaltWaterCompositionHCOMgAvg { get; set; }
                        public decimal IonsaltWaterCompositionSOMgAvg { get; set; }
                        public decimal IonsaltWaterCompositionCationsSumMgEqAvg { get; set; }
                        public decimal IonsaltWaterCompositionAnionsSumMgEqAvg { get; set; }
                        public decimal IonsaltWaterCompositionIonsSumMgEqAvg { get; set; }
                        public decimal IonsaltWaterCompositionCaPerEqAvg { get; set; }
                        public decimal IonsaltWaterCompositionMgPerEqAvg { get; set; }
                        public decimal IonsaltWaterCompositionNaKPerEqAvg { get; set; }
                        public decimal IonsaltWaterCompositionClPerEqAvg { get; set; }
                        public decimal IonsaltWaterCompositionHCOPerEqAvg { get; set; }
                        public decimal IonsaltWaterCompositionSOPerEqAvg { get; set; }
                        public decimal IonsaltWaterCompositionCaMgEqMax { get; set; }
                        public decimal IonsaltWaterCompositionMgMgEqMax { get; set; }
                        public decimal IonsaltWaterCompositionNaKMgEqMax { get; set; }
                        public decimal IonsaltWaterCompositionClMgEqMax { get; set; }
                        public decimal IonsaltWaterCompositionHCOMgEqMax { get; set; }
                        public decimal IonsaltWaterCompositionSOMgEqMax { get; set; }
                        public decimal IonsaltWaterCompositionCaMgMax { get; set; }
                        public decimal IonsaltWaterCompositionMgMgMax { get; set; }
                        public decimal IonsaltWaterCompositionNaKMgMax { get; set; }
                        public decimal IonsaltWaterCompositionClMgMax { get; set; }
                        public decimal IonsaltWaterCompositionHCOMgMax { get; set; }
                        public decimal IonsaltWaterCompositionSOMgMax { get; set; }
                        public decimal IonsaltWaterCompositionCationsSumMgEqMax { get; set; }
                        public decimal IonsaltWaterCompositionAnionsSumMgEqMax { get; set; }
                        public decimal IonsaltWaterCompositionIonsSumMgEqMax { get; set; }
                        public decimal IonsaltWaterCompositionCaPerEqMax { get; set; }
                        public decimal IonsaltWaterCompositionMgPerEqMax { get; set; }
                        public decimal IonsaltWaterCompositionNaKPerEqMax { get; set; }
                        public decimal IonsaltWaterCompositionClPerEqMax { get; set; }
                        public decimal IonsaltWaterCompositionHCOPerEqMax { get; set; }
                        public decimal IonsaltWaterCompositionSOPerEqMax { get; set; }
                        public decimal IonsaltWaterCompositionCaMgEqMin { get; set; }
                        public decimal IonsaltWaterCompositionMgMgEqMin { get; set; }
                        public decimal IonsaltWaterCompositionNaKMgEqMin { get; set; }
                        public decimal IonsaltWaterCompositionClMgEqMin { get; set; }
                        public decimal IonsaltWaterCompositionHCOMgEqMin { get; set; }
                        public decimal IonsaltWaterCompositionSOMgEqMin { get; set; }
                        public decimal IonsaltWaterCompositionCaMgMin { get; set; }
                        public decimal IonsaltWaterCompositionMgMgMin { get; set; }
                        public decimal IonsaltWaterCompositionNaKMgMin { get; set; }
                        public decimal IonsaltWaterCompositionClMgMin { get; set; }
                        public decimal IonsaltWaterCompositionHCOMgMin { get; set; }
                        public decimal IonsaltWaterCompositionSOMgMin { get; set; }
                        public decimal IonsaltWaterCompositionCationsSumMgEqMin { get; set; }
                        public decimal IonsaltWaterCompositionAnionsSumMgEqMin { get; set; }
                        public decimal IonsaltWaterCompositionIonsSumMgEqMin { get; set; }
                        public decimal IonsaltWaterCompositionCaPerEqMin { get; set; }
                        public decimal IonsaltWaterCompositionMgPerEqMin { get; set; }
                        public decimal IonsaltWaterCompositionNaKPerEqMin { get; set; }
                        public decimal IonsaltWaterCompositionClPerEqMin { get; set; }
                        public decimal IonsaltWaterCompositionHCOPerEqMin { get; set; }
                        public decimal IonsaltWaterCompositionSOPerEqMin { get; set; }

                        public decimal ToxicologicalIndicatorNH4Avg { get; set; }
                        public decimal ToxicologicalIndicatorNO2Avg { get; set; }
                        public decimal ToxicologicalIndicatorNO3Avg { get; set; }
                        public decimal ToxicologicalIndicatorPPO4Avg { get; set; }
                        public decimal ToxicologicalIndicatorCuAvg { get; set; }
                        public decimal ToxicologicalIndicatorZnAvg { get; set; }
                        public decimal ToxicologicalIndicatorMnAvg { get; set; }
                        public decimal ToxicologicalIndicatorPbAvg { get; set; }
                        public decimal ToxicologicalIndicatorNiAvg { get; set; }
                        public decimal ToxicologicalIndicatorCdAvg { get; set; }
                        public decimal ToxicologicalIndicatorCoAvg { get; set; }
                        public decimal ToxicologicalIndicatorIZVNH4Avg { get; set; }
                        public decimal ToxicologicalIndicatorIZVNO2Avg { get; set; }
                        public decimal ToxicologicalIndicatorIZVNO3Avg { get; set; }
                        public decimal ToxicologicalIndicatorIZVPPO4Avg { get; set; }
                        public decimal ToxicologicalIndicatorIZVCuAvg { get; set; }
                        public decimal ToxicologicalIndicatorIZVZnAvg { get; set; }
                        public decimal ToxicologicalIndicatorIZVMnAvg { get; set; }
                        public decimal ToxicologicalIndicatorIZVPbAvg { get; set; }
                        public decimal ToxicologicalIndicatorIZVNiAvg { get; set; }
                        public decimal ToxicologicalIndicatorIZVCdAvg { get; set; }
                        public decimal ToxicologicalIndicatorIZVCoAvg { get; set; }
                        public decimal ToxicologicalIndicatorKIZVkoNH4Avg { get; set; }
                        public decimal ToxicologicalIndicatorKIZVkoNO2Avg { get; set; }
                        public decimal ToxicologicalIndicatorKIZVkoNO3Avg { get; set; }
                        public decimal ToxicologicalIndicatorKIZVkoPPO4Avg { get; set; }
                        public decimal ToxicologicalIndicatorKIZVkoCuAvg { get; set; }
                        public decimal ToxicologicalIndicatorKIZVkoZnAvg { get; set; }
                        public decimal ToxicologicalIndicatorKIZVkoMnAvg { get; set; }
                        public decimal ToxicologicalIndicatorKIZVkoPbAvg { get; set; }
                        public decimal ToxicologicalIndicatorKIZVkoNiAvg { get; set; }
                        public decimal ToxicologicalIndicatorKIZVkoCdAvg { get; set; }
                        public decimal ToxicologicalIndicatorKIZVkoCoAvg { get; set; }
                        public decimal ToxicologicalIndicatorKIZVAvg { get; set; }
                        public decimal ToxicologicalIndicatorNH4Max { get; set; }
                        public decimal ToxicologicalIndicatorNO2Max { get; set; }
                        public decimal ToxicologicalIndicatorNO3Max { get; set; }
                        public decimal ToxicologicalIndicatorPPO4Max { get; set; }
                        public decimal ToxicologicalIndicatorCuMax { get; set; }
                        public decimal ToxicologicalIndicatorZnMax { get; set; }
                        public decimal ToxicologicalIndicatorMnMax { get; set; }
                        public decimal ToxicologicalIndicatorPbMax { get; set; }
                        public decimal ToxicologicalIndicatorNiMax { get; set; }
                        public decimal ToxicologicalIndicatorCdMax { get; set; }
                        public decimal ToxicologicalIndicatorCoMax { get; set; }
                        public decimal ToxicologicalIndicatorIZVNH4Max { get; set; }
                        public decimal ToxicologicalIndicatorIZVNO2Max { get; set; }
                        public decimal ToxicologicalIndicatorIZVNO3Max { get; set; }
                        public decimal ToxicologicalIndicatorIZVPPO4Max { get; set; }
                        public decimal ToxicologicalIndicatorIZVCuMax { get; set; }
                        public decimal ToxicologicalIndicatorIZVZnMax { get; set; }
                        public decimal ToxicologicalIndicatorIZVMnMax { get; set; }
                        public decimal ToxicologicalIndicatorIZVPbMax { get; set; }
                        public decimal ToxicologicalIndicatorIZVNiMax { get; set; }
                        public decimal ToxicologicalIndicatorIZVCdMax { get; set; }
                        public decimal ToxicologicalIndicatorIZVCoMax { get; set; }
                        public decimal ToxicologicalIndicatorKIZVkoNH4Max { get; set; }
                        public decimal ToxicologicalIndicatorKIZVkoNO2Max { get; set; }
                        public decimal ToxicologicalIndicatorKIZVkoNO3Max { get; set; }
                        public decimal ToxicologicalIndicatorKIZVkoPPO4Max { get; set; }
                        public decimal ToxicologicalIndicatorKIZVkoCuMax { get; set; }
                        public decimal ToxicologicalIndicatorKIZVkoZnMax { get; set; }
                        public decimal ToxicologicalIndicatorKIZVkoMnMax { get; set; }
                        public decimal ToxicologicalIndicatorKIZVkoPbMax { get; set; }
                        public decimal ToxicologicalIndicatorKIZVkoNiMax { get; set; }
                        public decimal ToxicologicalIndicatorKIZVkoCdMax { get; set; }
                        public decimal ToxicologicalIndicatorKIZVkoCoMax { get; set; }
                        public decimal ToxicologicalIndicatorKIZVMax { get; set; }
                        public decimal ToxicologicalIndicatorNH4Min { get; set; }
                        public decimal ToxicologicalIndicatorNO2Min { get; set; }
                        public decimal ToxicologicalIndicatorNO3Min { get; set; }
                        public decimal ToxicologicalIndicatorPPO4Min { get; set; }
                        public decimal ToxicologicalIndicatorCuMin { get; set; }
                        public decimal ToxicologicalIndicatorZnMin { get; set; }
                        public decimal ToxicologicalIndicatorMnMin { get; set; }
                        public decimal ToxicologicalIndicatorPbMin { get; set; }
                        public decimal ToxicologicalIndicatorNiMin { get; set; }
                        public decimal ToxicologicalIndicatorCdMin { get; set; }
                        public decimal ToxicologicalIndicatorCoMin { get; set; }
                        public decimal ToxicologicalIndicatorIZVNH4Min { get; set; }
                        public decimal ToxicologicalIndicatorIZVNO2Min { get; set; }
                        public decimal ToxicologicalIndicatorIZVNO3Min { get; set; }
                        public decimal ToxicologicalIndicatorIZVPPO4Min { get; set; }
                        public decimal ToxicologicalIndicatorIZVCuMin { get; set; }
                        public decimal ToxicologicalIndicatorIZVZnMin { get; set; }
                        public decimal ToxicologicalIndicatorIZVMnMin { get; set; }
                        public decimal ToxicologicalIndicatorIZVPbMin { get; set; }
                        public decimal ToxicologicalIndicatorIZVNiMin { get; set; }
                        public decimal ToxicologicalIndicatorIZVCdMin { get; set; }
                        public decimal ToxicologicalIndicatorIZVCoMin { get; set; }
                        public decimal ToxicologicalIndicatorKIZVkoNH4Min { get; set; }
                        public decimal ToxicologicalIndicatorKIZVkoNO2Min { get; set; }
                        public decimal ToxicologicalIndicatorKIZVkoNO3Min { get; set; }
                        public decimal ToxicologicalIndicatorKIZVkoPPO4Min { get; set; }
                        public decimal ToxicologicalIndicatorKIZVkoCuMin { get; set; }
                        public decimal ToxicologicalIndicatorKIZVkoZnMin { get; set; }
                        public decimal ToxicologicalIndicatorKIZVkoMnMin { get; set; }
                        public decimal ToxicologicalIndicatorKIZVkoPbMin { get; set; }
                        public decimal ToxicologicalIndicatorKIZVkoNiMin { get; set; }
                        public decimal ToxicologicalIndicatorKIZVkoCdMin { get; set; }
                        public decimal ToxicologicalIndicatorKIZVkoCoMin { get; set; }
                        public decimal ToxicologicalIndicatorKIZVMin { get; set; }

                        public decimal TransitionNoСhange { get; set; }
                        public decimal TransitionPermanent { get; set; }
                        public decimal TransitionNewPermanent { get; set; }
                        public decimal TransitionLostPermanent { get; set; }
                        public decimal TransitionSeasonal { get; set; }
                        public decimal TransitionNewSeasonal { get; set; }
                        public decimal TransitionLostSeasonal { get; set; }
                        public decimal TransitionSeasonalToPermanent { get; set; }
                        public decimal TransitionPermanentToDeasonal { get; set; }
                        public decimal TransitionEphemeralPermanent { get; set; }
                        public decimal TransitionEphemeralSeasonal { get; set; }
                        public decimal TransitionMaximumWater { get; set; }
                        public decimal TransitionPermanentPer { get; set; }
                        public decimal TransitionSeasonalPer { get; set; }

                        public decimal SeasonalitNoData { get; set; }
                        public decimal SeasonalitJanuary { get; set; }
                        public decimal SeasonalitFebruary { get; set; }
                        public decimal SeasonalitMarch { get; set; }
                        public decimal SeasonalitApril { get; set; }
                        public decimal SeasonalitMay { get; set; }
                        public decimal SeasonalitJune { get; set; }
                        public decimal SeasonalitJuly { get; set; }
                        public decimal SeasonalitAugust { get; set; }
                        public decimal SeasonalitSeptember { get; set; }
                        public decimal SeasonalitOctober { get; set; }
                        public decimal SeasonalitNovember { get; set; }
                        public decimal SeasonalitDecember { get; set; }

                        public decimal DynamicsLakeAreaNoDataPersAvg { get; set; }
                        public decimal DynamicsLakeAreaNotWaterAvg { get; set; }
                        public decimal DynamicsLakeAreaSeasonalWaterAreaAvg { get; set; }
                        public decimal DynamicsLakeAreaPermanentWaterAreaAvg { get; set; }
                        public decimal DynamicsLakeAreaMaximumWaterAreaAvg { get; set; }
                        public decimal DynamicsLakeAreaSeasonalWaterAreaPerAvg { get; set; }
                        public decimal DynamicsLakeAreaPermanentWaterAreaPerAvg { get; set; }
                        public decimal DynamicsLakeAreaNoDataPersMax { get; set; }
                        public decimal DynamicsLakeAreaNotWaterMax { get; set; }
                        public decimal DynamicsLakeAreaSeasonalWaterAreaMax { get; set; }
                        public decimal DynamicsLakeAreaPermanentWaterAreaMax { get; set; }
                        public decimal DynamicsLakeAreaMaximumWaterAreaMax { get; set; }
                        public decimal DynamicsLakeAreaSeasonalWaterAreaPerMax { get; set; }
                        public decimal DynamicsLakeAreaPermanentWaterAreaPerMax { get; set; }
                        public decimal DynamicsLakeAreaNoDataPersMin { get; set; }
                        public decimal DynamicsLakeAreaNotWaterMin { get; set; }
                        public decimal DynamicsLakeAreaSeasonalWaterAreaMin { get; set; }
                        public decimal DynamicsLakeAreaPermanentWaterAreaMin { get; set; }
                        public decimal DynamicsLakeAreaMaximumWaterAreaMin { get; set; }
                        public decimal DynamicsLakeAreaSeasonalWaterAreaPerMin { get; set; }
                        public decimal DynamicsLakeAreaPermanentWaterAreaPerMin { get; set; }
                    } 

                    public class Calculator
                    {
                        //public List<Lake> Lakes { get; set; }
                        public decimal FromLine(string line)
                        {
                            return string.IsNullOrEmpty(line) ? 0 : Convert.ToDecimal(line);
                        }

                        public int?[] Calculate()
                        {
                            List<Lake> Lakes = new List<Lake>();
                            " + codePopulateLakes + @"

                            " + "string fileName = @\"" + filePopulateLakes + "\";" + @"

                            string line;
                            StreamReader file = new StreamReader(fileName);
                            while ((line = file.ReadLine()) != null)
                            {
                                string[] lineS = line.Split('\t');
                                Lakes.Add(new Lake()
                                {
                                    Id = Convert.ToInt32(lineS[0]),
                                    LakeId = Convert.ToInt32(lineS[1]),
                                    Year = Convert.ToInt32(lineS[2]),
                                    // Analyze 3
                                    Area2015 = FromLine(lineS[3]),
                                    LakeShorelineLength2015 = FromLine(lineS[4]),

                                    LakesArchiveDataLakeLength = FromLine(lineS[5]),
                                    LakesArchiveDataLakeShorelineLength = FromLine(lineS[6]),
                                    LakesArchiveDataLakeMirrorArea = FromLine(lineS[7]),
                                    LakesArchiveDataLakeAbsoluteHeight = FromLine(lineS[8]),
                                    LakesArchiveDataLakeWidth = FromLine(lineS[9]),
                                    LakesArchiveDataLakeMaxDepth = FromLine(lineS[10]),
                                    LakesArchiveDataLakeWaterMass = FromLine(lineS[11]),

                                    LakesGlobalDataLake_area = FromLine(lineS[12]),
                                    LakesGlobalDataShore_len = FromLine(lineS[13]),
                                    LakesGlobalDataShore_dev = FromLine(lineS[14]),
                                    LakesGlobalDataVol_total = FromLine(lineS[15]),
                                    LakesGlobalDataDepth_avg = FromLine(lineS[16]),
                                    LakesGlobalDataDis_avg = FromLine(lineS[17]),
                                    LakesGlobalDataRes_time = FromLine(lineS[18]),
                                    LakesGlobalDataElevation = FromLine(lineS[19]),
                                    LakesGlobalDataSlope_100 = FromLine(lineS[20]),
                                    LakesGlobalDataWshd_area = FromLine(lineS[21]),

                                    TransitionNoСhange = FromLine(lineS[22]),
                                    TransitionPermanent = FromLine(lineS[23]),
                                    TransitionNewPermanent = FromLine(lineS[24]),
                                    TransitionLostPermanent = FromLine(lineS[25]),
                                    TransitionSeasonal = FromLine(lineS[26]),
                                    TransitionNewSeasonal = FromLine(lineS[27]),
                                    TransitionLostSeasonal = FromLine(lineS[28]),
                                    TransitionSeasonalToPermanent = FromLine(lineS[29]),
                                    TransitionPermanentToDeasonal = FromLine(lineS[30]),
                                    TransitionEphemeralPermanent = FromLine(lineS[31]),
                                    TransitionEphemeralSeasonal = FromLine(lineS[32]),
                                    TransitionMaximumWater = FromLine(lineS[33]),
                                    TransitionPermanentPer = FromLine(lineS[34]),
                                    TransitionSeasonalPer = FromLine(lineS[35]),

                                    SeasonalitNoData = FromLine(lineS[36]),
                                    SeasonalitJanuary = FromLine(lineS[37]),
                                    SeasonalitFebruary = FromLine(lineS[38]),
                                    SeasonalitMarch = FromLine(lineS[39]),
                                    SeasonalitApril = FromLine(lineS[40]),
                                    SeasonalitMay = FromLine(lineS[41]),
                                    SeasonalitJune = FromLine(lineS[42]),
                                    SeasonalitJuly = FromLine(lineS[43]),
                                    SeasonalitAugust = FromLine(lineS[44]),
                                    SeasonalitSeptember = FromLine(lineS[45]),
                                    SeasonalitOctober = FromLine(lineS[46]),
                                    SeasonalitNovember = FromLine(lineS[47]),
                                    SeasonalitDecember = FromLine(lineS[48]),

                                    WaterBalanceSurfaceFlowAvg = FromLine(lineS[49]),
                                    WaterBalanceSurfaceOutflowAvg = FromLine(lineS[50]),
                                    WaterBalanceUndergroundFlowAvg = FromLine(lineS[51]),
                                    WaterBalanceUndergroundOutflowAvg = FromLine(lineS[52]),
                                    WaterBalancePrecipitationAvg = FromLine(lineS[53]),
                                    WaterBalanceEvaporationAvg = FromLine(lineS[54]),
                                    WaterBalanceWaterBalanceReceiptAvg = FromLine(lineS[55]),
                                    WaterBalanceWaterBalanceExpenditureAvg = FromLine(lineS[56]),
                                    WaterBalanceDiscrepancyAvg = FromLine(lineS[57]),
                                    WaterBalanceSurfaceFlowPerAvg = FromLine(lineS[58]),
                                    WaterBalanceUndergroundFlowPerAvg = FromLine(lineS[59]),
                                    WaterBalancePrecipitationPerAvg = FromLine(lineS[60]),
                                    WaterBalanceSurfaceOutflowPerAvg = FromLine(lineS[61]),
                                    WaterBalanceUndergroundOutflowPerAvg = FromLine(lineS[62]),
                                    WaterBalanceEvaporationPerAvg = FromLine(lineS[63]),
                                    WaterBalanceWaterBalanceReceiptPerAvg = FromLine(lineS[64]),
                                    WaterBalanceWaterBalanceExpenditurePerAvg = FromLine(lineS[65]),
                                    WaterBalanceSurfaceFlowMax = FromLine(lineS[66]),
                                    WaterBalanceSurfaceOutflowMax = FromLine(lineS[67]),
                                    WaterBalanceUndergroundFlowMax = FromLine(lineS[68]),
                                    WaterBalanceUndergroundOutflowMax = FromLine(lineS[69]),
                                    WaterBalancePrecipitationMax = FromLine(lineS[70]),
                                    WaterBalanceEvaporationMax = FromLine(lineS[71]),
                                    WaterBalanceWaterBalanceReceiptMax = FromLine(lineS[72]),
                                    WaterBalanceWaterBalanceExpenditureMax = FromLine(lineS[73]),
                                    WaterBalanceDiscrepancyMax = FromLine(lineS[74]),
                                    WaterBalanceSurfaceFlowPerMax = FromLine(lineS[75]),
                                    WaterBalanceUndergroundFlowPerMax = FromLine(lineS[76]),
                                    WaterBalancePrecipitationPerMax = FromLine(lineS[77]),
                                    WaterBalanceSurfaceOutflowPerMax = FromLine(lineS[78]),
                                    WaterBalanceUndergroundOutflowPerMax = FromLine(lineS[79]),
                                    WaterBalanceEvaporationPerMax = FromLine(lineS[80]),
                                    WaterBalanceWaterBalanceReceiptPerMax = FromLine(lineS[81]),
                                    WaterBalanceWaterBalanceExpenditurePerMax = FromLine(lineS[82]),
                                    WaterBalanceSurfaceFlowMin = FromLine(lineS[83]),
                                    WaterBalanceSurfaceOutflowMin = FromLine(lineS[84]),
                                    WaterBalanceUndergroundFlowMin = FromLine(lineS[85]),
                                    WaterBalanceUndergroundOutflowMin = FromLine(lineS[86]),
                                    WaterBalancePrecipitationMin = FromLine(lineS[87]),
                                    WaterBalanceEvaporationMin = FromLine(lineS[88]),
                                    WaterBalanceWaterBalanceReceiptMin = FromLine(lineS[89]),
                                    WaterBalanceWaterBalanceExpenditureMin = FromLine(lineS[90]),
                                    WaterBalanceDiscrepancyMin = FromLine(lineS[91]),
                                    WaterBalanceSurfaceFlowPerMin = FromLine(lineS[92]),
                                    WaterBalanceUndergroundFlowPerMin = FromLine(lineS[93]),
                                    WaterBalancePrecipitationPerMin = FromLine(lineS[94]),
                                    WaterBalanceSurfaceOutflowPerMin = FromLine(lineS[95]),
                                    WaterBalanceUndergroundOutflowPerMin = FromLine(lineS[96]),
                                    WaterBalanceEvaporationPerMin = FromLine(lineS[97]),
                                    WaterBalanceWaterBalanceReceiptPerMin = FromLine(lineS[98]),
                                    WaterBalanceWaterBalanceExpenditurePerMin = FromLine(lineS[99]),

                                    WaterLevelWaterLavelMAvg = FromLine(lineS[100]),
                                    WaterLevelWaterLavelMMax = FromLine(lineS[101]),
                                    WaterLevelWaterLavelMMin = FromLine(lineS[102]),

                                    BathigraphicAndVolumetricCurveDataWaterLevelAvg = FromLine(lineS[103]),
                                    BathigraphicAndVolumetricCurveDataLakeAreaAvg = FromLine(lineS[104]),
                                    BathigraphicAndVolumetricCurveDataWaterMassVolumeAvg = FromLine(lineS[105]),
                                    BathigraphicAndVolumetricCurveDataWaterLevelMax = FromLine(lineS[106]),
                                    BathigraphicAndVolumetricCurveDataLakeAreaMax = FromLine(lineS[107]),
                                    BathigraphicAndVolumetricCurveDataWaterMassVolumeMax = FromLine(lineS[108]),
                                    BathigraphicAndVolumetricCurveDataWaterLevelMin = FromLine(lineS[109]),
                                    BathigraphicAndVolumetricCurveDataLakeAreaMin = FromLine(lineS[110]),
                                    BathigraphicAndVolumetricCurveDataWaterMassVolumeMin = FromLine(lineS[111]),

                                    GeneralHydrochemicalIndicatorMineralizationAvg = FromLine(lineS[112]),
                                    GeneralHydrochemicalIndicatorTotalHardnessAvg = FromLine(lineS[113]),
                                    GeneralHydrochemicalIndicatorDissOxygWaterAvg = FromLine(lineS[114]),
                                    GeneralHydrochemicalIndicatorPercentOxygWaterAvg = FromLine(lineS[115]),
                                    GeneralHydrochemicalIndicatorpHAvg = FromLine(lineS[116]),
                                    GeneralHydrochemicalIndicatorOrganicSubstancesAvg = FromLine(lineS[117]),
                                    GeneralHydrochemicalIndicatorMineralizationMax = FromLine(lineS[118]),
                                    GeneralHydrochemicalIndicatorTotalHardnessMax = FromLine(lineS[119]),
                                    GeneralHydrochemicalIndicatorDissOxygWaterMax = FromLine(lineS[120]),
                                    GeneralHydrochemicalIndicatorPercentOxygWaterMax = FromLine(lineS[121]),
                                    GeneralHydrochemicalIndicatorpHMax = FromLine(lineS[122]),
                                    GeneralHydrochemicalIndicatorOrganicSubstancesMax = FromLine(lineS[123]),
                                    GeneralHydrochemicalIndicatorMineralizationMin = FromLine(lineS[124]),
                                    GeneralHydrochemicalIndicatorTotalHardnessMin = FromLine(lineS[125]),
                                    GeneralHydrochemicalIndicatorDissOxygWaterMin = FromLine(lineS[126]),
                                    GeneralHydrochemicalIndicatorPercentOxygWaterMin = FromLine(lineS[127]),
                                    GeneralHydrochemicalIndicatorpHMin = FromLine(lineS[128]),
                                    GeneralHydrochemicalIndicatorOrganicSubstancesMin = FromLine(lineS[129]),

                                    IonsaltWaterCompositionCaMgEqAvg = FromLine(lineS[130]),
                                    IonsaltWaterCompositionMgMgEqAvg = FromLine(lineS[131]),
                                    IonsaltWaterCompositionNaKMgEqAvg = FromLine(lineS[132]),
                                    IonsaltWaterCompositionClMgEqAvg = FromLine(lineS[133]),
                                    IonsaltWaterCompositionHCOMgEqAvg = FromLine(lineS[134]),
                                    IonsaltWaterCompositionSOMgEqAvg = FromLine(lineS[135]),
                                    IonsaltWaterCompositionCaMgAvg = FromLine(lineS[136]),
                                    IonsaltWaterCompositionMgMgAvg = FromLine(lineS[137]),
                                    IonsaltWaterCompositionNaKMgAvg = FromLine(lineS[138]),
                                    IonsaltWaterCompositionClMgAvg = FromLine(lineS[139]),
                                    IonsaltWaterCompositionHCOMgAvg = FromLine(lineS[140]),
                                    IonsaltWaterCompositionSOMgAvg = FromLine(lineS[141]),
                                    IonsaltWaterCompositionCationsSumMgEqAvg = FromLine(lineS[142]),
                                    IonsaltWaterCompositionAnionsSumMgEqAvg = FromLine(lineS[143]),
                                    IonsaltWaterCompositionIonsSumMgEqAvg = FromLine(lineS[144]),
                                    IonsaltWaterCompositionCaPerEqAvg = FromLine(lineS[145]),
                                    IonsaltWaterCompositionMgPerEqAvg = FromLine(lineS[146]),
                                    IonsaltWaterCompositionNaKPerEqAvg = FromLine(lineS[147]),
                                    IonsaltWaterCompositionClPerEqAvg = FromLine(lineS[148]),
                                    IonsaltWaterCompositionHCOPerEqAvg = FromLine(lineS[149]),
                                    IonsaltWaterCompositionSOPerEqAvg = FromLine(lineS[150]),
                                    IonsaltWaterCompositionCaMgEqMax = FromLine(lineS[151]),
                                    IonsaltWaterCompositionMgMgEqMax = FromLine(lineS[152]),
                                    IonsaltWaterCompositionNaKMgEqMax = FromLine(lineS[153]),
                                    IonsaltWaterCompositionClMgEqMax = FromLine(lineS[154]),
                                    IonsaltWaterCompositionHCOMgEqMax = FromLine(lineS[155]),
                                    IonsaltWaterCompositionSOMgEqMax = FromLine(lineS[156]),
                                    IonsaltWaterCompositionCaMgMax = FromLine(lineS[157]),
                                    IonsaltWaterCompositionMgMgMax = FromLine(lineS[158]),
                                    IonsaltWaterCompositionNaKMgMax = FromLine(lineS[159]),
                                    IonsaltWaterCompositionClMgMax = FromLine(lineS[160]),
                                    IonsaltWaterCompositionHCOMgMax = FromLine(lineS[161]),
                                    IonsaltWaterCompositionSOMgMax = FromLine(lineS[162]),
                                    IonsaltWaterCompositionCationsSumMgEqMax = FromLine(lineS[163]),
                                    IonsaltWaterCompositionAnionsSumMgEqMax = FromLine(lineS[164]),
                                    IonsaltWaterCompositionIonsSumMgEqMax = FromLine(lineS[165]),
                                    IonsaltWaterCompositionCaPerEqMax = FromLine(lineS[166]),
                                    IonsaltWaterCompositionMgPerEqMax = FromLine(lineS[167]),
                                    IonsaltWaterCompositionNaKPerEqMax = FromLine(lineS[168]),
                                    IonsaltWaterCompositionClPerEqMax = FromLine(lineS[169]),
                                    IonsaltWaterCompositionHCOPerEqMax = FromLine(lineS[170]),
                                    IonsaltWaterCompositionSOPerEqMax = FromLine(lineS[171]),
                                    IonsaltWaterCompositionCaMgEqMin = FromLine(lineS[172]),
                                    IonsaltWaterCompositionMgMgEqMin = FromLine(lineS[173]),
                                    IonsaltWaterCompositionNaKMgEqMin = FromLine(lineS[174]),
                                    IonsaltWaterCompositionClMgEqMin = FromLine(lineS[175]),
                                    IonsaltWaterCompositionHCOMgEqMin = FromLine(lineS[176]),
                                    IonsaltWaterCompositionSOMgEqMin = FromLine(lineS[177]),
                                    IonsaltWaterCompositionCaMgMin = FromLine(lineS[178]),
                                    IonsaltWaterCompositionMgMgMin = FromLine(lineS[179]),
                                    IonsaltWaterCompositionNaKMgMin = FromLine(lineS[180]),
                                    IonsaltWaterCompositionClMgMin = FromLine(lineS[181]),
                                    IonsaltWaterCompositionHCOMgMin = FromLine(lineS[182]),
                                    IonsaltWaterCompositionSOMgMin = FromLine(lineS[183]),
                                    IonsaltWaterCompositionCationsSumMgEqMin = FromLine(lineS[184]),
                                    IonsaltWaterCompositionAnionsSumMgEqMin = FromLine(lineS[185]),
                                    IonsaltWaterCompositionIonsSumMgEqMin = FromLine(lineS[186]),
                                    IonsaltWaterCompositionCaPerEqMin = FromLine(lineS[187]),
                                    IonsaltWaterCompositionMgPerEqMin = FromLine(lineS[188]),
                                    IonsaltWaterCompositionNaKPerEqMin = FromLine(lineS[189]),
                                    IonsaltWaterCompositionClPerEqMin = FromLine(lineS[190]),
                                    IonsaltWaterCompositionHCOPerEqMin = FromLine(lineS[191]),
                                    IonsaltWaterCompositionSOPerEqMin = FromLine(lineS[192]),

                                    ToxicologicalIndicatorNH4Avg = FromLine(lineS[193]),
                                    ToxicologicalIndicatorNO2Avg = FromLine(lineS[194]),
                                    ToxicologicalIndicatorNO3Avg = FromLine(lineS[195]),
                                    ToxicologicalIndicatorPPO4Avg = FromLine(lineS[196]),
                                    ToxicologicalIndicatorCuAvg = FromLine(lineS[197]),
                                    ToxicologicalIndicatorZnAvg = FromLine(lineS[198]),
                                    ToxicologicalIndicatorMnAvg = FromLine(lineS[199]),
                                    ToxicologicalIndicatorPbAvg = FromLine(lineS[200]),
                                    ToxicologicalIndicatorNiAvg = FromLine(lineS[201]),
                                    ToxicologicalIndicatorCdAvg = FromLine(lineS[202]),
                                    ToxicologicalIndicatorCoAvg = FromLine(lineS[203]),
                                    ToxicologicalIndicatorIZVNH4Avg = FromLine(lineS[204]),
                                    ToxicologicalIndicatorIZVNO2Avg = FromLine(lineS[205]),
                                    ToxicologicalIndicatorIZVNO3Avg = FromLine(lineS[206]),
                                    ToxicologicalIndicatorIZVPPO4Avg = FromLine(lineS[207]),
                                    ToxicologicalIndicatorIZVCuAvg = FromLine(lineS[208]),
                                    ToxicologicalIndicatorIZVZnAvg = FromLine(lineS[209]),
                                    ToxicologicalIndicatorIZVMnAvg = FromLine(lineS[210]),
                                    ToxicologicalIndicatorIZVPbAvg = FromLine(lineS[211]),
                                    ToxicologicalIndicatorIZVNiAvg = FromLine(lineS[212]),
                                    ToxicologicalIndicatorIZVCdAvg = FromLine(lineS[213]),
                                    ToxicologicalIndicatorIZVCoAvg = FromLine(lineS[214]),
                                    ToxicologicalIndicatorKIZVkoNH4Avg = FromLine(lineS[215]),
                                    ToxicologicalIndicatorKIZVkoNO2Avg = FromLine(lineS[216]),
                                    ToxicologicalIndicatorKIZVkoNO3Avg = FromLine(lineS[217]),
                                    ToxicologicalIndicatorKIZVkoPPO4Avg = FromLine(lineS[218]),
                                    ToxicologicalIndicatorKIZVkoCuAvg = FromLine(lineS[219]),
                                    ToxicologicalIndicatorKIZVkoZnAvg = FromLine(lineS[220]),
                                    ToxicologicalIndicatorKIZVkoMnAvg = FromLine(lineS[221]),
                                    ToxicologicalIndicatorKIZVkoPbAvg = FromLine(lineS[222]),
                                    ToxicologicalIndicatorKIZVkoNiAvg = FromLine(lineS[223]),
                                    ToxicologicalIndicatorKIZVkoCdAvg = FromLine(lineS[224]),
                                    ToxicologicalIndicatorKIZVkoCoAvg = FromLine(lineS[225]),
                                    ToxicologicalIndicatorKIZVAvg = FromLine(lineS[226]),
                                    ToxicologicalIndicatorNH4Max = FromLine(lineS[227]),
                                    ToxicologicalIndicatorNO2Max = FromLine(lineS[228]),
                                    ToxicologicalIndicatorNO3Max = FromLine(lineS[229]),
                                    ToxicologicalIndicatorPPO4Max = FromLine(lineS[230]),
                                    ToxicologicalIndicatorCuMax = FromLine(lineS[231]),
                                    ToxicologicalIndicatorZnMax = FromLine(lineS[232]),
                                    ToxicologicalIndicatorMnMax = FromLine(lineS[233]),
                                    ToxicologicalIndicatorPbMax = FromLine(lineS[234]),
                                    ToxicologicalIndicatorNiMax = FromLine(lineS[235]),
                                    ToxicologicalIndicatorCdMax = FromLine(lineS[236]),
                                    ToxicologicalIndicatorCoMax = FromLine(lineS[237]),
                                    ToxicologicalIndicatorIZVNH4Max = FromLine(lineS[238]),
                                    ToxicologicalIndicatorIZVNO2Max = FromLine(lineS[239]),
                                    ToxicologicalIndicatorIZVNO3Max = FromLine(lineS[240]),
                                    ToxicologicalIndicatorIZVPPO4Max = FromLine(lineS[241]),
                                    ToxicologicalIndicatorIZVCuMax = FromLine(lineS[242]),
                                    ToxicologicalIndicatorIZVZnMax = FromLine(lineS[243]),
                                    ToxicologicalIndicatorIZVMnMax = FromLine(lineS[244]),
                                    ToxicologicalIndicatorIZVPbMax = FromLine(lineS[245]),
                                    ToxicologicalIndicatorIZVNiMax = FromLine(lineS[246]),
                                    ToxicologicalIndicatorIZVCdMax = FromLine(lineS[247]),
                                    ToxicologicalIndicatorIZVCoMax = FromLine(lineS[248]),
                                    ToxicologicalIndicatorKIZVkoNH4Max = FromLine(lineS[249]),
                                    ToxicologicalIndicatorKIZVkoNO2Max = FromLine(lineS[250]),
                                    ToxicologicalIndicatorKIZVkoNO3Max = FromLine(lineS[251]),
                                    ToxicologicalIndicatorKIZVkoPPO4Max = FromLine(lineS[252]),
                                    ToxicologicalIndicatorKIZVkoCuMax = FromLine(lineS[253]),
                                    ToxicologicalIndicatorKIZVkoZnMax = FromLine(lineS[254]),
                                    ToxicologicalIndicatorKIZVkoMnMax = FromLine(lineS[255]),
                                    ToxicologicalIndicatorKIZVkoPbMax = FromLine(lineS[256]),
                                    ToxicologicalIndicatorKIZVkoNiMax = FromLine(lineS[257]),
                                    ToxicologicalIndicatorKIZVkoCdMax = FromLine(lineS[258]),
                                    ToxicologicalIndicatorKIZVkoCoMax = FromLine(lineS[259]),
                                    ToxicologicalIndicatorKIZVMax = FromLine(lineS[260]),
                                    ToxicologicalIndicatorNH4Min = FromLine(lineS[261]),
                                    ToxicologicalIndicatorNO2Min = FromLine(lineS[262]),
                                    ToxicologicalIndicatorNO3Min = FromLine(lineS[263]),
                                    ToxicologicalIndicatorPPO4Min = FromLine(lineS[264]),
                                    ToxicologicalIndicatorCuMin = FromLine(lineS[265]),
                                    ToxicologicalIndicatorZnMin = FromLine(lineS[266]),
                                    ToxicologicalIndicatorMnMin = FromLine(lineS[267]),
                                    ToxicologicalIndicatorPbMin = FromLine(lineS[268]),
                                    ToxicologicalIndicatorNiMin = FromLine(lineS[269]),
                                    ToxicologicalIndicatorCdMin = FromLine(lineS[270]),
                                    ToxicologicalIndicatorCoMin = FromLine(lineS[271]),
                                    ToxicologicalIndicatorIZVNH4Min = FromLine(lineS[272]),
                                    ToxicologicalIndicatorIZVNO2Min = FromLine(lineS[273]),
                                    ToxicologicalIndicatorIZVNO3Min = FromLine(lineS[274]),
                                    ToxicologicalIndicatorIZVPPO4Min = FromLine(lineS[275]),
                                    ToxicologicalIndicatorIZVCuMin = FromLine(lineS[276]),
                                    ToxicologicalIndicatorIZVZnMin = FromLine(lineS[277]),
                                    ToxicologicalIndicatorIZVMnMin = FromLine(lineS[278]),
                                    ToxicologicalIndicatorIZVPbMin = FromLine(lineS[279]),
                                    ToxicologicalIndicatorIZVNiMin = FromLine(lineS[280]),
                                    ToxicologicalIndicatorIZVCdMin = FromLine(lineS[281]),
                                    ToxicologicalIndicatorIZVCoMin = FromLine(lineS[282]),
                                    ToxicologicalIndicatorKIZVkoNH4Min = FromLine(lineS[283]),
                                    ToxicologicalIndicatorKIZVkoNO2Min = FromLine(lineS[284]),
                                    ToxicologicalIndicatorKIZVkoNO3Min = FromLine(lineS[285]),
                                    ToxicologicalIndicatorKIZVkoPPO4Min = FromLine(lineS[286]),
                                    ToxicologicalIndicatorKIZVkoCuMin = FromLine(lineS[287]),
                                    ToxicologicalIndicatorKIZVkoZnMin = FromLine(lineS[288]),
                                    ToxicologicalIndicatorKIZVkoMnMin = FromLine(lineS[289]),
                                    ToxicologicalIndicatorKIZVkoPbMin = FromLine(lineS[290]),
                                    ToxicologicalIndicatorKIZVkoNiMin = FromLine(lineS[291]),
                                    ToxicologicalIndicatorKIZVkoCdMin = FromLine(lineS[292]),
                                    ToxicologicalIndicatorKIZVkoCoMin = FromLine(lineS[293]),
                                    ToxicologicalIndicatorKIZVMin = FromLine(lineS[294]),

                                    DynamicsLakeAreaNoDataPersAvg = FromLine(lineS[295]),
                                    DynamicsLakeAreaNotWaterAvg = FromLine(lineS[296]),
                                    DynamicsLakeAreaSeasonalWaterAreaAvg = FromLine(lineS[297]),
                                    DynamicsLakeAreaPermanentWaterAreaAvg = FromLine(lineS[298]),
                                    DynamicsLakeAreaMaximumWaterAreaAvg = FromLine(lineS[299]),
                                    DynamicsLakeAreaSeasonalWaterAreaPerAvg = FromLine(lineS[300]),
                                    DynamicsLakeAreaPermanentWaterAreaPerAvg = FromLine(lineS[301]),
                                    DynamicsLakeAreaNoDataPersMax = FromLine(lineS[302]),
                                    DynamicsLakeAreaNotWaterMax = FromLine(lineS[303]),
                                    DynamicsLakeAreaSeasonalWaterAreaMax = FromLine(lineS[304]),
                                    DynamicsLakeAreaPermanentWaterAreaMax = FromLine(lineS[305]),
                                    DynamicsLakeAreaMaximumWaterAreaMax = FromLine(lineS[306]),
                                    DynamicsLakeAreaSeasonalWaterAreaPerMax = FromLine(lineS[307]),
                                    DynamicsLakeAreaPermanentWaterAreaPerMax = FromLine(lineS[308]),
                                    DynamicsLakeAreaNoDataPersMin = FromLine(lineS[309]),
                                    DynamicsLakeAreaNotWaterMin = FromLine(lineS[310]),
                                    DynamicsLakeAreaSeasonalWaterAreaMin = FromLine(lineS[311]),
                                    DynamicsLakeAreaPermanentWaterAreaMin = FromLine(lineS[312]),
                                    DynamicsLakeAreaMaximumWaterAreaMin = FromLine(lineS[313]),
                                    DynamicsLakeAreaSeasonalWaterAreaPerMin = FromLine(lineS[314]),
                                    DynamicsLakeAreaPermanentWaterAreaPerMin = FromLine(lineS[315])
                                });
                            }
                            //File.ReadAllLines(fileName);

                            List<int?> rLakes = new List<int?>();

                            foreach (Lake lake in Lakes)
                            {
                                try
                                {
                                    if(
                                        " + codeFilter + @"
                                        )
                                    {
                                        rLakes.Add(lake.LakeId);
                                    }
                                }
                                catch
                                {

                                }
                            }

                            return rLakes
                                .Distinct()
                                .ToArray();
                        }
                    }
                }";
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(codeToCompile);

            //var coreDir = Path.GetDirectoryName(typeof(object).GetTypeInfo().Assembly.Location);
            //var mscorlib = MetadataReference.CreateFromFile(Path.Combine(coreDir, "mscorlib.dll"));

            List<MetadataReference> references = new List<MetadataReference>
            {
                    MetadataReference.CreateFromFile(typeof(Object).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Uri).GetTypeInfo().Assembly.Location)
            };


            var dd = typeof(Enumerable).GetTypeInfo().Assembly.Location;
            var coreDir = Directory.GetParent(dd);

            string[] notTheseDLL = new string[]
            {
                "ucrtbase.dll",
                "sos_amd64_amd64_4.6.26614.01.dll",
                "sos_amd64_amd64_4.6.27317.03.dll",
                "mscordaccore_amd64_amd64_4.6.27317.03.dll",
                "sos.dll",
                "mscorrc.dll",
                "mscorrc.debug.dll",
                "mscordbi.dll",
                "mscordaccore_amd64_amd64_4.6.26614.01.dll",
                "mscordaccore.dll",
                "Microsoft.DiaSymReader.Native.amd64.dll",
                "hostpolicy.dll",
                "dbgshim.dll",
                "coreclr.dll",
                "clrjit.dll",
                "clretwrc.dll",
                "clrcompression.dll",
                "api-ms-win-crt-utility-l1-1-0.dll",
                "api-ms-win-crt-time-l1-1-0.dll",
                "api-ms-win-crt-string-l1-1-0.dll",
                "api-ms-win-crt-stdio-l1-1-0.dll",
                "api-ms-win-crt-runtime-l1-1-0.dll",
                "api-ms-win-crt-process-l1-1-0.dll",
                "api-ms-win-crt-private-l1-1-0.dll",
                "api-ms-win-crt-multibyte-l1-1-0.dll",
                "api-ms-win-crt-math-l1-1-0.dll",
                "api-ms-win-crt-locale-l1-1-0.dll",
                "api-ms-win-crt-heap-l1-1-0.dll",
                "api-ms-win-crt-filesystem-l1-1-0.dll",
                "api-ms-win-crt-environment-l1-1-0.dll",
                "api-ms-win-crt-convert-l1-1-0.dll",
                "api-ms-win-crt-conio-l1-1-0.dll",
                "api-ms-win-core-util-l1-1-0.dll",
                "api-ms-win-core-timezone-l1-1-0.dll",
                "api-ms-win-core-sysinfo-l1-1-0.dll",
                "api-ms-win-core-synch-l1-2-0.dll",
                "api-ms-win-core-synch-l1-1-0.dll",
                "api-ms-win-core-string-l1-1-0.dll",
                "api-ms-win-core-rtlsupport-l1-1-0.dll",
                "api-ms-win-core-profile-l1-1-0.dll",
                "api-ms-win-core-processthreads-l1-1-1.dll",
                "api-ms-win-core-processthreads-l1-1-0.dll",
                "api-ms-win-core-processenvironment-l1-1-0.dll",
                "api-ms-win-core-namedpipe-l1-1-0.dll",
                "api-ms-win-core-memory-l1-1-0.dll",
                "api-ms-win-core-localization-l1-2-0.dll",
                "api-ms-win-core-libraryloader-l1-1-0.dll",
                "api-ms-win-core-interlocked-l1-1-0.dll",
                "api-ms-win-core-heap-l1-1-0.dll",
                "api-ms-win-core-handle-l1-1-0.dll",
                "api-ms-win-core-file-l2-1-0.dll",
                "api-ms-win-core-file-l1-2-0.dll",
                "api-ms-win-core-file-l1-1-0.dll",
                "api-ms-win-core-errorhandling-l1-1-0.dll",
                "api-ms-win-core-debug-l1-1-0.dll",
                "api-ms-win-core-datetime-l1-1-0.dll",
                "api-ms-win-core-console-l1-1-0.dll",
                "sni.dll",
                "libuv.dll",
                "hostfxr.dll",
                "sos_amd64_amd64_4.6.27414.06.dll",
                "mscordaccore_amd64_amd64_4.6.27414.06.dll",
                "sos_amd64_amd64_4.6.27617.04.dll",
                "mscordaccore_amd64_amd64_4.6.27617.04.dll",
            };

            foreach (var dllFile in Directory.GetFiles(coreDir.FullName, "*.dll"))
            {
                if(!notTheseDLL.Contains(Path.GetFileName(dllFile)))
                {
                    references.Add(MetadataReference.CreateFromFile(dllFile));
                }
            }

            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                //references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(references.ToArray());


            //CSharpCompilation compilation = CSharpCompilation.Create(
            //    assemblyName,
            //    syntaxTrees: new[] { syntaxTree },
            //    references: references,
            //    options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            ////test file
            //string line_;
            //StreamReader file_ = new StreamReader(filePopulateLakes);
            //while ((line_ = file_.ReadLine()) != null)
            //{
            //    try
            //    {
            //        string[] lineS = line_.Split('\t');
            //        int i1 = Convert.ToInt32(lineS[0]);
            //        i1 = Convert.ToInt32(lineS[1]);
            //        decimal d = string.IsNullOrEmpty(lineS[2]) ? 0 : Convert.ToDecimal(lineS[2]);
            //        d = string.IsNullOrEmpty(lineS[3]) ? 0 : Convert.ToDecimal(lineS[3]);
            //        d = string.IsNullOrEmpty(lineS[4]) ? 0 : Convert.ToDecimal(lineS[4]);
            //        d = string.IsNullOrEmpty(lineS[5]) ? 0 : Convert.ToDecimal(lineS[5]);
            //        d = string.IsNullOrEmpty(lineS[6]) ? 0 : Convert.ToDecimal(lineS[6]);
            //        d = string.IsNullOrEmpty(lineS[7]) ? 0 : Convert.ToDecimal(lineS[7]);
            //        d = string.IsNullOrEmpty(lineS[8]) ? 0 : Convert.ToDecimal(lineS[8]);
            //        d = string.IsNullOrEmpty(lineS[9]) ? 0 : Convert.ToDecimal(lineS[9]);
            //        d = string.IsNullOrEmpty(lineS[10]) ? 0 : Convert.ToDecimal(lineS[10]);
            //    }
            //    catch(Exception e)
            //    {
            //        break;
            //    }
            //}

            int?[] r = new int?[0];
            string message = "";
            if(checkFormula)
            {
                using (var ms = new MemoryStream())
                {
                    EmitResult result = compilation.Emit(ms);
                    if (!result.Success)
                    {
                        IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                            diagnostic.IsWarningAsError ||
                            diagnostic.Severity == DiagnosticSeverity.Error);
                        r = new int?[0];
                        message = _sharedLocalizer["AnErrorOccurredWhileUsingTheFormula"] + ": " + string.Join(". ", result.Diagnostics);
                    }
                    else
                    {
                        ms.Seek(0, SeekOrigin.Begin);
                        Assembly assembly = AssemblyLoadContext.Default.LoadFromStream(ms);
                        var type = assembly.GetType("RoslynCompile.Calculator");
                        var instance = assembly.CreateInstance("RoslynCompile.Calculator");
                        var meth = type.GetMember("Calculate").First() as MethodInfo;
                        r = meth.Invoke(instance, null) as int?[];
                    }
                }
            }
            else
            {
                message = _sharedLocalizer["FormulaContainsInvalidCharacters"];
            }

            var lakes = lakesToSearch
                .Where(l => r.Contains(l.LakeId))
                .OrderBy(l => l.Name)
                .ToArray();

            if (Adm1KATOId != null)
            {
                lakes = lakes.Where(l => _context.LakeKATO.Where(lk => lk.KATOId == Adm1KATOId).Select(lk => lk.LakeId).Contains(l.LakeId)).ToArray();
            }
            if (Adm2KATOId != null)
            {
                lakes = lakes.Where(l => _context.LakeKATO.Where(lk => lk.KATOId == Adm2KATOId).Select(lk => lk.LakeId).Contains(l.LakeId)).ToArray();
            }

            //List<string> columnCodesL = new List<string>(),
            //    columnNamesL = new List<string>();
            //if(Formula.Contains("Area2015"))
            //{
            //    columnCodesL.Add("Area2015");
            //    columnNamesL.Add(_sharedLocalizer["Area2015"]);
            //}
            //string[] columnCodes = columnCodesL.ToArray(),
            //    columnNames = columnNamesL.ToArray();

            for(int i=0;i<lakes.Count();i++)
            {
                // Analyze 4
                LakesArchiveData lakesArchiveData = _context.LakesArchiveData.FirstOrDefault(l => l.LakeId == lakes[i].LakeId);
                lakes[i].LakesArchiveDataLakeLength = lakesArchiveData?.LakeLength;
                lakes[i].LakesArchiveDataLakeShorelineLength = lakesArchiveData?.LakeShorelineLength;
                lakes[i].LakesArchiveDataLakeMirrorArea = lakesArchiveData?.LakeMirrorArea;
                lakes[i].LakesArchiveDataLakeAbsoluteHeight = lakesArchiveData?.LakeAbsoluteHeight;
                lakes[i].LakesArchiveDataLakeWidth = lakesArchiveData?.LakeWidth;
                lakes[i].LakesArchiveDataLakeMaxDepth = lakesArchiveData?.LakeMaxDepth;
                lakes[i].LakesArchiveDataLakeWaterMass = lakesArchiveData?.LakeWaterMass;

                LakesGlobalData lakesGlobalData = _context.LakesGlobalData.FirstOrDefault(l => l.LakeId == lakes[i].LakeId);
                lakes[i].LakesGlobalDataLake_area = lakesGlobalData?.Lake_area;
                lakes[i].LakesGlobalDataShore_len = lakesGlobalData?.Shore_len;
                lakes[i].LakesGlobalDataShore_dev = lakesGlobalData?.Shore_dev;
                lakes[i].LakesGlobalDataVol_total = lakesGlobalData?.Vol_total;
                lakes[i].LakesGlobalDataDepth_avg = lakesGlobalData?.Depth_avg;
                lakes[i].LakesGlobalDataDis_avg = lakesGlobalData?.Dis_avg;
                lakes[i].LakesGlobalDataRes_time = lakesGlobalData?.Res_time;
                lakes[i].LakesGlobalDataElevation = lakesGlobalData?.Elevation;
                lakes[i].LakesGlobalDataSlope_100 = lakesGlobalData?.Slope_100;
                lakes[i].LakesGlobalDataWshd_area = lakesGlobalData?.Wshd_area;

                List<WaterBalance> waterBalances = _context.WaterBalance.Where(l => l.LakeId == lakes[i].LakeId).ToList();
                lakes[i].WaterBalanceSurfaceFlowAvg = waterBalances.Select(j => j.SurfaceFlow).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].WaterBalanceSurfaceOutflowAvg = waterBalances.Select(j => j.SurfaceOutflow).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].WaterBalanceUndergroundFlowAvg = waterBalances.Select(j => j.UndergroundFlow).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].WaterBalanceUndergroundOutflowAvg = waterBalances.Select(j => j.UndergroundOutflow).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].WaterBalancePrecipitationAvg = waterBalances.Select(j => j.Precipitation).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].WaterBalanceEvaporationAvg = waterBalances.Select(j => j.Evaporation).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].WaterBalanceWaterBalanceReceiptAvg = waterBalances.Select(j => j.WaterBalanceReceipt).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].WaterBalanceWaterBalanceExpenditureAvg = waterBalances.Select(j => j.WaterBalanceExpenditure).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].WaterBalanceDiscrepancyAvg = waterBalances.Select(j => j.Discrepancy).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].WaterBalanceSurfaceFlowPerAvg = waterBalances.Select(j => j.SurfaceFlowPer).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].WaterBalanceUndergroundFlowPerAvg = waterBalances.Select(j => j.UndergroundFlowPer).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].WaterBalancePrecipitationPerAvg = waterBalances.Select(j => j.PrecipitationPer).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].WaterBalanceSurfaceOutflowPerAvg = waterBalances.Select(j => j.SurfaceOutflowPer).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].WaterBalanceUndergroundOutflowPerAvg = waterBalances.Select(j => j.UndergroundOutflowPer).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].WaterBalanceEvaporationPerAvg = waterBalances.Select(j => j.EvaporationPer).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].WaterBalanceWaterBalanceReceiptPerAvg = waterBalances.Select(j => j.WaterBalanceReceiptPer).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].WaterBalanceWaterBalanceExpenditurePerAvg = waterBalances.Select(j => j.WaterBalanceExpenditurePer).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].WaterBalanceSurfaceFlowMax = waterBalances.Select(j => j.SurfaceFlow).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].WaterBalanceSurfaceOutflowMax = waterBalances.Select(j => j.SurfaceOutflow).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].WaterBalanceUndergroundFlowMax = waterBalances.Select(j => j.UndergroundFlow).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].WaterBalanceUndergroundOutflowMax = waterBalances.Select(j => j.UndergroundOutflow).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].WaterBalancePrecipitationMax = waterBalances.Select(j => j.Precipitation).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].WaterBalanceEvaporationMax = waterBalances.Select(j => j.Evaporation).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].WaterBalanceWaterBalanceReceiptMax = waterBalances.Select(j => j.WaterBalanceReceipt).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].WaterBalanceWaterBalanceExpenditureMax = waterBalances.Select(j => j.WaterBalanceExpenditure).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].WaterBalanceDiscrepancyMax = waterBalances.Select(j => j.Discrepancy).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].WaterBalanceSurfaceFlowPerMax = waterBalances.Select(j => j.SurfaceFlowPer).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].WaterBalanceUndergroundFlowPerMax = waterBalances.Select(j => j.UndergroundFlowPer).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].WaterBalancePrecipitationPerMax = waterBalances.Select(j => j.PrecipitationPer).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].WaterBalanceSurfaceOutflowPerMax = waterBalances.Select(j => j.SurfaceOutflowPer).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].WaterBalanceUndergroundOutflowPerMax = waterBalances.Select(j => j.UndergroundOutflowPer).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].WaterBalanceEvaporationPerMax = waterBalances.Select(j => j.EvaporationPer).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].WaterBalanceWaterBalanceReceiptPerMax = waterBalances.Select(j => j.WaterBalanceReceiptPer).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].WaterBalanceWaterBalanceExpenditurePerMax = waterBalances.Select(j => j.WaterBalanceExpenditurePer).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].WaterBalanceSurfaceFlowMin = waterBalances.Select(j => j.SurfaceFlow).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].WaterBalanceSurfaceOutflowMin = waterBalances.Select(j => j.SurfaceOutflow).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].WaterBalanceUndergroundFlowMin = waterBalances.Select(j => j.UndergroundFlow).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].WaterBalanceUndergroundOutflowMin = waterBalances.Select(j => j.UndergroundOutflow).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].WaterBalancePrecipitationMin = waterBalances.Select(j => j.Precipitation).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].WaterBalanceEvaporationMin = waterBalances.Select(j => j.Evaporation).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].WaterBalanceWaterBalanceReceiptMin = waterBalances.Select(j => j.WaterBalanceReceipt).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].WaterBalanceWaterBalanceExpenditureMin = waterBalances.Select(j => j.WaterBalanceExpenditure).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].WaterBalanceDiscrepancyMin = waterBalances.Select(j => j.Discrepancy).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].WaterBalanceSurfaceFlowPerMin = waterBalances.Select(j => j.SurfaceFlowPer).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].WaterBalanceUndergroundFlowPerMin = waterBalances.Select(j => j.UndergroundFlowPer).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].WaterBalancePrecipitationPerMin = waterBalances.Select(j => j.PrecipitationPer).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].WaterBalanceSurfaceOutflowPerMin = waterBalances.Select(j => j.SurfaceOutflowPer).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].WaterBalanceUndergroundOutflowPerMin = waterBalances.Select(j => j.UndergroundOutflowPer).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].WaterBalanceEvaporationPerMin = waterBalances.Select(j => j.EvaporationPer).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].WaterBalanceWaterBalanceReceiptPerMin = waterBalances.Select(j => j.WaterBalanceReceiptPer).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].WaterBalanceWaterBalanceExpenditurePerMin = waterBalances.Select(j => j.WaterBalanceExpenditurePer).Where(j => j != null).DefaultIfEmpty(0).Min();

                List<WaterLevel> waterLevels = _context.WaterLevel.Where(l => l.LakeId == lakes[i].LakeId).ToList();
                lakes[i].WaterLevelWaterLavelMAvg = waterLevels.Select(j => j.WaterLavelM).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].WaterLevelWaterLavelMMax = waterLevels.Select(j => j.WaterLavelM).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].WaterLevelWaterLavelMMin = waterLevels.Select(j => j.WaterLavelM).Where(j => j != null).DefaultIfEmpty(0).Min();

                List<BathigraphicAndVolumetricCurveData> bathigraphicAndVolumetricCurveDatas = _context.BathigraphicAndVolumetricCurveData.Where(l => l.LakeId == lakes[i].LakeId).ToList();
                lakes[i].BathigraphicAndVolumetricCurveDataWaterLevelAvg = bathigraphicAndVolumetricCurveDatas.Select(j => j.WaterLevel).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].BathigraphicAndVolumetricCurveDataLakeAreaAvg = bathigraphicAndVolumetricCurveDatas.Select(j => j.LakeArea).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].BathigraphicAndVolumetricCurveDataWaterMassVolumeAvg = bathigraphicAndVolumetricCurveDatas.Select(j => j.WaterMassVolume).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].BathigraphicAndVolumetricCurveDataWaterLevelMax = bathigraphicAndVolumetricCurveDatas.Select(j => j.WaterLevel).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].BathigraphicAndVolumetricCurveDataLakeAreaMax = bathigraphicAndVolumetricCurveDatas.Select(j => j.LakeArea).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].BathigraphicAndVolumetricCurveDataWaterMassVolumeMax = bathigraphicAndVolumetricCurveDatas.Select(j => j.WaterMassVolume).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].BathigraphicAndVolumetricCurveDataWaterLevelMin = bathigraphicAndVolumetricCurveDatas.Select(j => j.WaterLevel).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].BathigraphicAndVolumetricCurveDataLakeAreaMin = bathigraphicAndVolumetricCurveDatas.Select(j => j.LakeArea).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].BathigraphicAndVolumetricCurveDataWaterMassVolumeMin = bathigraphicAndVolumetricCurveDatas.Select(j => j.WaterMassVolume).Where(j => j != null).DefaultIfEmpty(0).Min();

                List<GeneralHydrochemicalIndicator> generalHydrochemicalIndicators = _context.GeneralHydrochemicalIndicator.Where(l => l.LakeId == lakes[i].LakeId).ToList();
                lakes[i].GeneralHydrochemicalIndicatorMineralizationAvg = generalHydrochemicalIndicators.Select(j => j.Mineralization).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].GeneralHydrochemicalIndicatorTotalHardnessAvg = generalHydrochemicalIndicators.Select(j => j.TotalHardness).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].GeneralHydrochemicalIndicatorDissOxygWaterAvg = generalHydrochemicalIndicators.Select(j => j.DissOxygWater).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].GeneralHydrochemicalIndicatorPercentOxygWaterAvg = generalHydrochemicalIndicators.Select(j => j.PercentOxygWater).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].GeneralHydrochemicalIndicatorpHAvg = generalHydrochemicalIndicators.Select(j => j.pH).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].GeneralHydrochemicalIndicatorOrganicSubstancesAvg = generalHydrochemicalIndicators.Select(j => j.OrganicSubstances).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].GeneralHydrochemicalIndicatorMineralizationMax = generalHydrochemicalIndicators.Select(j => j.Mineralization).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].GeneralHydrochemicalIndicatorTotalHardnessMax = generalHydrochemicalIndicators.Select(j => j.TotalHardness).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].GeneralHydrochemicalIndicatorDissOxygWaterMax = generalHydrochemicalIndicators.Select(j => j.DissOxygWater).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].GeneralHydrochemicalIndicatorPercentOxygWaterMax = generalHydrochemicalIndicators.Select(j => j.PercentOxygWater).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].GeneralHydrochemicalIndicatorpHMax = generalHydrochemicalIndicators.Select(j => j.pH).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].GeneralHydrochemicalIndicatorOrganicSubstancesMax = generalHydrochemicalIndicators.Select(j => j.OrganicSubstances).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].GeneralHydrochemicalIndicatorMineralizationMin = generalHydrochemicalIndicators.Select(j => j.Mineralization).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].GeneralHydrochemicalIndicatorTotalHardnessMin = generalHydrochemicalIndicators.Select(j => j.TotalHardness).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].GeneralHydrochemicalIndicatorDissOxygWaterMin = generalHydrochemicalIndicators.Select(j => j.DissOxygWater).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].GeneralHydrochemicalIndicatorPercentOxygWaterMin = generalHydrochemicalIndicators.Select(j => j.PercentOxygWater).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].GeneralHydrochemicalIndicatorpHMin = generalHydrochemicalIndicators.Select(j => j.pH).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].GeneralHydrochemicalIndicatorOrganicSubstancesMin = generalHydrochemicalIndicators.Select(j => j.OrganicSubstances).Where(j => j != null).DefaultIfEmpty(0).Min();

                List<IonsaltWaterComposition> ionsaltWaterCompositions = _context.IonsaltWaterComposition.Where(l => l.LakeId == lakes[i].LakeId).ToList();
                lakes[i].IonsaltWaterCompositionCaMgEqAvg = ionsaltWaterCompositions.Select(j => j.CaMgEq).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].IonsaltWaterCompositionMgMgEqAvg = ionsaltWaterCompositions.Select(j => j.MgMgEq).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].IonsaltWaterCompositionNaKMgEqAvg = ionsaltWaterCompositions.Select(j => j.NaKMgEq).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].IonsaltWaterCompositionClMgEqAvg = ionsaltWaterCompositions.Select(j => j.ClMgEq).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].IonsaltWaterCompositionHCOMgEqAvg = ionsaltWaterCompositions.Select(j => j.HCOMgEq).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].IonsaltWaterCompositionSOMgEqAvg = ionsaltWaterCompositions.Select(j => j.SOMgEq).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].IonsaltWaterCompositionCaMgAvg = ionsaltWaterCompositions.Select(j => j.CaMg).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].IonsaltWaterCompositionMgMgAvg = ionsaltWaterCompositions.Select(j => j.MgMg).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].IonsaltWaterCompositionNaKMgAvg = ionsaltWaterCompositions.Select(j => j.NaKMg).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].IonsaltWaterCompositionClMgAvg = ionsaltWaterCompositions.Select(j => j.ClMg).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].IonsaltWaterCompositionHCOMgAvg = ionsaltWaterCompositions.Select(j => j.HCOMg).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].IonsaltWaterCompositionSOMgAvg = ionsaltWaterCompositions.Select(j => j.SOMg).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].IonsaltWaterCompositionCationsSumMgEqAvg = ionsaltWaterCompositions.Select(j => j.CationsSumMgEq).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].IonsaltWaterCompositionAnionsSumMgEqAvg = ionsaltWaterCompositions.Select(j => j.AnionsSumMgEq).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].IonsaltWaterCompositionIonsSumMgEqAvg = ionsaltWaterCompositions.Select(j => j.IonsSumMgEq).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].IonsaltWaterCompositionCaPerEqAvg = ionsaltWaterCompositions.Select(j => j.CaPerEq).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].IonsaltWaterCompositionMgPerEqAvg = ionsaltWaterCompositions.Select(j => j.MgPerEq).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].IonsaltWaterCompositionNaKPerEqAvg = ionsaltWaterCompositions.Select(j => j.NaKPerEq).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].IonsaltWaterCompositionClPerEqAvg = ionsaltWaterCompositions.Select(j => j.ClPerEq).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].IonsaltWaterCompositionHCOPerEqAvg = ionsaltWaterCompositions.Select(j => j.HCOPerEq).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].IonsaltWaterCompositionSOPerEqAvg = ionsaltWaterCompositions.Select(j => j.SOPerEq).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].IonsaltWaterCompositionCaMgEqMax = ionsaltWaterCompositions.Select(j => j.CaMgEq).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].IonsaltWaterCompositionMgMgEqMax = ionsaltWaterCompositions.Select(j => j.MgMgEq).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].IonsaltWaterCompositionNaKMgEqMax = ionsaltWaterCompositions.Select(j => j.NaKMgEq).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].IonsaltWaterCompositionClMgEqMax = ionsaltWaterCompositions.Select(j => j.ClMgEq).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].IonsaltWaterCompositionHCOMgEqMax = ionsaltWaterCompositions.Select(j => j.HCOMgEq).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].IonsaltWaterCompositionSOMgEqMax = ionsaltWaterCompositions.Select(j => j.SOMgEq).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].IonsaltWaterCompositionCaMgMax = ionsaltWaterCompositions.Select(j => j.CaMg).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].IonsaltWaterCompositionMgMgMax = ionsaltWaterCompositions.Select(j => j.MgMg).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].IonsaltWaterCompositionNaKMgMax = ionsaltWaterCompositions.Select(j => j.NaKMg).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].IonsaltWaterCompositionClMgMax = ionsaltWaterCompositions.Select(j => j.ClMg).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].IonsaltWaterCompositionHCOMgMax = ionsaltWaterCompositions.Select(j => j.HCOMg).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].IonsaltWaterCompositionSOMgMax = ionsaltWaterCompositions.Select(j => j.SOMg).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].IonsaltWaterCompositionCationsSumMgEqMax = ionsaltWaterCompositions.Select(j => j.CationsSumMgEq).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].IonsaltWaterCompositionAnionsSumMgEqMax = ionsaltWaterCompositions.Select(j => j.AnionsSumMgEq).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].IonsaltWaterCompositionIonsSumMgEqMax = ionsaltWaterCompositions.Select(j => j.IonsSumMgEq).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].IonsaltWaterCompositionCaPerEqMax = ionsaltWaterCompositions.Select(j => j.CaPerEq).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].IonsaltWaterCompositionMgPerEqMax = ionsaltWaterCompositions.Select(j => j.MgPerEq).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].IonsaltWaterCompositionNaKPerEqMax = ionsaltWaterCompositions.Select(j => j.NaKPerEq).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].IonsaltWaterCompositionClPerEqMax = ionsaltWaterCompositions.Select(j => j.ClPerEq).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].IonsaltWaterCompositionHCOPerEqMax = ionsaltWaterCompositions.Select(j => j.HCOPerEq).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].IonsaltWaterCompositionSOPerEqMax = ionsaltWaterCompositions.Select(j => j.SOPerEq).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].IonsaltWaterCompositionCaMgEqMin = ionsaltWaterCompositions.Select(j => j.CaMgEq).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].IonsaltWaterCompositionMgMgEqMin = ionsaltWaterCompositions.Select(j => j.MgMgEq).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].IonsaltWaterCompositionNaKMgEqMin = ionsaltWaterCompositions.Select(j => j.NaKMgEq).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].IonsaltWaterCompositionClMgEqMin = ionsaltWaterCompositions.Select(j => j.ClMgEq).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].IonsaltWaterCompositionHCOMgEqMin = ionsaltWaterCompositions.Select(j => j.HCOMgEq).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].IonsaltWaterCompositionSOMgEqMin = ionsaltWaterCompositions.Select(j => j.SOMgEq).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].IonsaltWaterCompositionCaMgMin = ionsaltWaterCompositions.Select(j => j.CaMg).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].IonsaltWaterCompositionMgMgMin = ionsaltWaterCompositions.Select(j => j.MgMg).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].IonsaltWaterCompositionNaKMgMin = ionsaltWaterCompositions.Select(j => j.NaKMg).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].IonsaltWaterCompositionClMgMin = ionsaltWaterCompositions.Select(j => j.ClMg).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].IonsaltWaterCompositionHCOMgMin = ionsaltWaterCompositions.Select(j => j.HCOMg).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].IonsaltWaterCompositionSOMgMin = ionsaltWaterCompositions.Select(j => j.SOMg).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].IonsaltWaterCompositionCationsSumMgEqMin = ionsaltWaterCompositions.Select(j => j.CationsSumMgEq).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].IonsaltWaterCompositionAnionsSumMgEqMin = ionsaltWaterCompositions.Select(j => j.AnionsSumMgEq).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].IonsaltWaterCompositionIonsSumMgEqMin = ionsaltWaterCompositions.Select(j => j.IonsSumMgEq).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].IonsaltWaterCompositionCaPerEqMin = ionsaltWaterCompositions.Select(j => j.CaPerEq).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].IonsaltWaterCompositionMgPerEqMin = ionsaltWaterCompositions.Select(j => j.MgPerEq).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].IonsaltWaterCompositionNaKPerEqMin = ionsaltWaterCompositions.Select(j => j.NaKPerEq).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].IonsaltWaterCompositionClPerEqMin = ionsaltWaterCompositions.Select(j => j.ClPerEq).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].IonsaltWaterCompositionHCOPerEqMin = ionsaltWaterCompositions.Select(j => j.HCOPerEq).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].IonsaltWaterCompositionSOPerEqMin = ionsaltWaterCompositions.Select(j => j.SOPerEq).Where(j => j != null).DefaultIfEmpty(0).Min();

                List<ToxicologicalIndicator> toxicologicalIndicators = _context.ToxicologicalIndicator.Where(l => l.LakeId == lakes[i].LakeId).ToList();
                lakes[i].ToxicologicalIndicatorNH4Avg = toxicologicalIndicators.Select(j => j.NH4).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].ToxicologicalIndicatorNO2Avg = toxicologicalIndicators.Select(j => j.NO2).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].ToxicologicalIndicatorNO3Avg = toxicologicalIndicators.Select(j => j.NO3).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].ToxicologicalIndicatorPPO4Avg = toxicologicalIndicators.Select(j => j.PPO4).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].ToxicologicalIndicatorCuAvg = toxicologicalIndicators.Select(j => j.Cu).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].ToxicologicalIndicatorZnAvg = toxicologicalIndicators.Select(j => j.Zn).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].ToxicologicalIndicatorMnAvg = toxicologicalIndicators.Select(j => j.Mn).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].ToxicologicalIndicatorPbAvg = toxicologicalIndicators.Select(j => j.Pb).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].ToxicologicalIndicatorNiAvg = toxicologicalIndicators.Select(j => j.Ni).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].ToxicologicalIndicatorCdAvg = toxicologicalIndicators.Select(j => j.Cd).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].ToxicologicalIndicatorCoAvg = toxicologicalIndicators.Select(j => j.Co).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].ToxicologicalIndicatorIZVNH4Avg = toxicologicalIndicators.Select(j => j.IZVNH4).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].ToxicologicalIndicatorIZVNO2Avg = toxicologicalIndicators.Select(j => j.IZVNO2).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].ToxicologicalIndicatorIZVNO3Avg = toxicologicalIndicators.Select(j => j.IZVNO3).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].ToxicologicalIndicatorIZVPPO4Avg = toxicologicalIndicators.Select(j => j.IZVPPO4).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].ToxicologicalIndicatorIZVCuAvg = toxicologicalIndicators.Select(j => j.IZVCu).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].ToxicologicalIndicatorIZVZnAvg = toxicologicalIndicators.Select(j => j.IZVZn).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].ToxicologicalIndicatorIZVMnAvg = toxicologicalIndicators.Select(j => j.IZVMn).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].ToxicologicalIndicatorIZVPbAvg = toxicologicalIndicators.Select(j => j.IZVPb).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].ToxicologicalIndicatorIZVNiAvg = toxicologicalIndicators.Select(j => j.IZVNi).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].ToxicologicalIndicatorIZVCdAvg = toxicologicalIndicators.Select(j => j.IZVCd).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].ToxicologicalIndicatorIZVCoAvg = toxicologicalIndicators.Select(j => j.IZVCo).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].ToxicologicalIndicatorKIZVkoNH4Avg = toxicologicalIndicators.Select(j => j.KIZVkoNH4).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].ToxicologicalIndicatorKIZVkoNO2Avg = toxicologicalIndicators.Select(j => j.KIZVkoNO2).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].ToxicologicalIndicatorKIZVkoNO3Avg = toxicologicalIndicators.Select(j => j.KIZVkoNO3).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].ToxicologicalIndicatorKIZVkoPPO4Avg = toxicologicalIndicators.Select(j => j.KIZVkoPPO4).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].ToxicologicalIndicatorKIZVkoCuAvg = toxicologicalIndicators.Select(j => j.KIZVkoCu).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].ToxicologicalIndicatorKIZVkoZnAvg = toxicologicalIndicators.Select(j => j.KIZVkoZn).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].ToxicologicalIndicatorKIZVkoMnAvg = toxicologicalIndicators.Select(j => j.KIZVkoMn).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].ToxicologicalIndicatorKIZVkoPbAvg = toxicologicalIndicators.Select(j => j.KIZVkoPb).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].ToxicologicalIndicatorKIZVkoNiAvg = toxicologicalIndicators.Select(j => j.KIZVkoNi).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].ToxicologicalIndicatorKIZVkoCdAvg = toxicologicalIndicators.Select(j => j.KIZVkoCd).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].ToxicologicalIndicatorKIZVkoCoAvg = toxicologicalIndicators.Select(j => j.KIZVkoCo).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].ToxicologicalIndicatorKIZVAvg = toxicologicalIndicators.Select(j => j.KIZV).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].ToxicologicalIndicatorNH4Max = toxicologicalIndicators.Select(j => j.NH4).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].ToxicologicalIndicatorNO2Max = toxicologicalIndicators.Select(j => j.NO2).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].ToxicologicalIndicatorNO3Max = toxicologicalIndicators.Select(j => j.NO3).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].ToxicologicalIndicatorPPO4Max = toxicologicalIndicators.Select(j => j.PPO4).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].ToxicologicalIndicatorCuMax = toxicologicalIndicators.Select(j => j.Cu).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].ToxicologicalIndicatorZnMax = toxicologicalIndicators.Select(j => j.Zn).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].ToxicologicalIndicatorMnMax = toxicologicalIndicators.Select(j => j.Mn).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].ToxicologicalIndicatorPbMax = toxicologicalIndicators.Select(j => j.Pb).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].ToxicologicalIndicatorNiMax = toxicologicalIndicators.Select(j => j.Ni).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].ToxicologicalIndicatorCdMax = toxicologicalIndicators.Select(j => j.Cd).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].ToxicologicalIndicatorCoMax = toxicologicalIndicators.Select(j => j.Co).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].ToxicologicalIndicatorIZVNH4Max = toxicologicalIndicators.Select(j => j.IZVNH4).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].ToxicologicalIndicatorIZVNO2Max = toxicologicalIndicators.Select(j => j.IZVNO2).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].ToxicologicalIndicatorIZVNO3Max = toxicologicalIndicators.Select(j => j.IZVNO3).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].ToxicologicalIndicatorIZVPPO4Max = toxicologicalIndicators.Select(j => j.IZVPPO4).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].ToxicologicalIndicatorIZVCuMax = toxicologicalIndicators.Select(j => j.IZVCu).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].ToxicologicalIndicatorIZVZnMax = toxicologicalIndicators.Select(j => j.IZVZn).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].ToxicologicalIndicatorIZVMnMax = toxicologicalIndicators.Select(j => j.IZVMn).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].ToxicologicalIndicatorIZVPbMax = toxicologicalIndicators.Select(j => j.IZVPb).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].ToxicologicalIndicatorIZVNiMax = toxicologicalIndicators.Select(j => j.IZVNi).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].ToxicologicalIndicatorIZVCdMax = toxicologicalIndicators.Select(j => j.IZVCd).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].ToxicologicalIndicatorIZVCoMax = toxicologicalIndicators.Select(j => j.IZVCo).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].ToxicologicalIndicatorKIZVkoNH4Max = toxicologicalIndicators.Select(j => j.KIZVkoNH4).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].ToxicologicalIndicatorKIZVkoNO2Max = toxicologicalIndicators.Select(j => j.KIZVkoNO2).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].ToxicologicalIndicatorKIZVkoNO3Max = toxicologicalIndicators.Select(j => j.KIZVkoNO3).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].ToxicologicalIndicatorKIZVkoPPO4Max = toxicologicalIndicators.Select(j => j.KIZVkoPPO4).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].ToxicologicalIndicatorKIZVkoCuMax = toxicologicalIndicators.Select(j => j.KIZVkoCu).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].ToxicologicalIndicatorKIZVkoZnMax = toxicologicalIndicators.Select(j => j.KIZVkoZn).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].ToxicologicalIndicatorKIZVkoMnMax = toxicologicalIndicators.Select(j => j.KIZVkoMn).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].ToxicologicalIndicatorKIZVkoPbMax = toxicologicalIndicators.Select(j => j.KIZVkoPb).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].ToxicologicalIndicatorKIZVkoNiMax = toxicologicalIndicators.Select(j => j.KIZVkoNi).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].ToxicologicalIndicatorKIZVkoCdMax = toxicologicalIndicators.Select(j => j.KIZVkoCd).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].ToxicologicalIndicatorKIZVkoCoMax = toxicologicalIndicators.Select(j => j.KIZVkoCo).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].ToxicologicalIndicatorKIZVMax = toxicologicalIndicators.Select(j => j.KIZV).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].ToxicologicalIndicatorNH4Min = toxicologicalIndicators.Select(j => j.NH4).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].ToxicologicalIndicatorNO2Min = toxicologicalIndicators.Select(j => j.NO2).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].ToxicologicalIndicatorNO3Min = toxicologicalIndicators.Select(j => j.NO3).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].ToxicologicalIndicatorPPO4Min = toxicologicalIndicators.Select(j => j.PPO4).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].ToxicologicalIndicatorCuMin = toxicologicalIndicators.Select(j => j.Cu).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].ToxicologicalIndicatorZnMin = toxicologicalIndicators.Select(j => j.Zn).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].ToxicologicalIndicatorMnMin = toxicologicalIndicators.Select(j => j.Mn).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].ToxicologicalIndicatorPbMin = toxicologicalIndicators.Select(j => j.Pb).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].ToxicologicalIndicatorNiMin = toxicologicalIndicators.Select(j => j.Ni).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].ToxicologicalIndicatorCdMin = toxicologicalIndicators.Select(j => j.Cd).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].ToxicologicalIndicatorCoMin = toxicologicalIndicators.Select(j => j.Co).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].ToxicologicalIndicatorIZVNH4Min = toxicologicalIndicators.Select(j => j.IZVNH4).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].ToxicologicalIndicatorIZVNO2Min = toxicologicalIndicators.Select(j => j.IZVNO2).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].ToxicologicalIndicatorIZVNO3Min = toxicologicalIndicators.Select(j => j.IZVNO3).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].ToxicologicalIndicatorIZVPPO4Min = toxicologicalIndicators.Select(j => j.IZVPPO4).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].ToxicologicalIndicatorIZVCuMin = toxicologicalIndicators.Select(j => j.IZVCu).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].ToxicologicalIndicatorIZVZnMin = toxicologicalIndicators.Select(j => j.IZVZn).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].ToxicologicalIndicatorIZVMnMin = toxicologicalIndicators.Select(j => j.IZVMn).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].ToxicologicalIndicatorIZVPbMin = toxicologicalIndicators.Select(j => j.IZVPb).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].ToxicologicalIndicatorIZVNiMin = toxicologicalIndicators.Select(j => j.IZVNi).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].ToxicologicalIndicatorIZVCdMin = toxicologicalIndicators.Select(j => j.IZVCd).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].ToxicologicalIndicatorIZVCoMin = toxicologicalIndicators.Select(j => j.IZVCo).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].ToxicologicalIndicatorKIZVkoNH4Min = toxicologicalIndicators.Select(j => j.KIZVkoNH4).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].ToxicologicalIndicatorKIZVkoNO2Min = toxicologicalIndicators.Select(j => j.KIZVkoNO2).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].ToxicologicalIndicatorKIZVkoNO3Min = toxicologicalIndicators.Select(j => j.KIZVkoNO3).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].ToxicologicalIndicatorKIZVkoPPO4Min = toxicologicalIndicators.Select(j => j.KIZVkoPPO4).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].ToxicologicalIndicatorKIZVkoCuMin = toxicologicalIndicators.Select(j => j.KIZVkoCu).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].ToxicologicalIndicatorKIZVkoZnMin = toxicologicalIndicators.Select(j => j.KIZVkoZn).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].ToxicologicalIndicatorKIZVkoMnMin = toxicologicalIndicators.Select(j => j.KIZVkoMn).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].ToxicologicalIndicatorKIZVkoPbMin = toxicologicalIndicators.Select(j => j.KIZVkoPb).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].ToxicologicalIndicatorKIZVkoNiMin = toxicologicalIndicators.Select(j => j.KIZVkoNi).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].ToxicologicalIndicatorKIZVkoCdMin = toxicologicalIndicators.Select(j => j.KIZVkoCd).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].ToxicologicalIndicatorKIZVkoCoMin = toxicologicalIndicators.Select(j => j.KIZVkoCo).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].ToxicologicalIndicatorKIZVMin = toxicologicalIndicators.Select(j => j.KIZV).Where(j => j != null).DefaultIfEmpty(0).Min();

                Transition transition = _context.Transition.FirstOrDefault(l => l.LakeId == lakes[i].LakeId);
                lakes[i].TransitionNoСhange = transition?.NoСhange;
                lakes[i].TransitionPermanent = transition?.Permanent;
                lakes[i].TransitionNewPermanent = transition?.NewPermanent;
                lakes[i].TransitionLostPermanent = transition?.LostPermanent;
                lakes[i].TransitionSeasonal = transition?.Seasonal;
                lakes[i].TransitionNewSeasonal = transition?.NewSeasonal;
                lakes[i].TransitionLostSeasonal = transition?.LostSeasonal;
                lakes[i].TransitionSeasonalToPermanent = transition?.SeasonalToPermanent;
                lakes[i].TransitionPermanentToDeasonal = transition?.PermanentToDeasonal;
                lakes[i].TransitionEphemeralPermanent = transition?.EphemeralPermanent;
                lakes[i].TransitionEphemeralSeasonal = transition?.EphemeralSeasonal;
                lakes[i].TransitionMaximumWater = transition?.MaximumWater;
                lakes[i].TransitionPermanentPer = transition?.PermanentPer;
                lakes[i].TransitionSeasonalPer = transition?.SeasonalPer;

                Seasonalit seasonalit = _context.Seasonalit.FirstOrDefault(l => l.LakeId == lakes[i].LakeId);
                lakes[i].SeasonalitNoData = seasonalit?.NoData;
                lakes[i].SeasonalitJanuary = seasonalit?.January;
                lakes[i].SeasonalitFebruary = seasonalit?.February;
                lakes[i].SeasonalitMarch = seasonalit?.March;
                lakes[i].SeasonalitApril = seasonalit?.April;
                lakes[i].SeasonalitMay = seasonalit?.May;
                lakes[i].SeasonalitJune = seasonalit?.June;
                lakes[i].SeasonalitJuly = seasonalit?.July;
                lakes[i].SeasonalitAugust = seasonalit?.August;
                lakes[i].SeasonalitSeptember = seasonalit?.September;
                lakes[i].SeasonalitOctober = seasonalit?.October;
                lakes[i].SeasonalitNovember = seasonalit?.November;
                lakes[i].SeasonalitDecember = seasonalit?.December;

                List<DynamicsLakeArea> dynamicsLakeAreas = _context.DynamicsLakeArea.Where(l => l.LakeId == lakes[i].LakeId).ToList();
                lakes[i].DynamicsLakeAreaNoDataPersAvg = dynamicsLakeAreas.Select(j => j.NoDataPers).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].DynamicsLakeAreaNotWaterAvg = dynamicsLakeAreas.Select(j => j.NotWater).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].DynamicsLakeAreaSeasonalWaterAreaAvg = dynamicsLakeAreas.Select(j => j.SeasonalWaterArea).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].DynamicsLakeAreaPermanentWaterAreaAvg = dynamicsLakeAreas.Select(j => j.PermanentWaterArea).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].DynamicsLakeAreaMaximumWaterAreaAvg = dynamicsLakeAreas.Select(j => j.MaximumWaterArea).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].DynamicsLakeAreaSeasonalWaterAreaPerAvg = dynamicsLakeAreas.Select(j => j.SeasonalWaterAreaPer).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].DynamicsLakeAreaPermanentWaterAreaPerAvg = dynamicsLakeAreas.Select(j => j.PermanentWaterAreaPer).Where(j => j != null).DefaultIfEmpty(0).Average();
                lakes[i].DynamicsLakeAreaNoDataPersMax = dynamicsLakeAreas.Select(j => j.NoDataPers).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].DynamicsLakeAreaNotWaterMax = dynamicsLakeAreas.Select(j => j.NotWater).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].DynamicsLakeAreaSeasonalWaterAreaMax = dynamicsLakeAreas.Select(j => j.SeasonalWaterArea).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].DynamicsLakeAreaPermanentWaterAreaMax = dynamicsLakeAreas.Select(j => j.PermanentWaterArea).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].DynamicsLakeAreaMaximumWaterAreaMax = dynamicsLakeAreas.Select(j => j.MaximumWaterArea).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].DynamicsLakeAreaSeasonalWaterAreaPerMax = dynamicsLakeAreas.Select(j => j.SeasonalWaterAreaPer).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].DynamicsLakeAreaPermanentWaterAreaPerMax = dynamicsLakeAreas.Select(j => j.PermanentWaterAreaPer).Where(j => j != null).DefaultIfEmpty(0).Max();
                lakes[i].DynamicsLakeAreaNoDataPersMin = dynamicsLakeAreas.Select(j => j.NoDataPers).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].DynamicsLakeAreaNotWaterMin = dynamicsLakeAreas.Select(j => j.NotWater).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].DynamicsLakeAreaSeasonalWaterAreaMin = dynamicsLakeAreas.Select(j => j.SeasonalWaterArea).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].DynamicsLakeAreaPermanentWaterAreaMin = dynamicsLakeAreas.Select(j => j.PermanentWaterArea).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].DynamicsLakeAreaMaximumWaterAreaMin = dynamicsLakeAreas.Select(j => j.MaximumWaterArea).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].DynamicsLakeAreaSeasonalWaterAreaPerMin = dynamicsLakeAreas.Select(j => j.SeasonalWaterAreaPer).Where(j => j != null).DefaultIfEmpty(0).Min();
                lakes[i].DynamicsLakeAreaPermanentWaterAreaPerMin = dynamicsLakeAreas.Select(j => j.PermanentWaterAreaPer).Where(j => j != null).DefaultIfEmpty(0).Min();
            }

            if (message == "")
            {
                message = _sharedLocalizer["Found"] + ": " + lakes.Count().ToString();
            }
            return Json(new
            {
                lakes,
                message,
                //columnCodes,
                //columnNames
            });
        }

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