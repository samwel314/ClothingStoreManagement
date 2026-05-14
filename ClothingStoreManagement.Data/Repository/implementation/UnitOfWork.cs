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
            Invoices = new InvoiceRepository(_db);
            Movements = new StockMovementRepository(_db);
            Users = new UserRepository(_db);    
            Shifts = new ShiftRepository(_db);      
            PaymentSources = new PaymentSourceRepository(_db);
            ShiftTransactions = new ShiftTransactionRepository(_db);
            InvoicePayments = new InvoicePaymentRepository(_db);
        }

        public IColorRepository Colors { get; private set; }
        public IProductRepository Products { get; private set; }
        public ISizeRepository Sizes { get; private set; }
        public ICategoryRepository Categories { get; private set; }

        public IProductProductVariantRepository ProductVariants { get; private set; }

        public IInvoiceRepository Invoices { get; private set; }
        public IStockMovementRepository Movements { get; private set; }
        public IUserRepository Users { get; private set; }
        public IShiftRepository Shifts { get; private set; }
        public IPaymentSourceRepository PaymentSources { get; private set; }
        public IShiftTransactionRepository ShiftTransactions { get; private set;     }
        public IInvoicePaymentRepository InvoicePayments     { get; private set; }
        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }
        public void Clear()
        {
            _db.ChangeTracker.Clear();
        }   
    }
}
