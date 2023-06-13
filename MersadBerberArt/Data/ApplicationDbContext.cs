using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MersadBerberArt.Models;

namespace MersadBerberArt.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<MersadBerberArt.Models.Art>? Art { get; set; }
        public DbSet<MersadBerberArt.Models.ArtType>? ArtType { get; set; }
    }
}