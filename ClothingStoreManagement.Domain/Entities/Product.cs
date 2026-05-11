namespace ClothingStoreManagement.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = null!;
        public string SKU { get; private set; } = null!; // الباركود - رقم المنتج - رقم تسلسلي
        public bool IsActive { get; private set; } = true;
        public DateTime CreatedAt { get; private set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; private set; }
        public int CategoryId { get; private set; }
        public Category Category { get; private set; } = null!; // حريمي - رجالي - أطفال
        Product()
        {

        }
        public Product(string Name, string SKU, int CategoryId)
        {
            ValidateName(Name);
            ValidateSKU(SKU);
            Id = Guid.NewGuid();
            this.Name = Name;
            this.SKU = SKU;
            this.CategoryId = CategoryId;
        }
        private void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required");
            if (name.Length > 100)
                throw new ArgumentException("Name must be less than 100 characters");
        }
        private void ValidateSKU(string SKU)
        {
            if (string.IsNullOrWhiteSpace(SKU))
                throw new ArgumentException("SKU is required");
        }

        public void UpdateName(string name)
        {
            ValidateName(name);
            Name = name;
            UpdatedAt = DateTime.Now;
        }
        public void UpdateSKU(string sku)
        {
            ValidateSKU(sku);
            SKU = sku;
            UpdatedAt = DateTime.Now;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.Now;
        }
        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.Now;
        }
        public void ChangeCategory(int categoryId)
        {
            CategoryId = categoryId;
            UpdatedAt = DateTime.Now;
        }
        public void UpdateChanges() // if add any change to the product we will call this method to update the updated at time
        {
            UpdatedAt = DateTime.Now;
        }
        public void AddVariant(ProductVariant variant)
        {
            if (variant == null)
                throw new ArgumentNullException(nameof(variant));
            if (variant.ProductId != Id)
                throw new ArgumentException("Variant does not belong to this product");
            if (Variants == null)
                Variants = new List<ProductVariant>();
            var variants = new List<ProductVariant>(Variants) { variant };
            Variants = variants;
            UpdatedAt = DateTime.Now;
        }
        public IEnumerable<ProductVariant> Variants { get; private set; } = null!;
    }
}
