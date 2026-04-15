using System;
using System.Collections.Generic;
using System.Text;

namespace ClothingStoreManagement.Data.Entities
{
    public class Product
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = null!;
        public string SKU { get; private set; } = null!; // الباركود - رقم المنتج - رقم تسلسلي
        public decimal SellingPrice { get; private set; }
        public decimal PurchasePrice { get; private set; }
        public bool IsActive { get; private set; } = true;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; private set; }
        public int CategoryId { get; private set; }
        public Category Category { get; private set; } = null!; // حريمي - رجالي - أطفال
        Product()
        {

        }
        public Product(string Name, string SKU, decimal SellingPrice, decimal PurchasePrice, int CategoryId)
        {
            ValidateName(Name);
            ValidateSKU(SKU);
            ValidateSellingPrice(SellingPrice);
            ValidatePurchasePrice(PurchasePrice);
            Id = Guid.NewGuid();
            this.Name = Name;
            this.SKU = SKU;
            this.SellingPrice = SellingPrice;
            this.PurchasePrice = PurchasePrice;
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
        private void ValidateMinimumStock(int minimumStock)
        {
            if (minimumStock < 0)
                throw new ArgumentException("minimumStock must be greater than zero or equal zero ");
        }
        public void UpdateName(string name)
        {
            ValidateName(name);
            Name = name;
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdateSKU(string sku)
        {
            ValidateSKU(sku);
            SKU = sku;
            UpdatedAt = DateTime.UtcNow;
        }
        private void ValidateSellingPrice(decimal sellingPrice)
        {
            if (sellingPrice <= 0)
                throw new ArgumentException("sellingPrice must be greater than zero ");
        }
        private void ValidatePurchasePrice(decimal purchasePrice)
        {
            if (purchasePrice <= 0)
                throw new ArgumentException("purchasePrice must be greater than zero ");
        }
        public void UpdateSellingPrice(decimal sellingPrice)
        {
            ValidateSellingPrice(sellingPrice);
            SellingPrice = sellingPrice;
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdatePurchasePrice(decimal purchasePrice)
        {
            ValidatePurchasePrice(purchasePrice);
            PurchasePrice = purchasePrice;
            UpdatedAt = DateTime.UtcNow;
        }
        public decimal ProfitPerUnit()
        {
            return SellingPrice - PurchasePrice;
        }
        public void UpdateMinimumStock(int minimumStock)
        {
            ValidateMinimumStock(minimumStock);
            UpdatedAt = DateTime.UtcNow;
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
        public void ChangeCategory(int categoryId)
        {
            CategoryId = categoryId;
            UpdatedAt = DateTime.UtcNow;
        }
        public IEnumerable<ProductVariant> Variants { get; private set; } = null!; 
    }
}
