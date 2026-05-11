using System.ComponentModel.DataAnnotations.Schema;

namespace ClothingStoreManagement.Domain.Entities
{
    public class Shift
    {
        public Shift(decimal initialCash, int userId)
        {
            ValidateCash(initialCash); 
            InitialCash = initialCash;
            UserId = userId;
            StartTime = DateTime.UtcNow;    
        }
        public int Id { get; private set; }
        public DateTime StartTime { get; private set; } 
        public DateTime? EndTime { get; private set; }
        public decimal? FinalCashInDrawer { get; private set; }
        public decimal InitialCash { get; private set; }
        public int UserId { get; private set; }
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
        public int ClosedByUserId { get; private set; } 
        [ForeignKey("ClosedByUserId")]
        public User ClosedByUser { get; set; } = null!; 
        public IEnumerable<Invoice> Invoices { get; set; } = new List<Invoice>(); 
        public IEnumerable<ShiftTransaction> ShiftTransactions { get; set; } = new List<ShiftTransaction>();    
        public bool IsActive { get; private set; } = true;  // means the shift is currently active and not closed yet
   
        private void ValidateCash(decimal cash)
        {
            if (cash < 0)
                throw new ArgumentException("Cash amount cannot be negative.");
        }
        public void CloseShift(decimal finalCashInDrawer)
            {
                if (!IsActive)
                    throw new InvalidOperationException("Shift is already closed.");
    
                EndTime = DateTime.Now;
                FinalCashInDrawer = finalCashInDrawer;
                IsActive = false;
        }

    }
}
