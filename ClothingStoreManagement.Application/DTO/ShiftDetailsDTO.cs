namespace ClothingStoreManagement.Application.DTO
{
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
