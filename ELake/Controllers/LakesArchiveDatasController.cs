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
    public class LakesArchiveDatasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;

        public LakesArchiveDatasController(ApplicationDbContext context,
            IHostingEnvironment hostingEnvironment,
            IStringLocalizer<SharedResources> sharedLocalizer)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _sharedLocalizer = sharedLocalizer;
        }

        // GET: LakesArchiveDatas
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Index(string SortOrder,
            int? LakeId,
            int? SurveyYear,
            int? Page)
        {
            var lakesArchiveDatas = _context.LakesArchiveData
                .Where(w => true);

            ViewBag.LakeIdFilter = LakeId;
            ViewBag.SurveyYearFilter = SurveyYear;

            ViewBag.LakeIdSort = SortOrder == "LakeId" ? "LakeIdDesc" : "LakeId";
            ViewBag.SurveyYearSort = SortOrder == "SurveyYear" ? "SurveyYearDesc" : "SurveyYear";

            if (LakeId != null)
            {
                lakesArchiveDatas = lakesArchiveDatas.Where(w => w.LakeId == LakeId);
            }
            if (SurveyYear != null)
            {
                lakesArchiveDatas = lakesArchiveDatas.Where(w => w.SurveyYear == SurveyYear);
            }

            switch (SortOrder)
            {
                case "LakeId":
                    lakesArchiveDatas = lakesArchiveDatas.OrderBy(w => w.LakeId);
                    break;
                case "LakeIdDesc":
                    lakesArchiveDatas = lakesArchiveDatas.OrderByDescending(w => w.LakeId);
                    break;
                case "SurveyYear":
                    lakesArchiveDatas = lakesArchiveDatas.OrderBy(w => w.SurveyYear);
                    break;
                case "SurveyYearDesc":
                    lakesArchiveDatas = lakesArchiveDatas.OrderByDescending(w => w.SurveyYear);
                    break;
                default:
                    lakesArchiveDatas = lakesArchiveDatas.OrderBy(w => w.Id);
                    break;
            }
            ViewBag.SortOrder = SortOrder;

            var pager = new Pager(lakesArchiveDatas.Count(), Page);

            var viewModel = new LakesArchiveDataIndexPageViewModel
            {
                Items = lakesArchiveDatas.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };

            return View(viewModel);
        }

        // GET: LakesArchiveDatas/Details/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lakesArchiveData = await _context.LakesArchiveData
                .SingleOrDefaultAsync(m => m.Id == id);
            if (lakesArchiveData == null)
            {
                return NotFound();
            }

            return View(lakesArchiveData);
        }

        // GET: LakesArchiveDatas/Create
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: LakesArchiveDatas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Create([Bind("Id,LakeId,SurveyYear,LakeLength,LakeShorelineLength,LakeMirrorArea,LakeAbsoluteHeight,LakeWidth,LakeMaxDepth,LakeWaterMass,ArchivalInfoSource")] LakesArchiveData lakesArchiveData)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lakesArchiveData);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lakesArchiveData);
        }

        // GET: LakesArchiveDatas/Edit/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lakesArchiveData = await _context.LakesArchiveData.SingleOrDefaultAsync(m => m.Id == id);
            if (lakesArchiveData == null)
            {
                return NotFound();
            }
            return View(lakesArchiveData);
        }

        // POST: LakesArchiveDatas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LakeId,SurveyYear,LakeLength,LakeShorelineLength,LakeMirrorArea,LakeAbsoluteHeight,LakeWidth,LakeMaxDepth,LakeWaterMass,ArchivalInfoSource")] LakesArchiveData lakesArchiveData)
        {
            if (id != lakesArchiveData.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lakesArchiveData);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LakesArchiveDataExists(lakesArchiveData.Id))
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
            return View(lakesArchiveData);
        }

        // GET: LakesArchiveDatas/Delete/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lakesArchiveData = await _context.LakesArchiveData
                .SingleOrDefaultAsync(m => m.Id == id);
            if (lakesArchiveData == null)
            {
                return NotFound();
            }

            return View(lakesArchiveData);
        }

        // POST: LakesArchiveDatas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lakesArchiveData = await _context.LakesArchiveData.SingleOrDefaultAsync(m => m.Id == id);
            _context.LakesArchiveData.Remove(lakesArchiveData);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LakesArchiveDataExists(int id)
        {
            return _context.LakesArchiveData.Any(e => e.Id == id);
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
                    List<LakesArchiveData> lakesArchiveDatas = new List<LakesArchiveData>();
                    for (int i = start_row; ; i++)
                    {
                        if (package.Workbook.Worksheets.FirstOrDefault().Cells[i, 1].Value == null)
                        {
                            break;
                        }
                        LakesArchiveData lakesArchiveData = new LakesArchiveData();

                        try
                        {
                            lakesArchiveData.LakeId = Convert.ToInt32(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 1].Value);
                            lakesArchiveData.SurveyYear = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 2].Value == null ?
                                (int?)null :
                                Convert.ToInt32(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 2].Value);
                            lakesArchiveData.LakeLength = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 3].Value == null ?
                                (decimal?)null :
                                Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 3].Value);
                            lakesArchiveData.LakeShorelineLength = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 4].Value == null ?
                                (decimal?)null :
                                Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 4].Value);
                            lakesArchiveData.LakeMirrorArea = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 5].Value == null ?
                                (decimal?)null :
                                Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 5].Value);
                            lakesArchiveData.LakeAbsoluteHeight = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 6].Value == null ?
                                (decimal?)null :
                                Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 6].Value);
                            lakesArchiveData.LakeWidth = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 7].Value == null ?
                                (decimal?)null :
                                Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 7].Value);
                            lakesArchiveData.LakeMaxDepth = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 8].Value == null ?
                                (decimal?)null :
                                Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 8].Value);
                            lakesArchiveData.LakeWaterMass = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 9].Value == null ?
                                (decimal?)null :
                                Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 9].Value);
                            lakesArchiveData.ArchivalInfoSource = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 10].Value?.ToString();
                        }
                        catch (Exception e)
                        {
                            ViewBag.Error = $"{_sharedLocalizer["Row"]} {i.ToString()}: " + e.Message + (e.InnerException == null ? "" : ": " + e.InnerException.Message);
                            break;
                        }

                        lakesArchiveDatas.Add(lakesArchiveData);
                        _context.Add(lakesArchiveDatas.LastOrDefault());
                    }
                    if (string.IsNullOrEmpty(ViewBag.Error))
                    {
                        _context.SaveChanges();
                        ViewBag.Report = $"{_sharedLocalizer["UploadedCount"]}: {lakesArchiveDatas.Count()}";
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
