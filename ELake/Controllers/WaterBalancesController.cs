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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using OfficeOpenXml;
using Microsoft.Extensions.Localization;

namespace ELake.Controllers
{
    public class WaterBalancesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;

        public WaterBalancesController(ApplicationDbContext context,
            IHostingEnvironment hostingEnvironment,
            IStringLocalizer<SharedResources> sharedLocalizer)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _sharedLocalizer = sharedLocalizer;
        }

        // GET: WaterBalances
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Index(string SortOrder,
            int? LakeId,
            int? Year,
            int? Page)
        {
            var waterBalances = _context.WaterBalance
                .Where(w => true);

            ViewBag.LakeIdFilter = LakeId;
            ViewBag.YearFilter = Year;

            ViewBag.LakeIdSort = SortOrder == "LakeId" ? "LakeIdDesc" : "LakeId";
            ViewBag.YearSort = SortOrder == "Year" ? "YearDesc" : "Year";

            if (LakeId != null)
            {
                waterBalances = waterBalances.Where(w => w.LakeId == LakeId);
            }
            if (Year != null)
            {
                waterBalances = waterBalances.Where(w => w.Year == Year);
            }

            switch (SortOrder)
            {
                case "LakeId":
                    waterBalances = waterBalances.OrderBy(w => w.LakeId);
                    break;
                case "LakeIdDesc":
                    waterBalances = waterBalances.OrderByDescending(w => w.LakeId);
                    break;
                case "Year":
                    waterBalances = waterBalances.OrderBy(w => w.Year);
                    break;
                case "YearDesc":
                    waterBalances = waterBalances.OrderByDescending(w => w.Year);
                    break;
                default:
                    waterBalances = waterBalances.OrderBy(w => w.Id);
                    break;
            }
            ViewBag.SortOrder = SortOrder;

            var pager = new Pager(waterBalances.Count(), Page);

            var viewModel = new WaterBalanceIndexPageViewModel
            {
                Items = waterBalances.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };

            return View(viewModel);
        }

        // GET: WaterBalances/Details/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var waterBalance = await _context.WaterBalance
                .SingleOrDefaultAsync(m => m.Id == id);
            if (waterBalance == null)
            {
                return NotFound();
            }

            return View(waterBalance);
        }

        // GET: WaterBalances/Create
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: WaterBalances/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Create([Bind("Id,LakeId,Year,SurfaceFlow,SurfaceOutflow,UndergroundFlow,UndergroundOutflow,Precipitation,Evaporation")] WaterBalance waterBalance)
        {
            if (ModelState.IsValid)
            {
                _context.Add(waterBalance);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(waterBalance);
        }

        // GET: WaterBalances/Edit/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var waterBalance = await _context.WaterBalance.SingleOrDefaultAsync(m => m.Id == id);
            if (waterBalance == null)
            {
                return NotFound();
            }
            return View(waterBalance);
        }

        // POST: WaterBalances/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LakeId,Year,SurfaceFlow,SurfaceOutflow,UndergroundFlow,UndergroundOutflow,Precipitation,Evaporation")] WaterBalance waterBalance)
        {
            if (id != waterBalance.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(waterBalance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WaterBalanceExists(waterBalance.Id))
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
            return View(waterBalance);
        }

        // GET: WaterBalances/Delete/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var waterBalance = await _context.WaterBalance
                .SingleOrDefaultAsync(m => m.Id == id);
            if (waterBalance == null)
            {
                return NotFound();
            }

            return View(waterBalance);
        }

        // POST: WaterBalances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var waterBalance = await _context.WaterBalance.SingleOrDefaultAsync(m => m.Id == id);
            _context.WaterBalance.Remove(waterBalance);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WaterBalanceExists(int id)
        {
            return _context.WaterBalance.Any(e => e.Id == id);
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
                    List<WaterBalance> waterBalances = new List<WaterBalance>();
                    for (int i = start_row; ; i++)
                    {
                        if (package.Workbook.Worksheets.FirstOrDefault().Cells[i, 1].Value == null)
                        {
                            break;
                        }
                        WaterBalance waterBalance = new WaterBalance();

                        try
                        {
                            waterBalance.LakeId = Convert.ToInt32(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 1].Value);
                            waterBalance.Year = Convert.ToInt32(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 2].Value);
                            waterBalance.SurfaceFlow = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 3].Value == null ?
                                (decimal?)null :
                                Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 3].Value);
                            waterBalance.SurfaceOutflow = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 4].Value == null ?
                                (decimal?)null :
                                Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 4].Value);
                            waterBalance.UndergroundFlow = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 5].Value == null ?
                                (decimal?)null :
                                Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 5].Value);
                            waterBalance.UndergroundOutflow = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 6].Value == null ?
                                (decimal?)null :
                                Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 6].Value);
                            waterBalance.Precipitation = Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 7].Value);
                            waterBalance.Evaporation = Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 8].Value);
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
