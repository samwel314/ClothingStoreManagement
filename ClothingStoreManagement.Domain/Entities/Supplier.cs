namespace ClothingStoreManagement.Domain.Entities
{
    public class Supplier
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string? AdditionalPhone { get; set; }

        public string? Address { get; set; }

        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<SupplierPurchase> Purchases { get; set; }
            = new List<SupplierPurchase>();

        public void ToggleStatus()
        {
            IsActive = !IsActive;
        }
    }
}
