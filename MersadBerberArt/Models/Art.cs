using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [ForeignKey("ArtTypeId")]
        public virtual ArtType ArtType { get; set; }
    }
}
