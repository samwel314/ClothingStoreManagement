namespace ClothingStoreManagement.Data.Repository
{
    public interface IUnitOfWork
    {
        IColorRepository Colors { get; }
        ISizeRepository Sizes { get; }
        IProductRepository Products { get; }
        ICategoryRepository Categories { get; }
        IProductProductVariantRepository ProductVariants { get; }
        IInvoiceRepository Invoices { get; }
        IStockMovementRepository Movements { get; } 
        IUserRepository Users { get; }  
        Task Save();
        void Clear(); //
    }

}
