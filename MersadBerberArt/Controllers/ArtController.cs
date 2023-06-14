using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MersadBerberArt.Data;
using MersadBerberArt.Models;
using System.Security.AccessControl;

namespace MersadBerberArt.Controllers
{
    public class ArtController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ArtController(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public IFormFile ArtImageFile { get; set; }

        // GET: Art
        public async Task<IActionResult> Index(string searchString, ArtTypeEnum? artType = null)
        {
            var artTypes = _context.ArtType.Select(a => a.Name).Distinct().ToList();
            ViewBag.ArtTypes = new SelectList(artTypes);

            var result = await _context.Art
            .Where(a => (!artType.HasValue || a.ArtType.Id == (int)artType.Value)
                         && (string.IsNullOrEmpty(searchString) || a.Name.Contains(searchString)))
                .Select(a => new ArtDisplayViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    ArtTypeName = a.ArtType.Name,
                    DateCreated = a.DateCreated.ToShortDateString(),
                    Description = a.Description,
                    ImageUrl = a.ImageUrl,
                    Price = $"{a.Price}€"
                })
                .ToListAsync();

            return _context.Art != null
                ? View(result)
                : Problem("Entity set 'ApplicationDbContext.Art' is null.");
        }

        // GET: Art/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Art == null)
                return NotFound();

            var art = await _context.Art.FindAsync(id);
            if (art == null)
                return NotFound();

            return View(art);
        }

        // GET: Art/Create
        public IActionResult Create()
        {
            var artTypes = _context.ArtType.ToList();
            ViewBag.ArtTypes = new SelectList(artTypes, "Id", "Name");

            return View();
        }

        // POST: Art/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,DateCreated,Price,ArtTypeId")] ArtViewModel artViewModel)
        {
            var artTypes = _context.ArtType.ToList();
            ViewBag.ArtTypes = new SelectList(artTypes, "Id", "Name", artViewModel.ArtTypeId);

            if (!ModelState.IsValid)
                return View(artViewModel);

            string uniqueFileName = "";
            if (ArtImageFile != null && ArtImageFile.Length > 0)
            {
                uniqueFileName = ArtImageFile.FileName;
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/arts", uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ArtImageFile.CopyToAsync(stream);
                }
            }
            else
                ModelState.AddModelError("ArtImageFile", "Please select a file.");

            _context.Add(new Art
            {
                Name = artViewModel.Name,
                Description = artViewModel.Description,
                DateCreated = artViewModel.DateCreated,
                Price = artViewModel.Price,
                ArtTypeId = artViewModel.ArtTypeId,
                ImageUrl = uniqueFileName
            });
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Art/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Art == null)
                return NotFound();

            var art = await _context.Art.FindAsync(id);
            if (art == null)
                return NotFound();

            return View(art);
        }

        // POST: Art/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ArtTypeId,Description,DateCreated,Price,ImageUrl")] Art art)
        {
            if (id != art.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(art);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArtExists(art.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(art);
        }

        // GET: Art/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Art == null)
                return NotFound();

            var art = await _context.Art.FindAsync(id);
            if (art == null)
                return NotFound();

            return View(art);
        }

        // POST: Art/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Art == null)
                return Problem("Entity set 'ApplicationDbContext.Art' is null.");

            var art = await _context.Art.FindAsync(id);
            if (art != null)
                _context.Art.Remove(art);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArtExists(int id) => (_context.Art?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}
