namespace ClothingStoreManagement.Application.DTO
{
    public class InvoiceItemDetailsDto
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; } = null!;
        public string SKU { get; set; } = null!;
        public string Color { get; set; } = null!; 
        public string HexCode { get; set; } = null!;    
        public string Size { get; set; } = null!;
        public decimal OriginalPrice { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; } = 1;
        public int AvailableQuantity { get; set; }
        private decimal _discount;
        public decimal Discount
        {
            get => _discount;
            set
            {
                if (value > 100) _discount = 100;
                else if (value < 0) _discount = 0;
                else _discount = value;
            }
        }
        public decimal DiscountedUnitPrice =>
            Price * (1 - Discount / 100m);

        public decimal Total =>
            DiscountedUnitPrice * Quantity;

        public decimal TotalPurchaseCost =>
            OriginalPrice * Quantity;

        public bool IsLosing =>
            Total < TotalPurchaseCost;
    }
}
