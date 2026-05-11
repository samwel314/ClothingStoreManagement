using System.ComponentModel.DataAnnotations.Schema;

namespace ClothingStoreManagement.Domain.Entities
{
    public class ShiftTransaction
    {
        public int Id { get; private set; }
        public int ShiftId { get; private set; }
        public decimal Amount { get; private set; }
        public TransactionType Type { get; private set; }
        public string Description { get; private set; } = null!;
        public DateTime CreatedAt { get; private set; } = DateTime.Now;
        public int UserId { get; private set; }
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
        private ShiftTransaction() { }

        public ShiftTransaction(int shiftId, decimal amount, TransactionType type, string description)
        {
            if (amount <= 0) throw new ArgumentException("المبلغ لازم يكون أكبر من صفر");
            if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("لازم توصف الفلوس دي رايحة فين");

            ShiftId = shiftId;
            Amount = amount;
            Type = type;
            Description = description;
        }
    }
}
