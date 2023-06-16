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
                ArtTypeName = art.ArtType.Name,
                DateCreated = art.DateCreated.ToShortDateString(),
                Description = art.Description,
                ImageUrl = art.ImageUrl,
                Price = $"{art.Price}€"
            };
        }

        public Art MapArtViewModelToArt(ArtViewModel artViewModel, string fileName)
        {
            string imageUrl = fileName == string.Empty ? artViewModel.ImageUrl : fileName;
            return new Art
            {
                Name = artViewModel.Name,
                Description = artViewModel.Description,
                DateCreated = artViewModel.DateCreated,
                Price = artViewModel.Price,
                ArtTypeId = artViewModel.ArtTypeId,
                ImageUrl = imageUrl
            };
        }

        public void MapArtViewModelToArt(ArtViewModel artViewModel, string fileName, Art art)
        {
            art.Name = artViewModel.Name;
            art.Description = artViewModel.Description;
            art.ArtTypeId = artViewModel.ArtTypeId;
            art.DateCreated = artViewModel.DateCreated;
            art.ImageUrl = fileName;
            art.Price = artViewModel.Price;
        }

        public ArtViewModel MapArtToArtViewModel(Art art)
        {
            return new ArtViewModel
            {
                Id = art.Id,
                Name = art.Name,
                Description = art.Description,
                DateCreated = art.DateCreated,
                Price = art.Price,
                ArtTypeId = art.ArtTypeId,
                ImageUrl = art.ImageUrl
            };
        }
    }
}
