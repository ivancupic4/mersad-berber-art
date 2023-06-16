using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MersadBerberArt.Data;
using MersadBerberArt.Services;

namespace MersadBerberArt.Controllers
{
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IArtService _artService;
        private readonly IModelMapper _modelMapper;

        public ShopController(ApplicationDbContext context, IArtService artService, IModelMapper modelMapper)
        {
            _context = context;
            _artService = artService;
            _modelMapper = modelMapper;
        }

        public async Task<IActionResult> Index(string searchString, int? artTypeId = null)
        {
            ViewBag.ArtTypes = _artService.GetArtTypesSelectList();

            var result = await _context.Art
                .Where(a => (!artTypeId.HasValue || a.ArtType.Id == artTypeId.Value)
                         && (string.IsNullOrEmpty(searchString) || a.Name.Contains(searchString)))
                .Select(a => _modelMapper.MapArtToArtDisplayViewModel(a))
                .ToListAsync();

            return _context.Art != null 
                ? View(result)
                : Problem("Entity set 'ApplicationDbContext.Art' is null.");
        }


    }
}
