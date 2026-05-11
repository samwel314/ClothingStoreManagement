namespace ClothingStoreManagement.Domain.Entities
{
    public class InvoicePayment
    {
        public int Id { get; private set; }
        public int InvoiceId { get; private set; }
        public decimal Amount { get; private set; }
        public int PaymentSourceId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public string? Reference { get; private set; } 
        public Invoice Invoice { get; private set; } = null!;
        public PaymentSource PaymentSource { get; private set; } = null!;
        private InvoicePayment() { }

        public InvoicePayment(int invoiceId, int paymentSourceId, decimal amount, string? reference = null)
        {
            if (amount <= 0)
                throw new ArgumentException("مبلغ الدفع يجب أن يكون أكبر من صفر");

            InvoiceId = invoiceId;
            PaymentSourceId = paymentSourceId;
            Amount = amount;
            Reference = reference;
            CreatedAt = DateTime.Now;
        }
    }
}
