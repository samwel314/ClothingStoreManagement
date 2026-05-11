namespace ClothingStoreManagement.Application.DTO
{
    public class  ShiftDTO
    {
        public int Id { get;  set; }
        public DateTime StartTime { get;  set; }
        public DateTime? EndTime { get;  set; }
        public decimal InitialCash { get;  set; }
        public decimal ExpectedCash { get;  set; }  
        public string OpenBy { get;  set; } = null!; 
        public string ?  CloseBy { get;  set; }
    }
}
