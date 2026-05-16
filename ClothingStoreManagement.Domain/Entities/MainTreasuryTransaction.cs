namespace ClothingStoreManagement.Domain.Entities
{
    public class MainTreasuryTransaction
    {
        public MainTreasuryTransaction(decimal amount, TreasuryTransactionType type, string notes, int? shiftId = null)
        {
            Amount = amount;
            Type = type;
            Notes = notes;
            ShiftId = shiftId;
            CreatedAt = DateTime.Now;    
        }
        public int Id { get; set; }
        public decimal Amount { get; private    set; } 
        public TreasuryTransactionType Type { get; private set; }
        public string Notes { get; private set; }
        public int? ShiftId { get; private set; }
        public Shift Shift { get; private set; } = null!; 
        public DateTime CreatedAt { get;   private  set; }
    }
}
