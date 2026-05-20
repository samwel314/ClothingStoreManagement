using ClothingStoreManagement.Domain.Entities;

namespace ClothingStoreManagement.Application.DTO
{
    public class DisplayTreasuryTransactionDTO
    {
        public int Id { get; set; }
        public decimal Amount { get;  set; }
        public TreasuryTransactionType Type { get;  set; }
        public string Notes { get;  set; }
        public int? ShiftId { get;  set; }
        public string By { get; set; }
        public DateTime CreatedAt { get;  set; }
    }
}
