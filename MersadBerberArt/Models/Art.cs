﻿namespace MersadBerberArt.Models
{
	public class Art
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public ArtType ArtTypeId { get; set; }
		public string Description { get; set; }
		public DateTime DateCreated { get; set; }
		public decimal Price { get; set; }
	}
}
