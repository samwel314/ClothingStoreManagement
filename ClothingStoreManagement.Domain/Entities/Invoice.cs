using System;
using System.Collections.Generic;
using System.Text;

namespace ClothingStoreManagement.Domain.Entities
{
    public class Invoice
    {
        public int Id { get; private set; }
        public string Serial { get; set; } = $"INV-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}"; public DateTime CreatedAt { get; private set; } = DateTime.Now; 
        public DateTime ? LastUpdatedAt { get; private set; } // لو غيرنا الحالة بتتغير
        public decimal TotalAmount { get; private set; } // مجموع سعر البيع لكل الأصناف في الفاتورة
        public InvoiceStatus Status { get; private set; } = InvoiceStatus.pending;
        public void CalcTotal( decimal price , int quantity , decimal discount , decimal oldAmount) // لما نضيف صنف جديد أو نعدل صنف موجود في الفاتورة لازم نعيد حساب المجموع
        {
            TotalAmount = TotalAmount - oldAmount + (price * quantity) * (1  - discount ) ;  
        }

        public void UpdateStatus(InvoiceStatus newStatus)
         {
             if (Status == InvoiceStatus.returned)
                 throw new InvalidOperationException("Can't change status of a returned invoice.");
             if (Status == InvoiceStatus.completed && newStatus == InvoiceStatus.pending)
                 throw new InvalidOperationException("Can't revert a completed invoice to pending.");
             Status = newStatus;
             LastUpdatedAt = DateTime.Now;
        }
        public IEnumerable<InvoiceItem> Items { get; private set; } = new List<InvoiceItem>();

    }
}
