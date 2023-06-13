using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MersadBerberArt.Data;
using MersadBerberArt.Models;

namespace MersadBerberArt.Controllers
{
    public class ArtController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ArtController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Art
        public async Task<IActionResult> Index()
        {
            return _context.Art != null 
                ? View(await _context.Art.ToListAsync()) 
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
            return View();
        }

        // POST: Art/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ArtTypeId,Description,DateCreated,Price,ImageUrl")] Art art)
        {
            if (ModelState.IsValid)
            {
                _context.Add(art);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(art);
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
