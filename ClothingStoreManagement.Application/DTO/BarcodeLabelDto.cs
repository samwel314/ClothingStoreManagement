namespace ClothingStoreManagement.Application.DTO
{
    public class BarcodeLabelDto
    {
        public string ProductName { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public string? Color { get; set; }
        public string? Size { get; set; }
        public byte[] BarcodeImage { get; set; } = [];
    }
}
