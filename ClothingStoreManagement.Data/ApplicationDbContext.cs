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
        public DbSet<MainTreasuryTransaction> TreasuryTransactions { get; set; }    
        public DbSet<Employee> Employees { get; set; }  
        public DbSet<EmployeeTransaction> EmployeeTransactions { get; set; }
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

            modelBuilder.Entity<User>().HasData(new User("samuel",
      "$2a$11$0.EXpw9/TA95xTDBf5rqWOa7eg2cqZC8ENIXtGqGRgAFG15UbGlJa", UserRole.Admin)
            {
                Id = 1
            });
            modelBuilder.Entity<PaymentSource>().HasData(new PaymentSource("كاش (نقدي)")
            {
                Id = 1,
                IsCashSource = true,
            });

            modelBuilder.Entity<Category>().HasData(
                new
                {
                    Id = 1,
                    Name = "حريمي",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = (DateTime?)null
                }
            ); 
            modelBuilder.Entity<Color>().HasData(
    new { Id = 1, Name = "أسود", Code = "BLK", HexCode = "#000000" },
    new { Id = 2, Name = "أبيض", Code = "WHT", HexCode = "#FFFFFF" },
    new { Id = 3, Name = "رمادي", Code = "GRY", HexCode = "#808080" },
    new { Id = 4, Name = "كحلي", Code = "NVY", HexCode = "#000080" },
    new { Id = 5, Name = "بيج", Code = "BGE", HexCode = "#F5F5DC" },
    new { Id = 6, Name = "بني", Code = "BRN", HexCode = "#A52A2A" },

    new { Id = 7, Name = "أحمر", Code = "RED", HexCode = "#FF0000" },
    new { Id = 8, Name = "أزرق", Code = "BLU", HexCode = "#0000FF" },
    new { Id = 9, Name = "أخضر", Code = "GRN", HexCode = "#008000" },
    new { Id = 10, Name = "أصفر", Code = "YLW", HexCode = "#FFFF00" },
    new { Id = 11, Name = "زيتي", Code = "OLV", HexCode = "#808000" },
    new { Id = 12, Name = "نبيتي", Code = "MRN", HexCode = "#800000" },
    new { Id = 13, Name = "وردي / بينك", Code = "PNK", HexCode = "#FFC0CB" },
    new { Id = 14, Name = "سماني / أوف وايت", Code = "OWH", HexCode = "#FAFAFA" }
);
            modelBuilder.Entity<Size>().HasData(
    // مقاسات الأحرف Standard (S - M - L - XL...)
    new { Id = 1, Name = "Small", Code = "S" },
    new { Id = 2, Name = "Medium", Code = "M" },
    new { Id = 3, Name = "Large", Code = "L" },
    new { Id = 4, Name = "X-Large", Code = "XL" },
    new { Id = 5, Name = "2X-Large", Code = "2XL" },
    new { Id = 6, Name = "3X-Large", Code = "3XL" },

    // مقاسات البنطلونات والأحذية والأحجام الرقمية
    new { Id = 7, Name = "28", Code = "28" },
    new { Id = 8, Name = "30", Code = "30" },
    new { Id = 9, Name = "32", Code = "32" },
    new { Id = 10, Name = "34", Code = "34" },
    new { Id = 11, Name = "36", Code = "36" },
    new { Id = 12, Name = "38", Code = "38" },
    new { Id = 13, Name = "40", Code = "40" },
    new { Id = 14, Name = "42", Code = "42" },
    new { Id = 15, Name = "44", Code = "44" }
);
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
            modelBuilder.Entity<MainTreasuryTransaction>(entity =>
            {
                entity.Property(e => e.Amount).HasConversion<double>();
            });
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.Property(e => e.BaseSalary).HasConversion<double>();
            });
            modelBuilder.Entity<EmployeeTransaction>(entity =>
            {
                entity.Property(e => e.Amount).HasConversion<double>();
            });
            modelBuilder.Entity<Shift>(entity =>
            {
                entity.Property(e => e.TotalSalesCash).HasConversion<double>();
                entity.Property(e => e.TotalSalesNonCash).HasConversion<double>();
                entity.Property(e => e.TotalReturns).HasConversion<double>();
                entity.Property(e => e.Difference).HasConversion<double>();
                entity.Property(e => e.TotalAdjustments).HasConversion<double>();
                entity.Property(e => e.TotalExpenses).HasConversion<double>();
            });
        }
    }
}
