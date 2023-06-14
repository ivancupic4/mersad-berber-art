using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MersadBerberArt.Models
{
	public class Art
	{
		public int Id { get; set; }
        public string Name { get; set; }
		public int ArtTypeId { get; set; }
        public string Description { get; set; }
		public DateTime DateCreated { get; set; }
        public decimal Price { get; set; }
		public string ImageUrl { get; set; }

        public virtual ArtType ArtType { get; set; }
    }
}
