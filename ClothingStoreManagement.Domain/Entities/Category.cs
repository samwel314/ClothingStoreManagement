namespace ClothingStoreManagement.Domain.Entities
{
    public class Category
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = null!;
        public bool IsActive { get; private set; } = true;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; private set; }
        private Category() { } // EF Core يحتاجه
        public Category(string name)
        {
            ValidateName(name);
            Name = name;
        }
        private void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required");
            if (name.Length > 100)
                throw new ArgumentException("Name must be less than 100 characters");
        }
        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }
        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }
        // to do this business we must pass it from app layer  
        public bool CanBeDeleted(int productsCount) => productsCount == 0;
        public void UpdateName(string name)
        {
            // app layer must check if the name is not the same as the current name to avoid unnecessary update
            ValidateName(name);
            Name = name;
            UpdatedAt = DateTime.UtcNow;
        }
        // nav 
        public IEnumerable<Product> Products { get; private set; } = null!;
    }
}
