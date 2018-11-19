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
using Microsoft.AspNetCore.Http;
using System.IO;
using OfficeOpenXml;

namespace ELake.Controllers
{
    public class GeneralHydrobiologicalIndicatorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;

        public GeneralHydrobiologicalIndicatorsController(ApplicationDbContext context,
            IHostingEnvironment hostingEnvironment,
            IStringLocalizer<SharedResources> sharedLocalizer)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _sharedLocalizer = sharedLocalizer;
        }

        // GET: GeneralHydrobiologicalIndicators
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Index(string SortOrder,
            int? LakeId,
            int? FishId,
            int? SurveyYear,
            int? Page)
        {
            var generalHydrobiologicalIndicators = _context.GeneralHydrobiologicalIndicator
                .Where(w => true);

            ViewBag.LakeIdFilter = LakeId;
            ViewBag.FishIdFilter = FishId;
            ViewBag.SurveyYearFilter = SurveyYear;

            ViewBag.LakeIdSort = SortOrder == "LakeId" ? "LakeIdDesc" : "LakeId";
            ViewBag.FishIdSort = SortOrder == "FishId" ? "FishIdDesc" : "FishId";
            ViewBag.SurveyYearSort = SortOrder == "SurveyYear" ? "SurveyYearDesc" : "SurveyYear";

            if (LakeId != null)
            {
                generalHydrobiologicalIndicators = generalHydrobiologicalIndicators.Where(w => w.LakeId == LakeId);
            }
            if (FishId != null)
            {
                generalHydrobiologicalIndicators = generalHydrobiologicalIndicators.Where(w => w.FishId == FishId);
            }
            if (SurveyYear != null)
            {
                generalHydrobiologicalIndicators = generalHydrobiologicalIndicators.Where(w => w.SurveyYear == SurveyYear);
            }

            switch (SortOrder)
            {
                case "LakeId":
                    generalHydrobiologicalIndicators = generalHydrobiologicalIndicators.OrderBy(w => w.LakeId);
                    break;
                case "LakeIdDesc":
                    generalHydrobiologicalIndicators = generalHydrobiologicalIndicators.OrderByDescending(w => w.LakeId);
                    break;
                case "FishId":
                    generalHydrobiologicalIndicators = generalHydrobiologicalIndicators.OrderBy(w => w.FishId);
                    break;
                case "FishIdDesc":
                    generalHydrobiologicalIndicators = generalHydrobiologicalIndicators.OrderByDescending(w => w.FishId);
                    break;
                case "SurveyYear":
                    generalHydrobiologicalIndicators = generalHydrobiologicalIndicators.OrderBy(w => w.SurveyYear);
                    break;
                case "SurveyYearDesc":
                    generalHydrobiologicalIndicators = generalHydrobiologicalIndicators.OrderByDescending(w => w.SurveyYear);
                    break;
                default:
                    generalHydrobiologicalIndicators = generalHydrobiologicalIndicators.OrderBy(w => w.Id);
                    break;
            }
            ViewBag.SortOrder = SortOrder;

            var pager = new Pager(generalHydrobiologicalIndicators.Count(), Page);

            var viewModel = new GeneralHydrobiologicalIndicatorIndexPageViewModel
            {
                Items = generalHydrobiologicalIndicators.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };

            return View(viewModel);
        }

        // GET: GeneralHydrobiologicalIndicators/Details/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var generalHydrobiologicalIndicator = await _context.GeneralHydrobiologicalIndicator
                .SingleOrDefaultAsync(m => m.Id == id);
            if (generalHydrobiologicalIndicator == null)
            {
                return NotFound();
            }

            return View(generalHydrobiologicalIndicator);
        }

        // GET: GeneralHydrobiologicalIndicators/Create
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: GeneralHydrobiologicalIndicators/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Create([Bind("Id,LakeId,FishId,SurveyYear,ZooplanktonBiomass,BenthosBiomass,SpeciesWealthIndex,FishCatchLimit,CurrentCommercialFishProductivity,PotentialFishProducts,PotentialGrowingVolume,CurrentUsage,RecommendedUse")] GeneralHydrobiologicalIndicator generalHydrobiologicalIndicator)
        {
            if (ModelState.IsValid)
            {
                _context.Add(generalHydrobiologicalIndicator);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(generalHydrobiologicalIndicator);
        }

        // GET: GeneralHydrobiologicalIndicators/Edit/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var generalHydrobiologicalIndicator = await _context.GeneralHydrobiologicalIndicator.SingleOrDefaultAsync(m => m.Id == id);
            if (generalHydrobiologicalIndicator == null)
            {
                return NotFound();
            }
            return View(generalHydrobiologicalIndicator);
        }

        // POST: GeneralHydrobiologicalIndicators/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LakeId,FishId,SurveyYear,ZooplanktonBiomass,BenthosBiomass,SpeciesWealthIndex,FishCatchLimit,CurrentCommercialFishProductivity,PotentialFishProducts,PotentialGrowingVolume,CurrentUsage,RecommendedUse")] GeneralHydrobiologicalIndicator generalHydrobiologicalIndicator)
        {
            if (id != generalHydrobiologicalIndicator.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(generalHydrobiologicalIndicator);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GeneralHydrobiologicalIndicatorExists(generalHydrobiologicalIndicator.Id))
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
            return View(generalHydrobiologicalIndicator);
        }

        // GET: GeneralHydrobiologicalIndicators/Delete/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var generalHydrobiologicalIndicator = await _context.GeneralHydrobiologicalIndicator
                .SingleOrDefaultAsync(m => m.Id == id);
            if (generalHydrobiologicalIndicator == null)
            {
                return NotFound();
            }

            return View(generalHydrobiologicalIndicator);
        }

        // POST: GeneralHydrobiologicalIndicators/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var generalHydrobiologicalIndicator = await _context.GeneralHydrobiologicalIndicator.SingleOrDefaultAsync(m => m.Id == id);
            _context.GeneralHydrobiologicalIndicator.Remove(generalHydrobiologicalIndicator);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GeneralHydrobiologicalIndicatorExists(int id)
        {
            return _context.GeneralHydrobiologicalIndicator.Any(e => e.Id == id);
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
                    List<GeneralHydrobiologicalIndicator> waterBalances = new List<GeneralHydrobiologicalIndicator>();
                    for (int i = start_row; ; i++)
                    {
                        if (package.Workbook.Worksheets.FirstOrDefault().Cells[i, 1].Value == null)
                        {
                            break;
                        }
                        GeneralHydrobiologicalIndicator waterBalance = new GeneralHydrobiologicalIndicator();

                        try
                        {
                            waterBalance.LakeId = Convert.ToInt32(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 1].Value);
                            waterBalance.FishId = Convert.ToInt32(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 2].Value);
                            waterBalance.SurveyYear = Convert.ToInt32(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 3].Value);
                            waterBalance.ZooplanktonBiomass = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 4].Value == null ?
                                (decimal?)null :
                                Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 4].Value);
                            waterBalance.SpeciesWealthIndex = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 5].Value == null ?
                                (decimal?)null :
                                Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 5].Value);
                            waterBalance.FishCatchLimit = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 6].Value == null ?
                                (decimal?)null :
                                Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 6].Value);
                            waterBalance.CurrentCommercialFishProductivity = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 7].Value == null ?
                                (decimal?)null :
                                Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 7].Value);
                            waterBalance.PotentialFishProducts = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 8].Value == null ?
                                (decimal?)null :
                                Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 8].Value);
                            waterBalance.PotentialGrowingVolume = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 9].Value == null ?
                                (decimal?)null :
                                Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 9].Value);
                            waterBalance.CurrentUsage = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 10].Value == null ?
                                (decimal?)null :
                                Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 10].Value);
                            waterBalance.RecommendedUse = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 11].Value == null ?
                                (decimal?)null :
                                Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 11].Value);
                        }
                        catch (Exception e)
                        {
                            ViewBag.Error = $"{_sharedLocalizer["Row"]} {i.ToString()}: " + e.Message + (e.InnerException == null ? "" : ": " + e.InnerException.Message);
                            break;
                        }

                        waterBalances.Add(waterBalance);
                        _context.Add(waterBalances.LastOrDefault());
                    }
                    if (string.IsNullOrEmpty(ViewBag.Error))
                    {
                        _context.SaveChanges();
                        ViewBag.Report = $"{_sharedLocalizer["UploadedCount"]}: {waterBalances.Count()}";
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
    }
}
