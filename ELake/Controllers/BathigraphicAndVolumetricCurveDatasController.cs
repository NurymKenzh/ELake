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
    public class BathigraphicAndVolumetricCurveDatasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;

        public BathigraphicAndVolumetricCurveDatasController(ApplicationDbContext context,
            IHostingEnvironment hostingEnvironment,
            IStringLocalizer<SharedResources> sharedLocalizer)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _sharedLocalizer = sharedLocalizer;
        }

        // GET: BathigraphicAndVolumetricCurveDatas
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Index(string SortOrder,
            int? LakeId,
            int? Page)
        {
            var bathigraphicAndVolumetricCurveDatas = _context.BathigraphicAndVolumetricCurveData
                .Where(w => true);

            ViewBag.LakeIdFilter = LakeId;

            ViewBag.LakeIdSort = SortOrder == "LakeId" ? "LakeIdDesc" : "LakeId";

            if (LakeId != null)
            {
                bathigraphicAndVolumetricCurveDatas = bathigraphicAndVolumetricCurveDatas.Where(w => w.LakeId == LakeId);
            }

            switch (SortOrder)
            {
                case "LakeId":
                    bathigraphicAndVolumetricCurveDatas = bathigraphicAndVolumetricCurveDatas.OrderBy(w => w.LakeId);
                    break;
                case "LakeIdDesc":
                    bathigraphicAndVolumetricCurveDatas = bathigraphicAndVolumetricCurveDatas.OrderByDescending(w => w.LakeId);
                    break;
                default:
                    bathigraphicAndVolumetricCurveDatas = bathigraphicAndVolumetricCurveDatas.OrderBy(w => w.Id);
                    break;
            }
            ViewBag.SortOrder = SortOrder;

            var pager = new Pager(bathigraphicAndVolumetricCurveDatas.Count(), Page);

            var viewModel = new BathigraphicAndVolumetricCurveDataIndexPageViewModel
            {
                Items = bathigraphicAndVolumetricCurveDatas.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };

            return View(viewModel);
        }

        // GET: BathigraphicAndVolumetricCurveDatas/Details/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bathigraphicAndVolumetricCurveData = await _context.BathigraphicAndVolumetricCurveData
                .SingleOrDefaultAsync(m => m.Id == id);
            if (bathigraphicAndVolumetricCurveData == null)
            {
                return NotFound();
            }

            return View(bathigraphicAndVolumetricCurveData);
        }

        // GET: BathigraphicAndVolumetricCurveDatas/Create
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: BathigraphicAndVolumetricCurveDatas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Create([Bind("Id,LakeId,WaterLevel,LakeArea,WaterMassVolume")] BathigraphicAndVolumetricCurveData bathigraphicAndVolumetricCurveData)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bathigraphicAndVolumetricCurveData);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bathigraphicAndVolumetricCurveData);
        }

        // GET: BathigraphicAndVolumetricCurveDatas/Edit/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bathigraphicAndVolumetricCurveData = await _context.BathigraphicAndVolumetricCurveData.SingleOrDefaultAsync(m => m.Id == id);
            if (bathigraphicAndVolumetricCurveData == null)
            {
                return NotFound();
            }
            return View(bathigraphicAndVolumetricCurveData);
        }

        // POST: BathigraphicAndVolumetricCurveDatas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LakeId,WaterLevel,LakeArea,WaterMassVolume")] BathigraphicAndVolumetricCurveData bathigraphicAndVolumetricCurveData)
        {
            if (id != bathigraphicAndVolumetricCurveData.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bathigraphicAndVolumetricCurveData);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BathigraphicAndVolumetricCurveDataExists(bathigraphicAndVolumetricCurveData.Id))
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
            return View(bathigraphicAndVolumetricCurveData);
        }

        // GET: BathigraphicAndVolumetricCurveDatas/Delete/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bathigraphicAndVolumetricCurveData = await _context.BathigraphicAndVolumetricCurveData
                .SingleOrDefaultAsync(m => m.Id == id);
            if (bathigraphicAndVolumetricCurveData == null)
            {
                return NotFound();
            }

            return View(bathigraphicAndVolumetricCurveData);
        }

        // POST: BathigraphicAndVolumetricCurveDatas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bathigraphicAndVolumetricCurveData = await _context.BathigraphicAndVolumetricCurveData.SingleOrDefaultAsync(m => m.Id == id);
            _context.BathigraphicAndVolumetricCurveData.Remove(bathigraphicAndVolumetricCurveData);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BathigraphicAndVolumetricCurveDataExists(int id)
        {
            return _context.BathigraphicAndVolumetricCurveData.Any(e => e.Id == id);
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
                    List<BathigraphicAndVolumetricCurveData> bathigraphicAndVolumetricCurveDatas = new List<BathigraphicAndVolumetricCurveData>();
                    for (int i = start_row; ; i++)
                    {
                        if (package.Workbook.Worksheets.FirstOrDefault().Cells[i, 1].Value == null)
                        {
                            break;
                        }
                        BathigraphicAndVolumetricCurveData bathigraphicAndVolumetricCurveData = new BathigraphicAndVolumetricCurveData();

                        try
                        {
                            bathigraphicAndVolumetricCurveData.LakeId = Convert.ToInt32(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 1].Value);
                            bathigraphicAndVolumetricCurveData.WaterLevel = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 2].Value == null ?
                                (decimal?)null :
                                Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 2].Value);
                            bathigraphicAndVolumetricCurveData.LakeArea = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 3].Value == null ?
                                (decimal?)null :
                                Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 3].Value);
                            bathigraphicAndVolumetricCurveData.WaterMassVolume = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 4].Value == null ?
                                (decimal?)null :
                                Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 4].Value);
                        }
                        catch (Exception e)
                        {
                            ViewBag.Error = $"{_sharedLocalizer["Row"]} {i.ToString()}: " + e.Message + (e.InnerException == null ? "" : ": " + e.InnerException.Message);
                            break;
                        }

                        bathigraphicAndVolumetricCurveDatas.Add(bathigraphicAndVolumetricCurveData);
                        _context.Add(bathigraphicAndVolumetricCurveDatas.LastOrDefault());
                    }
                    if (string.IsNullOrEmpty(ViewBag.Error))
                    {
                        _context.SaveChanges();
                        ViewBag.Report = $"{_sharedLocalizer["UploadedCount"]}: {bathigraphicAndVolumetricCurveDatas.Count()}";
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
