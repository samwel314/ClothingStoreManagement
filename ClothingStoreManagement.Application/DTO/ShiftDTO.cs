namespace ClothingStoreManagement.Application.DTO
{
    public class ShiftDTO
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public decimal InitialCash { get; set; }
        public string OpenBy { get; set; } = null!;
        public List<PaymentTypeSummary> PaymentMethods { get; set; } = new();

        public decimal TotalExpenses { get; set; }
        public decimal ExpectedCash =>
            InitialCash +
            (PaymentMethods.FirstOrDefault(p => p.IsCashSource)?.TotalAmount ?? 0) -
                     TotalExpenses ; 

    }
    public class PaymentTypeSummary
    {
        public string Name { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public bool IsCashSource { get; set; } 
    }
}
