using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MersadBerberArt.Data;
using MersadBerberArt.Models;
using System.Reflection;
using MersadBerberArt.Services;

namespace MersadBerberArt.Controllers
{
    public class ArtController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IArtService _artService;
        private readonly IModelMapper _modelMapper;

        public ArtController(ApplicationDbContext context, IArtService artService, IModelMapper modelMapper)
        {
            _context = context;
            _artService = artService;
            _modelMapper = modelMapper;
        }

        [BindProperty]
        public IFormFile ArtImageFile { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; } = 10;

        public async Task<IActionResult> Index(string searchString, int? artTypeId = null)
        {
            ViewBag.ArtTypes = _artService.GetArtTypesSelectList();

            var result = _artService.SearchArt(searchString, artTypeId, new PaginationData { PageIndex = PageIndex, PageSize = PageSize });

            return _context.Art != null
                ? View(result.Items)
                : Problem("Entity set 'ApplicationDbContext.Art' is null.");
        }

        public async Task<IActionResult> Details(int? id, string viewedFrom)
        {
            if (id == null || _context.Art == null)
                return NotFound();

            var art = _context.Art.Include(a => a.ArtType).FirstOrDefault(a => a.Id == id);
            if (art == null)
                return NotFound();

            ViewData["ViewedFrom"] = viewedFrom;

            return View(_modelMapper.MapArtToArtDisplayViewModel(art));
        }

        public IActionResult Create()
        {
            ViewBag.ArtTypes = _artService.GetArtTypesSelectList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,DateCreated,Price,ArtTypeId")] ArtViewModel artViewModel)
        {
            ViewBag.ArtTypes = _artService.GetArtTypesSelectList(artViewModel.ArtTypeId);

            RemoveSkippedPropertiesFromArtViewModelValidation();
            if (!ModelState.IsValid)
                return View(artViewModel);

            string uniqueFileName = "";
            if (ArtImageFile != null && ArtImageFile.Length > 0)
            {
                uniqueFileName = ArtImageFile.FileName;
                _artService.SaveFile(ArtImageFile);
            }
            else
                ModelState.AddModelError("ArtImageFile", "Please select a file.");

            artViewModel.Price = artViewModel.Price.Replace(".", ",");
            _context.Add(_modelMapper.MapArtViewModelToArt(artViewModel, uniqueFileName));
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

            ViewBag.ArtTypes = _artService.GetArtTypesSelectList(art.ArtTypeId);
            return View(_modelMapper.MapArtToArtViewModel(art));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ArtTypeId,Description,DateCreated,Price,ImageUrl")] ArtViewModel artViewModel)
        {
            ViewBag.ArtTypes = _artService.GetArtTypesSelectList(artViewModel.ArtTypeId);

            RemoveSkippedPropertiesFromArtViewModelValidation(new List<string> { nameof(ArtImageFile) });
            if (ModelState.IsValid)
            {
                var art = _context.Art.Find(id);
                if (art == null)
                    return NotFound();

                string uniqueFileName = art.ImageUrl;
                if (ArtImageFile != null && ArtImageFile.Length > 0 && art.ImageUrl != ArtImageFile.FileName)
                {
                    uniqueFileName = ArtImageFile.FileName;
                    _artService.DeleteFile(art.ImageUrl);
                    _artService.SaveFile(ArtImageFile);
                }

                artViewModel.Price = artViewModel.Price.Replace(".", ",");
                _modelMapper.MapArtViewModelToArt(artViewModel, uniqueFileName, art);

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

            return View(_modelMapper.MapArtToArtDisplayViewModel(art));
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
                _artService.DeleteFile(imageUrl);

            return RedirectToAction(nameof(Index));
        }

        private void RemoveSkippedPropertiesFromArtViewModelValidation(List<string> additionalPropertiesToRemove = null)
        {
            var excludedProperties = typeof(ArtViewModel).GetProperties()
                .Where(prop => prop.GetCustomAttribute<SkipValidationAttribute>() != null)
                .ToList();

            additionalPropertiesToRemove = additionalPropertiesToRemove ?? new List<string>();
            foreach (var propName in excludedProperties.Select(p => p.Name).Union(additionalPropertiesToRemove))
            {
                ModelState.Remove(propName);
            }
        }

        private bool ArtExists(int id) => (_context.Art?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}
