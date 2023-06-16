using MersadBerberArt.Data;
using MersadBerberArt.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Composition;

namespace MersadBerberArt.Services
{
    public interface IArtService
    {
        public SelectList GetArtTypesSelectList(int? artTypeId = null);
        public void SaveFile(IFormFile artImageFile);
        public void DeleteFile(string imageUrl);
    }

    public class ArtService : IArtService
    {
        private const string ImagesFolderPath = "wwwroot/images/arts";

        private readonly ApplicationDbContext _context;

        public ArtService(ApplicationDbContext context)
        {
            _context = context;
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
