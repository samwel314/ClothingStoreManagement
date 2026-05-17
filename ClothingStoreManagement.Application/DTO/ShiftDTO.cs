using System.ComponentModel;

namespace ClothingStoreManagement.Application.DTO
{
    public class ShiftDTO
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public decimal InitialCash { get; set; }
        public string OpenBy { get; set; } = null!;
        public decimal TotalAmounts => PaymentMethods.Select(p => p.TotalAmount).Sum(); 
        public List<PaymentTypeSummary> PaymentMethods { get; set; } = new();
        public decimal TotalExpenses { get; set; }
        public decimal Adjustments { get; set; }
        public decimal ExpectedCash =>
            InitialCash + Adjustments + 
            (PaymentMethods.FirstOrDefault(p => p.IsCashSource)?.TotalAmount ?? 0) -
                     Math.Abs(TotalExpenses); 

    }
    public class PaymentTypeSummary
    {
        public string Name { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public bool IsCashSource { get; set; } 
    }
    public class ShiftDetailsDTO
    {
        public int ShiftId { get; set; }
        public string OpenedByUserName { get; set; } = string.Empty;
        public string ClosedByUserName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsActive { get; set; }
        public decimal InitialCash { get; set; }         
        public decimal TotalSalesCash { get; set; }      
        public decimal TotalReturnsCash { get; set; }   
        public decimal TotalExpenses { get; set; }     
        public decimal TotalAdjustments { get; set; }    
        public decimal TotalSalesNonCash { get; set; }    
        public decimal FinalCashInDrawer { get; set; }    
        public decimal ExpectedCashInDrawer =>
            InitialCash + TotalSalesCash - TotalReturnsCash - TotalExpenses + TotalAdjustments;
        public decimal Difference => FinalCashInDrawer - ExpectedCashInDrawer;
        public IEnumerable<TransactionListDTO> Transactions { get; set; } 
    }
}
