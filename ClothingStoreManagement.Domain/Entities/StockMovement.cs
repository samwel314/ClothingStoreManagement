using System.ComponentModel.DataAnnotations.Schema;

namespace ClothingStoreManagement.Domain.Entities
{
    public class StockMovement
    {
        public int Id { get; set; }
        public Guid ProductVariantId { get; set; }
        [ForeignKey("ProductVariantId")]
        public ProductVariant ProductVariant { get; set; } = null!; 
        public int QuantityChange { get; set; }
        public int StockAfter { get; set; } 
        public MovementType Type { get; set; } 
        public string ? ReferenceId { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string ? CreatedBy { get; set; } 
    }
}
