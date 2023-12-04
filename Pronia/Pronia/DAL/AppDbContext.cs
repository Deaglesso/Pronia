using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pronia.Entities;

namespace Pronia.DAL
{


    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public DbSet<Slide> Slides { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductTag> ProductTags { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ProductEdition> ProductEditions { get; set; }
        public DbSet<Edition> Editions { get; set; }
        public DbSet<Platform> Platforms { get; set; }
        public DbSet<ProductPlatform> ProductPlatforms { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }

}
