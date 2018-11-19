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
    public class LakesGlobalDatasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;

        public LakesGlobalDatasController(ApplicationDbContext context,
            IHostingEnvironment hostingEnvironment,
            IStringLocalizer<SharedResources> sharedLocalizer)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _sharedLocalizer = sharedLocalizer;
        }

        // GET: LakesGlobalDatas
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Index(string SortOrder,
            int? LakeId,
            int? Page)
        {
            var lakesGlobalDatas = _context.LakesGlobalData
                .Where(w => true);

            ViewBag.LakeIdFilter = LakeId;

            ViewBag.LakeIdSort = SortOrder == "LakeId" ? "LakeIdDesc" : "LakeId";

            if (LakeId != null)
            {
                lakesGlobalDatas = lakesGlobalDatas.Where(w => w.LakeId == LakeId);
            }

            switch (SortOrder)
            {
                case "LakeId":
                    lakesGlobalDatas = lakesGlobalDatas.OrderBy(w => w.LakeId);
                    break;
                case "LakeIdDesc":
                    lakesGlobalDatas = lakesGlobalDatas.OrderByDescending(w => w.LakeId);
                    break;
                default:
                    lakesGlobalDatas = lakesGlobalDatas.OrderBy(w => w.Id);
                    break;
            }
            ViewBag.SortOrder = SortOrder;

            var pager = new Pager(lakesGlobalDatas.Count(), Page);

            var viewModel = new LakesGlobalDataIndexPageViewModel
            {
                Items = lakesGlobalDatas.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };

            return View(viewModel);
        }

        // GET: LakesGlobalDatas/Details/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lakesGlobalData = await _context.LakesGlobalData
                .SingleOrDefaultAsync(m => m.Id == id);
            if (lakesGlobalData == null)
            {
                return NotFound();
            }

            return View(lakesGlobalData);
        }

        // GET: LakesGlobalDatas/Create
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: LakesGlobalDatas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Create([Bind("Id,LakeId,Hylak_id,Lake_name_ENG,Lake_name_RU,Lake_name_KZ,Country_ENG,Country_RU,Country_KZ,Continent_ENG,Continent_RU,Continent_KZ,Lake_area,Shore_len,Shore_dev,Vol_total,Depth_avg,Dis_avg,Res_time,Elevation,Slope_100,Wshd_area,Pour_long,Pour_lat")] LakesGlobalData lakesGlobalData)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lakesGlobalData);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lakesGlobalData);
        }

        // GET: LakesGlobalDatas/Edit/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lakesGlobalData = await _context.LakesGlobalData.SingleOrDefaultAsync(m => m.Id == id);
            if (lakesGlobalData == null)
            {
                return NotFound();
            }
            return View(lakesGlobalData);
        }

        // POST: LakesGlobalDatas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LakeId,Hylak_id,Lake_name_ENG,Lake_name_RU,Lake_name_KZ,Country_ENG,Country_RU,Country_KZ,Continent_ENG,Continent_RU,Continent_KZ,Lake_area,Shore_len,Shore_dev,Vol_total,Depth_avg,Dis_avg,Res_time,Elevation,Slope_100,Wshd_area,Pour_long,Pour_lat")] LakesGlobalData lakesGlobalData)
        {
            if (id != lakesGlobalData.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lakesGlobalData);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LakesGlobalDataExists(lakesGlobalData.Id))
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
            return View(lakesGlobalData);
        }

        // GET: LakesGlobalDatas/Delete/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lakesGlobalData = await _context.LakesGlobalData
                .SingleOrDefaultAsync(m => m.Id == id);
            if (lakesGlobalData == null)
            {
                return NotFound();
            }

            return View(lakesGlobalData);
        }

        // POST: LakesGlobalDatas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lakesGlobalData = await _context.LakesGlobalData.SingleOrDefaultAsync(m => m.Id == id);
            _context.LakesGlobalData.Remove(lakesGlobalData);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LakesGlobalDataExists(int id)
        {
            return _context.LakesGlobalData.Any(e => e.Id == id);
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
                    List<LakesGlobalData> lakesGlobalDatas = new List<LakesGlobalData>();
                    for (int i = start_row; ; i++)
                    {
                        if (package.Workbook.Worksheets.FirstOrDefault().Cells[i, 1].Value == null)
                        {
                            break;
                        }
                        LakesGlobalData lakesGlobalData = new LakesGlobalData();

                        try
                        {
                            lakesGlobalData.LakeId = Convert.ToInt32(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 1].Value);
                            lakesGlobalData.Hylak_id = Convert.ToInt32(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 2].Value);
                            lakesGlobalData.Lake_name_ENG = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 3].Value?.ToString();
                            lakesGlobalData.Lake_name_RU = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 4].Value?.ToString();
                            lakesGlobalData.Lake_name_KZ = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 5].Value?.ToString();
                            lakesGlobalData.Country_ENG = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 6].Value?.ToString();
                            lakesGlobalData.Country_RU = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 7].Value?.ToString();
                            lakesGlobalData.Country_KZ = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 8].Value?.ToString();
                            lakesGlobalData.Continent_ENG = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 9].Value?.ToString();
                            lakesGlobalData.Continent_RU = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 10].Value?.ToString();
                            lakesGlobalData.Continent_KZ = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 11].Value?.ToString();
                            lakesGlobalData.Lake_area = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 12].Value == null ?
                                (int?)null :
                                Convert.ToInt32(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 12].Value);
                            lakesGlobalData.Shore_len = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 13].Value == null ?
                                (int?)null :
                                Convert.ToInt32(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 13].Value);
                            lakesGlobalData.Shore_dev = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 14].Value == null ?
                                (int?)null :
                                Convert.ToInt32(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 14].Value);
                            lakesGlobalData.Vol_total = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 15].Value == null ?
                                (int?)null :
                                Convert.ToInt32(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 15].Value);
                            lakesGlobalData.Depth_avg = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 16].Value == null ?
                                (int?)null :
                                Convert.ToInt32(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 16].Value);
                            lakesGlobalData.Dis_avg = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 17].Value == null ?
                                (int?)null :
                                Convert.ToInt32(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 17].Value);
                            lakesGlobalData.Res_time = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 18].Value == null ?
                                (int?)null :
                                Convert.ToInt32(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 18].Value);
                            lakesGlobalData.Elevation = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 19].Value == null ?
                                (int?)null :
                                Convert.ToInt32(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 19].Value);
                            lakesGlobalData.Slope_100 = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 20].Value == null ?
                                (int?)null :
                                Convert.ToInt32(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 20].Value);
                            lakesGlobalData.Wshd_area = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 21].Value == null ?
                                (int?)null :
                                Convert.ToInt32(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 21].Value);
                            lakesGlobalData.Pour_long = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 22].Value == null ?
                                (int?)null :
                                Convert.ToInt32(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 22].Value);
                            lakesGlobalData.Pour_lat = package.Workbook.Worksheets.FirstOrDefault().Cells[i, 23].Value == null ?
                                (int?)null :
                                Convert.ToInt32(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 23].Value);
                        }
                        catch (Exception e)
                        {
                            ViewBag.Error = $"{_sharedLocalizer["Row"]} {i.ToString()}: " + e.Message + (e.InnerException == null ? "" : ": " + e.InnerException.Message);
                            break;
                        }

                        lakesGlobalDatas.Add(lakesGlobalData);
                        _context.Add(lakesGlobalDatas.LastOrDefault());
                    }
                    if (string.IsNullOrEmpty(ViewBag.Error))
                    {
                        _context.SaveChanges();
                        ViewBag.Report = $"{_sharedLocalizer["UploadedCount"]}: {lakesGlobalDatas.Count()}";
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
