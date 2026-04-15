using System.Drawing;

namespace ClothingStoreManagement.Data.Entities
{
    public class ProductVariant
    {
        public ProductVariant(Guid parentId, string parentSKU, int sizeId, 
            string sizeCode , int colorId , string colorCode , int stockQuantity)
        {
            Id = Guid.NewGuid();
            ValidateStockQuantity(stockQuantity);

            ProductId = parentId;
            VariantSKU = parentSKU + "-" + sizeCode + "-" + colorCode;
            SizeId = sizeId;
            ColorId = colorId;
            StockQuantity = stockQuantity;
        }
        public Guid Id { get; private set; }
        public Guid ProductId { get; private set; } // الربط بالموديل الأب
        public string VariantSKU { get; private set; } = null!;      // الباركود الخاص (لون + مقاس)
        public int SizeId { get; private set; }
        public int ColorId { get; private set; }
        public int StockQuantity { get; private set; } // الكمية من النوع ده بالظبط   
        public Product Product { get; private set; } = null!; // الربط بالموديل الأب 
        public Color Color { get; private set; } = null!;
        public Size Size { get; private set; } = null!;
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
