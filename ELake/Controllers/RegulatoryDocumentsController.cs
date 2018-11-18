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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace ELake.Controllers
{
    public class RegulatoryDocumentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;

        public RegulatoryDocumentsController(ApplicationDbContext context,
            IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: RegulatoryDocuments
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Index(string SortOrder,
            string Name,
            string Number,
            int? Page)
        {
            var regulatoryDocuments = _context.RegulatoryDocument
                .Include(r => r.DocumentType)
                .Where(r => !r.Archival);

            ViewBag.NameFilter = Name;
            ViewBag.NumberFilter = Number;

            ViewBag.NameSort = SortOrder == "Name" ? "NameDesc" : "Name";
            ViewBag.NumberSort = SortOrder == "Number" ? "NumberDesc" : "Number";

            if (!string.IsNullOrEmpty(Name))
            {
                regulatoryDocuments = regulatoryDocuments.Where(w => w.Name.ToLower().Contains(Name.ToLower()));
            }
            if (!string.IsNullOrEmpty(Number))
            {
                regulatoryDocuments = regulatoryDocuments.Where(w => w.Number.ToLower().Contains(Number.ToLower()));
            }

            switch (SortOrder)
            {
                case "Name":
                    regulatoryDocuments = regulatoryDocuments.OrderBy(w => w.Name);
                    break;
                case "NameDesc":
                    regulatoryDocuments = regulatoryDocuments.OrderByDescending(w => w.Name);
                    break;
                case "Number":
                    regulatoryDocuments = regulatoryDocuments.OrderBy(w => w.Number);
                    break;
                case "NumberDesc":
                    regulatoryDocuments = regulatoryDocuments.OrderByDescending(w => w.Number);
                    break;
                default:
                    regulatoryDocuments = regulatoryDocuments.OrderBy(w => w.Id);
                    break;
            }
            ViewBag.SortOrder = SortOrder;

            var pager = new Pager(regulatoryDocuments.Count(), Page);

            var viewModel = new RegulatoryDocumentIndexPageViewModel
            {
                Items = regulatoryDocuments.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };

            return View(viewModel);
        }

        // GET: RegulatoryDocuments/Details/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var regulatoryDocument = await _context.RegulatoryDocument
                .Include(r => r.DocumentType)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (regulatoryDocument == null)
            {
                return NotFound();
            }

            return View(regulatoryDocument);
        }

        // GET: RegulatoryDocuments/Create
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult Create()
        {
            ViewData["DocumentTypeId"] = new SelectList(_context.DocumentType, "Id", "Name");
            ViewData["PreviousDocumentId"] = new SelectList(_context.RegulatoryDocument, "Id", "Name");

            return View();
        }

        // POST: RegulatoryDocuments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Create([Bind("Id,DocumentTypeId,Name,Number,ForceEntryYear,ForceEntryMonth,ForceEntryDay,File,PreviousDocumentId")] RegulatoryDocument regulatoryDocument,
            IFormFile DocFile)
        {
            if (ModelState.IsValid)
            {
                // не архивный при создании
                regulatoryDocument.Archival = false;
                regulatoryDocument.DeletingJustification = null;
                regulatoryDocument.NewDocumentId = null;
                regulatoryDocument.Description = null;

                _context.Add(regulatoryDocument);
                await _context.SaveChangesAsync();

                // сохранение файла
                string sContentRootPath = _hostingEnvironment.WebRootPath;
                sContentRootPath = Path.Combine(sContentRootPath, "RegulatoryDocuments", regulatoryDocument.Id.ToString());
                Directory.CreateDirectory(sContentRootPath);
                if(DocFile != null)
                {
                    string path_filename = Path.Combine(sContentRootPath, Path.GetFileName(DocFile.FileName));
                    using (var stream = new FileStream(Path.GetFullPath(path_filename), FileMode.Create))
                    {
                        await DocFile.CopyToAsync(stream);
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["DocumentTypeId"] = new SelectList(_context.DocumentType, "Id", "Name", regulatoryDocument.DocumentTypeId);
            ViewData["PreviousDocumentId"] = new SelectList(_context.RegulatoryDocument, "Id", "Name", regulatoryDocument.PreviousDocumentId);
            return View(regulatoryDocument);
        }

        // GET: RegulatoryDocuments/Edit/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var regulatoryDocument = await _context.RegulatoryDocument.SingleOrDefaultAsync(m => m.Id == id);
            if (regulatoryDocument == null)
            {
                return NotFound();
            }
            ViewData["DocumentTypeId"] = new SelectList(_context.DocumentType, "Id", "Name", regulatoryDocument.DocumentTypeId);
            ViewData["PreviousDocumentId"] = new SelectList(_context.RegulatoryDocument, "Id", "Name", regulatoryDocument.PreviousDocumentId);
            return View(regulatoryDocument);
        }

        // POST: RegulatoryDocuments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DocumentTypeId,Name,Number,ForceEntryYear,ForceEntryMonth,ForceEntryDay,File,PreviousDocumentId")] RegulatoryDocument regulatoryDocument,
            IFormFile DocFile)
        {
            if (id != regulatoryDocument.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // не архивный при редактировании
                    regulatoryDocument.Archival = false;
                    regulatoryDocument.DeletingJustification = null;
                    regulatoryDocument.NewDocumentId = null;
                    regulatoryDocument.Description = null;

                    _context.Update(regulatoryDocument);
                    await _context.SaveChangesAsync();

                    // сохранение файла
                    string sContentRootPath = _hostingEnvironment.WebRootPath;
                    sContentRootPath = Path.Combine(sContentRootPath, "RegulatoryDocuments", regulatoryDocument.Id.ToString());
                    Directory.CreateDirectory(sContentRootPath);
                    if (DocFile != null)
                    {
                        // удаление всех предыдущих файлов в папке
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
                        string path_filename = Path.Combine(sContentRootPath, Path.GetFileName(DocFile.FileName));
                        using (var stream = new FileStream(Path.GetFullPath(path_filename), FileMode.Create))
                        {
                            await DocFile.CopyToAsync(stream);
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RegulatoryDocumentExists(regulatoryDocument.Id))
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
            ViewData["DocumentTypeId"] = new SelectList(_context.DocumentType, "Id", "Name", regulatoryDocument.DocumentTypeId);
            ViewData["PreviousDocumentId"] = new SelectList(_context.RegulatoryDocument, "Id", "Name", regulatoryDocument.PreviousDocumentId);
            return View(regulatoryDocument);
        }

        // GET: RegulatoryDocuments/Delete/5
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var regulatoryDocument = await _context.RegulatoryDocument
                .Include(r => r.DocumentType)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (regulatoryDocument == null)
            {
                return NotFound();
            }

            return View(regulatoryDocument);
        }

        // POST: RegulatoryDocuments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var regulatoryDocument = await _context.RegulatoryDocument.SingleOrDefaultAsync(m => m.Id == id);
            _context.RegulatoryDocument.Remove(regulatoryDocument);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RegulatoryDocumentExists(int id)
        {
            return _context.RegulatoryDocument.Any(e => e.Id == id);
        }

        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> DownloadFile(int id)
        {
            string sContentRootPath = _hostingEnvironment.WebRootPath;
            sContentRootPath = Path.Combine(sContentRootPath, "RegulatoryDocuments", id.ToString());
            DirectoryInfo di = new DirectoryInfo(sContentRootPath);
            if(di.GetFiles().Count() >= 1)
            {
                string filePath = di.GetFiles().FirstOrDefault().FullName,
                fileName = di.GetFiles().FirstOrDefault().Name;
                return File(System.IO.File.ReadAllBytes(filePath), "application/octet-stream", fileName);
            }
            else
            {
                return RedirectToAction("Details", new { id = id });
            }
        }
    }
}
