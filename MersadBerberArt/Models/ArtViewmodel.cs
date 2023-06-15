using System.ComponentModel;

namespace MersadBerberArt.Models
{
    public class ArtViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ArtTypeId { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public decimal Price { get; set; }
        [SkipValidation]
        public string ImageUrl { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class SkipValidationAttribute : Attribute
    {
    }
}
