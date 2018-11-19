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
    public class TransitionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;

        public TransitionsController(ApplicationDbContext context,
            IHostingEnvironment hostingEnvironment,
            IStringLocalizer<SharedResources> sharedLocalizer)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _sharedLocalizer = sharedLocalizer;
        }

        // GET: Transitions
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Index(string SortOrder,
            int? LakeId,
            int? Page)
        {
            var transitions = _context.Transition
                .Where(w => true);

            ViewBag.LakeIdFilter = LakeId;

            ViewBag.LakeIdSort = SortOrder == "LakeId" ? "LakeIdDesc" : "LakeId";

            if (LakeId != null)
            {
                transitions = transitions.Where(w => w.LakeId == LakeId);
            }

            switch (SortOrder)
            {
                case "LakeId":
                    transitions = transitions.OrderBy(w => w.LakeId);
                    break;
                case "LakeIdDesc":
                    transitions = transitions.OrderByDescending(w => w.LakeId);
                    break;
                default:
                    transitions = transitions.OrderBy(w => w.Id);
                    break;
            }
            ViewBag.SortOrder = SortOrder;

            var pager = new Pager(transitions.Count(), Page);

            var viewModel = new TransitionIndexPageViewModel
            {
                Items = transitions.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };

            return View(viewModel);
        }

        // GET: Transitions/Details/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transition = await _context.Transition
                .SingleOrDefaultAsync(m => m.Id == id);
            if (transition == null)
            {
                return NotFound();
            }

            return View(transition);
        }

        // GET: Transitions/Create
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Transitions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Create([Bind("Id,LakeId,NoСhange,Permanent,NewPermanent,LostPermanent,Seasonal,NewSeasonal,LostSeasonal,SeasonalToPermanent,PermanentToDeasonal,EphemeralPermanent,EphemeralSeasonal")] Transition transition)
        {
            if (ModelState.IsValid)
            {
                _context.Add(transition);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(transition);
        }

        // GET: Transitions/Edit/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transition = await _context.Transition.SingleOrDefaultAsync(m => m.Id == id);
            if (transition == null)
            {
                return NotFound();
            }
            return View(transition);
        }

        // POST: Transitions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LakeId,NoСhange,Permanent,NewPermanent,LostPermanent,Seasonal,NewSeasonal,LostSeasonal,SeasonalToPermanent,PermanentToDeasonal,EphemeralPermanent,EphemeralSeasonal")] Transition transition)
        {
            if (id != transition.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transition);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransitionExists(transition.Id))
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
            return View(transition);
        }

        // GET: Transitions/Delete/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transition = await _context.Transition
                .SingleOrDefaultAsync(m => m.Id == id);
            if (transition == null)
            {
                return NotFound();
            }

            return View(transition);
        }

        // POST: Transitions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transition = await _context.Transition.SingleOrDefaultAsync(m => m.Id == id);
            _context.Transition.Remove(transition);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransitionExists(int id)
        {
            return _context.Transition.Any(e => e.Id == id);
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
                    List<Transition> transitions = new List<Transition>();
                    for (int i = start_row; ; i++)
                    {
                        if (package.Workbook.Worksheets.FirstOrDefault().Cells[i, 1].Value == null)
                        {
                            break;
                        }
                        Transition transition = new Transition();

                        try
                        {
                            transition.LakeId = Convert.ToInt32(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 1].Value);
                            transition.NoСhange = Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 2].Value);
                            transition.Permanent = Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 3].Value);
                            transition.NewPermanent = Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 4].Value);
                            transition.LostPermanent = Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 5].Value);
                            transition.Seasonal = Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 6].Value);
                            transition.NewSeasonal = Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 7].Value);
                            transition.LostSeasonal = Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 8].Value);
                            transition.SeasonalToPermanent = Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 9].Value);
                            transition.PermanentToDeasonal = Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 10].Value);
                            transition.EphemeralPermanent = Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 11].Value);
                            transition.EphemeralSeasonal = Convert.ToDecimal(package.Workbook.Worksheets.FirstOrDefault().Cells[i, 12].Value);
                        }
                        catch (Exception e)
                        {
                            ViewBag.Error = $"{_sharedLocalizer["Row"]} {i.ToString()}: " + e.Message + (e.InnerException == null ? "" : ": " + e.InnerException.Message);
                            break;
                        }

                        transitions.Add(transition);
                        _context.Add(transitions.LastOrDefault());
                    }
                    if (string.IsNullOrEmpty(ViewBag.Error))
                    {
                        _context.SaveChanges();
                        ViewBag.Report = $"{_sharedLocalizer["UploadedCount"]}: {transitions.Count()}";
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
