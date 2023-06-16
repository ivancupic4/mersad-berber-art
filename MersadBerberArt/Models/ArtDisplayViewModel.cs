using System.ComponentModel;

namespace MersadBerberArt.Models
{
    public class ArtDisplayViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [DisplayName("Art Type")]
        public string ArtType { get; set; }
        public string Description { get; set; }
        [DisplayName("Date Created")]
        public string DateCreated { get; set; }
        public string Price { get; set; }
        [DisplayName("Image Url")]
        public string ImageUrl { get; set; }
    }
}
