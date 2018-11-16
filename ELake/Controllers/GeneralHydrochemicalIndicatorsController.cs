using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ELake.Data;
using ELake.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using OfficeOpenXml;

namespace ELake.Controllers
{
    public class GeneralHydrochemicalIndicatorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;

        public GeneralHydrochemicalIndicatorsController(ApplicationDbContext context,
            IHostingEnvironment hostingEnvironment,
            IStringLocalizer<SharedResources> sharedLocalizer)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _sharedLocalizer = sharedLocalizer;
        }

        // GET: GeneralHydrochemicalIndicators
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Index(string SortOrder,
            int? LakeId,
            int? Year,
            int? Page)
        {
            var generalHydrochemicalIndicators = _context.GeneralHydrochemicalIndicator
                .Where(w => true);

            ViewBag.LakeIdFilter = LakeId;
            ViewBag.YearFilter = Year;

            ViewBag.LakeIdSort = SortOrder == "LakeId" ? "LakeIdDesc" : "LakeId";
            ViewBag.YearSort = SortOrder == "Year" ? "YearDesc" : "Year";

            if (LakeId != null)
            {
                generalHydrochemicalIndicators = generalHydrochemicalIndicators.Where(w => w.LakeId == LakeId);
            }
            if (Year != null)
            {
                generalHydrochemicalIndicators = generalHydrochemicalIndicators.Where(w => w.Year == Year);
            }

            switch (SortOrder)
            {
                case "LakeId":
                    generalHydrochemicalIndicators = generalHydrochemicalIndicators.OrderBy(w => w.LakeId);
                    break;
                case "LakeIdDesc":
                    generalHydrochemicalIndicators = generalHydrochemicalIndicators.OrderByDescending(w => w.LakeId);
                    break;
                case "Year":
                    generalHydrochemicalIndicators = generalHydrochemicalIndicators.OrderBy(w => w.Year);
                    break;
                case "YearDesc":
                    generalHydrochemicalIndicators = generalHydrochemicalIndicators.OrderByDescending(w => w.Year);
                    break;
                default:
                    generalHydrochemicalIndicators = generalHydrochemicalIndicators.OrderBy(w => w.Id);
                    break;
            }
            ViewBag.SortOrder = SortOrder;

            var pager = new Pager(generalHydrochemicalIndicators.Count(), Page);

            var viewModel = new GeneralHydrochemicalIndicatorIndexPageViewModel
            {
                Items = generalHydrochemicalIndicators.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };

            return View(viewModel);
        }

        // GET: GeneralHydrochemicalIndicators/Details/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var generalHydrochemicalIndicator = await _context.GeneralHydrochemicalIndicator
                .SingleOrDefaultAsync(m => m.Id == id);
            if (generalHydrochemicalIndicator == null)
            {
                return NotFound();
            }

            return View(generalHydrochemicalIndicator);
        }

        // GET: GeneralHydrochemicalIndicators/Create
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: GeneralHydrochemicalIndicators/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Create([Bind("Id,LakeId,Year,Mineralization,TotalHardness,DissOxygWater,PercentOxygWater,pH,OrganicSubstances")] GeneralHydrochemicalIndicator generalHydrochemicalIndicator)
        {
            if (ModelState.IsValid)
            {
                _context.Add(generalHydrochemicalIndicator);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(generalHydrochemicalIndicator);
        }

        // GET: GeneralHydrochemicalIndicators/Edit/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var generalHydrochemicalIndicator = await _context.GeneralHydrochemicalIndicator.SingleOrDefaultAsync(m => m.Id == id);
            if (generalHydrochemicalIndicator == null)
            {
                return NotFound();
            }
            return View(generalHydrochemicalIndicator);
        }

        // POST: GeneralHydrochemicalIndicators/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LakeId,Year,Mineralization,TotalHardness,DissOxygWater,PercentOxygWater,pH,OrganicSubstances")] GeneralHydrochemicalIndicator generalHydrochemicalIndicator)
        {
            if (id != generalHydrochemicalIndicator.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(generalHydrochemicalIndicator);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GeneralHydrochemicalIndicatorExists(generalHydrochemicalIndicator.Id))
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
            return View(generalHydrochemicalIndicator);
        }

        // GET: GeneralHydrochemicalIndicators/Delete/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var generalHydrochemicalIndicator = await _context.GeneralHydrochemicalIndicator
                .SingleOrDefaultAsync(m => m.Id == id);
            if (generalHydrochemicalIndicator == null)
            {
                return NotFound();
            }

            return View(generalHydrochemicalIndicator);
        }

        // POST: GeneralHydrochemicalIndicators/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var generalHydrochemicalIndicator = await _context.GeneralHydrochemicalIndicator.SingleOrDefaultAsync(m => m.Id == id);
            _context.GeneralHydrochemicalIndicator.Remove(generalHydrochemicalIndicator);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GeneralHydrochemicalIndicatorExists(int id)
        {
            return _context.GeneralHydrochemicalIndicator.Any(e => e.Id == id);
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
                    List<GeneralHydrochemicalIndicator> generalHydrochemicalIndicators = new List<GeneralHydrochemicalIndicator>();
                    for (int i = start_row; ; i++)
                    {
                        if (package.Workbook.Worksheets.FirstOrDefault().Cells[i, 1].Value == null)
                        {
                            break;
                        }
                        GeneralHydrochemicalIndicator generalHydrochemicalIndicator = new GeneralHydrochemicalIndicator();

                        try
                        {
                            generalHydrochemicalIndicator.LakeId = Convert.ToInt32(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 1].Value);
                            generalHydrochemicalIndicator.Year = Convert.ToInt32(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 2].Value);
                            generalHydrochemicalIndicator.Mineralization = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 3].Value == null ?
                                (decimal?)null :
                                Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 3].Value);
                            generalHydrochemicalIndicator.TotalHardness = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 4].Value == null ?
                                (decimal?)null :
                                Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 4].Value);
                            generalHydrochemicalIndicator.DissOxygWater = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 5].Value == null ?
                                (decimal?)null :
                                Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 5].Value);
                            generalHydrochemicalIndicator.PercentOxygWater = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 6].Value == null ?
                                (decimal?)null :
                                Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 6].Value);
                            generalHydrochemicalIndicator.pH = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 7].Value == null ?
                                (decimal?)null :
                                Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 7].Value);
                            generalHydrochemicalIndicator.OrganicSubstances = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 8].Value == null ?
                                (decimal?)null :
                                Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 8].Value);
                        }
                        catch (Exception e)
                        {
                            ViewBag.Error = $"{_sharedLocalizer["Row"]} {i.ToString()}: " + e.Message + (e.InnerException == null ? "" : ": " + e.InnerException.Message);
                            break;
                        }

                        generalHydrochemicalIndicators.Add(generalHydrochemicalIndicator);
                        _context.Add(generalHydrochemicalIndicators.LastOrDefault());
                    }
                    if (string.IsNullOrEmpty(ViewBag.Error))
                    {
                        _context.SaveChanges();
                        ViewBag.Report = $"{_sharedLocalizer["UploadedCount"]}: {generalHydrochemicalIndicators.Count()}";
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
