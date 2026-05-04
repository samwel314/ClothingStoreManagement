using System.ComponentModel.DataAnnotations.Schema;

namespace ClothingStoreManagement.Domain.Entities
{
    public class InvoiceItem
    {
        public InvoiceItem(int invoiceId, Guid productVariantId,
            int quantity, decimal sellingPrice, decimal purchasePrice, decimal discount = 0)
        {
            ValidateStockQuantity(quantity);
            ValidateSellingPrice(sellingPrice);
            ValidatePurchasePrice(purchasePrice);
            InvoiceId = invoiceId;
            ProductVariantId = productVariantId;
            Quantity = quantity;
            SellingPrice = sellingPrice;
            PurchasePrice = purchasePrice;
            Discount = discount;
        }
        public int Id { get; private set; }
        public int InvoiceId { get; private set; }
        public Guid ProductVariantId { get; private set; }
        public int Quantity { get; private set; }
        public decimal SellingPrice { get; private set; }
        public decimal PurchasePrice { get; private set; }
        public decimal Discount { get; private set; }
        public decimal TotalPrice => Quantity * SellingPrice * (1 - Discount / 100);
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
        private void ValidateStockQuantity(int quantity)
        {
            if (quantity < 1)
                throw new ArgumentException(" quantity must be at least 1.");
        }
        // nav 
        [ForeignKey("ProductVariantId")]
        public ProductVariant ProductVariant { get; private set; } = null!;
        [ForeignKey("InvoiceId")]
        public Invoice Invoice { get; private set; } = null!;
    }
}
