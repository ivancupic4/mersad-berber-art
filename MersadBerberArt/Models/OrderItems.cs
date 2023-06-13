namespace MersadBerberArt.Models
{
    public class OrderItems
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ArtId { get; set; }

        public virtual Order Order { get; set; }
        public virtual Art Art { get; set; }
    }
}
