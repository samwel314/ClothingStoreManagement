using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ClothingStoreManagement.Data
{
    public class DbFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>();

            var dbPath = Path.Combine(AppContext.BaseDirectory, "app.db");

            options.UseSqlite($"Data Source={dbPath}");

            return new ApplicationDbContext(options.Options);
        }
    }
}
