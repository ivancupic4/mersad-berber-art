using Microsoft.AspNetCore.Mvc.Rendering;

namespace MersadBerberArt.Models
{
    public class ArtSearchResult
    {
        public List<ArtDisplayViewModel> Items { get; set; }
        public PaginationData PaginationData { get; set; }
        public SelectList ArtTypes { get; set; }
    }
}
