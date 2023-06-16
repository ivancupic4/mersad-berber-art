using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MersadBerberArt.Models
{
    public class ArtViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ArtTypeId { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        [RegularExpression(@"^\d+(,\d{1,2})?(\.\d{1,2})?$", ErrorMessage = "Please enter a valid numeric value.")] 
        public string Price { get; set; }
        [SkipValidation]
        public string ImageUrl { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class SkipValidationAttribute : Attribute
    {
    }
}
