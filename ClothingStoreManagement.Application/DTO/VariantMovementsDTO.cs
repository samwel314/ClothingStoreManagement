using ClothingStoreManagement.Domain.Entities;

namespace ClothingStoreManagement.Application.DTO
{
    public class VariantMovementsDTO
    {
        public int Id { get; set; }
        public int QuantityChange { get; set; }
        public int StockAfter { get; set; }
        public MovementType Type { get; set; }
        public string? ReferenceId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
    }
}
