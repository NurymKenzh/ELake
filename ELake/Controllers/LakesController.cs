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
        public async Task<IActionResult> Analytics()
        {
            ViewBag.Adm1 = new SelectList(_context.KATO.Where(k => k.Level == 1).OrderBy(k => k.Name), "Id", "Name");
            return View();
        }


        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> CreateFileToAnalytics()
        {
            string sContentRootPath = _hostingEnvironment.WebRootPath;
            sContentRootPath = Path.Combine(sContentRootPath, "Analytics");
            string assemblyName = "lakes.txt";
            string filePopulateLakes = Path.Combine(sContentRootPath, assemblyName);
            using (StreamWriter file = new StreamWriter(filePopulateLakes))
            {
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
                    //WaterBalance waterBalance = _context.WaterBalance
                    //    .FirstOrDefault(l => l.LakeId == lake.LakeId);
                    //codePopulateLakes += @"Lakes.Add(new Lake()
                    //    {
                    //        Id = " + lake.Id.ToString() + @",
                    //        LakeId = " + lake.LakeId.ToString() + @",
                    //        Area2015 = " + lake.Area2015.ToString().Replace(',', '.') + @"M,
                    //        Shoreline2015 = " + lake.LakeShorelineLength2015.ToString().Replace(',', '.') + @"M,
                    //        ArchiveLength = " + PopulateDecimal(lakesArchiveData?.LakeLength) + @"
                    //        ArchiveShoreline = " + PopulateDecimal(lakesArchiveData?.LakeShorelineLength) + @"
                    //        ArchiveMirrorArea = " + PopulateDecimal(lakesArchiveData?.LakeMirrorArea) + @"
                    //        ArchiveAbsoluteHeight = " + PopulateDecimal(lakesArchiveData?.LakeAbsoluteHeight) + @"
                    //        ArchiveWidth = " + PopulateDecimal(lakesArchiveData?.LakeWidth) + @"
                    //        ArchiveMaxDepth = " + PopulateDecimal(lakesArchiveData?.LakeMaxDepth) + @"
                    //        ArchiveWaterMass = " + PopulateDecimal(lakesArchiveData?.LakeWaterMass) + @"
                    //        GlobalArea = " + PopulateDecimal(lakesGlobalData?.Lake_area) + @"
                    //        GlobalShoreLength = " + PopulateDecimal(lakesGlobalData?.Shore_len) + @"
                    //        GlobalIndentation = " + PopulateDecimal(lakesGlobalData?.Shore_dev) + @"
                    //        GlobalVolume = " + PopulateDecimal(lakesGlobalData?.Vol_total) + @"
                    //        GlobalDepth = " + PopulateDecimal(lakesGlobalData?.Depth_avg) + @"
                    //        GlobalFlow = " + PopulateDecimal(lakesGlobalData?.Dis_avg) + @"
                    //        GlobalStayTime = " + PopulateDecimal(lakesGlobalData?.Res_time) + @"
                    //        GlobalElevation = " + PopulateDecimal(lakesGlobalData?.Elevation) + @"
                    //        GlobalSlope = " + PopulateDecimal(lakesGlobalData?.Slope_100) + @"
                    //        GlobalCatchmentArea = " + PopulateDecimal(lakesGlobalData?.Wshd_area) + @"
                    //    });
                    //    ";
                    string line = lake.Id.ToString();
                    // Analyze2
                    line += "\t" + PopulateDecimal(lake.LakeId);
                    line += "\t" + PopulateDecimal(lake.Area2015);
                    line += "\t" + PopulateDecimal(lake.LakeShorelineLength2015);
                    line += "\t" + PopulateDecimal(lakesArchiveData?.LakeLength);
                    line += "\t" + PopulateDecimal(lakesArchiveData?.LakeShorelineLength);
                    line += "\t" + PopulateDecimal(lakesArchiveData?.LakeMirrorArea);
                    line += "\t" + PopulateDecimal(lakesArchiveData?.LakeAbsoluteHeight);
                    line += "\t" + PopulateDecimal(lakesArchiveData?.LakeWidth);
                    line += "\t" + PopulateDecimal(lakesArchiveData?.LakeMaxDepth);
                    line += "\t" + PopulateDecimal(lakesArchiveData?.LakeWaterMass);
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
                    line += "\t" + PopulateDecimal(_context.WaterBalance.Where(w => w.LakeId == lake.LakeId).DefaultIfEmpty().Average(w => w.SurfaceFlow));
                    line += "\t" + PopulateDecimal(_context.WaterBalance.Where(w => w.LakeId == lake.LakeId).DefaultIfEmpty().Average(w => w.SurfaceOutflow));
                    line += "\t" + PopulateDecimal(_context.WaterBalance.Where(w => w.LakeId == lake.LakeId).DefaultIfEmpty().Average(w => w.UndergroundFlow));
                    line += "\t" + PopulateDecimal(_context.WaterBalance.Where(w => w.LakeId == lake.LakeId).DefaultIfEmpty().Average(w => w.UndergroundOutflow));
                    line += "\t" + PopulateDecimal(_context.WaterBalance.Where(w => w.LakeId == lake.LakeId).DefaultIfEmpty().Average(w => w.Precipitation));
                    line += "\t" + PopulateDecimal(_context.WaterBalance.Where(w => w.LakeId == lake.LakeId).DefaultIfEmpty().Average(w => w.Evaporation));
                    line += "\t" + PopulateDecimal(_context.WaterBalance.Where(w => w.LakeId == lake.LakeId).DefaultIfEmpty().Max(w => w.SurfaceFlow));
                    line += "\t" + PopulateDecimal(_context.WaterBalance.Where(w => w.LakeId == lake.LakeId).DefaultIfEmpty().Max(w => w.SurfaceOutflow));
                    line += "\t" + PopulateDecimal(_context.WaterBalance.Where(w => w.LakeId == lake.LakeId).DefaultIfEmpty().Max(w => w.UndergroundFlow));
                    line += "\t" + PopulateDecimal(_context.WaterBalance.Where(w => w.LakeId == lake.LakeId).DefaultIfEmpty().Max(w => w.UndergroundOutflow));
                    line += "\t" + PopulateDecimal(_context.WaterBalance.Where(w => w.LakeId == lake.LakeId).DefaultIfEmpty().Max(w => w.Precipitation));
                    line += "\t" + PopulateDecimal(_context.WaterBalance.Where(w => w.LakeId == lake.LakeId).DefaultIfEmpty().Max(w => w.Evaporation));
                    line += "\t" + PopulateDecimal(_context.WaterBalance.Where(w => w.LakeId == lake.LakeId).DefaultIfEmpty().Min(w => w.SurfaceFlow));
                    line += "\t" + PopulateDecimal(_context.WaterBalance.Where(w => w.LakeId == lake.LakeId).DefaultIfEmpty().Min(w => w.SurfaceOutflow));
                    line += "\t" + PopulateDecimal(_context.WaterBalance.Where(w => w.LakeId == lake.LakeId).DefaultIfEmpty().Min(w => w.UndergroundFlow));
                    line += "\t" + PopulateDecimal(_context.WaterBalance.Where(w => w.LakeId == lake.LakeId).DefaultIfEmpty().Min(w => w.UndergroundOutflow));
                    line += "\t" + PopulateDecimal(_context.WaterBalance.Where(w => w.LakeId == lake.LakeId).DefaultIfEmpty().Min(w => w.Precipitation));
                    line += "\t" + PopulateDecimal(_context.WaterBalance.Where(w => w.LakeId == lake.LakeId).DefaultIfEmpty().Min(w => w.Evaporation));
                    line += "\t" + PopulateDecimal(_context.WaterLevel.Where(w => w.LakeId == lake.LakeId).DefaultIfEmpty().Average(w => w.WaterLavelM));
                    line += "\t" + PopulateDecimal(_context.WaterLevel.Where(w => w.LakeId == lake.LakeId).DefaultIfEmpty().Max(w => w.WaterLavelM));
                    line += "\t" + PopulateDecimal(_context.WaterLevel.Where(w => w.LakeId == lake.LakeId).DefaultIfEmpty().Min(w => w.WaterLavelM));
                    line += "\t" + PopulateDecimal(_context.BathigraphicAndVolumetricCurveData.Where(h => h.LakeId == lake.LakeId).DefaultIfEmpty().Average(h => h.WaterLevel));
                    line += "\t" + PopulateDecimal(_context.BathigraphicAndVolumetricCurveData.Where(h => h.LakeId == lake.LakeId).DefaultIfEmpty().Average(h => h.LakeArea));
                    line += "\t" + PopulateDecimal(_context.BathigraphicAndVolumetricCurveData.Where(h => h.LakeId == lake.LakeId).DefaultIfEmpty().Average(h => h.WaterMassVolume));
                    line += "\t" + PopulateDecimal(_context.BathigraphicAndVolumetricCurveData.Where(h => h.LakeId == lake.LakeId).DefaultIfEmpty().Max(h => h.WaterLevel));
                    line += "\t" + PopulateDecimal(_context.BathigraphicAndVolumetricCurveData.Where(h => h.LakeId == lake.LakeId).DefaultIfEmpty().Max(h => h.LakeArea));
                    line += "\t" + PopulateDecimal(_context.BathigraphicAndVolumetricCurveData.Where(h => h.LakeId == lake.LakeId).DefaultIfEmpty().Max(h => h.WaterMassVolume));
                    line += "\t" + PopulateDecimal(_context.BathigraphicAndVolumetricCurveData.Where(h => h.LakeId == lake.LakeId).DefaultIfEmpty().Min(h => h.WaterLevel));
                    line += "\t" + PopulateDecimal(_context.BathigraphicAndVolumetricCurveData.Where(h => h.LakeId == lake.LakeId).DefaultIfEmpty().Min(h => h.LakeArea));
                    line += "\t" + PopulateDecimal(_context.BathigraphicAndVolumetricCurveData.Where(h => h.LakeId == lake.LakeId).DefaultIfEmpty().Min(h => h.WaterMassVolume));
                    line += "\t" + PopulateDecimal(_context.GeneralHydrochemicalIndicator.Where(g => g.LakeId == lake.LakeId).DefaultIfEmpty().Average(g => g.Mineralization));
                    line += "\t" + PopulateDecimal(_context.GeneralHydrochemicalIndicator.Where(g => g.LakeId == lake.LakeId).DefaultIfEmpty().Average(g => g.TotalHardness));
                    line += "\t" + PopulateDecimal(_context.GeneralHydrochemicalIndicator.Where(g => g.LakeId == lake.LakeId).DefaultIfEmpty().Average(g => g.DissOxygWater));
                    line += "\t" + PopulateDecimal(_context.GeneralHydrochemicalIndicator.Where(g => g.LakeId == lake.LakeId).DefaultIfEmpty().Average(g => g.PercentOxygWater));
                    line += "\t" + PopulateDecimal(_context.GeneralHydrochemicalIndicator.Where(g => g.LakeId == lake.LakeId).DefaultIfEmpty().Average(g => g.pH));
                    line += "\t" + PopulateDecimal(_context.GeneralHydrochemicalIndicator.Where(g => g.LakeId == lake.LakeId).DefaultIfEmpty().Average(g => g.OrganicSubstances));
                    line += "\t" + PopulateDecimal(_context.GeneralHydrochemicalIndicator.Where(g => g.LakeId == lake.LakeId).DefaultIfEmpty().Max(g => g.Mineralization));
                    line += "\t" + PopulateDecimal(_context.GeneralHydrochemicalIndicator.Where(g => g.LakeId == lake.LakeId).DefaultIfEmpty().Max(g => g.TotalHardness));
                    line += "\t" + PopulateDecimal(_context.GeneralHydrochemicalIndicator.Where(g => g.LakeId == lake.LakeId).DefaultIfEmpty().Max(g => g.DissOxygWater));
                    line += "\t" + PopulateDecimal(_context.GeneralHydrochemicalIndicator.Where(g => g.LakeId == lake.LakeId).DefaultIfEmpty().Max(g => g.PercentOxygWater));
                    line += "\t" + PopulateDecimal(_context.GeneralHydrochemicalIndicator.Where(g => g.LakeId == lake.LakeId).DefaultIfEmpty().Max(g => g.pH));
                    line += "\t" + PopulateDecimal(_context.GeneralHydrochemicalIndicator.Where(g => g.LakeId == lake.LakeId).DefaultIfEmpty().Max(g => g.OrganicSubstances));
                    line += "\t" + PopulateDecimal(_context.GeneralHydrochemicalIndicator.Where(g => g.LakeId == lake.LakeId).DefaultIfEmpty().Min(g => g.Mineralization));
                    line += "\t" + PopulateDecimal(_context.GeneralHydrochemicalIndicator.Where(g => g.LakeId == lake.LakeId).DefaultIfEmpty().Min(g => g.TotalHardness));
                    line += "\t" + PopulateDecimal(_context.GeneralHydrochemicalIndicator.Where(g => g.LakeId == lake.LakeId).DefaultIfEmpty().Min(g => g.DissOxygWater));
                    line += "\t" + PopulateDecimal(_context.GeneralHydrochemicalIndicator.Where(g => g.LakeId == lake.LakeId).DefaultIfEmpty().Min(g => g.PercentOxygWater));
                    line += "\t" + PopulateDecimal(_context.GeneralHydrochemicalIndicator.Where(g => g.LakeId == lake.LakeId).DefaultIfEmpty().Min(g => g.pH));
                    line += "\t" + PopulateDecimal(_context.GeneralHydrochemicalIndicator.Where(g => g.LakeId == lake.LakeId).DefaultIfEmpty().Min(g => g.OrganicSubstances));
                    line += "\t" + PopulateDecimal(_context.IonsaltWaterComposition.Where(i => i.LakeId == lake.LakeId).DefaultIfEmpty().Average(i => i.CaMgEq));
                    line += "\t" + PopulateDecimal(_context.IonsaltWaterComposition.Where(i => i.LakeId == lake.LakeId).DefaultIfEmpty().Average(i => i.MgMgEq));
                    line += "\t" + PopulateDecimal(_context.IonsaltWaterComposition.Where(i => i.LakeId == lake.LakeId).DefaultIfEmpty().Average(i => i.NaKMgEq));
                    line += "\t" + PopulateDecimal(_context.IonsaltWaterComposition.Where(i => i.LakeId == lake.LakeId).DefaultIfEmpty().Average(i => i.ClMgEq));
                    line += "\t" + PopulateDecimal(_context.IonsaltWaterComposition.Where(i => i.LakeId == lake.LakeId).DefaultIfEmpty().Average(i => i.HCOMgEq));
                    line += "\t" + PopulateDecimal(_context.IonsaltWaterComposition.Where(i => i.LakeId == lake.LakeId).DefaultIfEmpty().Average(i => i.SOMgEq));
                    line += "\t" + PopulateDecimal(_context.IonsaltWaterComposition.Where(i => i.LakeId == lake.LakeId).DefaultIfEmpty().Max(i => i.CaMgEq));
                    line += "\t" + PopulateDecimal(_context.IonsaltWaterComposition.Where(i => i.LakeId == lake.LakeId).DefaultIfEmpty().Max(i => i.MgMgEq));
                    line += "\t" + PopulateDecimal(_context.IonsaltWaterComposition.Where(i => i.LakeId == lake.LakeId).DefaultIfEmpty().Max(i => i.NaKMgEq));
                    line += "\t" + PopulateDecimal(_context.IonsaltWaterComposition.Where(i => i.LakeId == lake.LakeId).DefaultIfEmpty().Max(i => i.ClMgEq));
                    line += "\t" + PopulateDecimal(_context.IonsaltWaterComposition.Where(i => i.LakeId == lake.LakeId).DefaultIfEmpty().Max(i => i.HCOMgEq));
                    line += "\t" + PopulateDecimal(_context.IonsaltWaterComposition.Where(i => i.LakeId == lake.LakeId).DefaultIfEmpty().Max(i => i.SOMgEq));
                    line += "\t" + PopulateDecimal(_context.IonsaltWaterComposition.Where(i => i.LakeId == lake.LakeId).DefaultIfEmpty().Min(i => i.CaMgEq));
                    line += "\t" + PopulateDecimal(_context.IonsaltWaterComposition.Where(i => i.LakeId == lake.LakeId).DefaultIfEmpty().Min(i => i.MgMgEq));
                    line += "\t" + PopulateDecimal(_context.IonsaltWaterComposition.Where(i => i.LakeId == lake.LakeId).DefaultIfEmpty().Min(i => i.NaKMgEq));
                    line += "\t" + PopulateDecimal(_context.IonsaltWaterComposition.Where(i => i.LakeId == lake.LakeId).DefaultIfEmpty().Min(i => i.ClMgEq));
                    line += "\t" + PopulateDecimal(_context.IonsaltWaterComposition.Where(i => i.LakeId == lake.LakeId).DefaultIfEmpty().Min(i => i.HCOMgEq));
                    line += "\t" + PopulateDecimal(_context.IonsaltWaterComposition.Where(i => i.LakeId == lake.LakeId).DefaultIfEmpty().Min(i => i.SOMgEq));
                    line += "\t" + PopulateDecimal(_context.ToxicologicalIndicator.Where(t => t.LakeId == lake.LakeId).DefaultIfEmpty().Average(t => t.NH4));
                    line += "\t" + PopulateDecimal(_context.ToxicologicalIndicator.Where(t => t.LakeId == lake.LakeId).DefaultIfEmpty().Average(t => t.NO2));
                    line += "\t" + PopulateDecimal(_context.ToxicologicalIndicator.Where(t => t.LakeId == lake.LakeId).DefaultIfEmpty().Average(t => t.NO3));
                    line += "\t" + PopulateDecimal(_context.ToxicologicalIndicator.Where(t => t.LakeId == lake.LakeId).DefaultIfEmpty().Average(t => t.PPO4));
                    line += "\t" + PopulateDecimal(_context.ToxicologicalIndicator.Where(t => t.LakeId == lake.LakeId).DefaultIfEmpty().Average(t => t.Cu));
                    line += "\t" + PopulateDecimal(_context.ToxicologicalIndicator.Where(t => t.LakeId == lake.LakeId).DefaultIfEmpty().Average(t => t.Zn));
                    line += "\t" + PopulateDecimal(_context.ToxicologicalIndicator.Where(t => t.LakeId == lake.LakeId).DefaultIfEmpty().Average(t => t.Mn));
                    line += "\t" + PopulateDecimal(_context.ToxicologicalIndicator.Where(t => t.LakeId == lake.LakeId).DefaultIfEmpty().Average(t => t.Pb));
                    line += "\t" + PopulateDecimal(_context.ToxicologicalIndicator.Where(t => t.LakeId == lake.LakeId).DefaultIfEmpty().Average(t => t.Ni));
                    line += "\t" + PopulateDecimal(_context.ToxicologicalIndicator.Where(t => t.LakeId == lake.LakeId).DefaultIfEmpty().Average(t => t.Cd));
                    line += "\t" + PopulateDecimal(_context.ToxicologicalIndicator.Where(t => t.LakeId == lake.LakeId).DefaultIfEmpty().Average(t => t.Co));
                    line += "\t" + PopulateDecimal(_context.ToxicologicalIndicator.Where(t => t.LakeId == lake.LakeId).DefaultIfEmpty().Max(t => t.NH4));
                    line += "\t" + PopulateDecimal(_context.ToxicologicalIndicator.Where(t => t.LakeId == lake.LakeId).DefaultIfEmpty().Max(t => t.NO2));
                    line += "\t" + PopulateDecimal(_context.ToxicologicalIndicator.Where(t => t.LakeId == lake.LakeId).DefaultIfEmpty().Max(t => t.NO3));
                    line += "\t" + PopulateDecimal(_context.ToxicologicalIndicator.Where(t => t.LakeId == lake.LakeId).DefaultIfEmpty().Max(t => t.PPO4));
                    line += "\t" + PopulateDecimal(_context.ToxicologicalIndicator.Where(t => t.LakeId == lake.LakeId).DefaultIfEmpty().Max(t => t.Cu));
                    line += "\t" + PopulateDecimal(_context.ToxicologicalIndicator.Where(t => t.LakeId == lake.LakeId).DefaultIfEmpty().Max(t => t.Zn));
                    line += "\t" + PopulateDecimal(_context.ToxicologicalIndicator.Where(t => t.LakeId == lake.LakeId).DefaultIfEmpty().Max(t => t.Mn));
                    line += "\t" + PopulateDecimal(_context.ToxicologicalIndicator.Where(t => t.LakeId == lake.LakeId).DefaultIfEmpty().Max(t => t.Pb));
                    line += "\t" + PopulateDecimal(_context.ToxicologicalIndicator.Where(t => t.LakeId == lake.LakeId).DefaultIfEmpty().Max(t => t.Ni));
                    line += "\t" + PopulateDecimal(_context.ToxicologicalIndicator.Where(t => t.LakeId == lake.LakeId).DefaultIfEmpty().Max(t => t.Cd));
                    line += "\t" + PopulateDecimal(_context.ToxicologicalIndicator.Where(t => t.LakeId == lake.LakeId).DefaultIfEmpty().Max(t => t.Co));
                    line += "\t" + PopulateDecimal(_context.ToxicologicalIndicator.Where(t => t.LakeId == lake.LakeId).DefaultIfEmpty().Min(t => t.NH4));
                    line += "\t" + PopulateDecimal(_context.ToxicologicalIndicator.Where(t => t.LakeId == lake.LakeId).DefaultIfEmpty().Min(t => t.NO2));
                    line += "\t" + PopulateDecimal(_context.ToxicologicalIndicator.Where(t => t.LakeId == lake.LakeId).DefaultIfEmpty().Min(t => t.NO3));
                    line += "\t" + PopulateDecimal(_context.ToxicologicalIndicator.Where(t => t.LakeId == lake.LakeId).DefaultIfEmpty().Min(t => t.PPO4));
                    line += "\t" + PopulateDecimal(_context.ToxicologicalIndicator.Where(t => t.LakeId == lake.LakeId).DefaultIfEmpty().Min(t => t.Cu));
                    line += "\t" + PopulateDecimal(_context.ToxicologicalIndicator.Where(t => t.LakeId == lake.LakeId).DefaultIfEmpty().Min(t => t.Zn));
                    line += "\t" + PopulateDecimal(_context.ToxicologicalIndicator.Where(t => t.LakeId == lake.LakeId).DefaultIfEmpty().Min(t => t.Mn));
                    line += "\t" + PopulateDecimal(_context.ToxicologicalIndicator.Where(t => t.LakeId == lake.LakeId).DefaultIfEmpty().Min(t => t.Pb));
                    line += "\t" + PopulateDecimal(_context.ToxicologicalIndicator.Where(t => t.LakeId == lake.LakeId).DefaultIfEmpty().Min(t => t.Ni));
                    line += "\t" + PopulateDecimal(_context.ToxicologicalIndicator.Where(t => t.LakeId == lake.LakeId).DefaultIfEmpty().Min(t => t.Cd));
                    line += "\t" + PopulateDecimal(_context.ToxicologicalIndicator.Where(t => t.LakeId == lake.LakeId).DefaultIfEmpty().Min(t => t.Co));
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
                    line += "\t" + PopulateDecimal(_context.DynamicsLakeArea.Where(d => d.LakeId == lake.LakeId).DefaultIfEmpty().Average(d => d.NoData));
                    line += "\t" + PopulateDecimal(_context.DynamicsLakeArea.Where(d => d.LakeId == lake.LakeId).DefaultIfEmpty().Average(d => d.NoWater));
                    line += "\t" + PopulateDecimal(_context.DynamicsLakeArea.Where(d => d.LakeId == lake.LakeId).DefaultIfEmpty().Average(d => d.SeasonalWaterArea));
                    line += "\t" + PopulateDecimal(_context.DynamicsLakeArea.Where(d => d.LakeId == lake.LakeId).DefaultIfEmpty().Average(d => d.PermanentWaterArea));
                    line += "\t" + PopulateDecimal(_context.DynamicsLakeArea.Where(d => d.LakeId == lake.LakeId).DefaultIfEmpty().Max(d => d.NoData));
                    line += "\t" + PopulateDecimal(_context.DynamicsLakeArea.Where(d => d.LakeId == lake.LakeId).DefaultIfEmpty().Max(d => d.NoWater));
                    line += "\t" + PopulateDecimal(_context.DynamicsLakeArea.Where(d => d.LakeId == lake.LakeId).DefaultIfEmpty().Max(d => d.SeasonalWaterArea));
                    line += "\t" + PopulateDecimal(_context.DynamicsLakeArea.Where(d => d.LakeId == lake.LakeId).DefaultIfEmpty().Max(d => d.PermanentWaterArea));
                    line += "\t" + PopulateDecimal(_context.DynamicsLakeArea.Where(d => d.LakeId == lake.LakeId).DefaultIfEmpty().Min(d => d.NoData));
                    line += "\t" + PopulateDecimal(_context.DynamicsLakeArea.Where(d => d.LakeId == lake.LakeId).DefaultIfEmpty().Min(d => d.NoWater));
                    line += "\t" + PopulateDecimal(_context.DynamicsLakeArea.Where(d => d.LakeId == lake.LakeId).DefaultIfEmpty().Min(d => d.SeasonalWaterArea));
                    line += "\t" + PopulateDecimal(_context.DynamicsLakeArea.Where(d => d.LakeId == lake.LakeId).DefaultIfEmpty().Min(d => d.PermanentWaterArea));

                    file.WriteLine(line);
                }
            }

            return View();
        }

        public bool CheckFormula(string Formula)
        {
            bool r = true;
            string formula_test = Formula;
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
            formula_test = formula_test.Replace("AND", "");
            formula_test = formula_test.Replace("OR", "");
            formula_test = formula_test.Replace("NOT", "");
            formula_test = formula_test.Replace("Area2015", "");
            formula_test = formula_test.Replace("Shoreline2015", "");
            formula_test = formula_test.Replace("ArchiveLength", "");
            formula_test = formula_test.Replace("ArchiveShoreline", "");
            formula_test = formula_test.Replace("ArchiveMirrorArea", "");
            formula_test = formula_test.Replace("ArchiveAbsoluteHeight", "");
            formula_test = formula_test.Replace("ArchiveWidth", "");
            formula_test = formula_test.Replace("ArchiveMaxDepth", "");
            formula_test = formula_test.Replace("ArchiveWaterMass", "");
            formula_test = formula_test.Replace("GlobalArea", "");
            formula_test = formula_test.Replace("GlobalShoreLength", "");
            formula_test = formula_test.Replace("GlobalIndentation", "");
            formula_test = formula_test.Replace("GlobalVolume", "");
            formula_test = formula_test.Replace("GlobalDepth", "");
            formula_test = formula_test.Replace("GlobalFlow", "");
            formula_test = formula_test.Replace("GlobalStayTime", "");
            formula_test = formula_test.Replace("GlobalElevation", "");
            formula_test = formula_test.Replace("GlobalSlope", "");
            formula_test = formula_test.Replace("GlobalCatchmentArea", "");
            formula_test = formula_test.Replace("WaterBalanceSurfaceFlowAvg", "");
            formula_test = formula_test.Replace("WaterBalanceSurfaceOutflowAvg", "");
            formula_test = formula_test.Replace("WaterBalanceUndergroundFlowAvg", "");
            formula_test = formula_test.Replace("WaterBalanceUndergroundOutflowAvg", "");
            formula_test = formula_test.Replace("WaterBalancePrecipitationAvg", "");
            formula_test = formula_test.Replace("WaterBalanceEvaporationAvg", "");
            formula_test = formula_test.Replace("WaterBalanceSurfaceFlowMax", "");
            formula_test = formula_test.Replace("WaterBalanceSurfaceOutflowMax", "");
            formula_test = formula_test.Replace("WaterBalanceUndergroundFlowMax", "");
            formula_test = formula_test.Replace("WaterBalanceUndergroundOutflowMax", "");
            formula_test = formula_test.Replace("WaterBalancePrecipitationMax", "");
            formula_test = formula_test.Replace("WaterBalanceEvaporationMax", "");
            formula_test = formula_test.Replace("WaterBalanceSurfaceFlowMin", "");
            formula_test = formula_test.Replace("WaterBalanceSurfaceOutflowMin", "");
            formula_test = formula_test.Replace("WaterBalanceUndergroundFlowMin", "");
            formula_test = formula_test.Replace("WaterBalanceUndergroundOutflowMin", "");
            formula_test = formula_test.Replace("WaterBalancePrecipitationMin", "");
            formula_test = formula_test.Replace("WaterBalanceEvaporationMin", "");
            formula_test = formula_test.Replace("WaterLevelWaterLavelAvg", "");
            formula_test = formula_test.Replace("WaterLevelWaterLavelMax", "");
            formula_test = formula_test.Replace("WaterLevelWaterLavelMin", "");
            formula_test = formula_test.Replace("HypsometricWaterLevelAvg", "");
            formula_test = formula_test.Replace("HypsometricAreaAvg", "");
            formula_test = formula_test.Replace("HypsometricVolumeAvg", "");
            formula_test = formula_test.Replace("HypsometricWaterLevelMax", "");
            formula_test = formula_test.Replace("HypsometricAreaMax", "");
            formula_test = formula_test.Replace("HypsometricVolumeMax", "");
            formula_test = formula_test.Replace("HypsometricWaterLevelMin", "");
            formula_test = formula_test.Replace("HypsometricAreaMin", "");
            formula_test = formula_test.Replace("HypsometricVolumeMin", "");
            formula_test = formula_test.Replace("GeneralHydroChemistryMineralizationAvg", "");
            formula_test = formula_test.Replace("GeneralHydroChemistryTotalHardnessAvg", "");
            formula_test = formula_test.Replace("GeneralHydroChemistryDissOxygWaterAvg", "");
            formula_test = formula_test.Replace("GeneralHydroChemistryPercentOxygWaterAvg", "");
            formula_test = formula_test.Replace("GeneralHydroChemistrypHAvg", "");
            formula_test = formula_test.Replace("GeneralHydroChemistryOrganicSubstancesAvg", "");
            formula_test = formula_test.Replace("GeneralHydroChemistryMineralizationMax", "");
            formula_test = formula_test.Replace("GeneralHydroChemistryTotalHardnessMax", "");
            formula_test = formula_test.Replace("GeneralHydroChemistryDissOxygWaterMax", "");
            formula_test = formula_test.Replace("GeneralHydroChemistryPercentOxygWaterMax", "");
            formula_test = formula_test.Replace("GeneralHydroChemistrypHMax", "");
            formula_test = formula_test.Replace("GeneralHydroChemistryOrganicSubstancesMax", "");
            formula_test = formula_test.Replace("GeneralHydroChemistryMineralizationMin", "");
            formula_test = formula_test.Replace("GeneralHydroChemistryTotalHardnessMin", "");
            formula_test = formula_test.Replace("GeneralHydroChemistryDissOxygWaterMin", "");
            formula_test = formula_test.Replace("GeneralHydroChemistryPercentOxygWaterMin", "");
            formula_test = formula_test.Replace("GeneralHydroChemistrypHMin", "");
            formula_test = formula_test.Replace("GeneralHydroChemistryOrganicSubstancesMin", "");
            formula_test = formula_test.Replace("IonsaltCaAvg", "");
            formula_test = formula_test.Replace("IonsaltMgAvg", "");
            formula_test = formula_test.Replace("IonsaltNaKAvg", "");
            formula_test = formula_test.Replace("IonsaltClAvg", "");
            formula_test = formula_test.Replace("IonsaltHCOAvg", "");
            formula_test = formula_test.Replace("IonsaltSOAvg", "");
            formula_test = formula_test.Replace("IonsaltCaMax", "");
            formula_test = formula_test.Replace("IonsaltMgMax", "");
            formula_test = formula_test.Replace("IonsaltNaKMax", "");
            formula_test = formula_test.Replace("IonsaltClMax", "");
            formula_test = formula_test.Replace("IonsaltHCOMax", "");
            formula_test = formula_test.Replace("IonsaltSOMax", "");
            formula_test = formula_test.Replace("IonsaltCaMin", "");
            formula_test = formula_test.Replace("IonsaltMgMin", "");
            formula_test = formula_test.Replace("IonsaltNaKMin", "");
            formula_test = formula_test.Replace("IonsaltClMin", "");
            formula_test = formula_test.Replace("IonsaltHCOMin", "");
            formula_test = formula_test.Replace("IonsaltSOMin", "");
            formula_test = formula_test.Replace("ToxicNH4Avg", "");
            formula_test = formula_test.Replace("ToxicNO2Avg", "");
            formula_test = formula_test.Replace("ToxicNO3Avg", "");
            formula_test = formula_test.Replace("ToxicPPO4Avg", "");
            formula_test = formula_test.Replace("ToxicCuAvg", "");
            formula_test = formula_test.Replace("ToxicZnAvg", "");
            formula_test = formula_test.Replace("ToxicMnAvg", "");
            formula_test = formula_test.Replace("ToxicPbAvg", "");
            formula_test = formula_test.Replace("ToxicNiAvg", "");
            formula_test = formula_test.Replace("ToxicCdAvg", "");
            formula_test = formula_test.Replace("ToxicCoAvg", "");
            formula_test = formula_test.Replace("ToxicNH4Max", "");
            formula_test = formula_test.Replace("ToxicNO2Max", "");
            formula_test = formula_test.Replace("ToxicNO3Max", "");
            formula_test = formula_test.Replace("ToxicPPO4Max", "");
            formula_test = formula_test.Replace("ToxicCuMax", "");
            formula_test = formula_test.Replace("ToxicZnMax", "");
            formula_test = formula_test.Replace("ToxicMnMax", "");
            formula_test = formula_test.Replace("ToxicPbMax", "");
            formula_test = formula_test.Replace("ToxicNiMax", "");
            formula_test = formula_test.Replace("ToxicCdMax", "");
            formula_test = formula_test.Replace("ToxicCoMax", "");
            formula_test = formula_test.Replace("ToxicNH4Min", "");
            formula_test = formula_test.Replace("ToxicNO2Min", "");
            formula_test = formula_test.Replace("ToxicNO3Min", "");
            formula_test = formula_test.Replace("ToxicPPO4Min", "");
            formula_test = formula_test.Replace("ToxicCuMin", "");
            formula_test = formula_test.Replace("ToxicZnMin", "");
            formula_test = formula_test.Replace("ToxicMnMin", "");
            formula_test = formula_test.Replace("ToxicPbMin", "");
            formula_test = formula_test.Replace("ToxicNiMin", "");
            formula_test = formula_test.Replace("ToxicCdMin", "");
            formula_test = formula_test.Replace("ToxicCoMin", "");
            formula_test = formula_test.Replace("TransitionNoСhange", "");
            formula_test = formula_test.Replace("TransitionPermanent", "");
            formula_test = formula_test.Replace("TransitionNewPermanent", "");
            formula_test = formula_test.Replace("TransitionLostPermanent", "");
            formula_test = formula_test.Replace("TransitionSeasonal", "");
            formula_test = formula_test.Replace("TransitionNewSeasonal", "");
            formula_test = formula_test.Replace("TransitionLostSeasonal", "");
            formula_test = formula_test.Replace("TransitionSeasonalToPermanent", "");
            formula_test = formula_test.Replace("TransitionPermanentToDeasonal", "");
            formula_test = formula_test.Replace("TransitionEphemeralPermanent", "");
            formula_test = formula_test.Replace("TransitionEphemeralSeasonal", "");
            formula_test = formula_test.Replace("SeasonalityNoData", "");
            formula_test = formula_test.Replace("SeasonalityJanuary", "");
            formula_test = formula_test.Replace("SeasonalityFebruary", "");
            formula_test = formula_test.Replace("SeasonalityMarch", "");
            formula_test = formula_test.Replace("SeasonalityApril", "");
            formula_test = formula_test.Replace("SeasonalityMay", "");
            formula_test = formula_test.Replace("SeasonalityJune", "");
            formula_test = formula_test.Replace("SeasonalityJuly", "");
            formula_test = formula_test.Replace("SeasonalityAugust", "");
            formula_test = formula_test.Replace("SeasonalitySeptember", "");
            formula_test = formula_test.Replace("SeasonalityOctober", "");
            formula_test = formula_test.Replace("SeasonalityNovember", "");
            formula_test = formula_test.Replace("SeasonalityDecember", "");
            formula_test = formula_test.Replace("DynamicsLakeAreaNoDataAvg", "");
            formula_test = formula_test.Replace("DynamicsLakeAreaNoWaterAvg", "");
            formula_test = formula_test.Replace("DynamicsLakeAreaSeasonalAvg", "");
            formula_test = formula_test.Replace("DynamicsLakeAreaPermanentAvg", "");
            formula_test = formula_test.Replace("DynamicsLakeAreaNoDataMax", "");
            formula_test = formula_test.Replace("DynamicsLakeAreaNoWaterMax", "");
            formula_test = formula_test.Replace("DynamicsLakeAreaSeasonalMax", "");
            formula_test = formula_test.Replace("DynamicsLakeAreaPermanentMax", "");
            formula_test = formula_test.Replace("DynamicsLakeAreaNoDataMin", "");
            formula_test = formula_test.Replace("DynamicsLakeAreaNoWaterMin", "");
            formula_test = formula_test.Replace("DynamicsLakeAreaSeasonalMin", "");
            formula_test = formula_test.Replace("DynamicsLakeAreaPermanentMin", "");

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
            // Analyze1
            //codeFilter = codeFilter.Replace(",", ".");
            codeFilter = codeFilter.Replace("AND", "&&");
            codeFilter = codeFilter.Replace("OR", "||");
            codeFilter = codeFilter.Replace("NOT", "!");
            codeFilter = codeFilter.Replace("Area2015", "lake.Area2015");
            codeFilter = codeFilter.Replace("Shoreline2015", "lake.Shoreline2015");
            codeFilter = codeFilter.Replace("ArchiveLength", "lake.ArchiveLength");
            codeFilter = codeFilter.Replace("ArchiveShoreline", "lake.ArchiveShoreline");
            codeFilter = codeFilter.Replace("ArchiveMirrorArea", "lake.ArchiveMirrorArea");
            codeFilter = codeFilter.Replace("ArchiveAbsoluteHeight", "lake.ArchiveAbsoluteHeight");
            codeFilter = codeFilter.Replace("ArchiveWidth", "lake.ArchiveWidth");
            codeFilter = codeFilter.Replace("ArchiveMaxDepth", "lake.ArchiveMaxDepth");
            codeFilter = codeFilter.Replace("ArchiveWaterMass", "lake.ArchiveWaterMass");
            codeFilter = codeFilter.Replace("GlobalArea", "lake.GlobalArea");
            codeFilter = codeFilter.Replace("GlobalShoreLength", "lake.GlobalShoreLength");
            codeFilter = codeFilter.Replace("GlobalIndentation", "lake.GlobalIndentation");
            codeFilter = codeFilter.Replace("GlobalVolume", "lake.GlobalVolume");
            codeFilter = codeFilter.Replace("GlobalDepth", "lake.GlobalDepth");
            codeFilter = codeFilter.Replace("GlobalFlow", "lake.GlobalFlow");
            codeFilter = codeFilter.Replace("GlobalStayTime", "lake.GlobalStayTime");
            codeFilter = codeFilter.Replace("GlobalElevation", "lake.GlobalElevation");
            codeFilter = codeFilter.Replace("GlobalSlope", "lake.GlobalSlope");
            codeFilter = codeFilter.Replace("GlobalCatchmentArea", "lake.GlobalCatchmentArea");
            codeFilter = codeFilter.Replace("WaterBalanceSurfaceFlowAvg", "lake.WaterBalanceSurfaceFlowAvg");
            codeFilter = codeFilter.Replace("WaterBalanceSurfaceOutflowAvg", "lake.WaterBalanceSurfaceOutflowAvg");
            codeFilter = codeFilter.Replace("WaterBalanceUndergroundFlowAvg", "lake.WaterBalanceUndergroundFlowAvg");
            codeFilter = codeFilter.Replace("WaterBalanceUndergroundOutflowAvg", "lake.WaterBalanceUndergroundOutflowAvg");
            codeFilter = codeFilter.Replace("WaterBalancePrecipitationAvg", "lake.WaterBalancePrecipitationAvg");
            codeFilter = codeFilter.Replace("WaterBalanceEvaporationAvg", "lake.WaterBalanceEvaporationAvg");
            codeFilter = codeFilter.Replace("WaterBalanceSurfaceFlowMax", "lake.WaterBalanceSurfaceFlowMax");
            codeFilter = codeFilter.Replace("WaterBalanceSurfaceOutflowMax", "lake.WaterBalanceSurfaceOutflowMax");
            codeFilter = codeFilter.Replace("WaterBalanceUndergroundFlowMax", "lake.WaterBalanceUndergroundFlowMax");
            codeFilter = codeFilter.Replace("WaterBalanceUndergroundOutflowMax", "lake.WaterBalanceUndergroundOutflowMax");
            codeFilter = codeFilter.Replace("WaterBalancePrecipitationMax", "lake.WaterBalancePrecipitationMax");
            codeFilter = codeFilter.Replace("WaterBalanceEvaporationMax", "lake.WaterBalanceEvaporationMax");
            codeFilter = codeFilter.Replace("WaterBalanceSurfaceFlowMin", "lake.WaterBalanceSurfaceFlowMin");
            codeFilter = codeFilter.Replace("WaterBalanceSurfaceOutflowMin", "lake.WaterBalanceSurfaceOutflowMin");
            codeFilter = codeFilter.Replace("WaterBalanceUndergroundFlowMin", "lake.WaterBalanceUndergroundFlowMin");
            codeFilter = codeFilter.Replace("WaterBalanceUndergroundOutflowMin", "lake.WaterBalanceUndergroundOutflowMin");
            codeFilter = codeFilter.Replace("WaterBalancePrecipitationMin", "lake.WaterBalancePrecipitationMin");
            codeFilter = codeFilter.Replace("WaterBalanceEvaporationMin", "lake.WaterBalanceEvaporationMin");
            codeFilter = codeFilter.Replace("WaterLevelWaterLavelAvg", "lake.WaterLevelWaterLavelAvg");
            codeFilter = codeFilter.Replace("WaterLevelWaterLavelMax", "lake.WaterLevelWaterLavelMax");
            codeFilter = codeFilter.Replace("WaterLevelWaterLavelMin", "lake.WaterLevelWaterLavelMin");
            codeFilter = codeFilter.Replace("HypsometricWaterLevelAvg", "lake.HypsometricWaterLevelAvg");
            codeFilter = codeFilter.Replace("HypsometricAreaAvg", "lake.HypsometricAreaAvg");
            codeFilter = codeFilter.Replace("HypsometricVolumeAvg", "lake.HypsometricVolumeAvg");
            codeFilter = codeFilter.Replace("HypsometricWaterLevelMax", "lake.HypsometricWaterLevelMax");
            codeFilter = codeFilter.Replace("HypsometricAreaMax", "lake.HypsometricAreaMax");
            codeFilter = codeFilter.Replace("HypsometricVolumeMax", "lake.HypsometricVolumeMax");
            codeFilter = codeFilter.Replace("HypsometricWaterLevelMin", "lake.HypsometricWaterLevelMin");
            codeFilter = codeFilter.Replace("HypsometricAreaMin", "lake.HypsometricAreaMin");
            codeFilter = codeFilter.Replace("HypsometricVolumeMin", "lake.HypsometricVolumeMin");
            codeFilter = codeFilter.Replace("GeneralHydroChemistryMineralizationAvg", "lake.GeneralHydroChemistryMineralizationAvg");
            codeFilter = codeFilter.Replace("GeneralHydroChemistryTotalHardnessAvg", "lake.GeneralHydroChemistryTotalHardnessAvg");
            codeFilter = codeFilter.Replace("GeneralHydroChemistryDissOxygWaterAvg", "lake.GeneralHydroChemistryDissOxygWaterAvg");
            codeFilter = codeFilter.Replace("GeneralHydroChemistryPercentOxygWaterAvg", "lake.GeneralHydroChemistryPercentOxygWaterAvg");
            codeFilter = codeFilter.Replace("GeneralHydroChemistrypHAvg", "lake.GeneralHydroChemistrypHAvg");
            codeFilter = codeFilter.Replace("GeneralHydroChemistryOrganicSubstancesAvg", "lake.GeneralHydroChemistryOrganicSubstancesAvg");
            codeFilter = codeFilter.Replace("GeneralHydroChemistryMineralizationMax", "lake.GeneralHydroChemistryMineralizationMax");
            codeFilter = codeFilter.Replace("GeneralHydroChemistryTotalHardnessMax", "lake.GeneralHydroChemistryTotalHardnessMax");
            codeFilter = codeFilter.Replace("GeneralHydroChemistryDissOxygWaterMax", "lake.GeneralHydroChemistryDissOxygWaterMax");
            codeFilter = codeFilter.Replace("GeneralHydroChemistryPercentOxygWaterMax", "lake.GeneralHydroChemistryPercentOxygWaterMax");
            codeFilter = codeFilter.Replace("GeneralHydroChemistrypHMax", "lake.GeneralHydroChemistrypHMax");
            codeFilter = codeFilter.Replace("GeneralHydroChemistryOrganicSubstancesMax", "lake.GeneralHydroChemistryOrganicSubstancesMax");
            codeFilter = codeFilter.Replace("GeneralHydroChemistryMineralizationMin", "lake.GeneralHydroChemistryMineralizationMin");
            codeFilter = codeFilter.Replace("GeneralHydroChemistryTotalHardnessMin", "lake.GeneralHydroChemistryTotalHardnessMin");
            codeFilter = codeFilter.Replace("GeneralHydroChemistryDissOxygWaterMin", "lake.GeneralHydroChemistryDissOxygWaterMin");
            codeFilter = codeFilter.Replace("GeneralHydroChemistryPercentOxygWaterMin", "lake.GeneralHydroChemistryPercentOxygWaterMin");
            codeFilter = codeFilter.Replace("GeneralHydroChemistrypHMin", "lake.GeneralHydroChemistrypHMin");
            codeFilter = codeFilter.Replace("GeneralHydroChemistryOrganicSubstancesMin", "lake.GeneralHydroChemistryOrganicSubstancesMin");
            codeFilter = codeFilter.Replace("IonsaltCaAvg", "lake.IonsaltCaAvg");
            codeFilter = codeFilter.Replace("IonsaltMgAvg", "lake.IonsaltMgAvg");
            codeFilter = codeFilter.Replace("IonsaltNaKAvg", "lake.IonsaltNaKAvg");
            codeFilter = codeFilter.Replace("IonsaltClAvg", "lake.IonsaltClAvg");
            codeFilter = codeFilter.Replace("IonsaltHCOAvg", "lake.IonsaltHCOAvg");
            codeFilter = codeFilter.Replace("IonsaltSOAvg", "lake.IonsaltSOAvg");
            codeFilter = codeFilter.Replace("IonsaltCaMax", "lake.IonsaltCaMax");
            codeFilter = codeFilter.Replace("IonsaltMgMax", "lake.IonsaltMgMax");
            codeFilter = codeFilter.Replace("IonsaltNaKMax", "lake.IonsaltNaKMax");
            codeFilter = codeFilter.Replace("IonsaltClMax", "lake.IonsaltClMax");
            codeFilter = codeFilter.Replace("IonsaltHCOMax", "lake.IonsaltHCOMax");
            codeFilter = codeFilter.Replace("IonsaltSOMax", "lake.IonsaltSOMax");
            codeFilter = codeFilter.Replace("IonsaltCaMin", "lake.IonsaltCaMin");
            codeFilter = codeFilter.Replace("IonsaltMgMin", "lake.IonsaltMgMin");
            codeFilter = codeFilter.Replace("IonsaltNaKMin", "lake.IonsaltNaKMin");
            codeFilter = codeFilter.Replace("IonsaltClMin", "lake.IonsaltClMin");
            codeFilter = codeFilter.Replace("IonsaltHCOMin", "lake.IonsaltHCOMin");
            codeFilter = codeFilter.Replace("IonsaltSOMin", "lake.IonsaltSOMin");
            codeFilter = codeFilter.Replace("ToxicNH4Avg", "lake.ToxicNH4Avg");
            codeFilter = codeFilter.Replace("ToxicNO2Avg", "lake.ToxicNO2Avg");
            codeFilter = codeFilter.Replace("ToxicNO3Avg", "lake.ToxicNO3Avg");
            codeFilter = codeFilter.Replace("ToxicPPO4Avg", "lake.ToxicPPO4Avg");
            codeFilter = codeFilter.Replace("ToxicCuAvg", "lake.ToxicCuAvg");
            codeFilter = codeFilter.Replace("ToxicZnAvg", "lake.ToxicZnAvg");
            codeFilter = codeFilter.Replace("ToxicMnAvg", "lake.ToxicMnAvg");
            codeFilter = codeFilter.Replace("ToxicPbAvg", "lake.ToxicPbAvg");
            codeFilter = codeFilter.Replace("ToxicNiAvg", "lake.ToxicNiAvg");
            codeFilter = codeFilter.Replace("ToxicCdAvg", "lake.ToxicCdAvg");
            codeFilter = codeFilter.Replace("ToxicCoAvg", "lake.ToxicCoAvg");
            codeFilter = codeFilter.Replace("ToxicNH4Max", "lake.ToxicNH4Max");
            codeFilter = codeFilter.Replace("ToxicNO2Max", "lake.ToxicNO2Max");
            codeFilter = codeFilter.Replace("ToxicNO3Max", "lake.ToxicNO3Max");
            codeFilter = codeFilter.Replace("ToxicPPO4Max", "lake.ToxicPPO4Max");
            codeFilter = codeFilter.Replace("ToxicCuMax", "lake.ToxicCuMax");
            codeFilter = codeFilter.Replace("ToxicZnMax", "lake.ToxicZnMax");
            codeFilter = codeFilter.Replace("ToxicMnMax", "lake.ToxicMnMax");
            codeFilter = codeFilter.Replace("ToxicPbMax", "lake.ToxicPbMax");
            codeFilter = codeFilter.Replace("ToxicNiMax", "lake.ToxicNiMax");
            codeFilter = codeFilter.Replace("ToxicCdMax", "lake.ToxicCdMax");
            codeFilter = codeFilter.Replace("ToxicCoMax", "lake.ToxicCoMax");
            codeFilter = codeFilter.Replace("ToxicNH4Min", "lake.ToxicNH4Min");
            codeFilter = codeFilter.Replace("ToxicNO2Min", "lake.ToxicNO2Min");
            codeFilter = codeFilter.Replace("ToxicNO3Min", "lake.ToxicNO3Min");
            codeFilter = codeFilter.Replace("ToxicPPO4Min", "lake.ToxicPPO4Min");
            codeFilter = codeFilter.Replace("ToxicCuMin", "lake.ToxicCuMin");
            codeFilter = codeFilter.Replace("ToxicZnMin", "lake.ToxicZnMin");
            codeFilter = codeFilter.Replace("ToxicMnMin", "lake.ToxicMnMin");
            codeFilter = codeFilter.Replace("ToxicPbMin", "lake.ToxicPbMin");
            codeFilter = codeFilter.Replace("ToxicNiMin", "lake.ToxicNiMin");
            codeFilter = codeFilter.Replace("ToxicCdMin", "lake.ToxicCdMin");
            codeFilter = codeFilter.Replace("ToxicCoMin", "lake.ToxicCoMin");
            codeFilter = codeFilter.Replace("TransitionNoСhange", "lake.TransitionNoСhange");
            codeFilter = codeFilter.Replace("TransitionPermanent", "lake.TransitionPermanent");
            codeFilter = codeFilter.Replace("TransitionNewPermanent", "lake.TransitionNewPermanent");
            codeFilter = codeFilter.Replace("TransitionLostPermanent", "lake.TransitionLostPermanent");
            codeFilter = codeFilter.Replace("TransitionSeasonal", "lake.TransitionSeasonal");
            codeFilter = codeFilter.Replace("TransitionNewSeasonal", "lake.TransitionNewSeasonal");
            codeFilter = codeFilter.Replace("TransitionLostSeasonal", "lake.TransitionLostSeasonal");
            codeFilter = codeFilter.Replace("TransitionSeasonalToPermanent", "lake.TransitionSeasonalToPermanent");
            codeFilter = codeFilter.Replace("TransitionPermanentToDeasonal", "lake.TransitionPermanentToDeasonal");
            codeFilter = codeFilter.Replace("TransitionEphemeralPermanent", "lake.TransitionEphemeralPermanent");
            codeFilter = codeFilter.Replace("TransitionEphemeralSeasonal", "lake.TransitionEphemeralSeasonal");
            codeFilter = codeFilter.Replace("SeasonalityNoData", "lake.SeasonalityNoData");
            codeFilter = codeFilter.Replace("SeasonalityJanuary", "lake.SeasonalityJanuary");
            codeFilter = codeFilter.Replace("SeasonalityFebruary", "lake.SeasonalityFebruary");
            codeFilter = codeFilter.Replace("SeasonalityMarch", "lake.SeasonalityMarch");
            codeFilter = codeFilter.Replace("SeasonalityApril", "lake.SeasonalityApril");
            codeFilter = codeFilter.Replace("SeasonalityMay", "lake.SeasonalityMay");
            codeFilter = codeFilter.Replace("SeasonalityJune", "lake.SeasonalityJune");
            codeFilter = codeFilter.Replace("SeasonalityJuly", "lake.SeasonalityJuly");
            codeFilter = codeFilter.Replace("SeasonalityAugust", "lake.SeasonalityAugust");
            codeFilter = codeFilter.Replace("SeasonalitySeptember", "lake.SeasonalitySeptember");
            codeFilter = codeFilter.Replace("SeasonalityOctober", "lake.SeasonalityOctober");
            codeFilter = codeFilter.Replace("SeasonalityNovember", "lake.SeasonalityNovember");
            codeFilter = codeFilter.Replace("SeasonalityDecember", "lake.SeasonalityDecember");
            codeFilter = codeFilter.Replace("DynamicsLakeAreaNoDataAvg", "lake.DynamicsLakeAreaNoDataAvg");
            codeFilter = codeFilter.Replace("DynamicsLakeAreaNoWaterAvg", "lake.DynamicsLakeAreaNoWaterAvg");
            codeFilter = codeFilter.Replace("DynamicsLakeAreaSeasonalAvg", "lake.DynamicsLakeAreaSeasonalAvg");
            codeFilter = codeFilter.Replace("DynamicsLakeAreaPermanentAvg", "lake.DynamicsLakeAreaPermanentAvg");
            codeFilter = codeFilter.Replace("DynamicsLakeAreaNoDataMax", "lake.DynamicsLakeAreaNoDataMax");
            codeFilter = codeFilter.Replace("DynamicsLakeAreaNoWaterMax", "lake.DynamicsLakeAreaNoWaterMax");
            codeFilter = codeFilter.Replace("DynamicsLakeAreaSeasonalMax", "lake.DynamicsLakeAreaSeasonalMax");
            codeFilter = codeFilter.Replace("DynamicsLakeAreaPermanentMax", "lake.DynamicsLakeAreaPermanentMax");
            codeFilter = codeFilter.Replace("DynamicsLakeAreaNoDataMin", "lake.DynamicsLakeAreaNoDataMin");
            codeFilter = codeFilter.Replace("DynamicsLakeAreaNoWaterMin", "lake.DynamicsLakeAreaNoWaterMin");
            codeFilter = codeFilter.Replace("DynamicsLakeAreaSeasonalMin", "lake.DynamicsLakeAreaSeasonalMin");
            codeFilter = codeFilter.Replace("DynamicsLakeAreaPermanentMin", "lake.DynamicsLakeAreaPermanentMin");

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
            

            // Analyze3, Analyze4
            string codeToCompile = @"using System;
                using System.Collections.Generic;
                using System.IO;
                //using System.Linq;
                namespace RoslynCompile
                {
                    // lake for analytics
                    public class Lake
                    {
                        public int Id { get; set; }
                        public int LakeId { get; set; }
                        public decimal Area2015 { get; set; }
                        public decimal Shoreline2015 { get; set; }
                        public decimal ArchiveLength { get; set; }
                        public decimal ArchiveShoreline { get; set; }
                        public decimal ArchiveMirrorArea { get; set; }
                        public decimal ArchiveAbsoluteHeight { get; set; }
                        public decimal ArchiveWidth { get; set; }
                        public decimal ArchiveMaxDepth { get; set; }
                        public decimal ArchiveWaterMass { get; set; }
                        public decimal GlobalArea { get; set; }
                        public decimal GlobalShoreLength { get; set; }
                        public decimal GlobalIndentation { get; set; }
                        public decimal GlobalVolume { get; set; }
                        public decimal GlobalDepth { get; set; }
                        public decimal GlobalFlow { get; set; }
                        public decimal GlobalStayTime { get; set; }
                        public decimal GlobalElevation { get; set; }
                        public decimal GlobalSlope { get; set; }
                        public decimal GlobalCatchmentArea { get; set; }
                        public decimal WaterBalanceSurfaceFlowAvg { get; set; }
                        public decimal WaterBalanceSurfaceOutflowAvg { get; set; }
                        public decimal WaterBalanceUndergroundFlowAvg { get; set; }
                        public decimal WaterBalanceUndergroundOutflowAvg { get; set; }
                        public decimal WaterBalancePrecipitationAvg { get; set; }
                        public decimal WaterBalanceEvaporationAvg { get; set; }
                        public decimal WaterBalanceSurfaceFlowMax { get; set; }
                        public decimal WaterBalanceSurfaceOutflowMax { get; set; }
                        public decimal WaterBalanceUndergroundFlowMax { get; set; }
                        public decimal WaterBalanceUndergroundOutflowMax { get; set; }
                        public decimal WaterBalancePrecipitationMax { get; set; }
                        public decimal WaterBalanceEvaporationMax { get; set; }
                        public decimal WaterBalanceSurfaceFlowMin { get; set; }
                        public decimal WaterBalanceSurfaceOutflowMin { get; set; }
                        public decimal WaterBalanceUndergroundFlowMin { get; set; }
                        public decimal WaterBalanceUndergroundOutflowMin { get; set; }
                        public decimal WaterBalancePrecipitationMin { get; set; }
                        public decimal WaterBalanceEvaporationMin { get; set; }
                        public decimal WaterLevelWaterLavelAvg { get; set; }
                        public decimal WaterLevelWaterLavelMax { get; set; }
                        public decimal WaterLevelWaterLavelMin { get; set; }
                        public decimal HypsometricWaterLevelAvg { get; set; }
                        public decimal HypsometricAreaAvg { get; set; }
                        public decimal HypsometricVolumeAvg { get; set; }
                        public decimal HypsometricWaterLevelMax { get; set; }
                        public decimal HypsometricAreaMax { get; set; }
                        public decimal HypsometricVolumeMax { get; set; }
                        public decimal HypsometricWaterLevelMin { get; set; }
                        public decimal HypsometricAreaMin { get; set; }
                        public decimal HypsometricVolumeMin { get; set; }
                        public decimal GeneralHydroChemistryMineralizationAvg { get; set; }
                        public decimal GeneralHydroChemistryTotalHardnessAvg { get; set; }
                        public decimal GeneralHydroChemistryDissOxygWaterAvg { get; set; }
                        public decimal GeneralHydroChemistryPercentOxygWaterAvg { get; set; }
                        public decimal GeneralHydroChemistrypHAvg { get; set; }
                        public decimal GeneralHydroChemistryOrganicSubstancesAvg { get; set; }
                        public decimal GeneralHydroChemistryMineralizationMax { get; set; }
                        public decimal GeneralHydroChemistryTotalHardnessMax { get; set; }
                        public decimal GeneralHydroChemistryDissOxygWaterMax { get; set; }
                        public decimal GeneralHydroChemistryPercentOxygWaterMax { get; set; }
                        public decimal GeneralHydroChemistrypHMax { get; set; }
                        public decimal GeneralHydroChemistryOrganicSubstancesMax { get; set; }
                        public decimal GeneralHydroChemistryMineralizationMin { get; set; }
                        public decimal GeneralHydroChemistryTotalHardnessMin { get; set; }
                        public decimal GeneralHydroChemistryDissOxygWaterMin { get; set; }
                        public decimal GeneralHydroChemistryPercentOxygWaterMin { get; set; }
                        public decimal GeneralHydroChemistrypHMin { get; set; }
                        public decimal GeneralHydroChemistryOrganicSubstancesMin { get; set; }
                        public decimal IonsaltCaAvg { get; set; }
                        public decimal IonsaltMgAvg { get; set; }
                        public decimal IonsaltNaKAvg { get; set; }
                        public decimal IonsaltClAvg { get; set; }
                        public decimal IonsaltHCOAvg { get; set; }
                        public decimal IonsaltSOAvg { get; set; }
                        public decimal IonsaltCaMax { get; set; }
                        public decimal IonsaltMgMax { get; set; }
                        public decimal IonsaltNaKMax { get; set; }
                        public decimal IonsaltClMax { get; set; }
                        public decimal IonsaltHCOMax { get; set; }
                        public decimal IonsaltSOMax { get; set; }
                        public decimal IonsaltCaMin { get; set; }
                        public decimal IonsaltMgMin { get; set; }
                        public decimal IonsaltNaKMin { get; set; }
                        public decimal IonsaltClMin { get; set; }
                        public decimal IonsaltHCOMin { get; set; }
                        public decimal IonsaltSOMin { get; set; }
                        public decimal ToxicNH4Avg { get; set; }
                        public decimal ToxicNO2Avg { get; set; }
                        public decimal ToxicNO3Avg { get; set; }
                        public decimal ToxicPPO4Avg { get; set; }
                        public decimal ToxicCuAvg { get; set; }
                        public decimal ToxicZnAvg { get; set; }
                        public decimal ToxicMnAvg { get; set; }
                        public decimal ToxicPbAvg { get; set; }
                        public decimal ToxicNiAvg { get; set; }
                        public decimal ToxicCdAvg { get; set; }
                        public decimal ToxicCoAvg { get; set; }
                        public decimal ToxicNH4Max { get; set; }
                        public decimal ToxicNO2Max { get; set; }
                        public decimal ToxicNO3Max { get; set; }
                        public decimal ToxicPPO4Max { get; set; }
                        public decimal ToxicCuMax { get; set; }
                        public decimal ToxicZnMax { get; set; }
                        public decimal ToxicMnMax { get; set; }
                        public decimal ToxicPbMax { get; set; }
                        public decimal ToxicNiMax { get; set; }
                        public decimal ToxicCdMax { get; set; }
                        public decimal ToxicCoMax { get; set; }
                        public decimal ToxicNH4Min { get; set; }
                        public decimal ToxicNO2Min { get; set; }
                        public decimal ToxicNO3Min { get; set; }
                        public decimal ToxicPPO4Min { get; set; }
                        public decimal ToxicCuMin { get; set; }
                        public decimal ToxicZnMin { get; set; }
                        public decimal ToxicMnMin { get; set; }
                        public decimal ToxicPbMin { get; set; }
                        public decimal ToxicNiMin { get; set; }
                        public decimal ToxicCdMin { get; set; }
                        public decimal ToxicCoMin { get; set; }
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
                        public decimal SeasonalityNoData { get; set; }
                        public decimal SeasonalityJanuary { get; set; }
                        public decimal SeasonalityFebruary { get; set; }
                        public decimal SeasonalityMarch { get; set; }
                        public decimal SeasonalityApril { get; set; }
                        public decimal SeasonalityMay { get; set; }
                        public decimal SeasonalityJune { get; set; }
                        public decimal SeasonalityJuly { get; set; }
                        public decimal SeasonalityAugust { get; set; }
                        public decimal SeasonalitySeptember { get; set; }
                        public decimal SeasonalityOctober { get; set; }
                        public decimal SeasonalityNovember { get; set; }
                        public decimal SeasonalityDecember { get; set; }
                        public decimal DynamicsLakeAreaNoDataAvg { get; set; }
                        public decimal DynamicsLakeAreaNoWaterAvg { get; set; }
                        public decimal DynamicsLakeAreaSeasonalAvg { get; set; }
                        public decimal DynamicsLakeAreaPermanentAvg { get; set; }
                        public decimal DynamicsLakeAreaNoDataMax { get; set; }
                        public decimal DynamicsLakeAreaNoWaterMax { get; set; }
                        public decimal DynamicsLakeAreaSeasonalMax { get; set; }
                        public decimal DynamicsLakeAreaPermanentMax { get; set; }
                        public decimal DynamicsLakeAreaNoDataMin { get; set; }
                        public decimal DynamicsLakeAreaNoWaterMin { get; set; }
                        public decimal DynamicsLakeAreaSeasonalMin { get; set; }
                        public decimal DynamicsLakeAreaPermanentMin { get; set; }

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
                                    Area2015 = FromLine(lineS[2]),
                                    Shoreline2015 = FromLine(lineS[3]),
                                    ArchiveLength = FromLine(lineS[4]),
                                    ArchiveShoreline = FromLine(lineS[5]),
                                    ArchiveMirrorArea = FromLine(lineS[6]),
                                    ArchiveAbsoluteHeight = FromLine(lineS[7]),
                                    ArchiveWidth = FromLine(lineS[8]),
                                    ArchiveMaxDepth = FromLine(lineS[9]),
                                    ArchiveWaterMass = FromLine(lineS[10]),
                                    GlobalArea = FromLine(lineS[11]),
                                    GlobalShoreLength = FromLine(lineS[12]),
                                    GlobalIndentation = FromLine(lineS[13]),
                                    GlobalVolume = FromLine(lineS[14]),
                                    GlobalDepth = FromLine(lineS[15]),
                                    GlobalFlow = FromLine(lineS[16]),
                                    GlobalStayTime = FromLine(lineS[17]),
                                    GlobalElevation = FromLine(lineS[18]),
                                    GlobalSlope = FromLine(lineS[19]),
                                    GlobalCatchmentArea = FromLine(lineS[20]),
                                    WaterBalanceSurfaceFlowAvg = FromLine(lineS[21]),
                                    WaterBalanceSurfaceOutflowAvg = FromLine(lineS[22]),
                                    WaterBalanceUndergroundFlowAvg = FromLine(lineS[23]),
                                    WaterBalanceUndergroundOutflowAvg = FromLine(lineS[24]),
                                    WaterBalancePrecipitationAvg = FromLine(lineS[25]),
                                    WaterBalanceEvaporationAvg = FromLine(lineS[26]),
                                    WaterBalanceSurfaceFlowMax = FromLine(lineS[27]),
                                    WaterBalanceSurfaceOutflowMax = FromLine(lineS[28]),
                                    WaterBalanceUndergroundFlowMax = FromLine(lineS[29]),
                                    WaterBalanceUndergroundOutflowMax = FromLine(lineS[30]),
                                    WaterBalancePrecipitationMax = FromLine(lineS[31]),
                                    WaterBalanceEvaporationMax = FromLine(lineS[32]),
                                    WaterBalanceSurfaceFlowMin = FromLine(lineS[33]),
                                    WaterBalanceSurfaceOutflowMin = FromLine(lineS[34]),
                                    WaterBalanceUndergroundFlowMin = FromLine(lineS[35]),
                                    WaterBalanceUndergroundOutflowMin = FromLine(lineS[36]),
                                    WaterBalancePrecipitationMin = FromLine(lineS[37]),
                                    WaterBalanceEvaporationMin = FromLine(lineS[38]),
                                    WaterLevelWaterLavelAvg = FromLine(lineS[39]),
                                    WaterLevelWaterLavelMax = FromLine(lineS[40]),
                                    WaterLevelWaterLavelMin = FromLine(lineS[41]),
                                    HypsometricWaterLevelAvg = FromLine(lineS[42]),
                                    HypsometricAreaAvg = FromLine(lineS[43]),
                                    HypsometricVolumeAvg = FromLine(lineS[44]),
                                    HypsometricWaterLevelMax = FromLine(lineS[45]),
                                    HypsometricAreaMax = FromLine(lineS[46]),
                                    HypsometricVolumeMax = FromLine(lineS[47]),
                                    HypsometricWaterLevelMin = FromLine(lineS[48]),
                                    HypsometricAreaMin = FromLine(lineS[49]),
                                    HypsometricVolumeMin = FromLine(lineS[50]),
                                    GeneralHydroChemistryMineralizationAvg = FromLine(lineS[51]),
                                    GeneralHydroChemistryTotalHardnessAvg = FromLine(lineS[52]),
                                    GeneralHydroChemistryDissOxygWaterAvg = FromLine(lineS[53]),
                                    GeneralHydroChemistryPercentOxygWaterAvg = FromLine(lineS[54]),
                                    GeneralHydroChemistrypHAvg = FromLine(lineS[55]),
                                    GeneralHydroChemistryOrganicSubstancesAvg = FromLine(lineS[56]),
                                    GeneralHydroChemistryMineralizationMax = FromLine(lineS[57]),
                                    GeneralHydroChemistryTotalHardnessMax = FromLine(lineS[58]),
                                    GeneralHydroChemistryDissOxygWaterMax = FromLine(lineS[59]),
                                    GeneralHydroChemistryPercentOxygWaterMax = FromLine(lineS[60]),
                                    GeneralHydroChemistrypHMax = FromLine(lineS[61]),
                                    GeneralHydroChemistryOrganicSubstancesMax = FromLine(lineS[62]),
                                    GeneralHydroChemistryMineralizationMin = FromLine(lineS[63]),
                                    GeneralHydroChemistryTotalHardnessMin = FromLine(lineS[64]),
                                    GeneralHydroChemistryDissOxygWaterMin = FromLine(lineS[65]),
                                    GeneralHydroChemistryPercentOxygWaterMin = FromLine(lineS[66]),
                                    GeneralHydroChemistrypHMin = FromLine(lineS[67]),
                                    GeneralHydroChemistryOrganicSubstancesMin = FromLine(lineS[68]),
                                    IonsaltCaAvg = FromLine(lineS[69]),
                                    IonsaltMgAvg = FromLine(lineS[70]),
                                    IonsaltNaKAvg = FromLine(lineS[71]),
                                    IonsaltClAvg = FromLine(lineS[72]),
                                    IonsaltHCOAvg = FromLine(lineS[73]),
                                    IonsaltSOAvg = FromLine(lineS[74]),
                                    IonsaltCaMax = FromLine(lineS[75]),
                                    IonsaltMgMax = FromLine(lineS[76]),
                                    IonsaltNaKMax = FromLine(lineS[77]),
                                    IonsaltClMax = FromLine(lineS[78]),
                                    IonsaltHCOMax = FromLine(lineS[79]),
                                    IonsaltSOMax = FromLine(lineS[80]),
                                    IonsaltCaMin = FromLine(lineS[81]),
                                    IonsaltMgMin = FromLine(lineS[82]),
                                    IonsaltNaKMin = FromLine(lineS[83]),
                                    IonsaltClMin = FromLine(lineS[84]),
                                    IonsaltHCOMin = FromLine(lineS[85]),
                                    IonsaltSOMin = FromLine(lineS[86]),
                                    ToxicNH4Avg = FromLine(lineS[87]),
                                    ToxicNO2Avg = FromLine(lineS[88]),
                                    ToxicNO3Avg = FromLine(lineS[89]),
                                    ToxicPPO4Avg = FromLine(lineS[90]),
                                    ToxicCuAvg = FromLine(lineS[91]),
                                    ToxicZnAvg = FromLine(lineS[92]),
                                    ToxicMnAvg = FromLine(lineS[93]),
                                    ToxicPbAvg = FromLine(lineS[94]),
                                    ToxicNiAvg = FromLine(lineS[95]),
                                    ToxicCdAvg = FromLine(lineS[96]),
                                    ToxicCoAvg = FromLine(lineS[97]),
                                    ToxicNH4Max = FromLine(lineS[98]),
                                    ToxicNO2Max = FromLine(lineS[99]),
                                    ToxicNO3Max = FromLine(lineS[100]),
                                    ToxicPPO4Max = FromLine(lineS[101]),
                                    ToxicCuMax = FromLine(lineS[102]),
                                    ToxicZnMax = FromLine(lineS[103]),
                                    ToxicMnMax = FromLine(lineS[104]),
                                    ToxicPbMax = FromLine(lineS[105]),
                                    ToxicNiMax = FromLine(lineS[106]),
                                    ToxicCdMax = FromLine(lineS[107]),
                                    ToxicCoMax = FromLine(lineS[108]),
                                    ToxicNH4Min = FromLine(lineS[109]),
                                    ToxicNO2Min = FromLine(lineS[110]),
                                    ToxicNO3Min = FromLine(lineS[111]),
                                    ToxicPPO4Min = FromLine(lineS[112]),
                                    ToxicCuMin = FromLine(lineS[113]),
                                    ToxicZnMin = FromLine(lineS[114]),
                                    ToxicMnMin = FromLine(lineS[115]),
                                    ToxicPbMin = FromLine(lineS[116]),
                                    ToxicNiMin = FromLine(lineS[117]),
                                    ToxicCdMin = FromLine(lineS[118]),
                                    ToxicCoMin = FromLine(lineS[119]),
                                    TransitionNoСhange = FromLine(lineS[120]),
                                    TransitionPermanent = FromLine(lineS[121]),
                                    TransitionNewPermanent = FromLine(lineS[122]),
                                    TransitionLostPermanent = FromLine(lineS[123]),
                                    TransitionSeasonal = FromLine(lineS[124]),
                                    TransitionNewSeasonal = FromLine(lineS[125]),
                                    TransitionLostSeasonal = FromLine(lineS[126]),
                                    TransitionSeasonalToPermanent = FromLine(lineS[127]),
                                    TransitionPermanentToDeasonal = FromLine(lineS[128]),
                                    TransitionEphemeralPermanent = FromLine(lineS[129]),
                                    TransitionEphemeralSeasonal = FromLine(lineS[130]),
                                    SeasonalityNoData = FromLine(lineS[131]),
                                    SeasonalityJanuary = FromLine(lineS[132]),
                                    SeasonalityFebruary = FromLine(lineS[133]),
                                    SeasonalityMarch = FromLine(lineS[134]),
                                    SeasonalityApril = FromLine(lineS[135]),
                                    SeasonalityMay = FromLine(lineS[136]),
                                    SeasonalityJune = FromLine(lineS[137]),
                                    SeasonalityJuly = FromLine(lineS[138]),
                                    SeasonalityAugust = FromLine(lineS[139]),
                                    SeasonalitySeptember = FromLine(lineS[140]),
                                    SeasonalityOctober = FromLine(lineS[141]),
                                    SeasonalityNovember = FromLine(lineS[142]),
                                    SeasonalityDecember = FromLine(lineS[143]),
                                    DynamicsLakeAreaNoDataAvg = FromLine(lineS[144]),
                                    DynamicsLakeAreaNoWaterAvg = FromLine(lineS[145]),
                                    DynamicsLakeAreaSeasonalAvg = FromLine(lineS[146]),
                                    DynamicsLakeAreaPermanentAvg = FromLine(lineS[147]),
                                    DynamicsLakeAreaNoDataMax = FromLine(lineS[148]),
                                    DynamicsLakeAreaNoWaterMax = FromLine(lineS[149]),
                                    DynamicsLakeAreaSeasonalMax = FromLine(lineS[150]),
                                    DynamicsLakeAreaPermanentMax = FromLine(lineS[151]),
                                    DynamicsLakeAreaNoDataMin = FromLine(lineS[152]),
                                    DynamicsLakeAreaNoWaterMin = FromLine(lineS[153]),
                                    DynamicsLakeAreaSeasonalMin = FromLine(lineS[154]),
                                    DynamicsLakeAreaPermanentMin = FromLine(lineS[155]),

                                });
                            }
                            //File.ReadAllLines(fileName);

                            List<int?> rLakes = new List<int?>();

                            foreach (Lake lake in Lakes)
                            {
                                if(
                                    " + codeFilter + @"
                                    )
                                {
                                    rLakes.Add(lake.LakeId);
                                }
                            }

                            return rLakes
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

            if (message == "")
            {
                message = _sharedLocalizer["Found"] + ": " + lakes.Count().ToString();
            }
            return Json(new
            {
                lakes,
                message
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