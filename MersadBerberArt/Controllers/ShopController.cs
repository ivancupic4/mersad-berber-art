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
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShopController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Shop
        public async Task<IActionResult> Index(ArtTypeEnum? artType = null)
        {
            var artTypes = _context.ArtType
                .Distinct()
                .Select(a => a.Name)
                .ToList();

            ViewBag.ArtTypes = new SelectList(artTypes);

            var result = await _context.Art
                .Where(a => !artType.HasValue || a.ArtType.Id == (int)artType.Value)
                .Select(a => new ArtViewModel 
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


    }
}
