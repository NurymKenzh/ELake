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
    public class FishKindsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;

        public FishKindsController(ApplicationDbContext context,
            IHostingEnvironment hostingEnvironment,
            IStringLocalizer<SharedResources> sharedLocalizer)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _sharedLocalizer = sharedLocalizer;
        }

        // GET: FishKinds
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Index(string SortOrder,
            int? FishId,
            int? FamilyId,
            int? Page)
        {
            var fishKinds = _context.FishKind
                .Where(w => true);

            ViewBag.FishIdFilter = FishId;
            ViewBag.FamilyIdFilter = FamilyId;

            ViewBag.FishIdSort = SortOrder == "FishId" ? "FishIdDesc" : "FishId";
            ViewBag.FamilyIdSort = SortOrder == "FamilyId" ? "FamilyIdDesc" : "FamilyId";

            if (FishId != null)
            {
                fishKinds = fishKinds.Where(w => w.FishId == FishId);
            }
            if (FamilyId != null)
            {
                fishKinds = fishKinds.Where(w => w.FamilyId == FamilyId);
            }

            switch (SortOrder)
            {
                case "FishId":
                    fishKinds = fishKinds.OrderBy(w => w.FishId);
                    break;
                case "FishIdDesc":
                    fishKinds = fishKinds.OrderByDescending(w => w.FishId);
                    break;
                case "FamilyId":
                    fishKinds = fishKinds.OrderBy(w => w.FamilyId);
                    break;
                case "FamilyIdDesc":
                    fishKinds = fishKinds.OrderByDescending(w => w.FamilyId);
                    break;
                default:
                    fishKinds = fishKinds.OrderBy(w => w.Id);
                    break;
            }
            ViewBag.SortOrder = SortOrder;

            var pager = new Pager(fishKinds.Count(), Page);

            var viewModel = new FishKindIndexPageViewModel
            {
                Items = fishKinds.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };

            return View(viewModel);
        }

        // GET: FishKinds/Details/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fishKind = await _context.FishKind
                .SingleOrDefaultAsync(m => m.Id == id);
            if (fishKind == null)
            {
                return NotFound();
            }

            return View(fishKind);
        }

        // GET: FishKinds/Create
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: FishKinds/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Create([Bind("Id,FishId,FamilyId,FishNameKK,FishNameRU,FishNameLA,FamilyNameKK,FamilyNameRU,FamilyNameLA")] FishKind fishKind)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fishKind);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fishKind);
        }

        // GET: FishKinds/Edit/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fishKind = await _context.FishKind.SingleOrDefaultAsync(m => m.Id == id);
            if (fishKind == null)
            {
                return NotFound();
            }
            return View(fishKind);
        }

        // POST: FishKinds/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FishId,FamilyId,FishNameKK,FishNameRU,FishNameLA,FamilyNameKK,FamilyNameRU,FamilyNameLA")] FishKind fishKind)
        {
            if (id != fishKind.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fishKind);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FishKindExists(fishKind.Id))
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
            return View(fishKind);
        }

        // GET: FishKinds/Delete/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fishKind = await _context.FishKind
                .SingleOrDefaultAsync(m => m.Id == id);
            if (fishKind == null)
            {
                return NotFound();
            }

            return View(fishKind);
        }

        // POST: FishKinds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fishKind = await _context.FishKind.SingleOrDefaultAsync(m => m.Id == id);
            _context.FishKind.Remove(fishKind);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FishKindExists(int id)
        {
            return _context.FishKind.Any(e => e.Id == id);
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
                    List<FishKind> fishKinds = new List<FishKind>();
                    for (int i = start_row; ; i++)
                    {
                        if (package.Workbook.Worksheets.FirstOrDefault().Cells[i, 1].Value == null)
                        {
                            break;
                        }
                        FishKind fishKind = new FishKind();

                        try
                        {
                            fishKind.FishId = Convert.ToInt32(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 1].Value);
                            fishKind.FamilyId = Convert.ToInt32(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 2].Value);
                            fishKind.FishNameKK = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 3].Value.ToString();
                            fishKind.FishNameRU = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 4].Value.ToString();
                            fishKind.FishNameLA = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 5].Value.ToString();
                            fishKind.FamilyNameKK = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 6].Value.ToString();
                            fishKind.FamilyNameRU = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 7].Value.ToString();
                            fishKind.FamilyNameLA = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 8].Value.ToString();
                        }
                        catch (Exception e)
                        {
                            ViewBag.Error = $"{_sharedLocalizer["Row"]} {i.ToString()}: " + e.Message + (e.InnerException == null ? "" : ": " + e.InnerException.Message);
                            break;
                        }

                        fishKinds.Add(fishKind);
                        _context.Add(fishKinds.LastOrDefault());
                    }
                    if (string.IsNullOrEmpty(ViewBag.Error))
                    {
                        _context.SaveChanges();
                        ViewBag.Report = $"{_sharedLocalizer["UploadedCount"]}: {fishKinds.Count()}";
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
