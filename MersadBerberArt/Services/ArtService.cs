using MersadBerberArt.Data;
using MersadBerberArt.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Composition;
using System.Drawing.Printing;

namespace MersadBerberArt.Services
{
    public interface IArtService
    {
        public SelectList GetArtTypesSelectList(int? artTypeId = null);
        public void SaveFile(IFormFile artImageFile);
        public void DeleteFile(string imageUrl);
        public ArtSearchResult SearchArt(string searchString, int? artTypeId, PaginationData paginationData = null);
    }

    public class ArtService : IArtService
    {
        private const string ImagesFolderPath = "wwwroot/images/arts";

        private readonly ApplicationDbContext _context;
		private readonly IModelMapper _modelMapper;

		public ArtService(ApplicationDbContext context, IModelMapper modelMapper)
        {
            _context = context;
            _modelMapper = modelMapper;

		}

        public ArtSearchResult SearchArt(string searchString, int? artTypeId, PaginationData paginationData = null)
        {
            paginationData = paginationData ?? new PaginationData { PageIndex = 1, PageSize = 10};

            var query = GetArtBasicSearchQuery(searchString, artTypeId);
            int totalSearchedItems = query.Count();
            int pageCount = (int)Math.Ceiling(totalSearchedItems / (double)paginationData.PageSize);

            if (paginationData.PageIndex > pageCount)
                paginationData.PageIndex = pageCount;

            query = query
                .Skip((paginationData.PageIndex - 1) * paginationData.PageSize)
                .Take(paginationData.PageSize);

            return new ArtSearchResult
            {
                Items = query.Select(a => _modelMapper.MapArtToArtDisplayViewModel(a)).ToList(),
                PaginationData = new PaginationData 
                { 
                    PageCount = pageCount, 
                    PageIndex = paginationData.PageIndex, 
                    PageSize = paginationData.PageSize 
                },
            };
		}

        private IQueryable<Art> GetArtBasicSearchQuery(string searchString, int? artTypeId)
        {
            return _context.Art.Include(a => a.ArtType)
                .Where(a => (!artTypeId.HasValue || a.ArtType.Id == artTypeId.Value)
                            && (string.IsNullOrEmpty(searchString) || a.Name.Contains(searchString)));
        }

        public SelectList GetArtTypesSelectList(int? artTypeId = null)
        {
            var artTypes = _context.ArtType.ToList();
            if (artTypeId.HasValue)
                return new SelectList(artTypes, nameof(ArtType.Id), nameof(ArtType.Name), artTypeId);
            else
                return new SelectList(artTypes, nameof(ArtType.Id), nameof(ArtType.Name));
        }

        public void SaveFile(IFormFile artImageFile)
        {
            string filePath = GetFilePath(artImageFile.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                artImageFile.CopyToAsync(stream);
            }
        }

        public void DeleteFile(string imageUrl)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), ImagesFolderPath, imageUrl);
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        private string GetFilePath(string fileName)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), ImagesFolderPath, fileName);
        }
    }
}
