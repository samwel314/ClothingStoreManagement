using ClothingStoreManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClothingStoreManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Product> Products { get; set; }   
        public DbSet<Category> Categories { get; set; } 
        public DbSet<Size> Sizes { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<ProductVariant>  ProductVariants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // -*-*-*-* Category
            modelBuilder.Entity<Category>().Property(c => c.Name).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Category>().HasMany(c => c.Products).WithOne(p => p.Category).HasForeignKey(p => p.CategoryId);
            //- *-*-*-* Product 
            modelBuilder.Entity<Product>().Property(p => p.Name).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Product>().Property(p => p.SKU).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Product>().HasIndex(p => p.SKU).IsUnique();
            modelBuilder.Entity<Product>().ToTable(t =>
            {
                t.HasCheckConstraint("CK_Product_SellingPrice_GreaterThanZero", "[SellingPrice] > 0 ");
                t.HasCheckConstraint("CK_Product_PurchasePrice_GreaterThanZero", "[PurchasePrice] > 0 ");
            });
            modelBuilder.Entity<Product>().Property(p => p.SellingPrice).IsRequired().HasPrecision(18, 2);
            modelBuilder.Entity<Product>().Property(p => p.PurchasePrice).IsRequired().HasPrecision(18, 2);
            //- *-*-* -Color

            modelBuilder.Entity<Color>().Property(p => p.Name).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Color>().Property(p => p.Code).IsRequired().HasMaxLength(7);
            // **-*- *Size  
            modelBuilder.Entity<Size>().Property(p => p.Name).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Size>().Property(p => p.Code).IsRequired().HasMaxLength(7);
            // -*-* ProductVariant 

            modelBuilder.Entity<ProductVariant>(entity => {
                entity.Property(p => p.VariantSKU).IsRequired().HasMaxLength(150);
                entity.HasIndex(p => p.VariantSKU).IsUnique(); 

                entity.HasOne(pv => pv.Product)
                      .WithMany(p => p.Variants)
                      .HasForeignKey(pv => pv.ProductId)
                      .OnDelete(DeleteBehavior.Cascade); 

                entity.HasOne(pv => pv.Color).WithMany().HasForeignKey(pv => pv.ColorId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(pv => pv.Size).WithMany().HasForeignKey(pv => pv.SizeId).OnDelete(DeleteBehavior.Restrict);
            
            
            
            });
        }
    }
}
