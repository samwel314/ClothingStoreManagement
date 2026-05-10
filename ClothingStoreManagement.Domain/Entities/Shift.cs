using System.ComponentModel.DataAnnotations.Schema;

namespace ClothingStoreManagement.Domain.Entities
{
    public class Shift
    {
        public Shift(decimal initialCash, int userId)
        {
            InitialCash = initialCash;
            UserId = userId;
            StartTime = DateTime.UtcNow;    
        }

        public int Id { get; private set; }
        public DateTime StartTime { get; private set; } 
        public DateTime? EndTime { get; private set; }
        public decimal ExpectedCash { get; private set; } 
        public decimal? FinalCashInDrawer { get; private set; }
        public decimal InitialCash { get; private set; }
        public int UserId { get; private set; }
        [ForeignKey("UserId")]
        public User User { get;  set; }  
        public bool IsActive { get; private set; } = true;  // means the shift is currently active and not closed yet
   

        public void SetExpectedCash(decimal expectedCash)
        {
            if (!IsActive)
                throw new InvalidOperationException("Shift is already closed.");
            ExpectedCash = expectedCash;
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
