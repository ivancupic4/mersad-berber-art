using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MersadBerberArt.Models;
using Microsoft.AspNetCore.Identity;

namespace MersadBerberArt.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Art>? Art { get; set; }
        public DbSet<ArtType>? ArtType { get; set; }
    }
}