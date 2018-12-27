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
using System.IO;
using OfficeOpenXml;
using Microsoft.AspNetCore.Http;

namespace ELake.Controllers
{
    public class DynamicsLakeAreasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;

        public DynamicsLakeAreasController(ApplicationDbContext context,
            IHostingEnvironment hostingEnvironment,
            IStringLocalizer<SharedResources> sharedLocalizer)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _sharedLocalizer = sharedLocalizer;
        }

        // GET: DynamicsLakeAreas
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Index(string SortOrder,
            int? LakeId,
            int? Year,
            int? Page)
        {
            var dynamicsLakeAreas = _context.DynamicsLakeArea
                .Where(w => true);

            ViewBag.LakeIdFilter = LakeId;
            ViewBag.YearFilter = Year;

            ViewBag.LakeIdSort = SortOrder == "LakeId" ? "LakeIdDesc" : "LakeId";
            ViewBag.YearSort = SortOrder == "Year" ? "YearDesc" : "Year";

            if (LakeId != null)
            {
                dynamicsLakeAreas = dynamicsLakeAreas.Where(w => w.LakeId == LakeId);
            }
            if (Year != null)
            {
                dynamicsLakeAreas = dynamicsLakeAreas.Where(w => w.Year == Year);
            }

            switch (SortOrder)
            {
                case "LakeId":
                    dynamicsLakeAreas = dynamicsLakeAreas.OrderBy(w => w.LakeId);
                    break;
                case "LakeIdDesc":
                    dynamicsLakeAreas = dynamicsLakeAreas.OrderByDescending(w => w.LakeId);
                    break;
                case "Year":
                    dynamicsLakeAreas = dynamicsLakeAreas.OrderBy(w => w.Year);
                    break;
                case "YearDesc":
                    dynamicsLakeAreas = dynamicsLakeAreas.OrderByDescending(w => w.Year);
                    break;
                default:
                    dynamicsLakeAreas = dynamicsLakeAreas.OrderBy(w => w.Id);
                    break;
            }
            ViewBag.SortOrder = SortOrder;

            var pager = new Pager(dynamicsLakeAreas.Count(), Page);

            var viewModel = new DynamicsLakeAreaIndexPageViewModel
            {
                Items = dynamicsLakeAreas.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };

            return View(viewModel);
        }

        // GET: DynamicsLakeAreas/Details/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dynamicsLakeArea = await _context.DynamicsLakeArea
                .SingleOrDefaultAsync(m => m.Id == id);
            if (dynamicsLakeArea == null)
            {
                return NotFound();
            }

            return View(dynamicsLakeArea);
        }

        // GET: DynamicsLakeAreas/Create
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: DynamicsLakeAreas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Create([Bind("Id,LakeId,Year,NoDataPers,NotWater,SeasonalWaterArea,PermanentWaterArea")] DynamicsLakeArea dynamicsLakeArea)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dynamicsLakeArea);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dynamicsLakeArea);
        }

        // GET: DynamicsLakeAreas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dynamicsLakeArea = await _context.DynamicsLakeArea.SingleOrDefaultAsync(m => m.Id == id);
            if (dynamicsLakeArea == null)
            {
                return NotFound();
            }
            return View(dynamicsLakeArea);
        }

        // POST: DynamicsLakeAreas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LakeId,Year,NoDataPers,NotWater,SeasonalWaterArea,PermanentWaterArea")] DynamicsLakeArea dynamicsLakeArea)
        {
            if (id != dynamicsLakeArea.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dynamicsLakeArea);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DynamicsLakeAreaExists(dynamicsLakeArea.Id))
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
            return View(dynamicsLakeArea);
        }

        // GET: DynamicsLakeAreas/Delete/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dynamicsLakeArea = await _context.DynamicsLakeArea
                .SingleOrDefaultAsync(m => m.Id == id);
            if (dynamicsLakeArea == null)
            {
                return NotFound();
            }

            return View(dynamicsLakeArea);
        }

        // POST: DynamicsLakeAreas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dynamicsLakeArea = await _context.DynamicsLakeArea.SingleOrDefaultAsync(m => m.Id == id);
            _context.DynamicsLakeArea.Remove(dynamicsLakeArea);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DynamicsLakeAreaExists(int id)
        {
            return _context.DynamicsLakeArea.Any(e => e.Id == id);
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
                    List<DynamicsLakeArea> dynamicsLakeAreas = new List<DynamicsLakeArea>();
                    for (int i = start_row; ; i++)
                    {
                        if (package.Workbook.Worksheets.FirstOrDefault().Cells[i, 1].Value == null)
                        {
                            break;
                        }
                        DynamicsLakeArea dynamicsLakeArea = new DynamicsLakeArea();

                        try
                        {
                            dynamicsLakeArea.LakeId = Convert.ToInt32(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 1].Value);
                            dynamicsLakeArea.Year = Convert.ToInt32(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 2].Value);
                            dynamicsLakeArea.NoDataPers = Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 3].Value);
                            dynamicsLakeArea.NotWater = Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 4].Value);
                            dynamicsLakeArea.SeasonalWaterArea = Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 5].Value);
                            dynamicsLakeArea.PermanentWaterArea = Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 6].Value);
                        }
                        catch (Exception e)
                        {
                            ViewBag.Error = $"{_sharedLocalizer["Row"]} {i.ToString()}: " + e.Message + (e.InnerException == null ? "" : ": " + e.InnerException.Message);
                            break;
                        }

                        dynamicsLakeAreas.Add(dynamicsLakeArea);
                        _context.Add(dynamicsLakeAreas.LastOrDefault());
                    }
                    if (string.IsNullOrEmpty(ViewBag.Error))
                    {
                        _context.SaveChanges();
                        ViewBag.Report = $"{_sharedLocalizer["UploadedCount"]}: {dynamicsLakeAreas.Count()}";
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
