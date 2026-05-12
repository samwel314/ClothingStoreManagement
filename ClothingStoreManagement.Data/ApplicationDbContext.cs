using BCrypt.Net;
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
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<StockMovement> Movements { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Shift> Shifts { get; set; }    
        public DbSet<InvoicePayment> InvoicePayments { get; set; }  
        public DbSet<PaymentSource> PaymentSources { get; set; }    
        public DbSet<ShiftTransaction> ShiftTransactions { get; set; }      
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

            modelBuilder.Entity<Product>()
                .Property(p => p.Id)
                .HasConversion(
                    v => v.ToString().ToLower(),
                    v => Guid.Parse(v));
            //- *-*-* -Color

            modelBuilder.Entity<Color>().Property(p => p.Name).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Color>().Property(p => p.Code).IsRequired().HasMaxLength(7);
            // **-*- *Size  
            modelBuilder.Entity<Size>().Property(p => p.Name).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Size>().Property(p => p.Code).IsRequired().HasMaxLength(7);
            // -*-* ProductVariant 

            modelBuilder.Entity<ProductVariant>(entity =>
            {
                entity.Property(p => p.VariantSKU).IsRequired().HasMaxLength(150);
                entity.HasIndex(p => p.VariantSKU).IsUnique();

                entity.HasOne(pv => pv.Product)
                      .WithMany(p => p.Variants)
                      .HasForeignKey(pv => pv.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(pv => pv.Color).WithMany().HasForeignKey(pv => pv.ColorId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(pv => pv.Size).WithMany().HasForeignKey(pv => pv.SizeId).OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<ProductVariant>().Property(p => p.SellingPrice).IsRequired().HasPrecision(18, 2);
                modelBuilder.Entity<ProductVariant>().Property(p => p.PurchasePrice).IsRequired().HasPrecision(18, 2);
                modelBuilder.Entity<ProductVariant>().ToTable(t =>
                {
                    t.HasCheckConstraint("CK_Product_SellingPrice_GreaterThanZero", "[SellingPrice] > 0 ");
                    t.HasCheckConstraint("CK_Product_PurchasePrice_GreaterThanZero", "[PurchasePrice] > 0 ");
                });
                modelBuilder.Entity<ProductVariant>(entity =>
                {
                    entity.Property(e => e.SellingPrice).HasConversion<double>();
                    entity.Property(e => e.PurchasePrice).HasConversion<double>();
                });

                modelBuilder.Entity<ProductVariant>()
                                    .Property(p => p.Id)
                                    .HasConversion(
                                        v => v.ToString().ToLower(),
                                        v => Guid.Parse(v));
            });

            modelBuilder.Entity<ProductVariant>()
                             .Property(p => p.ProductId)
                             .HasConversion(
                             v => v.ToString().ToLower(),
                             v => Guid.Parse(v));
            //-*-*-*-*Invoice
            modelBuilder.Entity<Invoice>().HasIndex(i => i.Serial).IsUnique();
            modelBuilder.Entity<Invoice>().Property(v => v.Serial).HasMaxLength(50);

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.Property(e => e.TotalAmount).HasConversion<double>();
                entity.Property(e => e.TotalAmountWithDiscount).HasConversion<double>();
            });

            // -*-*-*-* InvoiceItem
            modelBuilder.Entity<InvoiceItem>()
                .Property(p => p.ProductVariantId)
                .HasConversion(
                v => v.ToString().ToLower(),
                v => Guid.Parse(v));
            modelBuilder.Entity<InvoiceItem>(entity =>
            {
                entity.Property(e => e.SellingPrice).HasConversion<double>();
                entity.Property(e => e.PurchasePrice).HasConversion<double>();
                entity.Property(e => e.Discount).HasConversion<double>();
            });
            // *--* User 

            modelBuilder.Entity<User>()
            .HasIndex(u => u.UserName)
            .IsUnique();
            /// -***-* Shift 
            modelBuilder.Entity<Shift>()
    .HasIndex(s => s.EndTime)
    .HasFilter("[EndTime] IS NULL"); 

            modelBuilder.Entity<User>().HasData ( new User ("Samuel" ,
               "$2a$11$evS/J.Lp6vL8vL8vL8vL8ueXGvS/J.Lp6vL8vL8vL8vL8ueXG", UserRole.Admin)
            {
                    Id = 1  
            });
   
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var properties = entityType.GetProperties()
                    .Where(p => p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?));

                foreach (var property in properties)
                {
                    property.SetColumnType("DATETIME");
                }
            }
            modelBuilder.Entity<Shift>(entity =>
            {
                entity.Property(e => e.InitialCash).HasConversion<double>();
                entity.Property(e => e.FinalCashInDrawer).HasConversion<double>();
            });
            modelBuilder.Entity<InvoicePayment>(entity =>
            {
                entity.Property(e => e.Amount).HasConversion<double>();
            });
            modelBuilder.Entity<ShiftTransaction>(entity =>
            {
                entity.Property(e => e.Amount).HasConversion<double>();
            });

            modelBuilder.Entity<Shift>(entity =>
            {
                entity.Property(e => e.TotalSalesCash).HasConversion<double>();
                entity.Property(e => e.TotalSalesNonCash).HasConversion<double>();
                entity.Property(e => e.TotalReturns).HasConversion<double>();
                entity.Property(e => e.Difference).HasConversion<double>();

            });
        }
    }
}
