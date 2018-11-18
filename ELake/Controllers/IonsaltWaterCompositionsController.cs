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
    public class IonsaltWaterCompositionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;

        public IonsaltWaterCompositionsController(ApplicationDbContext context,
            IHostingEnvironment hostingEnvironment,
            IStringLocalizer<SharedResources> sharedLocalizer)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _sharedLocalizer = sharedLocalizer;
        }

        // GET: IonsaltWaterCompositions
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Index(string SortOrder,
            int? LakeId,
            int? Year,
            int? Page)
        {
            var ionsaltWaterCompositions = _context.IonsaltWaterComposition
                .Where(w => true);

            ViewBag.LakeIdFilter = LakeId;
            ViewBag.YearFilter = Year;

            ViewBag.LakeIdSort = SortOrder == "LakeId" ? "LakeIdDesc" : "LakeId";
            ViewBag.YearSort = SortOrder == "Year" ? "YearDesc" : "Year";

            if (LakeId != null)
            {
                ionsaltWaterCompositions = ionsaltWaterCompositions.Where(w => w.LakeId == LakeId);
            }
            if (Year != null)
            {
                ionsaltWaterCompositions = ionsaltWaterCompositions.Where(w => w.Year == Year);
            }

            switch (SortOrder)
            {
                case "LakeId":
                    ionsaltWaterCompositions = ionsaltWaterCompositions.OrderBy(w => w.LakeId);
                    break;
                case "LakeIdDesc":
                    ionsaltWaterCompositions = ionsaltWaterCompositions.OrderByDescending(w => w.LakeId);
                    break;
                case "Year":
                    ionsaltWaterCompositions = ionsaltWaterCompositions.OrderBy(w => w.Year);
                    break;
                case "YearDesc":
                    ionsaltWaterCompositions = ionsaltWaterCompositions.OrderByDescending(w => w.Year);
                    break;
                default:
                    ionsaltWaterCompositions = ionsaltWaterCompositions.OrderBy(w => w.Id);
                    break;
            }
            ViewBag.SortOrder = SortOrder;

            var pager = new Pager(ionsaltWaterCompositions.Count(), Page);

            var viewModel = new IonsaltWaterCompositionIndexPageViewModel
            {
                Items = ionsaltWaterCompositions.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };

            return View(viewModel);
        }

        // GET: IonsaltWaterCompositions/Details/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ionsaltWaterComposition = await _context.IonsaltWaterComposition
                .SingleOrDefaultAsync(m => m.Id == id);
            if (ionsaltWaterComposition == null)
            {
                return NotFound();
            }

            return View(ionsaltWaterComposition);
        }

        // GET: IonsaltWaterCompositions/Create
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: IonsaltWaterCompositions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Create([Bind("Id,LakeId,Year,LakePart,CaMgEq,MgMgEq,NaKMgEq,ClMgEq,HCOMgEq,SOMgEq")] IonsaltWaterComposition ionsaltWaterComposition)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ionsaltWaterComposition);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ionsaltWaterComposition);
        }

        // GET: IonsaltWaterCompositions/Edit/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ionsaltWaterComposition = await _context.IonsaltWaterComposition.SingleOrDefaultAsync(m => m.Id == id);
            if (ionsaltWaterComposition == null)
            {
                return NotFound();
            }
            return View(ionsaltWaterComposition);
        }

        // POST: IonsaltWaterCompositions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LakeId,Year,LakePart,CaMgEq,MgMgEq,NaKMgEq,ClMgEq,HCOMgEq,SOMgEq")] IonsaltWaterComposition ionsaltWaterComposition)
        {
            if (id != ionsaltWaterComposition.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ionsaltWaterComposition);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IonsaltWaterCompositionExists(ionsaltWaterComposition.Id))
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
            return View(ionsaltWaterComposition);
        }

        // GET: IonsaltWaterCompositions/Delete/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ionsaltWaterComposition = await _context.IonsaltWaterComposition
                .SingleOrDefaultAsync(m => m.Id == id);
            if (ionsaltWaterComposition == null)
            {
                return NotFound();
            }

            return View(ionsaltWaterComposition);
        }

        // POST: IonsaltWaterCompositions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ionsaltWaterComposition = await _context.IonsaltWaterComposition.SingleOrDefaultAsync(m => m.Id == id);
            _context.IonsaltWaterComposition.Remove(ionsaltWaterComposition);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IonsaltWaterCompositionExists(int id)
        {
            return _context.IonsaltWaterComposition.Any(e => e.Id == id);
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
                    List<IonsaltWaterComposition> ionsaltWaterCompositions = new List<IonsaltWaterComposition>();
                    for (int i = start_row; ; i++)
                    {
                        if (package.Workbook.Worksheets.FirstOrDefault().Cells[i, 1].Value == null)
                        {
                            break;
                        }
                        IonsaltWaterComposition ionsaltWaterComposition = new IonsaltWaterComposition();

                        try
                        {
                            ionsaltWaterComposition.LakeId = Convert.ToInt32(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 1].Value);
                            ionsaltWaterComposition.Year = Convert.ToInt32(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 2].Value);
                            ionsaltWaterComposition.LakePart = (LakePart)Convert.ToInt32(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 3].Value);
                            ionsaltWaterComposition.CaMgEq = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 4].Value == null ?
                                (decimal?)null :
                                Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 4].Value);
                            ionsaltWaterComposition.MgMgEq = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 5].Value == null ?
                                (decimal?)null :
                                Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 5].Value);
                            ionsaltWaterComposition.NaKMgEq = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 6].Value == null ?
                                (decimal?)null :
                                Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 6].Value);
                            ionsaltWaterComposition.ClMgEq = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 7].Value == null ?
                                (decimal?)null :
                                Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 7].Value);
                            ionsaltWaterComposition.HCOMgEq = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 8].Value == null ?
                                (decimal?)null :
                                Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 8].Value);
                            ionsaltWaterComposition.SOMgEq = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 9].Value == null ?
                                (decimal?)null :
                                Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 9].Value);
                        }
                        catch (Exception e)
                        {
                            ViewBag.Error = $"{_sharedLocalizer["Row"]} {i.ToString()}: " + e.Message + (e.InnerException == null ? "" : ": " + e.InnerException.Message);
                            break;
                        }

                        ionsaltWaterCompositions.Add(ionsaltWaterComposition);
                        _context.Add(ionsaltWaterCompositions.LastOrDefault());
                    }
                    if (string.IsNullOrEmpty(ViewBag.Error))
                    {
                        _context.SaveChanges();
                        ViewBag.Report = $"{_sharedLocalizer["UploadedCount"]}: {ionsaltWaterCompositions.Count()}";
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
