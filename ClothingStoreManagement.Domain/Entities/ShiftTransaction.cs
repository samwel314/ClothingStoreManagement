using System.ComponentModel.DataAnnotations.Schema;

namespace ClothingStoreManagement.Domain.Entities
{
    public class ShiftTransaction
    {
        public int Id { get; private set; }
        public int ShiftId { get; private set; }
        [ForeignKey("ShiftId")]
        public Shift Shift { get;  set; } = null!;
        public decimal Amount { get; private set; }
        public TransactionType Type { get; private set; }
        public string Description { get; private set; } = null!;
        public DateTime CreatedAt { get; private set; } = DateTime.Now;
        public int UserId { get; private set; }
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
        private ShiftTransaction() { }

        public ShiftTransaction(int userId, int shiftId, decimal amount, TransactionType type, string description)
        {
            UserId = userId;    
            ShiftId = shiftId;
            Amount = amount;
            Type = type;
            Description = description;
        }
    }
}
