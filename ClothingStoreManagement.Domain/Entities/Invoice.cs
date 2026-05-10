namespace ClothingStoreManagement.Domain.Entities
{
    public class Invoice
    {
        public int Id { get; private set; }
        public string Serial { get; set; } = $"INV-{DateTime.Now:yyyyMMdd}-{DateTime.Now:HHmmss}";
        public DateTime CreatedAt { get; private set; } = DateTime.Now;
        public DateTime? LastUpdatedAt { get; private set; } // لو غيرنا الحالة بتتغير        public DateTime? LastUpdatedAt { get; private set; } // لو غيرنا الحالة بتتغير
        public decimal TotalAmount { get; private set; } // مجموع سعر البيع لكل الأصناف في الفاتورة
        public decimal TotalAmountWithDiscount { get; private set; }
        public InvoiceStatus Status { get; private set; } = InvoiceStatus.pending;
        public int ShiftId { get; private set; }        
        public Shift Shift { get;  set; } = null!; // كل فاتورة مرتبطة بشفت معين 
        public void SetTotal(decimal total) => TotalAmount = total;
        public void SetShift(int shiftId) => ShiftId = shiftId;     
        public void SetTotalWithDiscount(decimal total) => TotalAmountWithDiscount = total;
        public void UpdateStatus(InvoiceStatus newStatus)
        {
            if (Status == InvoiceStatus.returned)
                throw new InvalidOperationException("Can't change status of a returned invoice.");
            if (Status == InvoiceStatus.completed && newStatus == InvoiceStatus.pending)
                throw new InvalidOperationException("Can't revert a completed invoice to pending.");
            Status = newStatus;
            LastUpdatedAt = DateTime.Now;
        }
        public IEnumerable<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();

    }
}
