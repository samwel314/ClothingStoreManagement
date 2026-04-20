using System.Drawing;

namespace ClothingStoreManagement.Domain.Entities
{
    public class ProductVariant
    {
        public ProductVariant(Guid parentId, string parentSKU, int sizeId, 
            string sizeCode , int colorId , string colorCode , int stockQuantity , 
            decimal sellingPrice ,decimal purchasePrice)
        {
            Id = Guid.NewGuid();
            ValidateStockQuantity(stockQuantity);
            ValidateSellingPrice(sellingPrice);
            ValidatePurchasePrice(purchasePrice);
            ProductId = parentId;
            VariantSKU = parentSKU + "-" + sizeCode + "-" + colorCode;
            SizeId = sizeId;
            ColorId = colorId;
            StockQuantity = stockQuantity;
            SellingPrice = sellingPrice;
            PurchasePrice = purchasePrice;
        }
        private ProductVariant()
        {
            
        }
        public Guid Id { get; private set; }
        public Guid ProductId { get; private set; } // الربط بالموديل الأب
        public string VariantSKU { get; private set; } = null!;      // الباركود الخاص (لون + مقاس)
        public int SizeId { get; private set; }
        public int ColorId { get; private set; }
        public decimal SellingPrice { get; private set; }
        public decimal PurchasePrice { get; private set; }
        public int StockQuantity { get; private set; } // الكمية من النوع ده بالظبط   
        public Product Product { get; private set; } = null!; // الربط بالموديل الأب 
        public Color Color { get; private set; } = null!;
        public Size Size { get; private set; } = null!;
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
        }
        public void UpdatePurchasePrice(decimal purchasePrice)
        {
            ValidatePurchasePrice(purchasePrice);
            PurchasePrice = purchasePrice;
        }
        public decimal ProfitPerUnit()
        {
            return SellingPrice - PurchasePrice;
        }
        private void ValidateStockQuantity(int stockQuantity)
        {
            if (stockQuantity < 0)
                throw new ArgumentException("Stock quantity cannot be negative.");
        }
        public void UpdateStockQuantity(int newStockQuantity)
        {
            ValidateStockQuantity(newStockQuantity);
            StockQuantity = newStockQuantity;
        }
        public void UpdateVariant(int sizeId, string sizeCode, int colorId, string colorCode, string parentSKU)
        {
            VariantSKU = $"{parentSKU}-{sizeCode}-{colorCode}";
            SizeId = sizeId;
            ColorId = colorId;
        }
    }
}
