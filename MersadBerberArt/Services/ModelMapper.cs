using MersadBerberArt.Models;

namespace MersadBerberArt.Services
{
    public interface IModelMapper
    {
        public ArtDisplayViewModel MapArtToArtDisplayViewModel(Art art);
        public Art MapArtViewModelToArt(ArtViewModel artViewModel, string uniqueFileName);
        public void MapArtViewModelToArt(ArtViewModel artViewModel, string fileName, Art art);
        public ArtViewModel MapArtToArtViewModel(Art art);
    }

    public class ModelMapper : IModelMapper
    {
        public ArtDisplayViewModel MapArtToArtDisplayViewModel(Art art)
        {
            return new ArtDisplayViewModel
            {
                Id = art.Id,
                Name = art.Name,
                ArtType = art.ArtType.Name,
                DateCreated = art.DateCreated.ToShortDateString(),
                Description = art.Description,
                ImageUrl = art.ImageUrl,
                Price = $"€{art.Price}"
            };
        }

        public Art MapArtViewModelToArt(ArtViewModel artViewModel, string fileName)
        {
            return new Art
            {
                Name = artViewModel.Name,
                Description = artViewModel.Description,
                DateCreated = artViewModel.DateCreated,
                Price = ParseStringToDecimal(artViewModel.Price),
                ArtTypeId = artViewModel.ArtTypeId,
                ImageUrl = fileName == string.Empty ? artViewModel.ImageUrl : fileName
            };
        }

        public void MapArtViewModelToArt(ArtViewModel artViewModel, string fileName, Art art)
        {
            art.Name = artViewModel.Name;
            art.Description = artViewModel.Description;
            art.ArtTypeId = artViewModel.ArtTypeId;
            art.DateCreated = artViewModel.DateCreated;
            art.ImageUrl = fileName;
            art.Price = ParseStringToDecimal(artViewModel.Price);
        }

        public ArtViewModel MapArtToArtViewModel(Art art)
        {
            return new ArtViewModel
            {
                Id = art.Id,
                Name = art.Name,
                Description = art.Description,
                DateCreated = art.DateCreated,
                Price = art.Price.ToString(),
                ArtTypeId = art.ArtTypeId,
                ImageUrl = art.ImageUrl
            };
        }

        decimal ParseStringToDecimal(string priceString)
        {
            decimal price = 0;
            if (!decimal.TryParse(priceString, out price))
                throw new FormatException("Price is not in correct format");

            return price;
        }
    }
}
