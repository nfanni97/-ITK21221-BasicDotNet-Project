using Microsoft.EntityFrameworkCore;

namespace RegistryApp.Models
{
    public class RegistryContext : DbContext
    {
        public RegistryContext(DbContextOptions<RegistryContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);//important for IdentityDbContext!

            //TODO: key for registryitem

            //TODO: seed
            modelBuilder.Entity<Category>().HasData(
                new Category{
                    Id = 1,
                    Name = "Bathroom"
                },
                new Category{
                    Id = 2,
                    Name = "Kitchen"
                }
            );
        }

        //TODO: dbsets
        public DbSet<Category> Categories { get; set; }
    }
}