using System.ComponentModel.DataAnnotations;

namespace ClothingStoreManagement.Application.DTO
{
    public class CreateShiftDTO
    {
        [Range (0.0 , (double) decimal.MaxValue)]
        public decimal InitialCash { get; set; } 
    }
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
