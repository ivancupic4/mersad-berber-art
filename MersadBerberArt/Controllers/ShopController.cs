using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MersadBerberArt.Data;
using MersadBerberArt.Services;
using MersadBerberArt.Models;

namespace MersadBerberArt.Controllers
{
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IArtService _artService;

        public ShopController(ApplicationDbContext context, IArtService artService)
        {
            _context = context;
            _artService = artService;
        }

        public List<Art> cartItems = new List<Art>();

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; } = 10;

        public async Task<IActionResult> Index(string searchString, int? artTypeId = null)
        {
            return _context.Art != null 
                ? View(_artService.SearchArt(searchString, artTypeId, new PaginationData { PageIndex = PageIndex, PageSize = PageSize }))
                : Problem("Entity set 'ApplicationDbContext.Art' is null.");
        }

        //public async Task<IActionResult> AddToCart(int artId)
        //{
        //    cartItems = (List<Art>)Session["Cart"];
        //}
    }
}
