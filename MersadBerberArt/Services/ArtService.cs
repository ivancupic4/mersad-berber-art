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
        public ArtSearchResult SearchArt(ArtSearchParameters artSearchParameters, PaginationData paginationData = null);
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

        public ArtSearchResult SearchArt(ArtSearchParameters filter, PaginationData paginationData = null)
        {
            paginationData = paginationData ?? new PaginationData();

            var query = GetArtBasicSearchQuery(filter);
            int totalSearchedItems = query.Count();
            int pageCount = (int)Math.Ceiling(totalSearchedItems / (double)paginationData.PageSize);

            if (paginationData.PageIndex > pageCount && pageCount > 0)
                paginationData.PageIndex = pageCount;

            query = query
                .Skip((paginationData.PageIndex - 1) * paginationData.PageSize)
                .Take(paginationData.PageSize);

            return new ArtSearchResult
            {
                Items = query.Select(a => _modelMapper.MapArtToArtDisplayViewModel(a)).ToList(),
                ArtTypes = GetArtTypesSelectList(filter.ArtTypeId),
                PaginationData = new PaginationData 
                { 
                    PageCount = pageCount, 
                    PageIndex = paginationData.PageIndex, 
                    PageSize = paginationData.PageSize 
                },
            };
		}

        private IQueryable<Art> GetArtBasicSearchQuery(ArtSearchParameters filter)
        {
            return _context.Art.Include(a => a.ArtType)
                .Where(a => (!filter.ArtTypeId.HasValue || a.ArtType.Id == filter.ArtTypeId.Value)
                            && (string.IsNullOrEmpty(filter.SearchString) || a.Name.Contains(filter.SearchString))
                            && (!filter.PriceFrom.HasValue || filter.PriceFrom.Value < a.Price)
                            && (!filter.PriceTo.HasValue || filter.PriceTo.Value > a.Price));
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
