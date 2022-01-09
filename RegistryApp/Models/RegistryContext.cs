using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RegistryApp.Models.Authentication;

namespace RegistryApp.Models
{
    public class RegistryContext : IdentityDbContext<ApplicationUser>
    {
        public RegistryContext(DbContextOptions<RegistryContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseLazyLoadingProxies(true);
            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);//important for IdentityDbContext!

            //TODO: key for registryitem
            modelBuilder.Entity<CategoryProduct>().HasKey(cp=> new {
                cp.CategoryId,
                cp.ProductId
            }
            );

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

            modelBuilder.Entity<Product>().HasData(
                new Product{
                    Id = 1,
                    Name = "Toaster",
                    PriceHuf = 15990,
                    Description = "Toast...yummy"
                },
                new Product{
                    Id = 2,
                    Name = "Vase",
                    PriceHuf = 3690,
                    Description = "Nothing to write home about"
                }
            );

            modelBuilder.Entity<CategoryProduct>().HasData(
                new CategoryProduct{
                    CategoryId = 2,
                    ProductId = 1
                },
                new CategoryProduct{
                    CategoryId = 1,
                    ProductId = 1
                }
            );
        }

        //TODO: dbsets
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}