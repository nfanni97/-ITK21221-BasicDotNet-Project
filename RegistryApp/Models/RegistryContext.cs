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

            modelBuilder.Entity<CategoryProduct>().HasKey(cp=> new {
                cp.CategoryId,
                cp.ProductId
            }
            );
            modelBuilder.Entity<RegistryItem>().HasKey(ri => new {ri.Id});
            modelBuilder.Entity<RegistryItem>()
                .HasOne(ri => ri.Buyer)
                .WithMany(b => b.ByMe)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_RegistryItem_AspNetUsers_BuyerId");
            modelBuilder.Entity<RegistryItem>()
                .HasOne(ri => ri.Recipient)
                .WithMany(r => r.ForMe)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_RegistryItem_AspNetUsers_RecipientId");
            modelBuilder.Entity<RegistryItem>()
                .HasOne(ri=> ri.Product)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_RegistryItem_Product_ProductId");

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
        public DbSet<RegistryItem> RegistryItems { get; set; }
    }
}