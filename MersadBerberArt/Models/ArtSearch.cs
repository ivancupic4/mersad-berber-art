using Microsoft.AspNetCore.Mvc.Rendering;

namespace MersadBerberArt.Models
{
    public class ArtSearchParameters
    {
        public ArtSearchParameters(string searchString, int? artTypeId = null, int? priceFrom = null, int? priceTo = null) 
        { 
            SearchString = searchString;
            ArtTypeId = artTypeId;
            PriceFrom = priceFrom;
            PriceTo = priceTo;
        }

        public string SearchString { get; set; }
        public int? ArtTypeId { get; set; }
        public int? PriceFrom { get; set; }
        public int? PriceTo { get; set; }
    }

    public class ArtSearchResult
    {
        public int TotalItems { get; set; }
        public List<ArtDisplayViewModel> Items { get; set; }
        public PaginationData PaginationData { get; set; }
        public SelectList ArtTypes { get; set; }
    }
}
