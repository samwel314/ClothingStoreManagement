namespace ClothingStoreManagement.Domain.Entities
{
    public class Shift
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; } 
        public DateTime? EndTime { get; set; }   

        public decimal InitialCash { get; set; } 

        public decimal? FinalCashInDrawer { get; set; } 
        public string? Notes { get; set; }              

        public int UserId { get; set; } 
    }
}
