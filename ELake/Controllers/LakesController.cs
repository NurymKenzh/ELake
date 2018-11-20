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
            Lake lake = _context.Lake.FirstOrDefault(l => l.LakeId == LakeId);
            string NameKK = lake?.NameKK,
                NameEN = lake?.NameEN,
                NameRU = lake?.NameRU,
                Name = lake?.Name,
                vhb = lake?.VHB,
                vhu = lake?.VHU,
                longitude = lake?.Longitude,
                latitude = lake?.Latitude;
            decimal? area2015 = lake?.Area2015,
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

            // КАТО
            List<KATO> katoes = new List<KATO>();
            foreach (var lakeKATO in _context.LakeKATO.Where(l => l.LakeId == LakeId))
            {
                katoes.Add(_context.KATO.FirstOrDefault(k => k.Id == lakeKATO.KATOId));
            }
            string adm1 = string.Join(", ", katoes.Where(k => k.Level == 1).Select(k => k.Name)),
                adm2 = string.Join(", ", katoes.Where(k => k.Level == 2).Select(k => k.Name)),
                adm3 = string.Join(", ", katoes.Where(k => k.Level == 3).Select(k => k.Name));

            // Архивные данные озера
            LakesArchiveData lakesArchiveData = _context
                .LakesArchiveData
                .FirstOrDefault(l => l.LakeId == LakeId);
            int? surveyyear = lakesArchiveData?.SurveyYear;
            decimal? lakelength = lakesArchiveData?.LakeLength,
                lakeshorelinelength = lakesArchiveData?.LakeShorelineLength,
                lakemirrorarea = lakesArchiveData?.LakeMirrorArea,
                lakeabsoluteheight = lakesArchiveData?.LakeAbsoluteHeight,
                lakewidth = lakesArchiveData?.LakeWidth,
                lakemaxdepth = lakesArchiveData?.LakeMaxDepth,
                lakewatermass = lakesArchiveData?.LakeWaterMass;
            string archivalinfosource = lakesArchiveData?.ArchivalInfoSource;

            // Глобальные данные озер
            LakesGlobalData lakesGlobalData = _context
                .LakesGlobalData
                .FirstOrDefault(l => l.LakeId == LakeId);
            int? hylak_id = lakesGlobalData?.Hylak_id;
            string lake_name_eng = lakesGlobalData?.Lake_name_ENG,
                lake_name_ru = lakesGlobalData?.Lake_name_RU,
                lake_name_kz = lakesGlobalData?.Lake_name_KZ,
                lake_name = lakesGlobalData?.Lake_name,
                country_eng = lakesGlobalData?.Country_ENG,
                country_ru = lakesGlobalData?.Country_RU,
                country_kz = lakesGlobalData?.Country_KZ,
                country = lakesGlobalData?.Country,
                continent_eng = lakesGlobalData?.Continent_ENG,
                continent_ru = lakesGlobalData?.Continent_RU,
                continent_kz = lakesGlobalData?.Continent_KZ,
                continent = lakesGlobalData?.Continent;
            decimal? lake_area = lakesGlobalData?.Lake_area,
                shore_len = lakesGlobalData?.Shore_len,
                shore_dev = lakesGlobalData?.Shore_dev,
                vol_total = lakesGlobalData?.Vol_total,
                depth_avg = lakesGlobalData?.Depth_avg,
                dis_avg = lakesGlobalData?.Dis_avg,
                res_time = lakesGlobalData?.Res_time,
                elevation = lakesGlobalData?.Elevation,
                slope_100 = lakesGlobalData?.Slope_100,
                wshd_area = lakesGlobalData?.Wshd_area,
                pour_long = lakesGlobalData?.Pour_long,
                pour_lat = lakesGlobalData?.Pour_lat;

            // WaterLevels
            WaterLevel[] waterlevels = _context.WaterLevel.Where(w => w.LakeId == LakeId).ToArray();
            int[] waterlevelsyears = waterlevels.Select(w => w.Year).ToArray();
            decimal[] waterlevelsm = waterlevels.Select(w => w.WaterLavelM).ToArray();

            // SurfaceFlow
            SurfaceFlow[] surfaceflows = _context.SurfaceFlow.Where(s => s.LakeId == LakeId).ToArray();
            int[] surfaceflowsyears = surfaceflows.Select(s => s.Year).ToArray();
            decimal[] surfaceflowsvalues = surfaceflows.Select(s => s.Value).ToArray();

            // Precipitation
            Precipitation[] precipitations = _context.Precipitation.Where(s => s.LakeId == LakeId).ToArray();
            int[] precipitationsyears = precipitations.Select(s => s.Year).ToArray();
            decimal[] precipitationsvalues = precipitations.Select(s => s.Value).ToArray();

            // UndergroundFlow
            UndergroundFlow[] undergroundflows = _context.UndergroundFlow.Where(s => s.LakeId == LakeId).ToArray();
            int[] undergroundflowsyears = undergroundflows.Select(s => s.Year).ToArray();
            decimal[] undergroundflowsvalues = undergroundflows.Select(s => s.Value).ToArray();

            // SurfaceOutflow
            SurfaceOutflow[] surfaceoutflows = _context.SurfaceOutflow.Where(s => s.LakeId == LakeId).ToArray();
            int[] surfaceoutflowsyears = surfaceoutflows.Select(s => s.Year).ToArray();
            decimal[] surfaceoutflowsvalues = surfaceoutflows.Select(s => s.Value).ToArray();

            // Evaporation
            Evaporation[] evaporations = _context.Evaporation.Where(s => s.LakeId == LakeId).ToArray();
            int[] evaporationsyears = evaporations.Select(s => s.Year).ToArray();
            decimal[] evaporationsvalues = evaporations.Select(s => s.Value).ToArray();

            // UndergroundOutflow
            UndergroundOutflow[] undergroundoutflows = _context.UndergroundOutflow.Where(s => s.LakeId == LakeId).ToArray();
            int[] undergroundoutflowsyears = undergroundoutflows.Select(s => s.Year).ToArray();
            decimal[] undergroundoutflowsvalues = undergroundoutflows.Select(s => s.Value).ToArray();

            return Json(new
            {
                NameKK,
                NameEN,
                NameRU,
                Name,
                vhb,
                vhu,
                area2015,
                lakeshorelinelength2015,
                longitude,
                latitude,
                adm1,
                adm2,
                adm3,
                surveyyear,
                lakelength,
                lakeshorelinelength,
                lakemirrorarea,
                lakeabsoluteheight,
                lakewidth,
                lakemaxdepth,
                lakewatermass,
                archivalinfosource,
                hylak_id,
                lake_name_eng,
                lake_name_ru,
                lake_name_kz,
                lake_name,
                country_eng,
                country_ru,
                country_kz,
                country,
                continent_eng,
                continent_ru,
                continent_kz,
                continent,
                lake_area,
                shore_len,
                shore_dev,
                vol_total,
                depth_avg,
                dis_avg,
                res_time,
                elevation,
                slope_100,
                wshd_area,
                pour_long,
                pour_lat,
                waterlevelsyears,
                waterlevelsm,
                surfaceflowsyears,
                surfaceflowsvalues,
                precipitationsyears,
                precipitationsvalues,
                undergroundflowsyears,
                undergroundflowsvalues,
                surfaceoutflowsyears,
                surfaceoutflowsvalues,
                evaporationsyears,
                evaporationsvalues,
                undergroundoutflowsyears,
                undergroundoutflowsvalues,
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
        public ActionResult GetLakes(string Search)
        {
            if (Search == null)
            {
                Search = "";
            }
            var lakes = _context.Lake
                .Where(l => l.Name.ToLower().Contains(Search.ToLower()))
                .OrderBy(l => l.Name)
                .ToArray();
            return Json(new
            {
                lakes
            });
        }
    }
}