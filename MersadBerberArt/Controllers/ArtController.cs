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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Reflection;

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

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Art == null)
                return NotFound();

            var art = await _context.Art.FindAsync(id);
            if (art == null)
                return NotFound();

            return View(art);
        }

        public IActionResult Create()
        {
            var artTypes = _context.ArtType.ToList();
            ViewBag.ArtTypes = new SelectList(artTypes, "Id", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,DateCreated,Price,ArtTypeId")] ArtViewModel artViewModel)
        {
            var artTypes = _context.ArtType.ToList();
            ViewBag.ArtTypes = new SelectList(artTypes, "Id", "Name", artViewModel.ArtTypeId);

            RemoveSkippedPropertiesFromArtViewModelValidation();
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

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Art == null)
                return NotFound();

            var art = await _context.Art.FindAsync(id);
            if (art == null)
                return NotFound();

            var artTypes = _context.ArtType.ToList();
            ViewBag.ArtTypes = new SelectList(artTypes, "Id", "Name", art.ArtTypeId);

            return View(new ArtViewModel
            {
                Id = art.Id,
                Name = art.Name,
                Description = art.Description,
                DateCreated = art.DateCreated,
                Price = art.Price,
                ArtTypeId = art.ArtTypeId,
                ImageUrl = art.ImageUrl
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ArtTypeId,Description,DateCreated,Price,ImageUrl")] ArtViewModel artViewModel)
        {
            var artTypes = _context.ArtType.ToList();
            ViewBag.ArtTypes = new SelectList(artTypes, "Id", "Name", artViewModel.ArtTypeId);

            RemoveSkippedPropertiesFromArtViewModelValidation();
            if (ModelState.IsValid)
            {
                var art = _context.Art.Find(id);
                if (art == null)
                    return NotFound();

                string uniqueFileName = "";
                if (ArtImageFile != null && ArtImageFile.Length > 0)
                {
                    uniqueFileName = ArtImageFile.FileName;
                    // If new file name is different from existing file name
                    if (art.ImageUrl != uniqueFileName)
                    {
                        // First Delete the existing image
                        string imageUrl = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/arts", art.ImageUrl);
                        if (System.IO.File.Exists(imageUrl))
                            System.IO.File.Delete(imageUrl);

                        // Then add the new image
                        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/arts", uniqueFileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            ArtImageFile.CopyToAsync(stream);
                        }
                    }
                }
                else
                    ModelState.AddModelError("ArtImageFile", "Please select a file.");

                art.Name = artViewModel.Name;
                art.Description = artViewModel.Description;
                art.ArtTypeId = artViewModel.ArtTypeId;
                art.DateCreated = artViewModel.DateCreated;
                art.ImageUrl = uniqueFileName;
                art.Price = artViewModel.Price;

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

            return View(artViewModel);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Art == null)
                return NotFound();

            var art = _context.Art.Include(a => a.ArtType).FirstOrDefault(a => a.Id == id);
            if (art == null)
                return NotFound();

            return View(new ArtDisplayViewModel
            {
                Id = art.Id,
                Name = art.Name,
                ArtTypeName = art.ArtType.Name,
                DateCreated = art.DateCreated.ToShortDateString(),
                Description = art.Description,
                ImageUrl = art.ImageUrl,
                Price = $"{art.Price}€"
            });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Art == null)
                return Problem("Entity set 'ApplicationDbContext.Art' is null.");

            var art = await _context.Art.FindAsync(id);
            string imageUrl = art.ImageUrl;
            if (art != null)
                _context.Art.Remove(art);

            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(imageUrl))
            {
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/arts", imageUrl);
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }

            return RedirectToAction(nameof(Index));
        }

        private void RemoveSkippedPropertiesFromArtViewModelValidation()
        {
            var excludedProperties = typeof(ArtViewModel).GetProperties()
                .Where(prop => prop.GetCustomAttribute<SkipValidationAttribute>() != null);

            foreach (var property in excludedProperties)
            {
                ModelState.Remove(property.Name);
            }
        }

        private bool ArtExists(int id) => (_context.Art?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}
