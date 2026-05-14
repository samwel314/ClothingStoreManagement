using System.ComponentModel.DataAnnotations;

namespace ClothingStoreManagement.Application.DTO
{
    public class CreateShiftDTO
    {
        [Range (0.0 , (double) decimal.MaxValue , ErrorMessage = "لا يمكن وضع قيمة اقل من صفر ") ]
        public decimal InitialCash { get; set; } 
    }
}
