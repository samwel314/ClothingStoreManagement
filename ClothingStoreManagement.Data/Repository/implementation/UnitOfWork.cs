using System;
using System.Collections.Generic;
using System.Text;

namespace ClothingStoreManagement.Data.Repository.implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Colors = new ColorRepository(_db);
            Sizes = new SizeRepository(_db);   
            Categories = new CategoryRepository(_db);
                Products = new ProductRepository(_db);
            ProductVariants = new ProductProductVariantRepository(_db); 
        }

        public IColorRepository Colors { get; private set; }
        public IProductRepository Products { get; private set; }    
        public ISizeRepository Sizes { get; private set; }
        public ICategoryRepository Categories { get; private set; }

        public IProductProductVariantRepository ProductVariants { get; private set; }

        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }
    }
}
