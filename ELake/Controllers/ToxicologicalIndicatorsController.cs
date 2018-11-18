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
    public class ToxicologicalIndicatorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;

        public ToxicologicalIndicatorsController(ApplicationDbContext context,
            IHostingEnvironment hostingEnvironment,
            IStringLocalizer<SharedResources> sharedLocalizer)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _sharedLocalizer = sharedLocalizer;
        }

        // GET: ToxicologicalIndicators
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Index(string SortOrder,
            int? LakeId,
            int? Year,
            int? Page)
        {
            var toxicologicalIndicators = _context.ToxicologicalIndicator
                .Where(w => true);

            ViewBag.LakeIdFilter = LakeId;
            ViewBag.YearFilter = Year;

            ViewBag.LakeIdSort = SortOrder == "LakeId" ? "LakeIdDesc" : "LakeId";
            ViewBag.YearSort = SortOrder == "Year" ? "YearDesc" : "Year";

            if (LakeId != null)
            {
                toxicologicalIndicators = toxicologicalIndicators.Where(w => w.LakeId == LakeId);
            }
            if (Year != null)
            {
                toxicologicalIndicators = toxicologicalIndicators.Where(w => w.Year == Year);
            }

            switch (SortOrder)
            {
                case "LakeId":
                    toxicologicalIndicators = toxicologicalIndicators.OrderBy(w => w.LakeId);
                    break;
                case "LakeIdDesc":
                    toxicologicalIndicators = toxicologicalIndicators.OrderByDescending(w => w.LakeId);
                    break;
                case "Year":
                    toxicologicalIndicators = toxicologicalIndicators.OrderBy(w => w.Year);
                    break;
                case "YearDesc":
                    toxicologicalIndicators = toxicologicalIndicators.OrderByDescending(w => w.Year);
                    break;
                default:
                    toxicologicalIndicators = toxicologicalIndicators.OrderBy(w => w.Id);
                    break;
            }
            ViewBag.SortOrder = SortOrder;

            var pager = new Pager(toxicologicalIndicators.Count(), Page);

            var viewModel = new ToxicologicalIndicatorIndexPageViewModel
            {
                Items = toxicologicalIndicators.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };

            return View(viewModel);
        }

        // GET: ToxicologicalIndicators/Details/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var toxicologicalIndicator = await _context.ToxicologicalIndicator
                .SingleOrDefaultAsync(m => m.Id == id);
            if (toxicologicalIndicator == null)
            {
                return NotFound();
            }

            return View(toxicologicalIndicator);
        }

        // GET: ToxicologicalIndicators/Create
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: ToxicologicalIndicators/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Create([Bind("Id,LakeId,Year,NH4,NO2,NO3,PPO4,Cu,Zn,Mn,Pb,Ni,Cd,Co")] ToxicologicalIndicator toxicologicalIndicator)
        {
            if (ModelState.IsValid)
            {
                _context.Add(toxicologicalIndicator);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(toxicologicalIndicator);
        }

        // GET: ToxicologicalIndicators/Edit/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var toxicologicalIndicator = await _context.ToxicologicalIndicator.SingleOrDefaultAsync(m => m.Id == id);
            if (toxicologicalIndicator == null)
            {
                return NotFound();
            }
            return View(toxicologicalIndicator);
        }

        // POST: ToxicologicalIndicators/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LakeId,Year,NH4,NO2,NO3,PPO4,Cu,Zn,Mn,Pb,Ni,Cd,Co")] ToxicologicalIndicator toxicologicalIndicator)
        {
            if (id != toxicologicalIndicator.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(toxicologicalIndicator);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ToxicologicalIndicatorExists(toxicologicalIndicator.Id))
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
            return View(toxicologicalIndicator);
        }

        // GET: ToxicologicalIndicators/Delete/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var toxicologicalIndicator = await _context.ToxicologicalIndicator
                .SingleOrDefaultAsync(m => m.Id == id);
            if (toxicologicalIndicator == null)
            {
                return NotFound();
            }

            return View(toxicologicalIndicator);
        }

        // POST: ToxicologicalIndicators/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var toxicologicalIndicator = await _context.ToxicologicalIndicator.SingleOrDefaultAsync(m => m.Id == id);
            _context.ToxicologicalIndicator.Remove(toxicologicalIndicator);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ToxicologicalIndicatorExists(int id)
        {
            return _context.ToxicologicalIndicator.Any(e => e.Id == id);
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
                    List<ToxicologicalIndicator> toxicologicalIndicators = new List<ToxicologicalIndicator>();
                    for (int i = start_row; ; i++)
                    {
                        if (package.Workbook.Worksheets.FirstOrDefault().Cells[i, 1].Value == null)
                        {
                            break;
                        }
                        ToxicologicalIndicator toxicologicalIndicator = new ToxicologicalIndicator();

                        try
                        {
                            toxicologicalIndicator.LakeId = Convert.ToInt32(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 1].Value);
                            toxicologicalIndicator.Year = Convert.ToInt32(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 2].Value);
                            toxicologicalIndicator.NH4 = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 3].Value == null ?
                                (decimal?)null :
                                Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 3].Value);
                            toxicologicalIndicator.NO2 = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 4].Value == null ?
                                (decimal?)null :
                                Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 4].Value);
                            toxicologicalIndicator.NO3 = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 5].Value == null ?
                                (decimal?)null :
                                Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 5].Value);
                            toxicologicalIndicator.PPO4 = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 6].Value == null ?
                                (decimal?)null :
                                Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 6].Value);
                            toxicologicalIndicator.Cu = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 7].Value == null ?
                                (decimal?)null :
                                Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 7].Value);
                            toxicologicalIndicator.Zn = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 8].Value == null ?
                                (decimal?)null :
                                Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 8].Value);
                            toxicologicalIndicator.Mn = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 9].Value == null ?
                                (decimal?)null :
                                Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 9].Value);
                            toxicologicalIndicator.Pb = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 10].Value == null ?
                                (decimal?)null :
                                Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 10].Value);
                            toxicologicalIndicator.Ni = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 11].Value == null ?
                                (decimal?)null :
                                Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 11].Value);
                            toxicologicalIndicator.Cd = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 12].Value == null ?
                                (decimal?)null :
                                Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 12].Value);
                            toxicologicalIndicator.Co = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 13].Value == null ?
                                (decimal?)null :
                                Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 13].Value);
                        }
                        catch (Exception e)
                        {
                            ViewBag.Error = $"{_sharedLocalizer["Row"]} {i.ToString()}: " + e.Message + (e.InnerException == null ? "" : ": " + e.InnerException.Message);
                            break;
                        }

                        toxicologicalIndicators.Add(toxicologicalIndicator);
                        _context.Add(toxicologicalIndicators.LastOrDefault());
                    }
                    if (string.IsNullOrEmpty(ViewBag.Error))
                    {
                        _context.SaveChanges();
                        ViewBag.Report = $"{_sharedLocalizer["UploadedCount"]}: {toxicologicalIndicators.Count()}";
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
