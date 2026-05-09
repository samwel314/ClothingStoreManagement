using System.ComponentModel;

namespace ClothingStoreManagement.Application.DTO
{
    public class HomeDTO
    {
        public DateOnly ThisDay { get; set; } = new DateOnly(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day);
        public int TotalDayInvoices { get; set; }
        public decimal TotalDayRevenue { get; set; }
        public decimal PreviousDayRevenue { get; set; }
        public int TotalProducts { get; set; }
        public IEnumerable <TopProductDTO>  TopProductDTOs { get; set; } 
        public double CalculatePercentageChange()
        {
            if (PreviousDayRevenue == 0)
                return TotalDayRevenue > 0 ? 100 : 0;
            double change = (double)((TotalDayRevenue - PreviousDayRevenue) / PreviousDayRevenue) * 100;
            return Math.Round(change, 1);
        }
 
    }
    public class TopProductDTO
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; }
        public string SizeName { get; set; }   // إضافة اسم المقاس
        public string ColorHex { get; set; }   // إضافة كود اللون (مثلاً #FF0000)
        public double TotalQuantity { get; set; }
        public int InvoicesCount { get; set; }
    }
    public class DailySalesDTO
    {
        public string Date { get; set; }    
        public decimal TotalAmount { get; set; }    
    }
}
